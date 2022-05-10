using System.Collections;
using UnityEngine;

public class PlayerGathering : MonoBehaviour
{
    [SerializeField] private PlayerTrigger mineTrigger;

    private PlayerInventory _inventory;
    private PlayerMovement _movement;

    private void Start()
    {
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        
        mineTrigger.RequireComponent<Gatherable>();
    }

    private void Update()
    {
        if (mineTrigger.Triggered && Input.GetKeyDown(KeyCode.G))
        {
            var selectedItem = _inventory.GetSelectedItem();
            var gatherable = mineTrigger.TriggerObject.GetComponent<Gatherable>();

            if (selectedItem is ToolItem tool && tool.target == gatherable.target)
            {
                StartCoroutine(Gather(gatherable, tool));
            }
        }
    }

    private IEnumerator Gather(Gatherable gatherable, ToolItem tool)
    {
        _movement.Frozen = true;
        yield return gatherable.Gather(tool);
        _movement.Frozen = false;
    }
}
