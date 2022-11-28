using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Building : MonoBehaviour
{

    public List<int> Cards;
    public CardArray AllCards;
    public int player = 0;
    public int units = 0;
    public int foodUpkeep = 0;
    public int HP = 5;
    public int MaxHP =5;
    public int Atk = 0;
    public int UnitsPTurn = 0;
    public int AAtk = 0;
    public int AHP = 0;
    public int Index;
    public StateManager state;
    public CardPrefabVFX VFX;
    public TextMeshPro HPT;
    public TextMeshPro ATKT;

    public Army armyPrefab;
    public Army army;
    public Army defender;
    public Building defending;

    public Player Owner;

    private bool JustBuilt = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Attacked(Army Attacker)
    {
        if (defender != null && defender.Size>0)
        {
            Attacker.Hurt(defender.Atk, defender.Size);
            defender.Hurt(Attacker.Atk, Attacker.Size);
            Attacker.Size = Attacker.parent.units;
            defender.Size = defender.parent.units;
        }
        HP -= Attacker.Atk * Attacker.Size;
        Attacker.Hurt(Atk, 0, true);
        HPT.text = HP.ToString();
        if (HP <= 0)
        {
            Kill();
        }
    }
    public void updateText()
    {
        HPT.text = HP.ToString();
        ATKT.text = Atk.ToString();
    }
    public void Hurt(int amount, bool blockable = false, bool AoE = false)
    {
        if (defender != null && defender.Size > 0)
        {
            defender.Hurt(amount, 0, AoE);
            amount -= defender.HP * defender.Size;
            defender.Size = defender.parent.units;
        }
        HP -= amount;
        HPT.text = HP.ToString();
        if (HP <= 0)
        {
            Kill();
        }
    }
    public void Kill()
    {
        //just disable for now
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        gameObject.SetActive(true);
        if (AllCards == null)
        {
            AllCards = GameObject.Find("All Cards").GetComponent<CardArray>();
        }
        if (Owner == null)
        {
            Owner = GameObject.Find("Players").transform.GetChild(player).GetComponent<Player>();

        }
        if (!Owner.Buildings.ContainsKey(GetInstanceID()))
        {
            Owner.Buildings.Add(GetInstanceID(), this);
        }
        VFX.setup(AllCards.Cards[Cards[0]]);
        units = 0;
        JustBuilt = true;
        gameObject.SetActive(true);
        Passives();
        JustBuilt = false;
    }
    public void Passives()
    {
        MaxHP = 0;
        Atk = 0;
        AAtk = 0;
        AHP = 0;
        if (units < 0)
        {
            units = 0;
        }
        UnitsPTurn = 0;
        Card card;
        Vector3 pos;
        for (int i = 0; i < Cards.Count; i++)
        {
            card = AllCards.Cards[Cards[i]];
            if (JustBuilt == false)
            {
                if (card.Effects != null)
                {
                    foreach (CardEffect effect in card.Effects)
                    {
                        effect.DoEffect(Owner, gameObject);
                    }
                }

                UnitsPTurn += card.UnitsPTurn;
                Owner.Materials += card.MaterialsPTurn;
                Owner.Food += card.FoodPTurn;
                Owner.Culture += card.CulturePTurn;
            }

            MaxHP += card.HP;
            Atk += card.Atk;

            AAtk += card.ArmyAtk;
            AHP += card.ArmyHP;
            if (JustBuilt)
            {
                HP = MaxHP;
            }
        }
        units += UnitsPTurn;
        if (units >0)
            {
            if (army == null)
                {
                army = Instantiate(armyPrefab, transform);
                pos = transform.position;
                pos.z -= .5f;
                army.transform.position = pos;
                }
            if (!army.gameObject.activeSelf)
            {
                army.gameObject.SetActive(true);
            }
            army.Atk = AAtk;
            army.HP = AHP;
            army.Size = units;
            army.parent = this;
            if (defending == null && defender == null)
            {
                //self defence!
                defender = army;
                defending = this;
            }
            else if (defending == null)
            {
                //this is kinda an edge case, but needs to be dealt with. Happens if Army attacked and a different army is defending this barracks. Gotta put the army SOMEWHERE.
            }
            }
    }
    private void OnDisable()
    {
        Owner.Buildings.Remove(GetInstanceID());
        while(Cards.Count >0)
        {
            Owner.MyGraveyard.AddCard(Cards[Cards.Count - 1]);
            Cards.RemoveAt(Cards.Count - 1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
