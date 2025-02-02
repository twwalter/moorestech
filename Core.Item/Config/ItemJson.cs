﻿using System.Runtime.Serialization;
using Core.Item.Config;

namespace Core.Config.Item
{
    [DataContract]
    internal class ItemJson
    {
        [DataMember(Name = "items")] private ItemConfigData[] _item;

        public ItemConfigData[] Items => _item;
    }
}