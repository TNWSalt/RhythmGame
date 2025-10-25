using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenu : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    private float scrollPos = 0;
    private float[] pos;
	[SerializeField] private int currentFocuseIndex;

	public int GetFocuseIndex() { return currentFocuseIndex; }

	private void Update()
	{
		pos = new float[transform.childCount];
		float distance = 1f / (pos.Length - 1f);
		for (int i = 0; i < pos.Length; i++)
		{
			pos[i] = distance * i;
		}

		if (Input.GetMouseButton(0))
		{
			scrollPos = scrollbar.value;
		}
		else 
		{
			for (int i = 0; i < pos.Length; i++)
			{
				if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2)) 
				{
					scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], .1f);
				}
			}
		}

		for (int i = 0; i < pos.Length; i++)
		{
			if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
			{
				transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), .1f);
				currentFocuseIndex = i;

				for (int j = 0; j < pos.Length; j++)
				{
					if(j != i) { transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(.8f, .8f), .1f); }					
				}
			}
		}
	}
}
