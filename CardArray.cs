using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardArray : MonoBehaviour
{
    public Dictionary<string, int> CardIndex = new Dictionary<string, int>();

    public Card[] Cards;
    private void Start()
    {
        for (int i = 0; i< Cards.Length; i++)
        {
            CardIndex.Add(Cards[i].Name, i);
        }

    }
}