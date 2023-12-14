using AF;
using charles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveDiggerConversationManager : GeneralConversationManager
{
    public override void DoEffect()
    {
        SoundManager.Instance.PlayFxClip(4);
        MapManager.Instance.UnlockDoor();

        DisplayConversationMessage(1);
    }
}
