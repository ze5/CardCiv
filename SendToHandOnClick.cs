using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendToHandOnClick : MonoBehaviour
{
    // Start is called before the first frame update
    public StateManager state;
    public Hand target;
    public Deck Graveyard;
    private Clickable btn;
    private Hand hnd;
    void Start()
    {
        btn = GetComponent<Clickable>();
        hnd = GetComponentInParent<Hand>();
        btn.Clicked.AddListener(click);
    }
    private void click()
    {
        target = state.curPlayer.MyHand;
        hnd.SendToHand(gameObject, target);
        hnd.SendAll(Graveyard);
        hnd.DrawX(5);
        hnd.UpdateUI();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
