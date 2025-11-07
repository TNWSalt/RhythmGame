using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : Note
{
	[SerializeField] private bool isHead;
	[SerializeField] private LineRenderer line;
	[SerializeField] private Transform tailTransform;
	[SerializeField] private bool isHolding, startHolding;

	private bool tailJudged;

	public override void OnEnable()
	{
		base.OnEnable();
		isHolding = false;
		isHead = false;
		startHolding = false;
		tailJudged = false;
	}

	public override void Update()
	{
		if (!isHolding && !startHolding)
		{
			transform.position -= transform.forward * Time.deltaTime * noteSpeed;
			if (isHead)
			{
				if (Mathf.Abs(judge.timer - noteTime) <= .2f && !added)
				{
					AddToPendingNotes();
				}
				else if (judge.timer > noteTime + .2f) //  �W�L���ӺV�� Note ���ɶ� 0.2 ���٨S��J�A�N���� Miss
				{
					Miss();
					isFinished = true;
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

		if (!tailJudged && tailTransform != null)
		{
			var tailNote = tailTransform.GetComponent<HoldNote>();
			if (judge.timer > tailNote.noteTime + .2f)
			{
				judge.Message(3, noteLaneNum); // Miss
				judge.CalculateScore(3);
				ReturnToPool();
				tailNote.ReturnToPool();
				tailJudged = true;
				Debug.Log("Hold auto MISS");
			}
		}
	}

	public override int Judgement(float currentTime, bool holding)
	{
		float timeLag = Mathf.Abs(currentTime - noteTime);
		Debug.Log(timeLag);
		//judge.RemovePendingNotes(this);
		//ReturnToPool();
		if (holding)
		{
			isHolding = true;
			startHolding = true;

			if (timeLag <= .1f) { return 0; }
			else if (timeLag <= .15f) { return 1; }
			else { return 2; }
		}
		else
		{
			isHolding = false;
			return EndJudge();
		}
	}

	private int EndJudge()
	{
		var tailNote = tailTransform.GetComponent<HoldNote>();
		float timeLag = Mathf.Abs(tailNote.noteTime - judge.timer);
		Debug.Log(timeLag);

		if (timeLag >= .2f)
		{
			judge.Message(3, noteLaneNum); // miss
			ReturnToPool();
			tailNote.ReturnToPool();
			Debug.Log("Hold tail missed");
			isFinished = true;
			return 3;
		}
		else if (timeLag <= .2f) 
		{
			int result;
			if (timeLag <= .1f) { result = 0; }
			else if (timeLag <= .15f) { result = 1; }
			else { result = 2; }

			judge.Message(result, noteLaneNum);
			ReturnToPool();
			tailNote.ReturnToPool();
			Debug.Log("Hold success (" + result + ")");
			isFinished = true;
			return result;
		}
		else if (tailNote.noteTime < judge.timer - .2f)
		{
			judge.Message(3, noteLaneNum); // miss
			ReturnToPool();
			tailNote.ReturnToPool();
			Debug.Log("Hold tail missed");
			isFinished = true;
			return 3;
		}

		isFinished = true;
		Debug.LogError("judge Worng!!!");
		return 3;
	}

	public void SetTail(Transform tail)
	{
		Debug.Log("set tail");
		tailTransform = tail;
		line.positionCount = 2;
		isHead = true;
	}

	public void SetIsHoldingToFalse() { isHolding = false; }
}