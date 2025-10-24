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
        // �P�_���֬O�_���񵲧�
        if (!audioSource.isPlaying && audioSource.time > 0)
        {
            End();
            Debug.Log("���ּ��񧹲��I");
        }
    }

    private void End() 
    {
        UIManager.GetInstance().ShowPanel("EndPanel");
    }
}
