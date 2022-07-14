﻿using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine;

internal class ItemsComposer : IServerPacket
{
    private readonly Item[] _objects;
    private readonly Room _room;

    public int MessageId => ServerPacketHeader.ItemsMessageComposer;

    public ItemsComposer(Item[] objects, Room room)
    {
        _objects = objects;
        _room = room;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInteger(1);
        packet.WriteInteger(_room.OwnerId);
        packet.WriteString(_room.OwnerName);
        packet.WriteInteger(_objects.Length);
        foreach (var item in _objects) WriteWallItem(packet, item, _room.OwnerId);
    }

    private void WriteWallItem(IOutgoingPacket packet, Item item, int userId)
    {
        packet.WriteString(item.Id.ToString());
        packet.WriteInteger(item.Data.SpriteId);
        try
        {
            packet.WriteString(item.WallCoord);
        }
        catch
        {
            packet.WriteString("");
        }
        ItemBehaviourUtility.GenerateWallExtradata(item, packet);
        packet.WriteInteger(-1);
        packet.WriteInteger(item.Data.Modes > 1 ? 1 : 0);
        packet.WriteInteger(userId);
    }
}