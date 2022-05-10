using System.Collections;
using UnityEngine;

public class PlayerGathering : MonoBehaviour
{
    [SerializeField] private PlayerTrigger mineTrigger;
    [SerializeField] private float floatSpeed;

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

            if (selectedItem is ToolItem tool && CompareTargets(tool.target, gatherable.target))
            {
                StartCoroutine(Gather(gatherable, tool));
            }
        }
    }

    private IEnumerator Gather(Gatherable gatherable, ToolItem tool)
    {
        _movement.Frozen = true;
        _inventory.BlockSelection = true;
        
        if (tool.target == ToolTarget.Trees)
        {
            var offsetPosition = _movement.Facing == PlayerFacing.Right ? Vector3.left : Vector3.right;
            yield return FloatTowards(transform.position + offsetPosition * 0.75f);
        }
        else
        {
            var targetPosition = new Vector3(transform.position.x, gatherable.transform.position.y);
            yield return FloatTowards(targetPosition);
        }

        tool.StartAnimation();
        yield return gatherable.Gather(tool);
        tool.StopAnimation();
        
        _movement.Frozen = false;
        _inventory.BlockSelection = false;
    }

    private IEnumerator FloatTowards(Vector3 targetPosition)
    {
        while (true)
        {
            var distance = Vector2.Distance(transform.position, targetPosition);

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                floatSpeed * Time.deltaTime);

            yield return new WaitForSeconds(0.001f);

            if (distance < 0.1f) break;
        }
    }

    private static bool CompareTargets(ToolTarget tool, ToolTarget gatherable)
    {
        if (gatherable == ToolTarget.AnyObject) return true;

        return tool == gatherable;
    }
}
