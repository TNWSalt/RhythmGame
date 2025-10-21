using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNWSalt
{
	public class UIManagerHandler : MonoBehaviour
	{
		private void Start()
		{
			UIManager.GetInstance().mainCanvasRoot = gameObject;
		}
	}
}