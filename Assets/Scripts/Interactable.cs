using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public ItemId inputHand;
    public bool pull;
    public ItemId outputHand;
    public ItemId outputGround;
    [SerializeField] UnityEvent events;

    public void Interact()
    {
        events.Invoke();
    }
}
