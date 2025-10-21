using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : Note
{
	[SerializeField] private bool isHead;
	[SerializeField] private LineRenderer line;
	[SerializeField] private Transform tailTransform;

	public override void Start()
	{
		base.Start();		
	}

	public override void Update()
	{
		//base.Update();
		if (!isHead || tailTransform == null) { return; }
		line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
		line.SetPosition(1, new Vector3(tailTransform.position.x, tailTransform.position.y, tailTransform.position.z - .1f));
	}

	public void SetTail(Transform tail) 
	{
		Debug.Log("set tail");
		tailTransform = tail;
		line.positionCount = 2;
		isHead = true;				
	}
}
