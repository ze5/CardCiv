using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public Player Owner;
    public TextMeshProUGUI Materials;
    public TextMeshProUGUI Food;
    public TextMeshProUGUI Units;
    public TextMeshProUGUI Culture;

    private void FixedUpdate()
    {
        Materials.text = "Materials: " + Owner.Materials.ToString();
        Food.text = "Food: " + Owner.Food.ToString();
        Units.text = "Units: " + Owner.FoodUpKeep.ToString();
        Culture.text = "Culture: " + Owner.Culture.ToString();
    }
}
