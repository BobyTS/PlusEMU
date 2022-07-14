﻿using System.Collections.Concurrent;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Wired.Boxes;

internal class AddonRandomEffectBox : IWiredItem
{
    public AddonRandomEffectBox(Room instance, Item item)
    {
        Instance = instance;
        Item = item;
        SetItems = new ConcurrentDictionary<int, Item>();
        if (SetItems.Count > 0)
            SetItems.Clear();
    }

    public Room Instance { get; set; }
    public Item Item { get; set; }
    public WiredBoxType Type => WiredBoxType.AddonRandomEffect;
    public ConcurrentDictionary<int, Item> SetItems { get; set; }
    public string StringData { get; set; }
    public bool BoolData { get; set; }
    public string ItemsData { get; set; }

    public void HandleSave(IIncomingPacket packet) { }

    public bool Execute(params object[] @params) => true;
}