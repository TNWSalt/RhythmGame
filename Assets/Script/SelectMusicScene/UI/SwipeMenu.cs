using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 1. 引用介面

// 2. 這個腳本現在必須和 ScrollRect 在同一個 GameObject 上
[RequireComponent(typeof(ScrollRect))]
public class SwipeMenu : MonoBehaviour, IBeginDragHandler, IEndDragHandler // 3. 實作介面
{
    // 4. (重要) 不再需要手動指定 Scrollbar，但仍需 Content
    [SerializeField] private Transform content;

    private ScrollRect scrollRect; // 5. 我們的主要控制對象
    private float[] pos;
    private float distance;

    private float posDistance; // 目標位置 (0-1)
    public int currentPosIndex { get; private set; }
    //public int GetFocuseIndex() { return currentPosIndex; }

    private bool isDragging = false; // 狀態：是否正在被使用者拖曳

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>(); // 6. 自動取得 ScrollRect

        // 7. (關鍵) 關閉 ScrollRect 的慣性，否則會和我們的 Lerp 衝突
        scrollRect.inertia = false;
    }

    // (由 SongSelectMenu.Start() 呼叫)
    public void InitializeSwipeMenu()
    {
        if (content.childCount == 0)
        {
            Debug.LogWarning("SwipeMenu 初始化失敗：content 為空！");
            pos = new float[0];
            return;
        }

        if (content.childCount == 1)
        {
            pos = new float[1];
            pos[0] = 0;
            distance = 0;
        }
        else
        {
            // 8. 依據 content 的子物件數量來初始化
            pos = new float[content.childCount];
            distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }
        }

        currentPosIndex = 0;
        if (pos.Length > 0)
        {
            posDistance = pos[currentPosIndex];
        }
        isDragging = false;

        // 9. (修改) 直接設定 ScrollRect 的標準化位置
        //    假設你是水平滑動
        scrollRect.horizontalNormalizedPosition = posDistance;
    }

    void Update()
    {
        // 10. 如果使用者沒有在拖曳，才執行自動對齊
        if (!isDragging)
        {
            // (我們使用 horizontalNormalizedPosition，假設是水平滑動)
            // (如果 scrollRect 的 value 和目標點 已經很接近了，就不要再 Lerp)
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - posDistance) > 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, posDistance, 0.1f);
            }
            else
            {
                // (足夠接近時，直接吸附)
                scrollRect.horizontalNormalizedPosition = posDistance;
            }
        }
    }

    // 11. (重要) 這個函數現在會被 ScrollRect 正確觸發
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // 12. (重要) 這個函數現在會被 ScrollRect 正確觸發
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // 當拖曳結束時，計算最近的目標點
        if (pos.Length > 0)
        {
            posDistance = pos[FindCurrentPosIndex()];
        }
    }

    // 13. (修改) 幫助函數，從 ScrollRect 讀取位置
    int FindCurrentPosIndex()
    {
        // (使用 horizontalNormalizedPosition，假設是水平滑動)
        float currentPos = scrollRect.horizontalNormalizedPosition;

        float minDistance = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < pos.Length; i++)
        {
            float dist = Mathf.Abs(currentPos - pos[i]);
            if (dist < minDistance)
            {
                minDistance = dist;
                index = i;
            }
        }
        currentPosIndex = index; // (順便更新當前 index)
        return index;
    }

    // 14. (修改) 按鈕功能現在會設定目標點，讓 Update() 去 Lerp
    public void Next()
    {
        if (currentPosIndex < pos.Length - 1)
        {
            isDragging = false; // 強制停止拖曳狀態
            currentPosIndex++;
            posDistance = pos[currentPosIndex];
        }
    }

    public void Previous()
    {
        if (currentPosIndex > 0)
        {
            isDragging = false; // 強制停止拖曳狀態
            currentPosIndex--;
            posDistance = pos[currentPosIndex];
        }
    }
}