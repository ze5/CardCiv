using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Army : MonoBehaviour
{
    public int Atk=0;
    public int HP=0;
    public int Size = 0;
    public bool attacked = false;
    public Building parent;
    public Transform AttackTarget;
    public Transform DefendTarget;
    public float defTime = 0.8f;
    public float atkTime = 0.2f;
    public LineRenderer line;

    public GameObject Done;
    public TextMeshPro SizeT;
    public TextMeshPro AH;

    public float elapsedTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Physics.Raycast()

    }
    public void Hurt(int Damage,int instances = 0, bool AOE = false)
    {
        if (AOE)
        {
            Damage *= Size;
        }
        if (instances > 0)
        {
            Damage *= instances;
        }
        print("army took:" + Damage.ToString());
        parent.units -= Damage / HP;

        
    }
    private void AttackAnim()
    {
        transform.position = Vector3.Slerp(transform.position, AttackTarget.position, (elapsedTime / atkTime) * 1.5f);
        elapsedTime += Time.deltaTime;
        if (elapsedTime>atkTime)
        {
            AttackTarget = null;
            DefendTarget = parent.transform;
            elapsedTime = 0;
        }
    }
    private void DefendAnim()
    {
        Vector3 def = DefendTarget.position;
        def.z -= 0.5f;
        if (DefendTarget != parent.transform)
        {
            def.y += 0.5f;
        }
        transform.position = Vector3.Lerp(transform.position, def, (elapsedTime / defTime) );
        elapsedTime += Time.deltaTime;
        if (elapsedTime > defTime)
        {
            elapsedTime = -1;
        }
    }
        private void FixedUpdate()
    {
        Done.SetActive(false);
        Size = parent.units;
        AH.text = Atk.ToString() + "/" + HP.ToString();
        SizeT.text = Size.ToString();

        if (AttackTarget != null)
        {
            AttackAnim();
            line.enabled = false;
        }
        else if (elapsedTime > -1 && DefendTarget != null)
        {
            DefendAnim();
        }
        else if (DefendTarget != null && attacked == false)
        {
            //draw a line from parent?
            if (DefendTarget != parent.transform)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(2, parent.transform.position);
                line.SetPosition(1, Vector3.Lerp(transform.position, parent.transform.position, 0.5f));
                line.enabled = true;
            }
            else
            {
                line.enabled = false;
            }
        }
        else if (attacked == true)
        {
            Done.SetActive(true);
            line.enabled = false;
        }

        if (parent.units <= 0)
        {
            //the army has been wiped. Should probably do animation stuff eventually.
            Kill();
        }
    }
    public void Kill()
    {
        if (AttackTarget == null)
        {
            attacked = false;
            DefendTarget = null;
            transform.position = parent.transform.position - (Vector3.forward*0.5f);
            parent.units = 0;
            gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
