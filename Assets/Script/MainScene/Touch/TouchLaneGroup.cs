using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchLaneGroup : MonoBehaviour
{
    [SerializeField] Renderer[] lanes;
	[SerializeField] Transform mainCamera;

	private Vector3 originalPos = new Vector3(0, 4, -2);
	private Vector3 startPos = new Vector3(0, 3, -1);

	private void Start()
	{
		mainCamera.transform.position = originalPos;
		foreach (var lane in lanes)
		{
			lane.material.DOFade(0, 0f);
		}

		MusicManager.GetInstance().OnMusicFinished += MusicDone;
		StartCoroutine(Init());
	}

	private IEnumerator Init()
	{
		mainCamera.DOLocalMove(startPos, 1f);
		yield return new WaitForSeconds(1.2f);

		Sequence seq = DOTween.Sequence();

		foreach (var lane in lanes)
		{
			// 加入並行（Join）讓它們同時淡入
			seq.Join(lane.material.DOFade(1f, 1f));
		}

		// 全部動畫結束後觸發事件
		seq.OnComplete(() =>
		{			
			StartSpawnNote();
		});
	}

	private void StartSpawnNote() 
	{
		Debug.Log("開始！");
		Judge.GetInstance().SetIsPlaying(true);
		MusicManager.GetInstance().PlayMusic(GameManager.GetInstance().GetSongData().music);
		//NoteSpawner.GetInstance().
	}

	private void MusicDone()
	{
		StartCoroutine(End());
	}

	private IEnumerator End() 
	{
		mainCamera.DOLocalMove(originalPos, 1f);
		yield return new WaitForSeconds(1.2f);

		Sequence seq = DOTween.Sequence();

		foreach (var lane in lanes)
		{
			seq.Join(lane.material.DOFade(0f, 1f));
		}

		// 全部動畫結束後觸發事件
		seq.OnComplete(() =>
		{
			ShowEndPanel();
		});
	}

	private void ShowEndPanel() 
	{
		Debug.Log("音樂結束!");
		FindObjectOfType<ResultPanel>().ShowResult();
		//UIManager.GetInstance().ShowPanel("EndPanel");
	}
}
