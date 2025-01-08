using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TakeDamageEvent : MonoBehaviour
{
    public UnityEvent _event;

    public void PlayEvent()
    {
        _event.Invoke();
    }
}