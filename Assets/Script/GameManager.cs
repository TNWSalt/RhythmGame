using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager GetInstance() { return instance; }

    [SerializeField] private SongData songData;
    public int difficulty { get; private set; } = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 切換場景也不銷毀
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;  // 鎖定 60 FPS
        QualitySettings.vSyncCount = 0;    // 關閉 VSync，讓 targetFrameRate 生效
    }

    public void SetSongData(SongData data, int index) 
    {
        songData.name = data.name;
        songData.music = data.music;
        songData.difficultyDatas = data.difficultyDatas;
        difficulty = index;
    }

    public SongData GetSongData() { return songData; }
}
