using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Accord.Statistics.Testing;
using CoreLib.Statistics;

namespace CoreLib
{
    /// <summary>
    /// Class which contains block statistics info
    /// </summary>
    public sealed class BlockStats
    {
        private readonly ConvertionObject _groupAConvertion = new ConvertionObject();
        private readonly ConvertionObject _groupBConvertion = new ConvertionObject();

        private readonly int _blockId;
        private readonly object _blockStatsLock = new object();
        private readonly TimeSpan _updatedTimeSpan = TimeSpan.FromSeconds(10);
        private readonly Timer _saveTimer = new Timer();
        /// <summary>
        /// ConvertionObject(value) for specific position(key)
        /// </summary>
        public Dictionary<int, ConvertionObject> PositionsConvertion { get; private set; }

        public ConvertionObject GroupAConvertion
        {
            get { return _groupAConvertion; }
        }

        public ConvertionObject GroupBConvertion
        {
            get { return _groupBConvertion; }
        }


        public BlockStats(int blockId)
        {
            _blockId = blockId;
            PositionsConvertion = new Dictionary<int, ConvertionObject>();
            _saveTimer.Elapsed += SaveAndClear;
            _saveTimer.Interval = _updatedTimeSpan.TotalMilliseconds;
            _saveTimer.Enabled = true;
        }

        public void AddClick(int position, long value)
        {
            lock (_blockStatsLock)
            {
                if (!PositionsConvertion.ContainsKey(position))
                {
                    PositionsConvertion[position] = new ConvertionObject();
                }

                PositionsConvertion[position].Clicks++;
                PositionsConvertion[position].Value += value;
            }
        }

        private void SaveAndClear(object source, ElapsedEventArgs e)
        {
            ConvertionObject groupAConvertion;
            ConvertionObject groupBConvertion;
            Dictionary<int, ConvertionObject> tempPositionsConvertion;

            lock (_blockStatsLock)
            {
                tempPositionsConvertion = PositionsConvertion;
                PositionsConvertion = new Dictionary<int, ConvertionObject>();

                groupAConvertion = _groupAConvertion;
                _groupAConvertion.Views = 0;
                _groupAConvertion.Clicks = 0;
                _groupAConvertion.Value = 0;

                groupBConvertion = _groupBConvertion;
                _groupBConvertion.Views = 0;
                _groupBConvertion.Clicks = 0;
                _groupBConvertion.Value = 0;
            }

            string connectionString = "data source=localhost;initial catalog = BlockOptimizationStats;" +
              " persist security info = True;Integrated Security = SSPI";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand(
                           "INSERT INTO [Convertions]([a_views],[a_clicks],[a_value],[b_views],[b_clicks],[b_value],[InsertDate]," +
                           "[BlockId] VALUES (@a_views, @a_clicks, @a_value, @b_views, @b_clicks, @b_value, @InsertDate)");

                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@a_views", groupAConvertion.Views);
                cmd.Parameters.AddWithValue("@a_clicks", groupAConvertion.Clicks);
                cmd.Parameters.AddWithValue("@a_value", groupAConvertion.Value);
                cmd.Parameters.AddWithValue("@b_views", groupBConvertion.Views);
                cmd.Parameters.AddWithValue("@b_clicks", groupBConvertion.Clicks);
                cmd.Parameters.AddWithValue("@b_value", groupBConvertion.Value);
                cmd.Parameters.AddWithValue("@BlockId", _blockId);

                if (StageOptimizer.ChangeTimeSpeed)
                {
                    cmd.Parameters.AddWithValue("@InsertDate", StageOptimizer.CurrentDate);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@InsertDate", DateTime.Now);
                }

                foreach (var dictEntry in tempPositionsConvertion)
                {
                    cmd = new SqlCommand(
                            "INSERT INTO [PositionsStats] (Position, BlockId, Views, Clicks, Value, InsertDate) VALUES (@Position," +
                            "@BlockId, @Views, @Clicks, @Value, @InsertDate)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Position", dictEntry.Key);
                    cmd.Parameters.AddWithValue("@BlockId", _blockId);
                    cmd.Parameters.AddWithValue("@Views", groupBConvertion.Views);
                    cmd.Parameters.AddWithValue("@Clicks", dictEntry.Value.Clicks);
                    cmd.Parameters.AddWithValue("@Value", dictEntry.Value.Value);

                    if (StageOptimizer.ChangeTimeSpeed)
                    {
                        cmd.Parameters.AddWithValue("@InsertDate", StageOptimizer.CurrentDate);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@InsertDate", DateTime.Now);
                    }

                    cmd.ExecuteNonQuery();
                }
            }

        }
    }
}
