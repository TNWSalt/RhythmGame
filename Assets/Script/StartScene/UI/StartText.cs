using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StartText : MonoBehaviour
{
    public float duration = 1f;
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Color color = text.color;
        color.a = maxAlpha;
        text.color = color;

        text.DOFade(minAlpha, duration).SetLoops(-1, LoopType.Yoyo);

    }
}
