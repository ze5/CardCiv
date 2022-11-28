using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeck : MonoBehaviour
{
    public Deck deck;
    public int DeckSize = 10;
    // Start is called before the first frame update
    void Start()
    {
        random();
        deck = GetComponent<Deck>();
    }
    public void random()
    {
        deck.Cards.Clear();
        for (int i = 0; i < DeckSize; i++)
        {
            deck.Cards.Add(Mathf.RoundToInt(Random.value * (deck.AllCards.Cards.Length - 1)));
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
