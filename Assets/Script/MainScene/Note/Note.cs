using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IObjectPool
{
	[SerializeField] protected string objectPoolName;
	[SerializeField] protected float noteSpeed = .1f;
	public float noteTime { get; protected set; }
	public int noteLaneNum { get; protected set; }
	protected Judge judge;
	protected InputManager inputManager;
	protected bool added;
	public bool isFinished { get; protected set; }

	public int pendingJudgeResult { get; protected set; }
	protected bool isJudged = false;

	public virtual void OnEnable()
	{
		inputManager = InputManager.GetInstance();
		judge = Judge.GetInstance();
		added = false;
		isFinished = false;
		isJudged = false;
		pendingJudgeResult = -1;
	}

	// (修改) Update 只負責移動和 "添加"
	public virtual void Update()
	{
		if (PauseManager.GetInstance().isPause) { return; }

		transform.position -= transform.forward * Time.deltaTime * noteSpeed;

		if (Mathf.Abs(judge.timer - noteTime) <= .2f && !added)
		{
			AddToPendingNotes();
		}

	}

	public void InitNote(float speed, float time, int laneNum)
	{
		noteSpeed = speed;
		noteLaneNum = laneNum;
		noteTime = time;
	}

	public virtual int Judgement(float currentTime, bool isHolding)
	{
		if (isHolding) // --- PRESS (按下) ---
		{
			if (isJudged) return -1;
			float timeLag = Mathf.Abs(currentTime - noteTime);
			if (timeLag >= .2f)
			{
				Debug.LogError("pendingNotes 出現 >= .2f 的Note");
				isFinished = true;
				ReturnToPool();
				return 4; // Miss
			}

			isJudged = true;
			isFinished = false; // TapNote 按下時 "尚未" 結束

			if (timeLag <= .1f) { pendingJudgeResult = 0; }
			else if (timeLag <= .15f) { pendingJudgeResult = 1; }
			else { pendingJudgeResult = 2; }

			return pendingJudgeResult;
		}
		else // --- RELEASE (放開) ---
		{
			if (!isJudged) return -1;

			isFinished = true; // TapNote 在此處才算 "結束"
			ReturnToPool();
			return -1; // 分數已在 Press 時加過
		}
	}

	public void ReturnToPool()
	{
		ObjectPoolManager.GetInstance().ReturnToPool(objectPoolName, gameObject);
	}

	protected void AddToPendingNotes()
	{
		added = true;
		judge.AddPendingNotes(this);
	}

	public virtual void OnMiss()
	{
		judge.Message(3, noteLaneNum);
		judge.CalculateCombo(3);
		isFinished = true;
		ReturnToPool(); 
		Debug.Log("Note Missed (Handled by OnMiss)");
	}

	public virtual void OnCancelPress()
	{
		if (isJudged)
		{
			Debug.Log("Note Press Cancelled.");
			isJudged = false;
			isFinished = false;
			pendingJudgeResult = -1;
		}
	}
}