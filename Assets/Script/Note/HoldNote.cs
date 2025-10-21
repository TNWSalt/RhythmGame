using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : Note
{
	[SerializeField] private LineRenderer line;
	[SerializeField] private Transform tailTransform;

	public override void Update()
	{
		//base.Update();
		line.SetPosition(0, new Vector3(transform.position.x, transform.position.y -.1f, transform.position.z));
		line.SetPosition(1, new Vector3(tailTransform.position.x, tailTransform.position.y - .1f, tailTransform.position.z+.1f));
	}
}
