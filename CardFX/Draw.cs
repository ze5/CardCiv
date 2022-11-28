using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : CardEffect
{
    public int CardsToDraw = 1;
    public override void DoEffect(Player player = null, GameObject source = null, GameObject target = null)
    {
        player.MyHand.DrawX(CardsToDraw);
    }
}
