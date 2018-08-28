using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Motion
{
    public class Config
    {
        private const string ConfigurationFileName = @"config.json";
        private static Dictionary<string, string> Values;

        static Config() {
            if (File.Exists(ConfigurationFileName)) {
                Values = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ConfigurationFileName));
            }
        }


        public static string Get(string key)
        {
            if (!Values.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return Values[key];
            }
        }
    }
}
