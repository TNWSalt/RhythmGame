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
}