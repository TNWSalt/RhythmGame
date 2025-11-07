using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager GetInstance() { return instance; }

    [SerializeField] private SongData songData;
    [SerializeField] private int difficulty;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ¤Á´«³õ´º¤]¤£¾P·´
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSongData(SongData data, int index) 
    {
        songData.name = data.name;
        songData.music = data.music;
        songData.difficultyDatas = data.difficultyDatas;
        difficulty = index;
    }

    public SongData GetSongData() { return songData; }
    public int GetDifficulty() { return difficulty; }
}
