using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentCombo;
    private TextMeshProUGUI comboText;

	private void Start()
	{
        comboText = GetComponent<TextMeshProUGUI>();
        comboText.DOFade(0f, 0f);
        currentCombo.text = "";
    }

	public void SetCombo(int combo) 
    {
        if (combo <= 0) 
        {
            comboText.DOFade(0f, 0f);
            currentCombo.DOFade(0, 0f);
            //gameObject.SetActive(false);
            currentCombo.text = "";
            return;
        }

        comboText.DOFade(1, 0f);
        currentCombo.DOFade(1, 0f);

        transform.localScale = Vector3.one * 0.3f;
        transform.DOScale(Vector3.one , 0.2f).SetEase(Ease.OutBack);
        currentCombo.text = combo.ToString();
    }
}
