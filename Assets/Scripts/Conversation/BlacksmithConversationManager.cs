using charles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithConversationManager : MerchantConversationManager
{
    public override void ItemEffect(int itemIndex)
    {
        switch(itemIndex)
        {
            case 0:
                pData.WeaponUpgrade += 1;

                break;

            case 1:

                pData.ArmorUpgrade += 1;
                break;
        }
    }

    public override bool CheckIfFull()
    {
        return false;
    }

    public override int GetItemInflatedPrice(int itemIndex)
    {
        switch (itemIndex)
        {
            case 0:
                return Items[itemIndex].price + pData.WeaponUpgrade * priceIncrease;

            case 1:
                return Items[itemIndex].price + pData.ArmorUpgrade * priceIncrease;
        }

        return 0;
    }
}
