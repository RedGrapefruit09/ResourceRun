using UnityEngine;

public class PlayerGathering : MonoBehaviour
{
    [SerializeField] private PlayerTrigger leftMineTrigger;
    [SerializeField] private PlayerTrigger rightMineTrigger;

    private PlayerInventory _inventory;
    private PlayerMovement _movement;

    private void Start()
    {
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        
        leftMineTrigger.RequireComponent<Gatherable>();
        rightMineTrigger.RequireComponent<Gatherable>();
    }
}
