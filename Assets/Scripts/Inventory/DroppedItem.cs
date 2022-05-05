using System.Collections;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    
    [HideInInspector] public Item originalItem;
    
    private Inventory _inventory;

    private void Start()
    {
        _inventory = FindObjectOfType<Inventory>();
        GetComponent<SpriteRenderer>().sprite = originalItem.GetComponent<SpriteRenderer>().sprite;
        //StartCoroutine(Rotate());
        StartCoroutine(Rescale());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(0f, 0f, rotationSpeed);
            yield return new WaitForSeconds(0.001f);
        }
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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        Destroy(gameObject);
    }
}
