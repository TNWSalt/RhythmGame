using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : Note
{
	[SerializeField] private bool isHead;
	[SerializeField] private LineRenderer line;
	[SerializeField] private Transform tailTransform;

	[SerializeField] public bool isHolding { get; private set; }
	[SerializeField] private bool startHolding;

	private bool tailJudged;
	// (pendingJudgeResult 已在父類 Note.cs)

	public override void OnEnable()
	{
		base.OnEnable(); // 呼叫父類 OnEnable
		isHolding = false;
		isHead = false;
		startHolding = false;
		tailJudged = false;
	}

	public override void Update()
	{
		if (PauseManager.GetInstance().isPause) { return; }

		if (!isHolding && !startHolding)
		{
			transform.position -= transform.forward * Time.deltaTime * noteSpeed;
			if (isHead)
			{
				if (Mathf.Abs(judge.timer - noteTime) <= .2f && !added)
				{
					AddToPendingNotes();
				}
			}
		}
		else if (isHolding && startHolding)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		}

		if (!isHead || tailTransform == null) { return; }
		line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
		line.SetPosition(1, new Vector3(tailTransform.position.x, tailTransform.position.y, tailTransform.position.z - .1f));

	}

	public override int Judgement(float currentTime, bool holding)
	{
		float timeLag = Mathf.Abs(currentTime - noteTime);

		if (holding) // --- PRESS (按下) ---
		{
			if (timeLag >= .2f)
			{
				isFinished = true;
				ReturnToPool();
				if (tailTransform != null) { tailTransform.GetComponent<HoldNote>().ReturnToPool(); }
				return 4;
			}

			isHolding = true;
			startHolding = true;
			isJudged = true;

			// (關鍵) isFinished 必須為 false
			isFinished = false;

			if (timeLag <= .1f) { pendingJudgeResult = 0; }
			else if (timeLag <= .15f) { pendingJudgeResult = 1; }
			else { pendingJudgeResult = 2; }

			return pendingJudgeResult;
		}
		else // --- RELEASE (放開) ---
		{
			isHolding = false;
			return EndJudge();
		}
	}

	private int EndJudge()
	{
		Debug.LogWarning("EndJudge");
		var tailNote = tailTransform.GetComponent<HoldNote>();
		float timeLag = Mathf.Abs(tailNote.noteTime - judge.timer);
		Debug.Log(timeLag);

		int result = 3;

		if (timeLag >= .2f)
		{
			result = 3;
			Debug.Log("Hold tail missed (>= .2f)");
		}
		else if (timeLag <= .2f)
		{
			if (timeLag <= .1f) { result = 0; }
			else if (timeLag <= .15f) { result = 1; }
			else { result = 2; }
			Debug.Log("Hold success (" + result + ")");
		}

		isFinished = true; // HoldNote 在此結束
		tailJudged = true;
		ReturnToPool();
		tailNote.ReturnToPool();

		return result;
	}

	public void SetTail(Transform tail)
	{
		Debug.Log("set tail");
		tailTransform = tail;
		line.positionCount = 2;
		isHead = true;
	}

	public void SetIsHoldingToFalse() { isHolding = false; }

	public override void OnCancelPress()
	{
		if (!isJudged) { return; }	
		Debug.Log("HoldNote.OnCancelPress() was called. This shouldn't happen, but we are ignoring it.");		
	}

	public override void OnMiss()
	{
		base.OnMiss();

		if (tailTransform != null)
		{
			// 確保 Tail 也被移除和銷毀
			var tailNote = tailTransform.GetComponent<HoldNote>();
			if (tailNote != null)
			{
				judge.RemovePendingNotes(tailNote); // (安全) 在 Judge.cs 之外呼叫 Remove(object) 是安全的
				tailNote.ReturnToPool();
			}
		}
	}
}