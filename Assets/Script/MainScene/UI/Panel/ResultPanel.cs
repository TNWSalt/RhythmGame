using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
public class ResultTextAnim
{
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;
    public Vector2 startPos;
    public Vector2 targetPos;
}

public class ResultPanel : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI songName;
    [SerializeField] private TextMeshProUGUI maxCombo;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private Button continueButton;


    private CanvasGroup maxComboCanvasGroup;

    [Header("Stats")]
    [SerializeField] private List<ResultTextAnim> statTexts;

    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image blackPanel;

    private Judge judge;

    private void Awake()
    {
        ResetState();
    }

	private void Start()
	{
        judge = Judge.GetInstance();
    }

	private void ResetState()
    {
        maxComboCanvasGroup = maxCombo.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;        

        songName.alpha = 0;
        songName.rectTransform.anchoredPosition =
            new Vector2(-800, songName.rectTransform.anchoredPosition.y);

        maxComboCanvasGroup.alpha = 0;
        maxCombo.rectTransform.anchoredPosition =
            new Vector2(-800, maxCombo.rectTransform.anchoredPosition.y);

        foreach (var item in statTexts)
        {
            item.canvasGroup.alpha = 0;
            item.text.rectTransform.anchoredPosition = item.startPos;
        }

        continueButton.GetComponent<Image>().DOFade(0, 0f);
    }

    public void ShowResult()
    {       
        PlaySequence();
        SetResult();        
    }

    private void PlaySequence()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(blackPanel.DOFade(1, 1f));
        seq.Append(canvasGroup.DOFade(1, 0.1f));
        seq.Append(blackPanel.DOFade(0, 1f));        

        seq.Append(songName.DOFade(1, 1f).SetEase(Ease.OutSine));
        seq.Join(songName.rectTransform.DOAnchorPosX(70, 1f).SetEase(Ease.OutExpo));

        seq.Append(maxComboCanvasGroup.DOFade(1, 1f).SetEase(Ease.OutSine));
        seq.Join(maxCombo.rectTransform.DOAnchorPosX(70, 1f).SetEase(Ease.OutExpo));

        foreach (var item in statTexts)
        {
            seq.AppendInterval(0.3f);
            seq.Append(item.canvasGroup.DOFade(1, 0.5f));
            seq.Join(item.text.rectTransform.DOAnchorPos(item.targetPos, 0.5f)
                .SetEase(Ease.OutSine));
        }

        seq.Append(continueButton.GetComponent<Image>().DOFade(1, 1f));
    }

    private void SetResult() 
    {
        Debug.Log("SetResult");
        maxCombo.text = judge.maxCombo.ToString();
        finalScore.text = judge.finalScore.ToString();

		for (int i = 0; i < statTexts.Count; i++)
		{
            statTexts[i].text.text = judge.resultCount[i].ToString();
        }
        Debug.Log("SetResultEND");
    }
}