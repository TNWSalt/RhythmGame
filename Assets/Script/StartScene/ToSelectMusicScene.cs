using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToSelectMusicScene : MonoBehaviour
{
	private void Update()
	{
		if (Input.anyKey) 
		{
			SceneController.GetInstance().LoadNextScene();
		}
	}
}
