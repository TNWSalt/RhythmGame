/*using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JudgeTxetMessage
{
public string judgeText;
public Color textColor;
}

public class Judge : MonoBehaviour
{
    public static Judge instance;
    public static Judge GetInstance() { return instance; }

    [Header("Combo")]
    [SerializeField] private int combo;
    public int maxCombo { get; private set; }
    [SerializeField] private int[] resultCount = new int[4];

    [Header("分數")]
    [SerializeField] private int totalScore;
    [SerializeField] private int currentScore;
    public int finalScore { get; private set; }
  
    [SerializeField] JudgeTxetMessage[] judgeMessage;
    [SerializeField] GameObject messagePrefab;    
    [SerializeField] private List<Note> pendingNotes;
    public float timer { get; private set; }

    private Note[] pendingPressNote = new Note[4];

    private ObjectPoolManager poolManager;
    private GameManager gameManager;

    private void Awake()
	{
        if (instance != null) { return; }
        instance = this;
	}

	private void Start()
    {
        poolManager = ObjectPoolManager.GetInstance();
        gameManager = GameManager.GetInstance();
        finalScore = 0;
        currentScore = 0;
        totalScore = 0;
        timer = 0;
    }

    void Update()
    {
        if (PauseManager.GetInstance().isPause) { return; }
        timer += Time.deltaTime;        
    }
    
    //  顯示對應的判定 UI（Perfect / Great / Bad / Miss）
    public void Message(int judgeResult, int noteLaneNum)
    {
        Debug.Log(judgeResult);
        var text = poolManager.SpwanFromPool(messagePrefab.name,
            new Vector3(noteLaneNum - 1.5f, 0.76f, 0.15f),
            Quaternion.Euler(45, 0, 0)).GetComponent<JudgeText>();
        text.SetText(judgeMessage[judgeResult]);
    }

    public void AddPendingNotes(Note note)
    {
        pendingNotes.Add(note);
    }

    public void RemovePendingNotes(Note note)
    {
        pendingNotes.Remove(note);
    }

    public void JudgementNote(int laneNum, bool isHolding) 
    {
        for (int i = pendingNotes.Count - 1; i >= 0; i--)
        {
            var note = pendingNotes[i];

            if (note.noteLaneNum != laneNum) { continue; }

            int result = note.Judgement(timer, isHolding);
            Debug.Log(result);
            if (result == 4) { continue; }
            AddResult(result);
            CalculateScore(result);
            Message(result, note.noteLaneNum);
            CalculateCombo(result);

            // 如果音符被判定完（如 PERFECT, MISS），就移出
            if (note.isFinished)
            {
                pendingNotes.RemoveAt(i);
            }
        }
    }

    public void CalculateCombo(int result) 
    {
        if (result >= 2) 
        {
            combo = 0;           
        }
        else
        {
            combo++;            
            if (combo >= maxCombo) { maxCombo = combo; }
        }
        FindObjectOfType<ComboText>().SetCombo(combo);
    }

    public void AddResult(int result)
    {
        if (result >= 0 && result < resultCount.Length)
        {
            resultCount[result]++;
            Debug.Log($"Result {result} 次數：{resultCount[result]}");
        }
        else
        {
            Debug.LogWarning($"結果 {result} 超出範圍（0~{resultCount.Length - 1}）");
        }
    }

    public int[] GetJudgeResult() { return resultCount; }

    public void CalculateScore(int result)
    {
        if (result == 0)
        {
            currentScore += 3;
        }
        else if (result == 1)
        {
            currentScore += 2;
        }
        else if (result == 2)
        {
            currentScore++;
        }

        float s = (currentScore / (float)totalScore) * 1000000;
        finalScore = Mathf.RoundToInt(s);
        FindObjectOfType<ScoreCalculation>().SetText(finalScore.ToString());
        //scoreText.text = score.ToString();
    }

    public void CalculateTotalScore(int noteNum)
    {
        totalScore = noteNum * 3;
    }
}*/

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JudgeTxetMessage
{
    public string judgeText;
    public Color textColor;
}

public class Judge : MonoBehaviour
{
    public static Judge instance;
    public static Judge GetInstance() { return instance; }

    [Header("Combo")]
    [SerializeField] private int combo;
    public int maxCombo { get; private set; }
    [SerializeField] private int[] resultCount = new int[4];

    [Header("分數")]
    [SerializeField] private int totalScore;
    [SerializeField] private int currentScore;
    public int finalScore { get; private set; }

    [SerializeField] JudgeTxetMessage[] judgeMessage;
    [SerializeField] GameObject messagePrefab;
    [SerializeField] private List<Note> pendingNotes;
    public float timer { get; private set; }

    // (你已有的變數，用來儲存 "預判" 的 Note)
    [SerializeField] private Note[] pendingPressNote = new Note[4];

    private ObjectPoolManager poolManager;
    private GameManager gameManager;

    // (Awake, Start 保持不變)
    #region Unchanged Methods
    private void Awake()
    {
        if (instance != null) { return; }
        instance = this;
    }

    private void Start()
    {
        poolManager = ObjectPoolManager.GetInstance();
        gameManager = GameManager.GetInstance();
        finalScore = 0;
        currentScore = 0;
        totalScore = 0;
        timer = 0;

        // (新) 初始化 pendingPressNote
        pendingPressNote = new Note[4];
    }
    #endregion

    void Update()
    {
        if (PauseManager.GetInstance().isPause) { return; }
        timer += Time.deltaTime;

        for (int i = pendingNotes.Count - 1; i >= 0; i--)
        {
            if (i >= pendingNotes.Count) { continue; }

            var note = pendingNotes[i];

            // (你需要 Note.cs 有 noteTime 屬性)
            // (我們假設 0.2f 是你的 Miss 判定點)
            float missTimeThreshold = 0.2f; 
            
            // 檢查是否超時
            if (note.noteTime + missTimeThreshold <= timer)
            {
                if (note.isFinished)
                {
                    // Note 已經被 JudgementNote(false) 處理並銷毀
                    // 它不應該在這裡，但我們還是安全地移除它
                    pendingNotes.RemoveAt(i);
                    continue; 
                }

                // 檢查 Note 是否正在被按住 (HoldNote)
                if (note is HoldNote hn && hn.isHolding)
                {
                    // 正在長按，不要判定它 Miss
                    continue; 
                }

                // --- 如果執行到這裡，表示 Note 確實 Miss 了 ---

                // 1. 呼叫 Note 的 OnMiss 函數 (它會處理銷毀和特效)
                note.OnMiss();

                // 2. (安全) 從列表中移除
                //    因為 OnMiss() 函數不再修改 pendingNotes 列表，
                //    所以在這裡 RemoveAt(i) 是 100% 安全的。
                pendingNotes.RemoveAt(i);
            }
        }
    }

    //輔助函數：從 pendingNotes 中找到指定軌道上最接近的 Note
    private Note FindClosestNoteInLane(int laneNum, Type noteType = null)
    {
        Note closestNote = null;
        float minTimeDiff = float.MaxValue;
        float missTimeThreshold = 0.2f; // (這個值應與 Note.Judgement 匹配)

        foreach (Note note in pendingNotes)
        {
            if (note.noteLaneNum != laneNum) { continue; }
            if (noteType != null && note.GetType() != noteType) { continue; }

            float timeDiff = Mathf.Abs(note.noteTime - timer);
            if (timeDiff < minTimeDiff)
            {
                minTimeDiff = timeDiff;
                closestNote = note;
            }
        }

        // 如果最接近的 Note 在判定範圍內
        if (minTimeDiff <= missTimeThreshold)
        {
            return closestNote;
        }
        return null;
    }

    public void Message(int judgeResult, int noteLaneNum)
    {
        Debug.Log(judgeResult);
        var text = poolManager.SpwanFromPool(messagePrefab.name,
            new Vector3(noteLaneNum - 1.5f, 1f, 0.15f),
            Quaternion.Euler(45, 0, 0)).GetComponent<JudgeText>();
        text.SetText(judgeMessage[judgeResult]);
    }

    public void AddPendingNotes(Note note)
    {
        pendingNotes.Add(note);
    }

    public void RemovePendingNotes(Note note)
    {
        pendingNotes.Remove(note);
    }

    public void CalculateCombo(int result)
    {
        if (result >= 2) // (假設 2=Bad, 3=Miss(如果有的話), 4=Miss)
        {
            combo = 0;
        }
        else
        {
            combo++;
            if (combo >= maxCombo) { maxCombo = combo; }
        }
        FindObjectOfType<ComboText>().SetCombo(combo);
    }

    public void AddResult(int result)
    {
        if (result >= 0 && result < resultCount.Length)
        {
            resultCount[result]++;
        }
        else
        {
            Debug.LogError($"結果 {result} 超出範圍（0~{resultCount.Length - 1}）");
        }
    }

    public int[] GetJudgeResult() { return resultCount; }

    public void CalculateScore(int result)
    {
        if (result == 0)
        {
            currentScore += 3;
        }
        else if (result == 1)
        {
            currentScore += 2;
        }
        else if (result == 2)
        {
            currentScore++;
        }

        float s = (currentScore / (float)totalScore) * 1000000;
        finalScore = Mathf.RoundToInt(s);
        FindObjectOfType<ScoreCalculation>().SetText(finalScore.ToString());
    }

    public void CalculateTotalScore(int noteNum)
    {
        totalScore = noteNum * 3;
    }

    /// <summary>
    /// 撤銷分數 (給 CancelPress 使用)
    /// </summary>
    public void RemoveResult(int result)
    {
        if (result >= 0 && result < resultCount.Length)
        {
            resultCount[result]--;
        }
    }

    /// <summary>
    /// 撤銷分數 (給 CancelPress 使用)
    /// </summary>
    public void RemoveScore(int result)
    {
        if (result == 0) { currentScore -= 3; }
        else if (result == 1) { currentScore -= 2; }
        else if (result == 2) { currentScore -= 1; }

        float s = (currentScore / (float)totalScore) * 1000000;
        finalScore = Mathf.RoundToInt(s);
        FindObjectOfType<ScoreCalculation>().SetText(finalScore.ToString());
    }

    /// <summary>
    /// 撤銷 Combo (給 CancelPress 使用)
    /// </summary>
    public void RemoveCombo()
    {
        combo--;
        if (combo < 0) combo = 0;
        FindObjectOfType<ComboText>().SetCombo(combo);
    }

    /// <summary>
    /// 處理 Press 和 Release 事件
    /// </summary>
    public void JudgementNote(int laneNum, bool isHolding)
    {
        if (isHolding) // --- PRESS (按下) ---
        {
            Note noteToJudge = FindClosestNoteInLane(laneNum);
            if (noteToJudge == null) { return; }

            // (新) 如果是 SwipeNote，Press 事件不應該觸發它
            if (noteToJudge is SwipeNote)
            {
                Debug.Log("Judge: Press ignored (SwipeNote detected)");
                return;
            }

            int result = noteToJudge.Judgement(timer, true);

            if (result >= 0 && result <= 2) // 0:Parfect, 1:Good, 2:Bad
            {
                if (!(noteToJudge is HoldNote)) //不存HoldNote
                {
                    
                }
                pendingPressNote[laneNum] = noteToJudge;

                AddResult(result);
                CalculateScore(result);
                Message(result, noteToJudge.noteLaneNum);
                CalculateCombo(result);

                if (noteToJudge.isFinished)
                {
                    pendingNotes.Remove(noteToJudge);
                }
            }
            else if (result == 4) // Miss
            {
                AddResult(result);
                CalculateCombo(result);
                Message(result, noteToJudge.noteLaneNum);
                if (noteToJudge.isFinished) // Miss 應該會 isFinished
                {
                    pendingNotes.Remove(noteToJudge);
                }
            }
        }
        else // --- RELEASE (放開) ---
        {
            Note noteToRelease = pendingPressNote[laneNum];

            Debug.LogWarning("RELEASE");

            // 情況 1: 正在釋放一個 "待處理" 的 TapNote
            if (noteToRelease != null && !(noteToRelease is HoldNote))
            {
                Debug.Log("Judge: Releasing pending TapNote");
                // 呼叫 Judgement(false) 讓 TapNote 銷毀 (需要 Note.cs 配合)
                noteToRelease.Judgement(timer, false);

                if (noteToRelease.isFinished) // (TapNote 應在此處 isFinished)
                {
                    pendingNotes.Remove(noteToRelease);
                }
                pendingPressNote[laneNum] = null;
            }
            else
            {
                // 情況 2: 釋放一個 HoldNote (或者沒有待處理 Note)
                HoldNote holdingNote = null;
                foreach (Note note in pendingNotes)
                {
                    if (note.noteLaneNum != laneNum) { continue; }
                    // (需要 HoldNote.cs 有 'isHolding' 狀態)
                    if (note is HoldNote hn && hn.isHolding)
                    {
                        holdingNote = hn;
                        Debug.LogWarning(holdingNote);
                        break;
                    }
                }

                if (holdingNote != null)
                {
                    int result = holdingNote.Judgement(timer, false); // 呼叫 HoldNote 的 Release

                    if (result >= 0 && result <= 4)
                    {
                        AddResult(result);
                        CalculateScore(result);
                        Message(result, holdingNote.noteLaneNum);
                        CalculateCombo(result);
                        if (holdingNote.isFinished) // HoldNote Release 應
                        {
                            pendingNotes.Remove(holdingNote);
                        }
                    }
                }
            }

            // 如果 "待處理" 的是 HoldNote，在此清除
            if (noteToRelease != null && noteToRelease is HoldNote)
            {
                pendingPressNote[laneNum] = null;
            }
        }
    }

    /// <summary>
    /// 由 InputManager 在 OnLaneDragStart 時呼叫
    /// </summary>
    public void CancelPress(int index)
    {
        Note noteToCancel = pendingPressNote[index];

        if (noteToCancel is HoldNote hn && hn.isHolding)
        {
            Debug.Log("CancelPress ignored: currently holding a HoldNote");
            return;
        }

        if (noteToCancel != null)
        {
            Debug.Log("Judge: Canceling Press on lane " + index);

            int resultToCancel = noteToCancel.pendingJudgeResult;

            noteToCancel.OnCancelPress();

            if (resultToCancel >= 0 && resultToCancel <= 2)
            {
                RemoveResult(resultToCancel);
                RemoveScore(resultToCancel);
                RemoveCombo();
            }

            // 4. 清除
            pendingPressNote[index] = null;
        }
    }

    /// <summary>
    /// (新) 由 InputManager 在 OnLaneSwipeEnd 時呼叫
    /// </summary>
    public void JudgementSwipeNote(int laneNum, SwipeDirection direction)
    {
        SwipeNote swipeNote = FindClosestNoteInLane(laneNum, typeof(SwipeNote)) as SwipeNote;
        if (swipeNote != null)
        {
            int result = swipeNote.JudgementSwipe(timer, direction);

            if (result >= 0 && result <= 4)
            {
                AddResult(result);
                CalculateScore(result);
                Message(result, swipeNote.noteLaneNum);
                CalculateCombo(result);

                if (!swipeNote.isFinished) { return; }
                pendingNotes.Remove(swipeNote);
            }
            return;
        }

        //Note holdNoteCandidate = FindClosestNoteInLane(laneNum, typeof(HoldNote));
        Note pendingNote = pendingPressNote[laneNum];
        Debug.Log(pendingNote);

        if (pendingNote is HoldNote holdNote)
        {
            Debug.Log("JudgementSwipeNote: HoldNote Release detected via swipe");

            // 呼叫 Release 判定
            JudgementNote(laneNum, false);
            // 這樣尾端就會被判定，並回收 HoldNote
        }
    }
}