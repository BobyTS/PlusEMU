﻿using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Inventory.Trading;

internal class TradingAcceptComposer : IServerPacket
{
    private readonly int _userId;
    private readonly bool _accept;
    public int MessageId => ServerPacketHeader.TradingAcceptMessageComposer;

    public TradingAcceptComposer(int userId, bool accept)
    {
        _userId = userId;
        _accept = accept;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInteger(_userId);
        packet.WriteInteger(_accept ? 1 : 0);
    }
}