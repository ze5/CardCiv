using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyFarms : CardValidTargets
{
    public override bool Evalute(GameObject target)
    {
        //This just checks if the target is a building and is a farm
        Building building;
        if (target.TryGetComponent<Building>(out building))
        {
            for (int i = 0; i < building.Cards.Count; i++)
            {
                if (building.AllCards.Cards[building.Cards[i]].FoodPTurn > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
