using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Caching.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CoreLib.Statistics;
using SmartOptimizer;

namespace CoreLib
{
    public class StageOptimizer
    {
        private readonly object _stupidLock = new object();
        private readonly Random _rnd = new Random();
        private const double GroupBRate = 0.8; //0.2
       // private AdsSet _currentAdsSet = new AdsSet(new List<string>());
        readonly MemoryCache _userSessionsCache = new MemoryCache("CachingProvider");
        readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy() {SlidingExpiration = TimeSpan.FromMinutes(10)};
        readonly Dictionary<int, AdsBlock> _adsBlocks = new Dictionary<int, AdsBlock>();
        private const bool StealthMode = false;
        public const bool ChangeTimeSpeed = true;
        private readonly TimeSpan _timeOffset = TimeSpan.FromSeconds(1);
        public static DateTime CurrentDate { get; set; } = DateTime.Now;

        public void AddOrUpdateBlock(int blockId, List<string> refsList)
        {
            if (_adsBlocks.ContainsKey(blockId))
            {
                _adsBlocks[blockId] = new AdsBlock(blockId, refsList);
            }
            else
            {
                _adsBlocks.Add(blockId, new AdsBlock(blockId, refsList));
            }
        }

        public AdsBlock GeAdsBlock(int blockId)
        {
            return _adsBlocks[blockId];
        }

        public List<string> GetDataPositions(int blockId, string userId, string sessionId)
        {
            lock(_stupidLock)
            {
                AdsBlock adsBlock;
                try
                {
                    adsBlock = _adsBlocks[blockId];
                }
                catch (KeyNotFoundException)
                {
                    throw new InvalidOperationException("The is no added block with id: " + blockId);
                }

                bool isInBgroup = false;
                List<string> refsList;
                if (ShouldAddUserToBGroup() && !StealthMode)
                {
                    refsList = adsBlock.NextRandomShuffle().ToList();
                    isInBgroup = true;

                    adsBlock.BlockStats.GroupBConvertion.Views++;
                }
                else
                {
                    refsList = adsBlock.BaseRefsList;
                    adsBlock.BlockStats.GroupAConvertion.Views++;
                }

                var curSession = new UserSession(sessionId,
                     userId,
                     null,
                     isInBgroup,
                     adsBlock,
                     refsList);

                _userSessionsCache.Add(curSession.Id, curSession, _cachePolicy);

                if (ChangeTimeSpeed)
                {
                    CurrentDate += _timeOffset;
                }

                return refsList;
            }
        }

        public void SetSessionResult(string sessionId, string finalLink, int value)
        {
            lock (_stupidLock)
            {
                if (!_userSessionsCache.Contains(sessionId))
                {
                    Trace.TraceWarning("Session with id: {0} not found in the list of started sessions.", sessionId);
                    return;
                }

                UserSession session = (UserSession)_userSessionsCache[sessionId];
                BlockStats blockStats = session.Block.BlockStats;

                _userSessionsCache.Remove(sessionId);

                if (finalLink == null)
                {
                    Trace.TraceWarning("Session with id: {0} ended with null finalLink.", sessionId);
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    return;
                }

                if (session.InBGroup)
                {
                    blockStats.GroupBConvertion.Clicks++;
                    blockStats.GroupBConvertion.Value += value;
                }
                else
                {
                    blockStats.GroupAConvertion.Clicks++;
                    blockStats.GroupAConvertion.Value += value;
                }

                int positionId = session.ShowedList.IndexOf(finalLink);
                blockStats.AddClick(positionId, value);

                if (session.InBGroup)
                {
                    session.Block.ClickOnRef(finalLink, value);
                }
            }
        }

        /// <summary>
        /// Should we add to group with random permutations where we teach
        /// </summary>
        /// <returns></returns>
        private bool ShouldAddUserToBGroup()
        {
            double curVal = _rnd.NextDouble();
            bool result = curVal < GroupBRate;
            return result;
        }


        //private void SaveAndClear(object source, ElapsedEventArgs e)
        //{
        //    string connectionString = "data source=localhost;initial catalog = BlockOptimizationStats;" +
        //                              " persist security info = True;Integrated Security = SSPI";

        //    //Dictionary<int, ConvertionObject> tempPositionsConvertion;
        //    ConvertionObject groupAConvertion;
        //    ConvertionObject groupBConvertion;
        //    Dictionary<int, BlockStats> tempStats;

        //    lock (_stupidLock)
        //    {
        //        tempStats = _blocksStats;
        //        _blocksStats = new Dictionary<int, BlockStats>();

        //        groupAConvertion = _groupAConvertion;
        //        _groupAConvertion.Views = 0;
        //        _groupAConvertion.Clicks = 0;
        //        _groupAConvertion.Value = 0;

        //        groupBConvertion = _groupBConvertion;
        //        _groupBConvertion.Views = 0;
        //        _groupBConvertion.Clicks = 0;
        //        _groupBConvertion.Value = 0;
        //    }

        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //            var cmd = new SqlCommand(
        //                    "INSERT INTO [Convertions]([a_views],[a_clicks],[a_value],[b_views],[b_clicks],[b_value],[InsertDate]," +
        //                    "[BlockId] VALUES (@a_views, @a_clicks, @a_value, @b_views, @b_clicks, @b_value, @InsertDate)");

        //            cmd.CommandType = CommandType.Text;
        //            cmd.Connection = connection;
        //            cmd.Parameters.AddWithValue("@a_views", groupAConvertion.Views);
        //            cmd.Parameters.AddWithValue("@a_clicks", groupAConvertion.Clicks);
        //            cmd.Parameters.AddWithValue("@a_value", groupAConvertion.Value);
        //            cmd.Parameters.AddWithValue("@b_views", groupBConvertion.Views);
        //            cmd.Parameters.AddWithValue("@b_clicks", groupBConvertion.Clicks);
        //            cmd.Parameters.AddWithValue("@b_value", groupBConvertion.Value);
        //            cmd.Parameters.AddWithValue("@BlockId", blockId);

        //        if (StageOptimizer.ChangeTimeSpeed)
        //            {
        //                cmd.Parameters.AddWithValue("@InsertDate", StageOptimizer.CurrentDate);
        //            }
        //            else
        //            {
        //                cmd.Parameters.AddWithValue("@InsertDate", DateTime.Now);
        //            }
        //    }

        //    foreach (var blockStatEntry in tempStats)
        //    {
        //        BlockStats blockStat = blockStatEntry.Value;
        //        int blockId = blockStatEntry.Key;
        //       // AdsBlock block = _adsBlocks[blockId];
        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            foreach (var dictEntry in blockStat.PositionsConvertion)
        //            {
        //                SqlCommand cmd =
        //                    new SqlCommand(
        //                        "INSERT INTO [Convertions] (Position, BlockId, Views, Clicks, Value, InsertDate) VALUES (@Position, @BlockId, @Views, @Clicks, @Value, @InsertDate)");
        //                cmd.CommandType = CommandType.Text;
        //                cmd.Connection = connection;
        //                cmd.Parameters.AddWithValue("@Position", dictEntry.Key);
        //                cmd.Parameters.AddWithValue("@BlockId", blockId);
        //                cmd.Parameters.AddWithValue("@Views", groupBConvertion.Views);
        //                cmd.Parameters.AddWithValue("@Clicks", dictEntry.Value.Clicks);
        //                cmd.Parameters.AddWithValue("@Value", dictEntry.Value.Value);

        //                if (StageOptimizer.ChangeTimeSpeed)
        //                {
        //                    cmd.Parameters.AddWithValue("@InsertDate", StageOptimizer.CurrentDate);
        //                }
        //                else
        //                {
        //                    cmd.Parameters.AddWithValue("@InsertDate", DateTime.Now);
        //                }

        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }

       
    }
}
