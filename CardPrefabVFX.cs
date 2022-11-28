using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPrefabVFX : MonoBehaviour
{
    // Start is called before the first frame update
    public MeshRenderer CardArt;
    public TextMeshPro Name;
    public TextMeshPro Cost;
    public TextMeshPro Description;
    public TextMeshPro ATK;
    public TextMeshPro HP;
    public TextMeshPro Type;
    public bool RotateWithSlerp = true;
    public bool highlighted = false;
    public Vector3 Rotate = new Vector3(0,45,0);

    private bool hidden = false;
    private Collider _collider;

    private float highTime = 0;
    private Quaternion StartRotate;
    private Vector3 StartPos;
    private Vector3 EndPos;
    private float TimeElapsed;
    private float SlerpTime;
    public float InvisTime;
    void Start()
    {
        _collider = GetComponent<Collider>();

            StartRotate = transform.rotation;

    }
    public void setup(Card card)
    {
        transform.rotation = StartRotate;
        StartPos = transform.position;
        CardArt.material.SetTexture("_MainTex", card.gfx.texture);
        Name.text = card.Name;
        Cost.text = card.Cost.ToString();
        Description.text = card.Discription;
        if (card.type == Card.CardType.Building)
        {
            if (card.Atk > 0)
            {
                ATK.text = card.Atk.ToString();
            }
            else
            {
                ATK.text = "";
            }
            HP.text = card.HP.ToString();
            Type.text = "Building";
            if (card.UnitsPTurn > 0)
            {
                Type.text += "- Barracks";
            }
            if (card.FoodPTurn > 0)
            {
                Type.text += "- Farm";
            }
            if (card.MaterialsPTurn > 0)
            {
                Type.text += " - Mine";
            }
        }
        else
        {
            ATK.text = "";
            HP.text = "";
            if (card.type == Card.CardType.Instant)
            {
                Type.text = "Instant";
            }
            else if (card.type == Card.CardType.Upgrade)
            {
                Type.text = "Upgrade";
            }
            else if (card.type == Card.CardType.Targeted)
            {
                Type.text = "Instant - Targeted";
            }
        }
        SlerpTime = -1;
    }
    public void StartSlerpTo(Vector3 target, float time)
    {
        EndPos = target;
        SlerpTime = time;
        TimeElapsed = 0;
        StartPos = transform.position;
    }
    public void Highlight()
    {
        highTime = 0.1f;
        if (!highlighted)
        {
            transform.position = new Vector3(StartPos.x, StartPos.y, StartPos.z - 0.1f);
            highlighted = true;
        }
    }
    private void SlerpRotate()
    {
        if (Rotate.sqrMagnitude > 0)
        {
            if (EndPos.x > StartPos.x + 0.1f)
            {
                if (TimeElapsed < SlerpTime / 2)
                {
                    transform.rotation = Quaternion.Lerp(StartRotate, Quaternion.Euler(Rotate), TimeElapsed / SlerpTime / 2);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(Quaternion.Euler(Rotate), StartRotate, (TimeElapsed - (SlerpTime / 2)) / SlerpTime / 2);
                }
            }
            else if (EndPos.x < StartPos.x - 0.1f)
            {
                if (TimeElapsed < SlerpTime / 2)
                {
                    transform.rotation = Quaternion.Lerp(StartRotate, Quaternion.Euler(-Rotate), TimeElapsed / SlerpTime / 2);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(Quaternion.Euler(-Rotate), StartRotate, (TimeElapsed - (SlerpTime / 2)) / SlerpTime / 2);
                }
            }
        }
        else if (StartRotate.eulerAngles.sqrMagnitude >0)
        {
            transform.rotation = Quaternion.Slerp(StartRotate, Quaternion.identity, TimeElapsed / SlerpTime);
        }
    }
    private void Slerp()
    {
            _collider.enabled = false;
        transform.position = Vector3.Slerp(StartPos, EndPos, TimeElapsed / SlerpTime);
        if (RotateWithSlerp)
        {
            SlerpRotate();
        }
        TimeElapsed += Time.deltaTime;
        if (TimeElapsed >= SlerpTime)
        {
            _collider.enabled = true;
        }
    }
    private void Hide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MeshRenderer mesh;
            if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out mesh))
            {
                mesh.enabled = false;
            }
        }
        hidden = true;
    }
    private void UnHide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MeshRenderer mesh;
            if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out mesh))
            {
                mesh.enabled = true;
            }
        }
        hidden = false;
    }
    private void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (SlerpTime > TimeElapsed)
        {
            Slerp();
        }
        if (InvisTime > 0)
        {
            if (!hidden)
            {
                Hide();
            }
            InvisTime -= Time.deltaTime;
        }
        else if(hidden)
        {
            UnHide();
        }
        highTime -= Time.deltaTime;
        if (highlighted && highTime <0)
        {
            transform.position = StartPos;
            highlighted = false;
        }
    }
}
