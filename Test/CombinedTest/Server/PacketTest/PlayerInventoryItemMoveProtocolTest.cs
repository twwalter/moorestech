using System.Collections.Generic;
using Core.Item;
using Core.Item.Util;
using Game.PlayerInventory.Interface;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PlayerInventory;
using Server;
using Server.Util;

namespace Test.CombinedTest.Server.PacketTest
{
    public class PlayerInventoryItemMoveProtocolTest
    {
        [Test]
        public void InventoryItemMove()
        {
            int playerId = 1;
            int playerSlotIndex = 2;
            int blockInventorySlotIndex = 0;

            //初期設定----------------------------------------------------------

            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var itemStackFactory = serviceProvider.GetService<ItemStackFactory>();

            //プレイヤーのインベントリの設定
            var payload = new List<byte>();
            payload.AddRange(ToByteList.Convert((short) 3));
            payload.AddRange(ToByteList.Convert(playerId));
            packet.GetPacketResponse(payload);
            var playerInventoryData =
                serviceProvider.GetService<IPlayerInventoryDataStore>().GetInventoryData(playerId);


            //アイテムの設定
            playerInventoryData.SetItem(0, itemStackFactory.Create(1, 5));
            playerInventoryData.SetItem(1, itemStackFactory.Create(1, 1));
            playerInventoryData.SetItem(2, itemStackFactory.Create(2, 1));

            //実際に移動させてテスト
            //全てのアイテムを移動させるテスト
            packet.GetPacketResponse(PlayerInventoryItemMove(0, 3, 5, playerId));
            Assert.AreEqual(playerInventoryData.GetItem(0), itemStackFactory.CreatEmpty());
            Assert.AreEqual(playerInventoryData.GetItem(3), itemStackFactory.Create(1, 5));

            //一部のアイテムを移動させるテスト
            packet.GetPacketResponse(PlayerInventoryItemMove(3, 0, 3, playerId));
            Assert.AreEqual(playerInventoryData.GetItem(0), itemStackFactory.Create(1, 3));
            Assert.AreEqual(playerInventoryData.GetItem(3), itemStackFactory.Create(1, 2));

            //一部のアイテムを移動しようとするが他にスロットがあるため失敗するテスト
            packet.GetPacketResponse(PlayerInventoryItemMove(0, 2, 1, playerId));
            Assert.AreEqual(playerInventoryData.GetItem(0), itemStackFactory.Create(1, 3));
            Assert.AreEqual(playerInventoryData.GetItem(2), itemStackFactory.Create(2, 1));

            //全てのアイテムを移動させるテスト
            packet.GetPacketResponse(PlayerInventoryItemMove(0, 2, 3, playerId));
            Assert.AreEqual(playerInventoryData.GetItem(0), itemStackFactory.Create(2, 1));
            Assert.AreEqual(playerInventoryData.GetItem(2), itemStackFactory.Create(1, 3));

            //アイテムを加算するテスト
            packet.GetPacketResponse(PlayerInventoryItemMove(2, 1, 3, playerId));
            Assert.AreEqual(playerInventoryData.GetItem(2), itemStackFactory.CreatEmpty());
            Assert.AreEqual(playerInventoryData.GetItem(1), itemStackFactory.Create(1, 4));
        }

        private List<byte> PlayerInventoryItemMove(int fromSlot, int toSlot, int itemCount, int playerId)
        {
            var payload = new List<byte>();
            payload.AddRange(ToByteList.Convert((short) 6));
            payload.AddRange(ToByteList.Convert(playerId));
            payload.AddRange(ToByteList.Convert(fromSlot));
            payload.AddRange(ToByteList.Convert(toSlot));
            payload.AddRange(ToByteList.Convert(itemCount));
            return payload;
        }
    }
}