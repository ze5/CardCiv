using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Clickable : MonoBehaviour
{
    [Serializable]
    public class ClickedEvent : UnityEvent { }
    [SerializeField]
    private ClickedEvent m_OnClick = new ClickedEvent();

    public ClickedEvent Clicked
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }
    private void OnMouseDown()
    {
        m_OnClick.Invoke();
    }

}
