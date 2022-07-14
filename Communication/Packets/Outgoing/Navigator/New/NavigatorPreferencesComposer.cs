﻿using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Navigator.New;

// TODO @80O: Implement
internal class NavigatorPreferencesComposer : IServerPacket
{
    public int MessageId => ServerPacketHeader.NavigatorPreferencesMessageComposer;

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInteger(68); //X
        packet.WriteInteger(42); //Y
        packet.WriteInteger(425); //Width
        packet.WriteInteger(592); //Height
        packet.WriteBoolean(false); //Show or hide saved searches.
        packet.WriteInteger(0); //No idea?
    }
}