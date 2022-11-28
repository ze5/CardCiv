using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class MouseOver : MonoBehaviour
{

    [Serializable]
    public class HoverEvent : UnityEvent { }
    [SerializeField]
    private HoverEvent m_OnMouseover = new HoverEvent();

    public HoverEvent Hover
    {
        get { return m_OnMouseover; }
        set { m_OnMouseover = value; }
    }
    private void OnMouseOver()
    {
        m_OnMouseover.Invoke();
    }

}