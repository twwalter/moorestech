using System;
using System.Collections.Generic;
using Core.Block.BlockInventory;
using Core.Block.Config.LoadConfig.Param;
using Core.Electric;
using Core.Inventory;
using Core.Item;
using Core.Item.Util;
using Core.Ore;
using Core.Update;

namespace Core.Block.Blocks.Miner
{
    public class VanillaMiner : IBlock, IBlockElectric, IBlockInventory, IUpdate
    {
        private readonly int _blockId;
        private readonly int _intId;
        private readonly int _requestPower;
        private readonly int _miningTime;
        private readonly int _miningItemId;
        private readonly ItemStackFactory _itemStackFactory;
        private readonly List<IBlockInventory> _connectInventory = new();
        private readonly List<IItemStack> _outputSlot;

        private int _nowPower = 0;
        private double _remainingMillSecond;

        public VanillaMiner(int blockId, int intId, int requestPower, int miningItemId, int miningTime, int outputSlot,
            ItemStackFactory itemStackFactory)
        {
            _blockId = blockId;
            _intId = intId;
            _requestPower = requestPower;
            _miningItemId = miningItemId;
            _miningTime = miningTime;
            _remainingMillSecond = miningTime;
            _itemStackFactory = itemStackFactory;
            GameUpdate.AddUpdateObject(this);
            _outputSlot = CreateEmptyItemStacksList.Create(outputSlot, itemStackFactory);
        }

        public void Update()
        {
            _remainingMillSecond -= GameUpdate.UpdateTime * (_nowPower / (double) _requestPower);

            if (_remainingMillSecond <= 0)
            {
                _remainingMillSecond = _miningTime;

                //空きスロットを探索し、あるならアイテムを挿入
                var addItem = _itemStackFactory.Create(_miningItemId, 1);
                for (int i = 0; i < _outputSlot.Count; i++)
                {
                    if (!_outputSlot[i].IsAllowedToAdd(addItem)) continue;
                    //空きスロットに掘ったアイテムを挿入
                    _outputSlot[i] = _outputSlot[i].AddItem(addItem).ProcessResultItemStack;
                    break;
                }
            }

            _nowPower = 0;
            InsertConnectInventory();
        }

        //TODO 複数アウトプットスロットがあるときは優先順位を順番に駆るロジックを組む
        void InsertConnectInventory()
        {
            for (int i = 0; i < _outputSlot.Count; i++)
            {
                foreach (var connect in _connectInventory)
                {
                    _outputSlot[i] = connect.InsertItem(_outputSlot[i]);
                }
            }
        }

        public string GetSaveState()
        {
            throw new System.NotImplementedException();
        }

        public void AddOutputConnector(IBlockInventory blockInventory)
        {
            _connectInventory.Add(blockInventory);
        }

        public void RemoveOutputConnector(IBlockInventory blockInventory)
        {
            _connectInventory.Remove(blockInventory);
        }

        public IItemStack InsertItem(IItemStack itemStack)
        {
            return itemStack;
        }

        public int GetIntId()
        {
            return _intId;
        }

        public int GetBlockId()
        {
            return _blockId;
        }

        public int GetRequestPower()
        {
            return _requestPower;
        }

        public void SupplyPower(int power)
        {
            _nowPower = power;
        }
    }
}