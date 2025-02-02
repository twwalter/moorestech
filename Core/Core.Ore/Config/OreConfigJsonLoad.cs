using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Core.Ore.Config
{
    public class OreConfigJsonLoad
    {
        public Dictionary<int, OreConfigDataElement> Load()
        {
            //JSONをロードする
            var json = File.ReadAllText(ConfigPath.ConfigPath.OreConfigPath);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes((json)));
            ms.Seek(0, SeekOrigin.Begin);
            var serializer = new DataContractJsonSerializer(typeof(OreConfigData));
            var data = serializer.ReadObject(ms) as OreConfigData;

            //Dictionaryに変換する
            var dic = new Dictionary<int, OreConfigDataElement>();
            foreach (var item in data.OreElements)
            {
                dic.Add(item.OreId, item);
            }

            return dic;
        }
    }
}