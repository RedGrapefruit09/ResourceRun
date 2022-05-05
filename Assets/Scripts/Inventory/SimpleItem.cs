using UnityEngine;

public class SimpleItem : Item
{
    [SerializeField] private string tooltip;

    private PlayerMovement _playerMovement;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        transform.SetParent(_playerMovement.transform);
        transform.localScale = new Vector3(0.75f, 0.75f, 1f);
    }

    public override void OnPickedUp()
    {
        
    }

    public override void OnDropped()
    {
        
    }

    public override void OnSelected()
    {
        gameObject.SetActive(true);
    }

    public override void OnDeselected()
    {
        gameObject.SetActive(false);
    }

    public override string BuildTooltip()
    {
        return tooltip;
    }

    private void Update()
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