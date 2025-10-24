using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCalculation : MonoBehaviour
{    
    [SerializeField] private TextMeshProUGUI scoreText;

	private void Start()
	{
        scoreText.text = "0";
    }	

    public void SetText(string score) 
    {
        scoreText.text = score.ToString();
    }

    
}
 