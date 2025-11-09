using UnityEngine;
using UnityEngine.EventSystems;

public class TouchLane3D : MonoBehaviour
{
    [SerializeField] private int laneIndex;
    [SerializeField] private float dragThreshold = 10f; // 判定拖動的最小距離

    private InputManager inputManager;
    private bool isDragging = false;
    private Vector2 dragStartPosition;

    void Start()
    {
        inputManager = InputManager.GetInstance();
    }

    void Update()
    {
        if (PauseManager.GetInstance().isPause) return;

        // --- 多指觸控 ---
        foreach (Touch touch in Input.touches)
        {
            HandleTouch(touch.position, touch.phase);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // --- 滑鼠模擬 ---
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition, TouchPhase.Began);
        }
        else if (Input.GetMouseButton(0))
        {
            HandleTouch(Input.mousePosition, TouchPhase.Moved);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleTouch(Input.mousePosition, TouchPhase.Ended);
        }
#endif
    }

    private void HandleTouch(Vector2 screenPosition, TouchPhase phase)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 0.1f);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != GetComponent<Collider>()) return;

            switch (phase)
            {
                case TouchPhase.Began:
                    isDragging = false;
                    dragStartPosition = screenPosition;
                    inputManager.OnLanePress(laneIndex);
                    break;

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
                    isDragging = false;
                    break;
            }
        }
    }
}
