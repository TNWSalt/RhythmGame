using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class JudgeText : ObjectPool
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private JudgeTxetMessage defaultTextMessge;

	void Start()
    {
        text.raycastTarget = false;
        SetText(defaultTextMessge);
        //text = GetComponentInChildren<TextMeshProUGUI>();
        transform.localScale = Vector3.one * 0.1f;
        transform.DOScale(Vector3.one*.5f, 0.2f).SetEase(Ease.OutBack);            
    }

	public void SetText(JudgeTxetMessage message) 
    {
        text.text = message.judgeText;
        text.color = message.textColor;
    }
}