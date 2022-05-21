using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ResourceRun.Player
{
    /// <summary>
    /// A script for a <see cref="GameObject"/> with a trigger <see cref="BoxCollider2D"/> that spectates that <see cref="BoxCollider2D"/>
    /// and registers the colliding object if a certain condition (predicate) is met.
    /// </summary>
    public class PlayerTrigger : MonoBehaviour
    {
        private Func<GameObject, bool> _predicate = obj => true;
        
        /// <summary>
        /// Whether this trigger is active
        /// </summary>
        public bool Triggered { get; private set; }
        
        /// <summary>
        /// The colliding <see cref="GameObject"/>. If <see cref="Triggered"/> is <see langword="false"/>, then this is <see langword="null"/>
        /// </summary>
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

        /// <summary>
        /// Sets a custom condition represented by the delegate passed into this method.
        /// </summary>
        /// <param name="requirement">The delegate representing the condition</param>
        public void Require(Func<GameObject, bool> requirement)
        {
            _predicate = requirement;
        }

        /// <summary>
        /// Sets up a condition that requires a component of the given type to be present on the checked <see cref="GameObject"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the required component</typeparam>
        public void RequireComponent<T>() where T : Object
        {
            _predicate = obj => obj.GetComponent<T>() != null;
        }
    }
}