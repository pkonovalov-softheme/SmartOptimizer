using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Timers;

namespace CoreLib.Statistics
{
    /// <summary>
    /// Class which contains block statistics info
    /// </summary>
    public sealed class BlockPositionStats
    {
        const string ConnectionString = "data source=localhost;initial catalog = BlockOptimizationStats; persist security info = True;Integrated Security = SSPI";

        private ConvertionObject _groupAConvertion = new ConvertionObject();
        private ConvertionObject _groupBConvertion = new ConvertionObject();
        private readonly int _blockId;
        private readonly object _blockStatsLock = new object();
        private readonly TimeSpan _updatedTimeSpan = TimeSpan.FromSeconds(5);
        private readonly Timer _saveTimer = new Timer();
        private static readonly SqlConnection Connection = new SqlConnection(ConnectionString);

        /// <summary>
        /// ConvertionObject(value) for specific position(key)
        /// </summary>
        public Dictionary<int, ConvertionObject> PositionsConvertion { get; private set; }
        
        public ConvertionObject GroupAConvertion
        {
            get { return _groupAConvertion; }
            set { _groupAConvertion = value; }
        }

        public ConvertionObject GroupBConvertion
        {
            get { return _groupBConvertion; }
            set { _groupBConvertion = value; }
        }

        public int BlockId
        {
            get { return _blockId; }
        }


        public BlockPositionStats(int blockId)
        {
            _blockId = blockId;
            PositionsConvertion = new Dictionary<int, ConvertionObject>();
            _saveTimer.Elapsed += SaveAndClear;
            _saveTimer.Interval = _updatedTimeSpan.TotalMilliseconds;
            _saveTimer.Enabled = true;

            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    Utils.DbgBreak();
                    Trace.TraceError("Open connection failed with exception: {0}", ex.Message);
                }
            }
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
            try
            {
                ConvertionObject groupAConvertion;
                ConvertionObject groupBConvertion;
                Dictionary<int, ConvertionObject> tempPositionsConvertion;

                lock (_blockStatsLock)
                {
                    tempPositionsConvertion = PositionsConvertion;
                    PositionsConvertion = new Dictionary<int, ConvertionObject>();

                    groupAConvertion = _groupAConvertion;
                    _groupAConvertion = new ConvertionObject();

                    groupBConvertion = _groupBConvertion;
                    _groupBConvertion = new ConvertionObject();
                }

                Trace.TraceInformation("Saving stats to SQL DB...");


                var cmd = new SqlCommand(
                            "INSERT INTO [Convertions]([AGviews],[AGclicks],[AGvalue],[BGviews],[BGclicks],[BGvalue],[InsertDate]," +
                            "[BlockId]) VALUES (@AGviews, @AGclicks, @AGvalue, @BGviews, @BGclicks, @BGvalue, @InsertDate, @BlockId)");

                cmd.CommandType = CommandType.Text;
                cmd.Connection = Connection;
                cmd.Parameters.AddWithValue("@AGviews", groupAConvertion.Views);
                cmd.Parameters.AddWithValue("@AGclicks", groupAConvertion.Clicks);
                cmd.Parameters.AddWithValue("@AGvalue", groupAConvertion.Value);
                cmd.Parameters.AddWithValue("@BGviews", groupBConvertion.Views);
                cmd.Parameters.AddWithValue("@BGclicks", groupBConvertion.Clicks);
                cmd.Parameters.AddWithValue("@BGvalue", groupBConvertion.Value);
                cmd.Parameters.AddWithValue("@BlockId", _blockId);

                if (groupBConvertion.Views == 0)
                {
                    Utils.DbgBreak();
                }
                if (StageOptimizer.ChangeTimeSpeed)
                {
                    cmd.Parameters.AddWithValue("@InsertDate", StageOptimizer.CurrentDate);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@InsertDate", DateTime.Now);
                }

                cmd.ExecuteNonQuery();

                foreach (var dictEntry in tempPositionsConvertion)
                {
                    cmd = new SqlCommand(
                            "INSERT INTO [PositionsStats] (Position, BlockId, Views, Clicks, Value, InsertDate) VALUES (@Position," +
                            "@BlockId, @Views, @Clicks, @Value, @InsertDate)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = Connection;
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
            catch (Exception ex)
            {
                Utils.DbgBreak();
                Trace.TraceError("SaveAndClear failed with exception: {0}", ex.Message);
            }
        }
    }
}
