using Code.Scripts.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : SingletonBase<InventoryManager>
{
    [SerializeField] private LayerMask _inventoryLayerMask = -1; // Default to all layers, should be set to "Inventory" layer
    [SerializeField] private float _dragDistance = 5f;
    [SerializeField] private float _scrollSpeed = 25f; // Increased speed for better feel
    [SerializeField] private float _minDragDepth = 0.5f;
    [SerializeField] private float _maxDragDepth = 10f;
    [SerializeField] private float _raycastDistance = 100f;
    
    private InventoryItem _hoveredItem;
    private InventoryItem _draggedItem;
    private Camera _mainCamera;
    private float _currentDragDepth;
    
    protected override bool PersistBetweenScenes => false;

    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            _mainCamera = FindFirstObjectByType<Camera>();
        }
    }

    private void Update()
    {
        if (_mainCamera == null) return;
        
        HandleHover();
        
        if (_draggedItem != null)
        {
            HandleScrollDepth();
            UpdateDraggedItemPosition();
        }
    }

    private void HandleScrollDepth()
    {
        if (Mouse.current == null) return;
        
        // Scroll y is typically in increments of 120 or similar, so we normalize it
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Normalize scroll value for more consistent behavior across hardware
            float normalizedScroll = scroll / 120f;
            _currentDragDepth += normalizedScroll * _scrollSpeed;
            _currentDragDepth = Mathf.Clamp(_currentDragDepth, _minDragDepth, _maxDragDepth);
        }
    }

    private void HandleHover()
    {
        if (_draggedItem != null)
        {
            _hoveredItem = null;
            return;
        }

        if (Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);
        
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _inventoryLayerMask))
        {
            if (hit.collider.TryGetComponent(out InventoryItem item))
            {
                _hoveredItem = item;
            }
            else
            {
                _hoveredItem = null;
            }
        }
        else
        {
            _hoveredItem = null;
        }
    }

    private void UpdateDraggedItemPosition()
    {
        if (Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);
        
        Vector3 targetPoint = ray.GetPoint(_currentDragDepth);
        _draggedItem.UpdateTargetPosition(targetPoint);
    }

    public void OnInventoryClicked(InputAction.CallbackContext context)
    {
        if (context.started && _hoveredItem != null)
        {
            StartDragging(_hoveredItem);
        }
        else if (context.canceled)
        {
            StopDragging();
        }
    }

    private void StartDragging(InventoryItem item)
    {
        _draggedItem = item;
        _draggedItem.StartDragging();
        
        // Initial depth is the distance from camera to the item
        _currentDragDepth = Vector3.Distance(_mainCamera.transform.position, item.transform.position);
        _currentDragDepth = Mathf.Clamp(_currentDragDepth, _minDragDepth, _maxDragDepth);
    }

    private void StopDragging()
    {
        if (_draggedItem != null)
        {
            _draggedItem.StopDragging();
            _draggedItem = null;
        }
    }

    public InventoryItem HoveredItem => _hoveredItem;
}