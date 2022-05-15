using UnityEngine;
using UnityEngine.Serialization;

public class SimpleItem : Item
{
    [FormerlySerializedAs("tooltip")] [SerializeField] private string tooltipLine;

    private PlayerMovement _playerMovement;
    private SpriteRenderer _spriteRenderer;

    protected virtual void Start()
    {
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        transform.SetParent(_playerMovement.transform);
        transform.localScale = new Vector3(0.75f, 0.75f, 1f);
    }

    public override void OnSelected()
    {
        gameObject.SetActive(true);
    }

    public override void OnDeselected()
    {
        gameObject.SetActive(false);
    }

    public override void BuildTooltip(ItemTooltip tooltip)
    {
        tooltip.Add(tooltipLine);
    }

    protected virtual void Update()
    {
        if (_playerMovement.Facing == PlayerFacing.Left)
        {
            transform.localPosition = new Vector3(-0.2f, -0.1f, -5f);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
            _spriteRenderer.flipY = false;
        }
        else
        {
            transform.localPosition = new Vector3(0.15f, -0.115f, -5f);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
            _spriteRenderer.flipY = true;
        }
    }
}