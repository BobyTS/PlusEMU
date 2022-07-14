﻿using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine;

internal class RoomVisualizationSettingsComposer : IServerPacket
{
    private readonly int _walls;
    private readonly int _floor;
    private readonly bool _hideWalls;

    public int MessageId => ServerPacketHeader.RoomVisualizationSettingsMessageComposer;

    public RoomVisualizationSettingsComposer(int walls, int floor, bool hideWalls)
    {
        _walls = walls;
        _floor = floor;
        _hideWalls = hideWalls;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteBoolean(_hideWalls);
        packet.WriteInteger(_walls);
        packet.WriteInteger(_floor);
    }
}