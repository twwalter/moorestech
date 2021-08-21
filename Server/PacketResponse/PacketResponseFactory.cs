﻿using System;
using System.Collections.Generic;
using System.Text;
using industrialization.Server.PacketResponse.ProtocolImplementation;
using industrialization.Server.Util;

namespace industrialization.Server.PacketResponse
{
    public static class PacketResponseFactory
    {
        delegate List<byte[]> Responses(byte[] payload);
        private static List<Responses> _packetResponseList = new List<Responses>();

        private static void Init()
        {
            _packetResponseList.Add(DummyProtocol.GetResponse);
            _packetResponseList.Add(PutBlockProtocol.GetResponse);
            _packetResponseList.Add(BlockCoordinateRequestProtocolResponse.GetResponse);
            _packetResponseList.Add(InventoryContentResponseProtocol.GetResponse);
        }
        
        public static List<byte[]> GetPacketResponse(byte[] payload)
        {
            if (_packetResponseList.Count == 0) Init();

            return _packetResponseList[new ByteArrayEnumerator(payload).MoveNextToGetShort()](payload);
        }
    }
}