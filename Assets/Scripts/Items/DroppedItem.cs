using System.Collections;
using ResourceRun.Player;
using UnityEngine;

namespace ResourceRun.Items
{
    /// <summary>
    /// A dropped item is a wrapper for an item instance that has been dropped onto the ground.
    /// A dropped item is animated and de-spawns (disappears) after a certain amount of time.
    /// The player can pick up a dropped item back into their inventory by simply colliding with the dropped item, if there's enough space
    /// in the <see cref="PlayerInventory"/>.
    /// </summary>
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField] [Tooltip("The amount of time in seconds, after which the dropped item instance disappears")]
        private float despawnTime;

        private PlayerInventory _inventory;

        /// <summary>
        /// The actual item instance that has been hidden and wrapped under this <see cref="DroppedItem"/> instance.
        /// </summary>
        public Item OriginalItem { private get; set; }

        private void Start()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -1f);

            _inventory = FindObjectOfType<PlayerInventory>();
            GetComponent<SpriteRenderer>().sprite = OriginalItem.GetComponent<SpriteRenderer>().sprite;

            StartCoroutine(Rescale());
            StartCoroutine(Despawn());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;

            if (_inventory.InsertItem(OriginalItem)) Destroy(gameObject);
        }

        private IEnumerator Rescale()
        {
            while (true)
            {
                for (var i = 0; i < 25; ++i)
                {
                    transform.localScale = new Vector3(transform.localScale.x - 0.01f, transform.localScale.y - 0.01f);
                    yield return new WaitForSeconds(0.05f);
                }

                for (var i = 0; i < 25; ++i)
                {
                    transform.localScale = new Vector3(transform.localScale.x + 0.01f, transform.localScale.y + 0.01f);
                    yield return new WaitForSeconds(0.05f);
                }

                yield return new WaitForSeconds(0.001f);
            }
        }

        private IEnumerator Despawn()
        {
            yield return new WaitForSeconds(despawnTime);
            Destroy(OriginalItem);
            Destroy(gameObject);
        }
    }
}