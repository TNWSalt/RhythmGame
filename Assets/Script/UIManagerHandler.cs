using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManagerHandler : MonoBehaviour
{
	private void Start()
	{
		UIManager.GetInstance().mainCanvasRoot = gameObject;
	}
}
