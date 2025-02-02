﻿using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Core.Config.Item;
using static System.Int32;

namespace Core.Item.Config
{
    public class TestItemConfig : IItemConfig
    {
        private ItemConfigData[] _itemDatas;

        public TestItemConfig()
        {
            var json = File.ReadAllText(ConfigPath.ConfigPath.ItemConfigPath);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes((json)));
            ms.Seek(0, SeekOrigin.Begin);
            var serializer = new DataContractJsonSerializer(typeof(ItemJson));
            var data = serializer.ReadObject(ms) as ItemJson;
            _itemDatas = data.Items;
        }

        public ItemConfigData GetItemConfig(int id)
        {
            //アイテムが登録されてないときの仮
            if (_itemDatas.Length - 1 < id)
            {
                return new ItemConfigData("undefined id " + id, id, MaxValue);
            }

            return _itemDatas[id];
        }
    }

    [DataContract]
    public class ItemConfigData
    {
        [DataMember(Name = "name")] private string _name;
        [DataMember(Name = "id")] private int _id;
        [DataMember(Name = "stacks")] private int _stack;

        public ItemConfigData(string name, int id, int stack)
        {
            _name = name;
            _id = id;
            _stack = stack;
        }

        public string Name => _name;
        public int Id => _id;
        public int Stack => _stack;
    }
}