using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Deck MyDeck;
    public Hand MyHand;
    public Deck MyGraveyard;
    public Dictionary<int, Building> Buildings = new Dictionary<int, Building>();
    public int Food = 0;
    public int Materials = 0;
    public int Culture = 0;
    public int FoodUpKeep = 0;
    public int Cannibal = 0;
    public int Drafts = 0;
    public int playerNum = 0;
}
