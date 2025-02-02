﻿using System;
using System.Collections.Generic;
using Core.Block.BlockFactory;
using Core.Block.Blocks;
using Game.World.Interface.DataStore;
using Game.World.Interface.Event;
using World.Event;

namespace World.DataStore
{
    /// <summary>
    /// ワールドに存在するブロックとその座標の対応づけを行います。
    /// </summary>
    public class WorldBlockDatastore : IWorldBlockDatastore
    {
        //メインのデータストア
        private readonly Dictionary<int, WorldBlockData> _blockMasterDictionary = new();

        //座標とキーの紐づけ
        private readonly Dictionary<Coordinate, int> _coordinateDictionary = new();


        readonly IBlock _nullBlock = new NullBlock();
        private readonly BlockPlaceEvent _blockPlaceEvent;
        private readonly BlockRemoveEvent _blockRemoveEvent;
        private readonly BlockFactory _blockFactory;

        public WorldBlockDatastore(IBlockPlaceEvent blockPlaceEvent, BlockFactory blockFactory,
            IBlockRemoveEvent blockRemoveEvent)
        {
            _blockFactory = blockFactory;
            _blockRemoveEvent = (BlockRemoveEvent) blockRemoveEvent;
            _blockPlaceEvent = (BlockPlaceEvent) blockPlaceEvent;
        }

        public bool AddBlock(IBlock block, int x, int y, BlockDirection blockDirection)
        {
            //既にキーが登録されてないか、同じ座標にブロックを置こうとしてないかをチェック
            if (!_blockMasterDictionary.ContainsKey(block.GetIntId()) &&
                !_coordinateDictionary.ContainsKey(new Coordinate(x, y)))
            {
                var c = new Coordinate(x, y);
                var data = new WorldBlockData(block, x, y, blockDirection);
                _blockMasterDictionary.Add(block.GetIntId(), data);
                _coordinateDictionary.Add(c, block.GetIntId());
                _blockPlaceEvent.OnBlockPlaceEventInvoke(new BlockPlaceEventProperties(c, data.Block, blockDirection));

                return true;
            }

            return false;
        }

        public bool RemoveBlock(int x, int y)
        {
            var intId = GetBlockId(x, y);
            if (!_blockMasterDictionary.ContainsKey(intId)) return false;

            var data = _blockMasterDictionary[intId];

            _blockRemoveEvent.OnBlockRemoveEventInvoke(new BlockRemoveEventProperties(
                new Coordinate(x, y), data.Block));

            _blockMasterDictionary.Remove(intId);
            _coordinateDictionary.Remove(new Coordinate(x, y));
            return true;
        }


        public IBlock GetBlock(int x, int y)
        {
            var c = new Coordinate(x, y);
            if (_coordinateDictionary.ContainsKey(c)) return _blockMasterDictionary[_coordinateDictionary[c]].Block;
            return _nullBlock;
        }

        public (int, int) GetBlockPosition(int intId)
        {
            if (_blockMasterDictionary.ContainsKey(intId))
            {
                var data = _blockMasterDictionary[intId];
                return (data.X, data.Y);
            }

            throw new Exception("ブロックがありません");
        }

        public BlockDirection GetBlockDirection(int x, int y)
        {
            var c = new Coordinate(x, y);
            if (_coordinateDictionary.ContainsKey(c))
                return _blockMasterDictionary[_coordinateDictionary[c]].BlockDirection;
            return BlockDirection.North;
        }

        public List<SaveBlockData> GetSaveBlockDataList()
        {
            var list = new List<SaveBlockData>();
            foreach (var block in _blockMasterDictionary)
            {
                list.Add(new SaveBlockData(
                    block.Value.X,
                    block.Value.Y,
                    block.Value.Block.GetBlockId(),
                    block.Value.Block.GetIntId(),
                    block.Value.Block.GetSaveState(),
                    (int) block.Value.BlockDirection));
            }

            return list;
        }

        public void LoadBlockDataList(List<SaveBlockData> saveBlockDataList)
        {
            foreach (var block in saveBlockDataList)
            {
                AddBlock(
                    _blockFactory.Load(block.BlockId, block.IntId, block.State),
                    block.X,
                    block.Y,
                    (BlockDirection) block.Direction);
            }
        }
        
        public bool Exists(int x, int y) { return GetBlock(x, y).GetBlockId() != BlockConst.BlockConst.EmptyBlockId; }
        private int GetBlockId(int x, int y) { return _coordinateDictionary[new Coordinate(x, y)]; }
    }
}