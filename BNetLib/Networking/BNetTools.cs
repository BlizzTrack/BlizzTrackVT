using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BNetLib.Networking
{
    internal class KeyType
    {
        public KeyType(string type, string key)
        {
            Type = type;
            Key = key;
        }

        public string Key { get; }
        public string Type { get; }
    }

    public class BNetTools<T>
    {
        private readonly List<Dictionary<string, dynamic>> _values = new List<Dictionary<string, dynamic>>();
        
        public (T Value, int Seqn) Parse(IEnumerable<string> lines)
        {
            var keys = new List<KeyType>();
            var enumerable = lines as string[] ?? lines.ToArray();
            var keysLine = enumerable.Skip(1).Take(1).First();
            var seqn = 0;

            foreach (var key in keysLine.Split("|"))
            {
                var item = key.Split("!");
                var itemType = item.Last().Split(":").First();

                keys.Add(new KeyType(itemType, item.First()));
            }

            foreach (var line in enumerable.Skip(2))
            {
                if (line.StartsWith("## seqn ="))
                {
                    var f = line.Replace("## seqn =", "").Trim();
                    int.TryParse(f, out var seqn1);
                    seqn = seqn1;
                    continue;
                }

                if(line.Trim().Length == 0 ) continue;

                var values = line.Split('|');

                var lineItem = new Dictionary<string, dynamic>();

                for (var i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    var item = values[i];

                    if (key.Type.Equals("dec", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int.TryParse(item, out var seqn1);
                        lineItem[key.Key] = seqn1;
                        continue;
                    }

                    lineItem[key.Key] = item == "" ? "versions" : item;
                }

                _values.Add(lineItem);
            }

            var ff = JsonConvert.SerializeObject(_values);

            return (JsonConvert.DeserializeObject<T>(ff), seqn);

        }
    }
}
