﻿using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Catalog;

internal class CatalogOfferComposer : IServerPacket
{
    private readonly CatalogItem _item;
    public int MessageId => ServerPacketHeader.CatalogOfferMessageComposer;

    public CatalogOfferComposer(CatalogItem item)
    {
        _item = item;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInteger(_item.OfferId);
        packet.WriteString(_item.Data.ItemName);
        packet.WriteBoolean(false); //IsRentable
        packet.WriteInteger(_item.CostCredits);
        if (_item.CostDiamonds > 0)
        {
            packet.WriteInteger(_item.CostDiamonds);
            packet.WriteInteger(5); // Diamonds
        }
        else
        {
            packet.WriteInteger(_item.CostPixels);
            packet.WriteInteger(0); // Type of PixelCost
        }
        packet.WriteBoolean(ItemUtility.CanGiftItem(_item));
        packet.WriteInteger(string.IsNullOrEmpty(_item.Badge) ? 1 : 2); //Count 1 item if there is no badge, otherwise count as 2.
        if (!string.IsNullOrEmpty(_item.Badge))
        {
            packet.WriteString("b");
            packet.WriteString(_item.Badge);
        }
        packet.WriteString(_item.Data.Type.ToString());
        if (_item.Data.Type.ToString().ToLower() == "b")
            packet.WriteString(_item.Data.ItemName); //Badge name.
        else
        {
            packet.WriteInteger(_item.Data.SpriteId);
            if (_item.Data.InteractionType == InteractionType.Wallpaper || _item.Data.InteractionType == InteractionType.Floor || _item.Data.InteractionType == InteractionType.Landscape)
                packet.WriteString(_item.Name.Split('_')[2]);

            // TODO @80O: Dont make this static hardcoded page 9
            else if (_item.PageId == 9) //Bots
            {
                CatalogBot cataBot = null;
                if (!PlusEnvironment.GetGame().GetCatalog().TryGetBot(_item.ItemId, out cataBot))
                    packet.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                else
                    packet.WriteString(cataBot.Figure);
            }
            else if (_item.ExtraData != null)
                packet.WriteString(_item.ExtraData != null ? _item.ExtraData : string.Empty);
            packet.WriteInteger(_item.Amount);
            packet.WriteBoolean(_item.IsLimited); // IsLimited
            if (_item.IsLimited)
            {
                packet.WriteInteger(_item.LimitedEditionStack);
                packet.WriteInteger(_item.LimitedEditionStack - _item.LimitedEditionSells);
            }
        }
        packet.WriteInteger(0); // club_level
        packet.WriteBoolean(ItemUtility.CanSelectAmount(_item));
        packet.WriteBoolean(false); // TODO: Figure out
        packet.WriteString(""); //previewImage -> e.g; catalogue/pet_lion.png
    }
}