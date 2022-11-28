using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : CardEffect
{
    public int Amount = 1;
    public bool Blockable = false;
    public bool AoE = false;
    public bool AllArmys = false;
    public bool AllBuildings = false;
    public bool TargetAllPlayers = false;
    public override void DoEffect(Player player = null, GameObject source = null, GameObject target = null)
    {
        Building TargetBuilding;
        Army TargetArmy;
        Player TargetPlayer = null;
        if (target != null)
        {
            // only damage the target
            if (!AllArmys && !AllBuildings && !TargetAllPlayers)
            {
                if (target.TryGetComponent<Building>(out TargetBuilding))
                {
                    TargetBuilding.Hurt(Amount, Blockable, AoE);
                }
                else if (target.TryGetComponent<Army>(out TargetArmy))
                {
                    TargetArmy.Hurt(Amount, 0, AoE);
                }
            }
            if (!TargetAllPlayers)
            {
                if (AllArmys)
                {
                    if (target.TryGetComponent<Building>(out TargetBuilding))
                    {
                        TargetPlayer = TargetBuilding.Owner;
                        hurtArmys(TargetPlayer);
                    }
                    else if (target.TryGetComponent<Army>(out TargetArmy))
                    {
                        TargetPlayer = TargetArmy.parent.Owner;
                        hurtArmys(TargetPlayer);
                    }
                    else if (target.TryGetComponent<Player>(out TargetPlayer))
                    {
                        hurtArmys(TargetPlayer);
                    }
                }
                if (AllBuildings)
                {
                    if (target.TryGetComponent<Building>(out TargetBuilding))
                    {
                        TargetPlayer = TargetBuilding.Owner;
                        hurtBuildings(TargetPlayer);
                    }
                    else if (target.TryGetComponent<Army>(out TargetArmy))
                    {
                        TargetPlayer = TargetArmy.parent.Owner;
                        hurtBuildings(TargetPlayer);
                    }
                    else if (target.TryGetComponent<Player>(out TargetPlayer))
                    {
                        hurtBuildings(TargetPlayer);
                    }
                }
            }
        }
        if (TargetAllPlayers)
        {
            if (player != null)
            {
                foreach (Player Target in player.transform.parent.GetComponentsInChildren<Player>())
                {
                    if (AllBuildings)
                    {
                        hurtBuildings(Target);
                    }
                    if (AllArmys)
                    {
                        hurtArmys(Target);
                    }
                }
            }
            else if (target != null)
            {
                // really this shouldnt happen, but just incase?
                if (target.TryGetComponent<Building>(out TargetBuilding))
                {
                    TargetPlayer = TargetBuilding.Owner;

                }
                else if (target.TryGetComponent<Army>(out TargetArmy))
                {
                    TargetPlayer = TargetArmy.parent.Owner;

                }
                else 
                {
                    if (!target.TryGetComponent<Player>(out TargetPlayer))
                    {
                        //dumbest last ditch effort. REALLY Shouldnt come to this.
                        TargetPlayer = GameObject.Find("Players").GetComponentInChildren<Player>();
                    }
                }
                
            }
            else
            {
                //no target or player passed
                TargetPlayer = GameObject.Find("Players").GetComponentInChildren<Player>();
            }
            if (TargetPlayer != null)
            {
                foreach (Player Target in TargetPlayer.transform.parent.GetComponentsInChildren<Player>())
                {
                    if (AllBuildings)
                    {
                        hurtBuildings(Target);
                    }
                    if (AllArmys)
                    {
                        hurtArmys(Target);
                    }
                }
            }
        }
    }
    public void hurtBuildings(Player TargetPlayer)
    {
        foreach(Building building in TargetPlayer.Buildings.Values)
        {
            building.Hurt(Amount, Blockable, AoE);
        }
    }
    public void hurtArmys(Player TargetPlayer)
    {
        foreach (Building building in TargetPlayer.Buildings.Values)
        {
            if (building.units > 0)
            {
                if (building.army != null)
                {
                    building.army.Hurt(Amount, 0, AoE);
                }
            }
        }
    }
}
