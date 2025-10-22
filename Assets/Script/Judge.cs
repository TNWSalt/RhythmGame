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

    [SerializeField] private int combo, maxCombo;
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
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;        
    }
    
    //  顯示對應的判定 UI（Perfect / Great / Bad / Miss）
    public void Message(int judgeResult, int noteLaneNum)
    {
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
		foreach (var note in pendingNotes)
		{
            if (note.GetLaneNumber() == laneNum) 
            {
                int result = note.Judgement(timer, isHolding);
                Message(result, note.GetLaneNumber());
                CalculateCombo(result);
            }
		}
    }

    public void CalculateCombo(int result) 
    {
        if (result >= 2) { combo = 0; }
        else
        {
            combo++;
            if (combo >= maxCombo) { maxCombo = combo; }
        }
    }

    public float GetCurrentTime() { return timer; }
}