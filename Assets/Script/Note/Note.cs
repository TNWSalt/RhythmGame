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

	public virtual void OnEnable()
	{
		inputManager = InputManager.GetInstance();
		judge = Judge.GetInstance();
		added = false;
	}

	public virtual void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * noteSpeed;

		if (Mathf.Abs(judge.GetCurrentTime() - noteTime) <= .2f && !added) 
		{
			added = true;
			judge.AddPendingNotes(this);			
		}
		else if (judge.GetCurrentTime() > noteTime + .2f) //  超過應該敲擊 Note 的時間 0.2 秒還沒輸入，就視為 Miss
		{
			judge.Message(3, noteLaneNum);
			judge.RemovePendingNotes(this);
			judge.CalculateCombo(3);
			ReturnToPool();
			Debug.Log("Miss");
			// return pool
		}
	}

	public void InitNote(float speed, float time, int laneNum) 
	{
		noteSpeed = speed;
		noteLaneNum = laneNum;
		noteTime = time;
	}

	public int GetLaneNumber() { return noteLaneNum; }

	/*public virtual int Judgement(float currentTime) 
	{
		float timeLag = Mathf.Abs(currentTime - noteTime);
		Debug.Log(timeLag);
		judge.RemovePendingNotes(this);
		ReturnToPool();
		switch (timeLag) 
		{
			case <=.1f:
				return 0;
			case <=.15f:
				return 1;
			case <=.2f:
				return 2;
			default:
				return 3;				
		}		
	}*/

	public virtual int Judgement(float currentTime, bool isHolding)
	{
		float timeLag = Mathf.Abs(currentTime - noteTime);
		Debug.Log(timeLag);
		judge.RemovePendingNotes(this);
		ReturnToPool();
		switch (timeLag)
		{
			case <= .1f:
				return 0;
			case <= .15f:
				return 1;
			case <= .2f:
				return 2;
			default:
				return 3;
		}
	}

	public void ReturnToPool()
	{
		ObjectPoolManager.GetInstance().ReturnToPool(objectPoolName, gameObject);
	}
}
