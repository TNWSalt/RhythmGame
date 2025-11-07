using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DifficultyData 
{	
	public int diff;
	public TextAsset chart;
}

public enum GameDifficulty
{
	Easy,
	Normal,
	Hard,
	Expert
}

public class DifficultySelection : MonoBehaviour
{
    [SerializeField] private Transform leftSide, rightSide, diffContent;
	[SerializeField] private SongData selectedData;

	[SerializeField] private TextMeshProUGUI songName, songDescription, vocal;
	[SerializeField] private int diffCount;

	[SerializeField, Header("Panel Alpha")] private float alpha;
	private Image panel;

	[Header("難度選擇")]
	[SerializeField] private List<Button> buttons;                     // UI 按鈕列表
	[SerializeField] private Color selectedColor = Color.green;        // 被選中時顏色
	[SerializeField] private Color normalColor = Color.white;          // 未選中顏色
	[SerializeField] private GameDifficulty currentDifficulty;         // 目前難度

	private void OnEnable()
	{		
		panel.DOFade(alpha, 0f);
		leftSide.DOLocalMove(new Vector2(0, -2000), 0);
		rightSide.DOLocalMove(new Vector2(0, -2000), 0);
		StartCoroutine(Enable());

		// 綁定按鈕事件
		for (int i = 0; i < buttons.Count; i++)
		{
			int index = i; // 防止閉包問題
			buttons[i].onClick.AddListener(() => OnDifficultySelected(index));
		}		
		// 預設選擇第一個（或可改）
		SetDifficulty(GameDifficulty.Easy);
	}

	private void Start()
	{
		panel.DOFade(alpha, 0f);
		for (int i = 0; i < diffContent.childCount; i++)
		{
			diffContent.GetChild(i).gameObject.SetActive(false);
		}
		leftSide.DOLocalMove(new Vector2(0, -2000), 0);
		rightSide.DOLocalMove(new Vector2(0, -2000), 0);
		gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		for (int i = 0; i < diffContent.childCount; i++)
		{
			diffContent.GetChild(i).gameObject.SetActive(false);
		}
	}

	private IEnumerator Enable() 
	{
		leftSide.DOLocalMoveY(0, .5f).SetEase(Ease.OutSine);
		rightSide.DOLocalMoveY(0, .5f).SetEase(Ease.OutSine);

		yield return new WaitForSeconds(.6f);

		leftSide.DOLocalMoveX(-350, .5f).SetEase(Ease.OutSine);
		rightSide.DOLocalMoveX(450, .5f).SetEase(Ease.OutSine);
	}

	private IEnumerator Disable()
	{
		leftSide.DOLocalMoveX(0, .5f).SetEase(Ease.OutSine);
		rightSide.DOLocalMoveX(0, .5f).SetEase(Ease.OutSine);		

		yield return new WaitForSeconds(.6f);

		leftSide.DOLocalMoveY(-2000, .5f).SetEase(Ease.OutSine);
		rightSide.DOLocalMoveY(-2000, .5f).SetEase(Ease.OutSine);

		yield return new WaitForSeconds(.2f);

		panel.DOFade(0, .3f);
		gameObject.SetActive(false);
	}

	public void Init(SongData data) 
	{
		selectedData = data;

		songName.text = selectedData.name;
		songDescription.text = selectedData.description;
		vocal.text = selectedData.vocal;
		diffCount = selectedData.difficultyDatas.Count;
		currentDifficulty = GameDifficulty.Easy;

		for (int i = 0; i < diffCount; i++)
		{
			var child = diffContent.GetChild(i);
			child.gameObject.SetActive(true);
			child.GetComponentInChildren<TextMeshProUGUI>().text = selectedData.difficultyDatas[i].diff.ToString();
		}
		if (diffCount == 0) { GameObject.Find("StartButton").GetComponent<Button>().interactable = false; return; }
		else { GameObject.Find("StartButton").GetComponent<Button>().interactable = true; }
		GameManager.GetInstance().SetSongData(selectedData, (int)currentDifficulty);
	}	

	public void ClickBackButton() 
	{
		StartCoroutine(Disable());
	}

	public void ClickStartButton() 
	{
		GameManager.GetInstance().SetSongData(selectedData, (int)currentDifficulty);
		SceneController.GetInstance().LoadNextScene();
	}

	private void OnDifficultySelected(int index)
	{
		GameDifficulty difficulty = (GameDifficulty)index;
		SetDifficulty(difficulty);
	}

	private void SetDifficulty(GameDifficulty difficulty)
	{
		currentDifficulty = difficulty;

		// 更新按鈕外觀
		for (int i = 0; i < buttons.Count; i++)
		{
			var colors = buttons[i].colors;
			colors.normalColor = (i == (int)difficulty) ? selectedColor : normalColor;
			buttons[i].colors = colors;
		}

		// 這裡可以呼叫遊戲邏輯（例如通知 GameManager）
		Debug.Log("難度設定為：" + currentDifficulty);				
	}
}