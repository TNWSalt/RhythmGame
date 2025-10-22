using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : Note
{
	[SerializeField] private bool isHead;
	[SerializeField] private LineRenderer line;
	[SerializeField] private Transform tailTransform;
	[SerializeField] private bool isHolding, startHolding;

	public override void OnEnable()
	{
		base.OnEnable();
		isHolding = false;
		isHead = false;
		startHolding = false;
	}

	public override void Update()
	{
		if (!isHolding && !startHolding)
		{
			base.Update();
		}
		else if (isHolding && startHolding) 
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			Holding();
		}
		else if (!isHolding && startHolding)
		{
			judge.Message(3, noteLaneNum);
			judge.RemovePendingNotes(this);
			ReturnToPool();
			Debug.Log("Miss");
		}

		if (!isHead || tailTransform == null) { return; }
		line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
		line.SetPosition(1, new Vector3(tailTransform.position.x, tailTransform.position.y, tailTransform.position.z - .1f));
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
		}
		else 
		{
			isHolding = false;
			Debug.Log(isHolding);
		}
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

	private void Holding() 
	{
		if (Mathf.Abs(tailTransform.position.z - transform.position.z) <= .2f) 
		{
			Debug.Log("success");
			//judge.Message(0, noteLaneNum);
			judge.RemovePendingNotes(this);
			ReturnToPool();

			var tailNote = tailTransform.GetComponent<Note>();
			judge.RemovePendingNotes(tailNote);
			tailNote.ReturnToPool();			
		}
	}

	public void SetTail(Transform tail) 
	{
		Debug.Log("set tail");
		tailTransform = tail;
		line.positionCount = 2;
		isHead = true;				
	}
}
