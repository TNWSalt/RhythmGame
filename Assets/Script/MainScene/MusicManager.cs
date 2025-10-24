using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

	public void Start()
	{
		audioSource.clip = GameManager.GetInstance().GetSongData().music;
		audioSource.Play();
	}

    private void Update()
    {
        // 判斷音樂是否播放結束
        if (!audioSource.isPlaying && audioSource.time > 0)
        {
            End();
            Debug.Log("音樂播放完畢！");
        }
    }

    private void End() 
    {
        UIManager.GetInstance().ShowPanel("EndPanel");
    }
}
