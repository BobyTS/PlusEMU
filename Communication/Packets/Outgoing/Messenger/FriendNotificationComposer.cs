﻿using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger;

internal class FriendNotificationComposer : IServerPacket
{
    private readonly int _userId;
    private readonly MessengerEventTypes _type;
    private readonly string _data;
    public int MessageId => ServerPacketHeader.FriendNotificationMessageComposer;

    public FriendNotificationComposer(int userId, MessengerEventTypes type, string data)
    {
        _userId = userId;
        _type = type;
        _data = data;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteString(_userId.ToString());
        packet.WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(_type));
        packet.WriteString(_data);
    }
}