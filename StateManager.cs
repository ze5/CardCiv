using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateManager : MonoBehaviour
{
    public TextMeshProUGUI TurnUI;
    public Deck DraftDeck;
    public Deck DraftHand;
    public Deck DraftGrave;
    public int Step = 0;
    public int drafts;
    public GameObject River;
    public Player curPlayer;
    public int playercnt;
    public GameBoard board;
    public Vector3 InstantLocation;
    public bool targetInstant = false;

    private int unresolvedCard = -1;
    private Army CurrentArmy;
    private int Armysize = 0;
    private List<Building> Barracks = new List<Building>();
    private int Turn = 0;
    private Transform players;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.Find("Players").transform;
        playercnt = players.childCount;
        curPlayer = players.GetChild(Turn % playercnt).GetComponent<Player>();
        PassivePhase();
    }
    public void SelectedArm(Army selected)
    {
        if (Step == 3)
        {
            //its the combat step
            if (selected.parent.Owner == curPlayer && !selected.attacked)
            {
                //Army was selected by the player who's turn it is, durring combat, and hasn't already attacked this turn
                CurrentArmy = selected;
                board.Selected(CurrentArmy.transform);
            }
        }
        if (Step == 2 && unresolvedCard > -1)
        {
            TargetedCard(selected.gameObject);
        }
    }
    public void SelectedBuilding(Building selected)
    {

        if (Step == 3)
        {
            //its the combat step
            if (CurrentArmy != null)
            {
                if (selected.Owner == CurrentArmy.parent.Owner == curPlayer && CurrentArmy.parent.defending != selected)
                {
                    //player has targeted their own building with an army on their turn, defend it
                    selected.defender = CurrentArmy;
                    CurrentArmy.parent.defending = selected;
                    CurrentArmy.DefendTarget = selected.transform;
                    CurrentArmy.elapsedTime = 0;
                    CurrentArmy = null;
                }
                else if (CurrentArmy.parent.Owner == curPlayer && selected.Owner != curPlayer)
                {
                    print("attacking");
                    //player has targeted an opponents building
                    //first STOP defending
                    if (CurrentArmy.parent.defending != null)
                    {
                        CurrentArmy.parent.defending.defender = null;
                        CurrentArmy.parent.defending = null;
                    }
                    //then attack
                    selected.Attacked(CurrentArmy);
                    CurrentArmy.AttackTarget =selected.transform;
                    CurrentArmy.attacked = true;
                    CurrentArmy = null;
                }
                board.Select.gameObject.SetActive(false);
            }
        }
        if (Step == 2 && unresolvedCard > -1)
        {
            TargetedCard(selected.gameObject);
            board.Select.gameObject.SetActive(false);
        }
    }
    public void TargetedCard(GameObject Target)
    {
        Card curCard = curPlayer.MyHand.GetCardInfo(unresolvedCard);
        Building upgradeTarget;
        bool valid = false;
        if (curCard.validTargets != null && curCard.validTargets.Length >0)
        {
            for (int i =0; i < curCard.validTargets.Length; i++)
            {
                if (curCard.validTargets[i].Evalute(Target))
                {
                    valid = true;
                    break;
                }
            }
        }
        else
        {
            valid = true;
        }
        if (valid)
        {
            curPlayer.MyHand.PlayCardAnim(unresolvedCard, Target.transform.position);
            if (curCard.type == Card.CardType.Targeted)
            {
                foreach (CardEffect effect in curCard.Effects)
                {
                    effect.DoEffect(curPlayer, null, Target);
                }
                curPlayer.MyHand.GiveCard(curPlayer.MyGraveyard, unresolvedCard);
            }
            else if(curCard.type == Card.CardType.Upgrade)
            {
                if (Target.TryGetComponent<Building>(out upgradeTarget))
                {
                    upgradeTarget.Cards.Add(curPlayer.MyHand.Cards[unresolvedCard]);
                    upgradeTarget.Atk += curCard.Atk;
                    upgradeTarget.MaxHP += curCard.HP;
                    upgradeTarget.HP += curCard.HP;
                    upgradeTarget.AAtk += curCard.ArmyAtk;
                    upgradeTarget.AHP += curCard.ArmyHP;
                    if (upgradeTarget.army != null)
                    {
                        upgradeTarget.army.Atk = upgradeTarget.AAtk;
                        upgradeTarget.army.HP = upgradeTarget.AHP;
                    }
                    upgradeTarget.updateText();
                    curPlayer.MyHand.RemoveCard(unresolvedCard);
                }
            }
            
            curPlayer.Materials -= curCard.Cost;
            curPlayer.Food -= curCard.FoodCost;
            curPlayer.Culture -= curCard.CultureCost;
            unresolvedCard = -1;
            targetInstant = false;
        }
    }
    public void Mouseover(GameObject mouseover)
    {
        if (Step == 3 || targetInstant)
        {
            Building building;
            Army army = null;
            if (mouseover.TryGetComponent<Building>(out building) || mouseover.TryGetComponent<Army>(out army))
            {
                board.Highlight.position = mouseover.GetComponent<Collider>().bounds.center;
                board.Highlight.localScale = mouseover.GetComponent<Collider>().bounds.size * 1.05f;
                board.Highlight.gameObject.SetActive(true);
                board.highTimer = 0f;
                if (building != null)
                {
                    if (building.Owner == curPlayer)
                    {
                        board.HighlightedEnemy = false;
                    }
                    else
                    {
                        board.HighlightedEnemy = true;
                    }
                    board.buildingHighlighted = true;
                }
                else if (army != null)
                {
                    if (army.parent.Owner == curPlayer)
                    {
                        board.HighlightedEnemy = false;
                    }
                    else
                    {
                        board.HighlightedEnemy = true;
                    }
                }
            }
            else
            {
                board.Highlight.gameObject.SetActive(false);
            }
            //combat step
            if (CurrentArmy != null)
            {
                
              //  if (mouseover)
            }
        }
    }
    public void Drafted()
    {
        curPlayer.Drafts--;
    }
    public void PlayCard(Hand hand, int card)
    {
        if (Step == 2 && hand == curPlayer.MyHand)
        {
            print("playing");
            //it is the card phase of the player trying to play a card. Resolve the card effect
            Card curCard = hand.GetCardInfo(card);
            Building building;
            if (curCard.Cost <= curPlayer.Materials && curCard.FoodCost <= curPlayer.Food)
            {
                print("can afford");
                //you can play the card
                if (curCard.type == Card.CardType.Instant)
                {
                    print("is instant");
                    
                    //spend resources, put the card in the graveyard
                    curPlayer.Materials -= curCard.Cost;
                    curPlayer.Food -= curCard.FoodCost;
                    curPlayer.MyHand.PlayCardAnim(card, InstantLocation);
                    curPlayer.MyHand.GiveCard(curPlayer.MyGraveyard, card);
                    //resolve the Instant.
                    foreach (CardEffect effect in curCard.Effects)
                    {
                        effect.DoEffect(curPlayer);
                        print("effect");
                    }
                    //curPlayer.MyHand.UpdateUI();
                }
                if (curCard.type == Card.CardType.Building)
                {
                    building = board.GetFreeSpace(Turn % playercnt);
                    if (building == null)
                    {
                        print("building is null");
                        //ui shit, you cant play the card
                    }
                    else
                    {
                        print("played new blding");
                        curPlayer.Materials -= curCard.Cost;
                        curPlayer.Food -= curCard.FoodCost;
                        if (building.Cards.Count > 0)
                        {
                            building.Cards.Clear();
                        }
                            building.Cards.Add(curPlayer.MyHand.Cards[card]);
                        building.player = Turn % players.childCount;

                        building.Setup();
                        building.VFX.InvisTime = curPlayer.MyHand.SlerpTime;
                        curPlayer.MyHand.PlayCardAnim(card, building.transform.position);
                        //building.VFX.setup(curPlayer.MyHand.GetCardInfo(card));
                        curPlayer.MyHand.RemoveCard(card);
                       // building.gameObject.SetActive(true);
                       // curPlayer.MyHand.UpdateUI();
                    }
                }
                if (curCard.type == Card.CardType.Targeted || curCard.type == Card.CardType.Upgrade)
                {
                    targetInstant = true;
                    board.Selected(curPlayer.MyHand.CardsUI[card].transform);
                    unresolvedCard = card;
                }
                else
                {
                    targetInstant = false;
                    unresolvedCard = -1;
                }
            }

            //else ....do a ui thing. Make Clippy show up and explain market economics or something IDK what do I look like a UX Wizard or some shit?
        }
    }

    public void NextStep()
    {
        board.buildingHighlighted = false;
        board.Highlight.gameObject.SetActive(false);
        board.line.enabled = false;
        board.Select.gameObject.SetActive(false);
        targetInstant = false;
        unresolvedCard = -1;

        Step++;
    }
    void PassivePhase()
    {
        //set drafts to 1 at start of first phase, this will skip the draft step if no drafts available
        drafts = 1;
        //set curDrafts to 0
        curPlayer.Drafts = 0;
        //zero FoodUpKeep as well
        curPlayer.FoodUpKeep = 0;
        //clear the barracks list
        Barracks.Clear();
        //zero army size
        Armysize = 0;
        foreach (Building building in curPlayer.Buildings.Values)
        {
            if (building.gameObject.activeSelf)
            {
                building.Passives();
                print("passives");
                if (building.units > 0)
                {
                    //mostly to help with Combat & Upkeep steps
                    curPlayer.FoodUpKeep += building.units;
                    Armysize += building.units;
                    Barracks.Add(building);
                    building.army.attacked = false;
                }
                curPlayer.FoodUpKeep += building.foodUpkeep;
            }
        }
        Step++;
    }
    void DraftPhase()
    {
        River.SetActive(true);
        if (curPlayer.Drafts < drafts)
        {
            River.SetActive(false);
            if (Step==1)
            {
                Step++;
            }
        }
        if (DraftDeck.Cards.Count + DraftHand.Cards.Count + DraftGrave.Cards.Count == 0)
        {
            //in the event that all cards have been drafted, draft becomes draw
            River.SetActive(false);
            curPlayer.MyHand.DrawX(curPlayer.Drafts - drafts + 1);
            if (Step == 1)
            {
                Step++;
            }
        }
    }
    void CardPhase()
    {
        curPlayer.MyHand.gameObject.SetActive(true);
        //card logic

        //might not be the best way to handle this. Currently just toggling active on the hand UI.
        //Step++;
    }
    void CombatPhase()
    {
        board.BoardActive();
    }
    void UpkeepPhase()
    {
        //upkeep logic
        while (Armysize > 0 && curPlayer.FoodUpKeep > curPlayer.Food )
        {
            //kill random units until you can feed them all, or everyones dead
            Barracks[Mathf.RoundToInt(Random.value * (Barracks.Count - 1))].units--;
            if (curPlayer.Cannibal > 0)
            {
                curPlayer.Food+= curPlayer.Cannibal;
            }
            curPlayer.FoodUpKeep--;
            Armysize--;
        }

        curPlayer.Food -= curPlayer.FoodUpKeep;
        EndTurn();
    }
    void EndTurn()
    {
        Turn++;
        curPlayer = players.GetChild(Turn % players.childCount).GetComponent<Player>();
        TurnUI.text = "Player " + ((Turn % players.childCount) + 1).ToString() + "'s turn";
        Step = 0;
    }
    private void FixedUpdate()
    {
        if (Step == 0)
        {
            PassivePhase();
        }
        if (Step == 1)
        {
            DraftPhase();
        }
        if (Step == 2)
        {
            CardPhase();
            if (drafts<curPlayer.Drafts)
            {
                //incase of instant draft
                DraftPhase();
            }
            if (targetInstant)
            {
                board.BoardActive();
            }
            else
            {
                board.buildingHighlighted = false;
                board.Highlight.gameObject.SetActive(false);
                board.line.enabled = false;
                board.Select.gameObject.SetActive(false);
            }
        }
        if (Step == 3)
        {
            CombatPhase();
        }
        if (Step > 3)
        {
            UpkeepPhase();
        }
    }
}
