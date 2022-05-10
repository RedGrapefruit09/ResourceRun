using System;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public bool Triggered { get; private set; }
    public GameObject TriggerObject { get; private set; }

    private Func<GameObject, bool> _predicate = obj => true;

    public void Require(Func<GameObject, bool> requirement)
    {
        _predicate = requirement;
    }

    public void RequireComponent<T>() where T : UnityEngine.Object
    {
        _predicate = obj => obj.GetComponent<T>() != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (_predicate.Invoke(obj))
        {
            Triggered = true;
            TriggerObject = obj;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (_predicate.Invoke(obj))
        {
            Triggered = true;
            TriggerObject = obj;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (_predicate.Invoke(obj))
        {
            Triggered = false;
            TriggerObject = null;
        }
    }
}