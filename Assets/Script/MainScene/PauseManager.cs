using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public static PauseManager GetInstance() { return instance; }

    public bool isPause { get; private set; }
    
	[SerializeField] private TextMeshProUGUI countdownText;

	private void Awake()
	{
		if (instance != null) { Destroy(gameObject); return; }
		instance = this;
	}

	private void Update()
	{
		if (isPause) { return; }

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			Pause();			
		}
	}

	public void Continue() 
	{
		var text = Instantiate(countdownText, FindObjectOfType<UIManagerHandler>().gameObject.transform);

		StartCoroutine(ShowCountdownText(text));
	}

	private IEnumerator ShowCountdownText(TextMeshProUGUI text) 
	{
		for (int i = 3; i > 0; i--)
		{
			text.text = i.ToString();
			text.DOFade(0f, 1f);
			text.transform.DOScale(2, 1);

			yield return new WaitForSeconds(1f);

			text.DOFade(1, 0);
			text.transform.DOScale(1, 0);
		}

		Pause(false);
		Destroy(text);
	}

	public void Pause(bool pause = true) 
	{ 
		isPause = pause;

		if (isPause) { UIManager.GetInstance().ShowPanel("PausePanel"); }		
	}
}
