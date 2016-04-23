using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public static class Utils
    {
        private static readonly ThreadSafeRandom Rng = new ThreadSafeRandom();

        public static void DbgBreak()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        public static T DeserializeJSON<T>(string json)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                return (T)deserializer.ReadObject(ms);
            }
        }

        public static string ToJson<T>(T data)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, data);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        public static IEnumerable<string> Shuffle(IEnumerable<string> toShuffle)
        {
            return toShuffle.OrderBy(x => Rng.Next());
        }
    }
}
