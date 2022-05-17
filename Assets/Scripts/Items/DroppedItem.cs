using System.Collections;
using ResourceRun.Player;
using UnityEngine;

namespace ResourceRun.Items
{
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField] private float despawnTime;

        private PlayerInventory _inventory;

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