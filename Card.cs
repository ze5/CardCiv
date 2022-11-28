using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardType
    {
        Building, Upgrade, Instant, Targeted
    }
    public CardValidTargets[] validTargets;
    public CardType type;
    public int Cost;
    public int FoodCost = 0;
    public int CultureCost = 0;
    public Sprite gfx;
    public string Name;
    public string Discription;
    public CardEffect[] Effects;
    public int Atk = 0;
    public int HP = 5;
    public int ArmyAtk = 0;
    public int ArmyHP = 0;
    public int UnitsPTurn = 0;
    public int MaterialsPTurn = 0;
    public int FoodPTurn = 0;
    public int CulturePTurn = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
