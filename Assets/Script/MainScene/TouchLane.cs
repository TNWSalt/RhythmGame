using UnityEngine;
using UnityEngine.EventSystems;

public class TouchLane : MonoBehaviour
{
    [SerializeField]
    private int laneIndex; // 在 Inspector 中設定 0, 1, 2, 3

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
        // 通知 InputManager：一個 "Press" 開始了 (可能是 Tap 或 Hold)
        inputManager.OnLanePress(laneIndex);
    }

   /* public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStartPosition = eventData.position;
        // 通知 InputManager：這確定是一個 Drag/Swipe，請取消 Tap/Hold 狀態
        inputManager.OnLaneDragStart(laneIndex);
    }

    // (IDragHandler 必須存在才能讓 OnEndDrag 運作，但我們不需要在裡面寫程式碼)
    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 這是 Drag/Swipe 的結束
        inputManager.OnLaneSwipeEnd(laneIndex, dragStartPosition, eventData.position);
        isDragging = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 如果這個 Up 事件 *不是* Drag 的結尾 (表示這是一個單純的 Tap/Hold)
        if (!isDragging)
        {
            // 通知 InputManager： Tap/Hold 結束了
            inputManager.OnLaneRelease(laneIndex);
        }
        isDragging = false; // 重設狀態
    }*/
}