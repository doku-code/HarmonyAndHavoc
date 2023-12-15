using charles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerConversationManager : MerchantConversationManager
{
    public override void ItemEffect(int itemIndex)
    {
        pData.HealPlayer(pData.MaxOrder);
    }

    public override bool CheckIfFull()
    {
        return pData.ActualOrder == pData.MaxOrder;
    }

    public override int GetItemInflatedPrice(int itemIndex)
    {
        return pData.TotalHeals * priceIncrease + Items[0].price;
    }
}