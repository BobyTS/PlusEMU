﻿using System.Collections.Generic;
using System.Linq;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Database;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Rooms;
using Dapper;

namespace Plus.Communication.Packets.Incoming.Groups;

internal class RemoveGroupMemberEvent : IPacketEvent
{
    private readonly IGroupManager _groupManager;
    private readonly IRoomManager _roomManager;
    private readonly IDatabase _database;
    private readonly ICacheManager _cacheManager;

    public RemoveGroupMemberEvent(IGroupManager groupManager, IRoomManager roomManager, IDatabase database, ICacheManager cacheManager)
    {
        _groupManager = groupManager;
        _roomManager = roomManager;
        _database = database;
        _cacheManager = cacheManager;
    }

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();
        if (!_groupManager.TryGetGroup(groupId, out var group))
            return;
        if (userId == session.GetHabbo().Id)
        {
            if (group.IsMember(userId))
                group.DeleteMember(userId);
            if (group.IsAdmin(userId))
            {
                if (group.IsAdmin(userId))
                    group.TakeAdmin(userId);
                if (!_roomManager.TryGetRoom(group.RoomId, out var room))
                    return;
                var user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                if (user != null)
                {
                    user.RemoveStatus("flatctrl 1");
                    user.UpdateNeeded = true;
                    if (user.GetClient() != null)
                        user.GetClient().SendPacket(new YouAreControllerComposer(0));
                }
            }
            using (var connection = _database.Connection())
            {
                connection.Execute(
                    "DELETE FROM `group_memberships` WHERE `group_id` = @groupId AND `user_id` = @userId", new { groupId = groupId, userId = userId});
            }
            session.SendPacket(new GroupInfoComposer(group, session));
            if (session.GetHabbo().GetStats().FavouriteGroupId == groupId)
            {
                session.GetHabbo().GetStats().FavouriteGroupId = 0;
                using (var connection = _database.Connection())
                {
                    connection.Execute("UPDATE `user_stats` SET `groupid` = '0' WHERE `id` = @userId LIMIT 1", new { userId = userId });
                }
                if (group.AdminOnlyDeco == 0)
                {
                    if (!_roomManager.TryGetRoom(group.RoomId, out var room))
                        return;
                    var user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                    if (user != null)
                    {
                        user.RemoveStatus("flatctrl 1");
                        user.UpdateNeeded = true;
                        if (user.GetClient() != null)
                            user.GetClient().SendPacket(new YouAreControllerComposer(0));
                    }
                }
                if (session.GetHabbo().InRoom && session.GetHabbo().CurrentRoom != null)
                {
                    var user = session.GetHabbo().CurrentRoom.GetRoomUserManager()
                        .GetRoomUserByHabbo(session.GetHabbo().Id);
                    if (user != null)
                    {
                        session.GetHabbo().CurrentRoom
                            .SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
                    }
                    session.GetHabbo().CurrentRoom
                        .SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
                }
                else
                    session.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
            }
            return;
        }
        if (group.CreatorId == session.GetHabbo().Id || group.IsAdmin(session.GetHabbo().Id))
        {
            if (!group.IsMember(userId))
                return;
            if (group.IsAdmin(userId) && group.CreatorId != session.GetHabbo().Id)
            {
                session.SendNotification(
                    "Sorry, only group creators can remove other administrators from the group.");
                return;
            }
            if (group.IsAdmin(userId))
                group.TakeAdmin(userId);
            if (group.IsMember(userId))
                group.DeleteMember(userId);
            var members = new List<UserCache>();
            var memberIds = group.GetAllMembers;
            foreach (var id in memberIds.ToList())
            {
                var groupMember = _cacheManager.GenerateUser(id);
                if (groupMember == null)
                    continue;
                if (!members.Contains(groupMember))
                    members.Add(groupMember);
            }
            var finishIndex = 14 < members.Count ? 14 : members.Count;
            var membersCount = members.Count;
            session.SendPacket(new GroupMembersComposer(group, members.Take(finishIndex).ToList(), membersCount, 1,
                group.CreatorId == session.GetHabbo().Id || group.IsAdmin(session.GetHabbo().Id), 0, ""));
        }
    }
}