﻿using System;
using System.Collections.Generic;
using Core.Block.BlockFactory;
using Core.Block.RecipeConfig;
using Core.Item;
using Game.World.Interface.DataStore;
using Game.World.Interface.Util;
using Microsoft.Extensions.DependencyInjection;
using Server.Util;

namespace Server.Protocol.PacketResponse
{
    public class PutBlockProtocol : IPacketResponse
    {
        private readonly IWorldBlockDatastore _worldBlockDatastore;
        private readonly BlockFactory blockFactory;
        private readonly IMachineRecipeConfig _recipeConfig;
        private readonly ItemStackFactory _itemStackFactory;

        public PutBlockProtocol(ServiceProvider serviceProvider)
        {
            _worldBlockDatastore = serviceProvider.GetService<IWorldBlockDatastore>();
            blockFactory = serviceProvider.GetService<BlockFactory>();
            _recipeConfig = serviceProvider.GetService<IMachineRecipeConfig>();
            _itemStackFactory = serviceProvider.GetService<ItemStackFactory>();
        }

        public List<byte[]> GetResponse(List<byte> payload)
        {
            //パケットのパース、接続元、接続先のインスタンス取得
            var payloadData = new ByteArrayEnumerator(payload);
            payloadData.MoveNextToGetShort();
            int blockId = payloadData.MoveNextToGetInt();
            payloadData.MoveNextToGetShort();
            int x = payloadData.MoveNextToGetInt();
            int y = payloadData.MoveNextToGetInt();
            Console.WriteLine("Place Block blockID:" + blockId + " x:" + x + " y:" + y);

            var block = blockFactory.Create(blockId, IntId.NewIntId());

            //TODO このプロトコルは確定で北向きに設置する　今後向にも対応させる
            _worldBlockDatastore.AddBlock(block, x, y, BlockDirection.North);
            //返すものはない
            return new List<byte[]>();
        }
    }
}