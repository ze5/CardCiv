using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Hand : Deck
{
    public List<GameObject> CardsUI;
    public GameObject uicardprefab;
    public int StartingHand = 5;
    public Deck OwnersDeck;
    public StateManager State;
    public int cheapest = int.MaxValue;
    public float SlerpTime = 0.5f;
    public CardPrefabVFX DrawCard;
    public CardPrefabVFX PlayCardA;
    private Vector3 DrawStart;

    private int indexAdded = 100;
    private int indexRemoved = 100;
    public Dictionary<GameObject, int> UIint = new Dictionary<GameObject, int>();
    private bool animating = false;
    private RectTransform rect;
    // Start is called before the first frame update
    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        Invoke("Setup", 0);
    }
    void Setup()
    {
        DrawStart = DrawCard.transform.position;
        DrawX(StartingHand);
        UpdateUI();
    }
    public override void IndexesMoved(int index, bool added, Deck from, int fromIndex)
    {
        //hackish ifstatment wrapper to prevent this from firing on initial setup
        if (Time.frameCount > 1 && animating == false)
        {
            if (added)
            {
                indexAdded = index;
            }
            else
            {
                indexRemoved = index;
            }
            Invoke("MoveUI", Time.fixedDeltaTime);
                //CardsUI[Cards.Count -1].gameObject.SetActive
        }
       
    }
    private void MoveUI()
    {
        int i = 0;
        int offset;
        if (!animating)
        {
            while (i < Cards.Count +1)
            {
                offset = 0;
                if (i >= indexRemoved)
                {
                    offset -= 1;
                }

                if (i >= indexAdded)
                {
                    offset += 1;
                }
                if (i == indexRemoved)
                {
                    CardsUI[i].SetActive(false);//this should get changed to custom anim code
                }

                //print("told it to slerp");
                if (i + offset < Cards.Count && i + offset >= 0)
                {
                    if (i > CardsUI.Count -1)
                    {
                        UpdateUI();
                    }
                    CardsUI[i].GetComponent<CardPrefabVFX>().StartSlerpTo(GetCardPosition(i + offset), SlerpTime);
                }

                print("Offset:" + offset);
                i++;
            }
            //print("Removed: " + indexRemoved.ToString() + " Added:" + indexAdded.ToString());
            animating = true;
            Invoke("UpdateUI", SlerpTime + Time.fixedDeltaTime);
        }

    }
    public void DrawX(int number)
    {
        for (int i=0;i<number;i++)
        {
            Draw();
        }

    }
    public void Draw(int index = 0)
    {
        TakeCard(OwnersDeck, index);
        DrawCard.gameObject.SetActive(true);
        DrawCard.setup(GetCardInfo(0));
        DrawCard.StartSlerpTo(GetCardPosition(0), SlerpTime);
        //UpdateUI();
    }
    public void PlayCardAnim(int card, Vector3 location)
    {
        PlayCardA.gameObject.SetActive(true);
        PlayCardA.transform.position = GetCardPosition(card);
        PlayCardA.setup(GetCardInfo(card));
        PlayCardA.StartSlerpTo(location, SlerpTime);
    }
    public void SendAll(Deck Target)
    {
        while (Cards.Count > 0)
        {
            GiveCard(Target);
        }
    }
    public void PlayCard(GameObject card)
    {

        int i = UIint[card];
        State.PlayCard(this, i);
    }
    public void SendToHand(GameObject card, Hand destination)
    {
        int i = UIint[card];

                GiveCard(destination, i, 0);
               // destination.UpdateUI();
                //UpdateUI();
    }
    public void UpdateUI()
    {
        DrawCard.transform.position = DrawStart;
        DrawCard.gameObject.SetActive(false);
        PlayCardA.gameObject.SetActive(false);
        animating = false;
        indexAdded = 100;
        indexRemoved = 100;
        cheapest = 1000;
        while (Cards.Count > CardsUI.Count)
        {
            CardsUI.Add(Instantiate(uicardprefab, transform));
            UIint.Add(CardsUI[CardsUI.Count -1], CardsUI.Count - 1);
            print("for whatever reason, updating UI dictionary");
        }
        for (int i = 0; i < CardsUI.Count; i++)
        {
            if (i < Cards.Count)
            {
                SetupCard(i);
            }
            else
            {
                CardsUI[i].SetActive(false);
            }
        }
    }
    void SetupCard(int i)
    {
        if (GetCardInfo(i) != null)
        {
            if (GetCardInfo(i).Cost < cheapest)
            {
                cheapest = GetCardInfo(i).Cost;
            }
            CardsUI[i].SetActive(true);
            
            CardsUI[i].transform.position = GetCardPosition(i);

            CardsUI[i].GetComponent<CardPrefabVFX>().setup(GetCardInfo(i));
            CardsUI[i].transform.rotation = GetCardRotation(i);
        }
        else
        {
            //I need to figure out why
            print(i + " is null, " + Cards[i]);
        }
    }
    Vector3 GetCardPosition(int i)
    {
        Vector3 pos = transform.position;
        pos.x += i * (rect.rect.width / Cards.Count);
        Vector3 Slerp;
        if (i < (Cards.Count / 2))
        {
            Slerp = Vector3.Slerp(Vector3.zero, Vector3.one, (float)(i) / (float)(Cards.Count - 1));
        }
        else
        {
            Slerp = Vector3.Slerp(Vector3.one, Vector3.zero, (float)(i) / (float)(Cards.Count - 1));
        }
        pos.y += Slerp.y*200;
        //pos.y = transform.position.y + (Mathf.Sqrt(pos.y) * 10);
        pos.z = 4;
        pos = Camera.main.ScreenToWorldPoint(pos);
        //pos.z = -i/(float)Cards.Count/10f;

        pos.z = -Slerp.z/10f;
        pos.z -= 4;
        if (i == 0 || i == Cards.Count - 1)
        {
            pos.y -= 0.3f;
        }
        return pos;
    }
    Quaternion GetCardRotation(int i)
    {
        Quaternion min = Quaternion.Euler(0, 0, -15);
        Quaternion max = Quaternion.Euler(0, 0, 15);
        return Quaternion.Slerp(max, min, i / (float)(Cards.Count - 1));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
