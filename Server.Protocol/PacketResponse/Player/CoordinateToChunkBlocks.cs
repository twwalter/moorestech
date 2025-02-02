﻿using Game.World.Interface;
using Game.World.Interface.DataStore;
using Server.Protocol.PacketResponse.Const;

namespace Server.PacketHandle.PacketResponse.Player
{
    public static class CoordinateToChunkBlocks
    {
        public static int[,] Convert(Coordinate coordinate, IWorldBlockDatastore worldBlockDatastore)
        {
            //その座標のチャンクの原点
            var x = coordinate.X / ChunkResponseConst.ChunkSize * ChunkResponseConst.ChunkSize;
            var y = coordinate.Y / ChunkResponseConst.ChunkSize * ChunkResponseConst.ChunkSize;

            var blocks = new int[ChunkResponseConst.ChunkSize, ChunkResponseConst.ChunkSize];

            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    blocks[i, j] = worldBlockDatastore.GetBlock(
                        x + i,
                        y + j).GetBlockId();
                }
            }

            return blocks;
        }
    }
}