using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndPanel : BasePanel
{
	[SerializeField] TextMeshProUGUI score, combo, prefact, good, bad, miss;

	public void OnEnable()
	{
		var result = Judge.GetInstance().GetJudgeResult();
		prefact.text = result[0].ToString();
		good.text = result[1].ToString();
		bad.text = result[2].ToString();
		miss.text = result[3].ToString();
		score.text = Judge.GetInstance().finalScore.ToString();
		combo.text = Judge.GetInstance().maxCombo.ToString();
	}

	public void ClickButton() 
	{
		SceneController.GetInstance().LoadScene("SelectMusicScene");
	}
}
