using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class CustomInteractable : MonoBehaviour
{

    [HideInInspector]
    public CustomHand m_ActiveHand = null;

    public void Action(GameObject RSVP)
    {
        print("Action [press x to show rsvp]");

    }

    public string GetText()
    {
        string content = GetComponentInChildren<TMP_Text>().text;
        return content;
    }

}
