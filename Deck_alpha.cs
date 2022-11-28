using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck_alpha : MonoBehaviour
{
    // Start is called before the first frame update
    public int TopCard = -1;
    public List<Card> Cards;
    public List<int> Order;
    void Start()
    {
        Shuffle();
    }
    public void Shuffle()
    {
        int rng;
        List<int> indexSet = new List<int>();
        //make a list of all ints between 0 & amount of cards in deck
        for (int i = 0; i < Order.Count; i++)
        {
            indexSet.Add(i);
        }
        while (indexSet.Count > 0)
        {
            //set values of order to random numbers from the index set while removing them from indexset to ensure no repeats... sodoku or whatever.
            print(indexSet.Count);
            rng = Mathf.RoundToInt(Random.Range(0, indexSet.Count - 1));
            Order[indexSet.Count - 1] = indexSet[rng];
            indexSet.RemoveAt(rng);
        }
    }
    void UpdateOrders(int index)
    {
        for (int i=0; i< Order.Count; i++)
        {
            if (Order[i] > index)
            {
                //shift orders back one
                Order[i]--;
            }
        }
    }
    public void RemoveCard(int index = -1)
    {
        if (Cards.Count > 0)
        {
            if (index > -1)
            {
                Cards.RemoveAt(Order[index]);
                UpdateOrders(Order[index]);
                Order.RemoveAt(index);
                if (index < TopCard)
                {
                    TopCard--;
                }
            }
            else
            {
                //removes the card at -index from from top, with -1 being top. 
                RemoveCard(TopCard - index);

            }
        }
    }

    public void AddCard(Card card, int index = -1)
    {
        Cards.Add(card);
        if (index > -1)
        {
            //card is placed at specific position in deck
            Order.Insert(index, Cards.Count - 1);
        }
        else
        {
            if (TopCard - index < Order.Count)
            {
                //card is placed -index from top of deck, -1 would be the top card in the deck.
                Order.Insert(TopCard - index, Cards.Count - 1);
            }
            else
            {
                //if you tell it to put a card passed the end of the deck, it will just add it to the end.
                Order.Add(Cards.Count - 1);
            }
        }
    }
    public Card GetCard(int index = -1)
    {
        print(TopCard - index);
        if (index > -1 && index < Order.Count)
        {
            //positive value means SPECIFIC card in deck or graveyard.
            return Cards[Order[index]];
        }
        else if (TopCard - index > -1 && TopCard - index < Order.Count)
        {
            //gets -index from top of deck
            return Cards[Order[TopCard - index]];
        }
        else
        {
            //it asked for a card our of range... Probably figure out what to do here?
            return null;
        }

    }
    public Card TopDeck()
    {
        if (TopCard > Cards.Count -2)
        {
            // deck is empty, do whatever needs to be done

            TopCard = -1;
        }
        return Cards[Order[TopCard++]];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
