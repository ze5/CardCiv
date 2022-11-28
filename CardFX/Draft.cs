using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draft : CardEffect
{
    public int CardsToDraft = 1;
    public override void DoEffect(Player player = null, GameObject source = null, GameObject target = null)
    {
        player.Drafts += CardsToDraft;
    }
}
