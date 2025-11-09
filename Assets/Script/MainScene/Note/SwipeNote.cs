using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeNoteType
{
    Any,
    Left,
    Right
}

public class SwipeNote : Note
{
    // (可選) 箭頭圖示
    // [SerializeField] private GameObject visual_Any;
    // [SerializeField] private GameObject visual_Left;
    // [SerializeField] private GameObject visual_Right;

    public SwipeNoteType requiredType { get; private set; }

    public void Initialize(float speed, float time, int laneNum, SwipeNoteType type)
    {
        base.InitNote(speed, time, laneNum); // 呼叫 Note.InitNote
        this.requiredType = type;

        // (在這裡根據 type 顯示不同圖示)
        // visual_Left.SetActive(type == SwipeNoteType.Left);
        // visual_Right.SetActive(type == SwipeNoteType.Right);
        // visual_Any.SetActive(type == SwipeNoteType.Any);
    }

    public override int Judgement(float currentTime, bool isHolding)
    {
        return -1; // -1 代表忽略 SwipeNote 對 Press/Release 沒有反應
    }

    public override void OnCancelPress()
    {
        // 沒作用 SwipeNote 不能被取消
    }

    public override void Update()
    {
        if (PauseManager.GetInstance().isPause) { return; }

        transform.position -= transform.forward * Time.deltaTime * noteSpeed;

        if (Mathf.Abs(judge.timer - noteTime) <= .2f && !added)
        {
            AddToPendingNotes();
        }
        else if (judge.timer > noteTime + .2f)
        {
            if (isFinished) { return; }
            OnMiss();
        }
    }

    /// <summary>
    /// SwipeNote 專用的 Judgement，由 Judge.JudgementSwipeNote 呼叫
    /// </summary>
    /// <returns>0-2 (成功), 4 (Miss)</returns>
    public int JudgementSwipe(float currentTime, SwipeDirection inputDirection)
    {
        if (isJudged) { return -1; } // 判過了

        float timeLag = Mathf.Abs(currentTime - noteTime);

        // 1. 判定時間 (Miss)
        if (timeLag >= .2f)
        {
            isFinished = true;
            ReturnToPool();
            return 4; // Miss (判定範圍外)
        }

        // 2. 判定方向
        bool directionMatch = false;
        switch (requiredType)
        {
            case SwipeNoteType.Any:
                // 任意滑動 Note 接受 "任何" 方向 (Up, Down, Left, Right)
                directionMatch = true;
                break;

            case SwipeNoteType.Left:
                directionMatch = (inputDirection == SwipeDirection.Left);
                break;

            case SwipeNoteType.Right:
                directionMatch = (inputDirection == SwipeDirection.Right);
                break;
        }

        isJudged = true; // 標記
        isFinished = true;
        ReturnToPool(); // 滑動 Note 立即摧毀

        // 3. 檢查方向是否匹配
        if (!directionMatch)
        {
            return 4; // Miss (方向錯誤)
        }

        // 4. 檢查時間 (方向正確)
        if (timeLag <= .1f) { return 0; }
        else if (timeLag <= .15f) { return 1; }
        else { return 2; }
    }
}