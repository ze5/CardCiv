using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffect : MonoBehaviour
{
    public abstract void DoEffect(Player player = null, GameObject source = null, GameObject target = null);

}
