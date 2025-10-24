using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IObjectPool
{
	[SerializeField] protected string objectPoolName;
	[SerializeField] protected float noteSpeed = .1f;
	[SerializeField] protected float noteTime;
	[SerializeField] protected int noteLaneNum;
	protected Judge judge;
	protected InputManager inputManager;
	protected bool added;
	protected bool isFinished;

	public virtual void OnEnable()
	{
		inputManager = InputManager.GetInstance();
		judge = Judge.GetInstance();
		added = false;
		isFinished = false;
	}

	public virtual void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * noteSpeed;

		if (Mathf.Abs(judge.GetCurrentTime() - noteTime) <= .2f && !added) 
		{
			AddToPendingNotes();			
		}
		else if (judge.GetCurrentTime() > noteTime + .2f) //  超過應該敲擊 Note 的時間 0.2 秒還沒輸入，就視為 Miss
		{
			Miss();
			//isFinished = true;
		}
	}

	public void InitNote(float speed, float time, int laneNum) 
	{
		noteSpeed = speed;
		noteLaneNum = laneNum;
		noteTime = time;
	}

	public int GetLaneNumber() { return noteLaneNum; }

	public virtual int Judgement(float currentTime, bool isHolding)
	{
		float timeLag = Mathf.Abs(currentTime - noteTime);
		Debug.Log(timeLag);

		if (timeLag >= .2f) { Debug.LogError("pendingNotes 出現>= .2f的Note"); return 4; }
		//judge.RemovePendingNotes(this);
		isFinished = true;
		ReturnToPool();

		if (timeLag <= .1f) { return 0; }
		else if (timeLag <= .15f) { return 1; }
		else{ return 2; }
		//else { return 3; }
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

	protected void Miss() 
	{
		judge.Message(3, noteLaneNum);		
		//judge.RemovePendingNotes(this);
		judge.CalculateCombo(3);
		isFinished = true;
		ReturnToPool();		
		Debug.Log("Miss");
	}

	public bool IsFinished() { return isFinished; }
}
