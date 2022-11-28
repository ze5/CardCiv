using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public CardArray AllCards;
    public List<int> Cards;


    public Deck GraveYard;
    public bool ShuffleGraveIntoDeck = true;
    private void Start()
    {
        AllCards = GameObject.Find("All Cards").GetComponent<CardArray>();
        Shuffle();
        
    }
    public void DeckEmpty()
    {
        print("DeckEmpty");
        if (ShuffleGraveIntoDeck && GraveYard != null)
        {
            print("Tryingtotakegraveyard");
            TakeCards(GraveYard, GraveYard.Cards.Count);
            Shuffle();
        }
    }
    public void Shuffle()
    {
        List<int> tmp = new List<int>(Cards);
        Cards.Clear();
        int rng;
        while(tmp.Count >0)
        {
            rng = Mathf.RoundToInt(Random.Range(0, tmp.Count - 1));
            Cards.Add(tmp[rng]);
            tmp.RemoveAt(rng);
        }
    }
    public void MoveCard(int from, int to)
    {
        Cards.Insert(to, from);
        if (from > to)
        {
            //from index has increased from the insert
            Cards.Remove(from + 1);
        }
        else
        {
            Cards.Remove(from);
        }
    }
    public void AddCard(int CardID, int index = 0, Deck from = null, int fromIndex = -1)
    {
        //another "why bother one..."
        Cards.Insert(index, CardID);
        IndexesMoved(index, true, from, fromIndex);
    }
    public virtual void IndexesMoved(int index, bool added, Deck from, int fromIndex)
    {
        //this is for hand animation code.
    }
    public void GiveCard(Deck Target, int index = 0, int location = 0)
    {
        if (Cards.Count > index)
        {
            Target.AddCard(Cards[index], location, this);
            RemoveCard(index);
        }
        if (Cards.Count <1)
        {
            DeckEmpty();
        }
    }
    public void TakeCards(Deck Target, int number, int location = 0, int index = 0)
    {
        for (int i =0; i< number; i++)
        {
            TakeCard(Target, location, index);
        }
    }
    public void TakeCard(Deck Target, int location=0, int index =0)
    {
        Target.GiveCard(this, location, index);
    }
    public Card GetCardInfo(int index)
    {
        if (Cards.Count > index)
        {
            return AllCards.Cards[Cards[index]];
        }
        else
        {
            print("getcard info returned null at " + index);
            return null;
        }
    }
    public void RemoveCard(int index)
    {
        Cards.RemoveAt(index);
        IndexesMoved(index, false, null, -1);
        if (Cards.Count < 1)
        {
            DeckEmpty();
        }
    }
}
