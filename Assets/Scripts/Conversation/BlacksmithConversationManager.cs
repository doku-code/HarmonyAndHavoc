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
}
