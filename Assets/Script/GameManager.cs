using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager GetInstance() { return instance; }

    [SerializeField] private SongData songData;
    public string songName = "";
    public AudioClip music;

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

    public void SetSongData(SongData data) 
    {
        songData.name = data.name;
        songData.music = data.music;
        songData.musicChart = data.musicChart;
    }

    public SongData GetSongData() { return songData; }
}
