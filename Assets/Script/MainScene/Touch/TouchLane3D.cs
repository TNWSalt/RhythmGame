using UnityEngine;
using UnityEngine.EventSystems;

public class TouchLane3D : MonoBehaviour
{
    private const int NoPointerId = int.MinValue;
    private const int MousePointerId = -1;

    [SerializeField] private int laneIndex;
    [SerializeField] private float dragThreshold = 10f;

    private InputManager inputManager;
    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private int activePointerId = NoPointerId;
    private Collider laneCollider;

    void Start()
    {
        inputManager = InputManager.GetInstance();
        laneCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (PauseManager.GetInstance().isPause) return;

        foreach (Touch touch in Input.touches)
        {
            HandleTouch(touch.fingerId, touch.position, touch.phase);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(MousePointerId, Input.mousePosition, TouchPhase.Began);
        }
        else if (Input.GetMouseButton(0))
        {
            HandleTouch(MousePointerId, Input.mousePosition, TouchPhase.Moved);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleTouch(MousePointerId, Input.mousePosition, TouchPhase.Ended);
        }
#endif
    }

    private void HandleTouch(int pointerId, Vector2 screenPosition, TouchPhase phase)
    {
        bool isActivePointer = activePointerId == pointerId;

        if (!isActivePointer)
        {
            if (phase != TouchPhase.Began) { return; }
            if (!PointerHitsThisLane(screenPosition)) { return; }

            activePointerId = pointerId;
            isDragging = false;
            dragStartPosition = screenPosition;
            inputManager.OnLanePress(laneIndex);
            return;
        }

        switch (phase)
        {
            case TouchPhase.Moved:
                if (!isDragging && Vector2.Distance(dragStartPosition, screenPosition) > dragThreshold)
                {
                    isDragging = true;
                    inputManager.OnLaneDragStart(laneIndex);
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isDragging)
                {
                    inputManager.OnLaneSwipeEnd(laneIndex, dragStartPosition, screenPosition);
                }
                else
                {
                    inputManager.OnLaneRelease(laneIndex);
                }
                ResetPointer();
                break;
        }
    }

    private bool PointerHitsThisLane(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 0.1f);

        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider == laneCollider;
    }

    private void ResetPointer()
    {
        activePointerId = NoPointerId;
        isDragging = false;
    }
}
