using UnityEngine;
using UnityEngine.EventSystems;

public class TouchLane : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
                                    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private int laneIndex;

    private InputManager inputManager;
    private bool isDragging = false; 
    private Vector2 dragStartPosition;

    void Start()
    {
        inputManager = InputManager.GetInstance();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false; 
        dragStartPosition = eventData.position; 

        inputManager.OnLanePress(laneIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true; 

        inputManager.OnLaneDragStart(laneIndex);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 通知 InputManager "滑動結束了"，並傳入起始點和結束點
        inputManager.OnLaneSwipeEnd(laneIndex, dragStartPosition, eventData.position);
        isDragging = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
        {
            inputManager.OnLaneRelease(laneIndex);
        }

        isDragging = false; // 重設狀態
    }
}