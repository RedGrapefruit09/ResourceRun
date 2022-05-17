using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ResourceRun.Player
{
    public class PlayerTrigger : MonoBehaviour
    {
        private Func<GameObject, bool> _predicate = obj => true;
        public bool Triggered { get; private set; }
        public GameObject TriggerObject { get; private set; }

        private void Update()
        {
            if (TriggerObject == null) Triggered = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var obj = collision.gameObject;

            if (_predicate.Invoke(obj) && obj != null)
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

        private void OnTriggerStay2D(Collider2D collision)
        {
            var obj = collision.gameObject;

            if (_predicate.Invoke(obj) && obj != null)
            {
                Triggered = true;
                TriggerObject = obj;
            }
        }

        public void Require(Func<GameObject, bool> requirement)
        {
            _predicate = requirement;
        }

        public void RequireComponent<T>() where T : Object
        {
            _predicate = obj => obj.GetComponent<T>() != null;
        }
    }
}