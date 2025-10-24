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
    [SerializeField] private int combo, maxCombo;
    [SerializeField] private int[] resultCount = new int[4];

    [Header("����")]
    [SerializeField] private int totalScore;
    [SerializeField] private int currentScore;
    [SerializeField] private int finalScore;

    [Header("�p�ɾ�")]
    [SerializeField] float timer;
    [SerializeField] JudgeTxetMessage[] judgeMessage;
    [SerializeField] GameObject messagePrefab;
    private ObjectPoolManager poolManager;
    [SerializeField] private List<Note> pendingNotes;

	private void Awake()
	{
        if (instance != null) { return; }
        instance = this;
	}

	private void Start()
    {
        poolManager = ObjectPoolManager.GetInstance();
        finalScore = 0;
        currentScore = 0;
        totalScore = 0;
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;        
    }
    
    //  ��ܹ������P�w UI�]Perfect / Great / Bad / Miss�^
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

            if (note.GetLaneNumber() != laneNum) { continue; }

            int result = note.Judgement(timer, isHolding);
            Debug.Log(result);
            if (result == 4) { continue; }
            AddResult(result);
            CalculateScore(result);
            Message(result, note.GetLaneNumber());
            CalculateCombo(result);

            // �p�G���ųQ�P�w���]�p PERFECT, MISS�^�A�N���X
            if (note.IsFinished())
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

    public float GetCurrentTime() { return timer; }

    public void AddResult(int result)
    {
        if (result >= 0 && result < resultCount.Length)
        {
            resultCount[result]++;
            Debug.Log($"Result {result} ���ơG{resultCount[result]}");
        }
        else
        {
            Debug.LogWarning($"���G {result} �W�X�d��]0~{resultCount.Length - 1}�^");
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

    public int GetFinalScore() { return finalScore; }
    public int GetMaxCombo() { return maxCombo; }
}