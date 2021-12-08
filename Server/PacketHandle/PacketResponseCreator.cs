﻿using System.Collections.Generic;
using Core.Block.Config;
using Microsoft.Extensions.DependencyInjection;
using PlayerInventory;
using Server.Event;
using Server.PacketHandle.PacketResponse;
using Server.Util;
using World;

namespace Server.PacketHandle
{
    public class PacketResponseCreator
    {
        private List<IPacketResponse> _packetResponseList;


        public PacketResponseCreator(ServiceProvider serviceProvider)
        {
            _packetResponseList = new List<IPacketResponse>();
            _packetResponseList.Add(new DummyProtocol());
            _packetResponseList.Add(new PutBlockProtocol(serviceProvider));
            _packetResponseList.Add(new PlayerCoordinateSendProtocol(serviceProvider.GetService<WorldBlockDatastore>()));
            _packetResponseList.Add(new PlayerInventoryResponseProtocol(serviceProvider.GetService<PlayerInventoryDataStore>()));
            _packetResponseList.Add(new SendEventProtocol(serviceProvider.GetService<EventProtocolProvider>()));
            _packetResponseList.Add(new BlockInventoryPlayerInventoryMoveItemProtocol(serviceProvider));
            _packetResponseList.Add(new PlayerInventoryMoveItemProtocol(serviceProvider));
        }

        public List<byte[]> GetPacketResponse(List<byte> payload)
        {
            return _packetResponseList[new ByteArrayEnumerator(payload).MoveNextToGetShort()].GetResponse(payload);
        }
    }
}