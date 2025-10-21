using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public string name;  // 曲名
    public int maxBlock; // 最大軌道數（Lane 數）
    public int BPM;      //  BPM（歌曲節奏速度）
    public int offset;   //  開始時間的偏移值
    public NoteInfo[] notes; //  Note（音符）資訊的列表
}

[Serializable]
public class NoteInfo
{
    public int type;  //  Note 的類型（例如普通音符、長按音符等）
    public int num;   //  被放在第幾拍的位置
    public int block; //  被放在哪一個軌道（Lane）
    public int LPB;   //  每一拍被細分成幾個等份（tick 數）
    public NoteInfo[] notes;
}

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner instance;
    public static NoteSpawner GetInstance() { return instance; }

    public int noteNum;// Note 總數    
    [SerializeField] private string songName;// 歌曲名稱    
    public List<int> LaneNum = new List<int>();//每個 Note 的軌道編號    
    public List<int> NoteType = new List<int>();//每個 Note 的種類   
    public List<float> NotesTime = new List<float>(); // 每個 Note 與判定線重合的時間（秒）    
    public List<GameObject> NotesObj = new List<GameObject>();//用來儲存實體化出來的 Note 物件    
    [SerializeField] private float NotesSpeed;// Note 的移動速度    
    [SerializeField] GameObject noteObj;// 用來放 Note 的 Prefab（預製物）
    [SerializeField] GameObject holdNoteObj;

    private void Awake()
	{
        if (instance != null) { Debug.LogError("More than one NoteSpawner!"); Destroy(gameObject); return; }

        instance = this;    
	}

	void OnEnable()
    {
        noteNum = 0;
        Load(songName);
    }

    private void Load(string SongName)
    {
        // 讀取 JSON 譜面檔
        string inputString = Resources.Load<TextAsset>(SongName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);

        //設定 Note 的總數
        noteNum = inputJson.notes.Length;

        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            //計算每個 Note 應該出現的時間點
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
            float beatSec = kankaku * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.01f;

            //把資料加入對應的 List
            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);

            float z = NotesTime[i] * NotesSpeed;
            //實體化 Note 物件
            NotesObj.Add(Instantiate(noteObj, new Vector3(inputJson.notes[i].block - 1.5f, 0.55f, z), Quaternion.identity));
        }
    }

    private void ParseNotes(NoteInfo note)
    {
        float kankaku = 60f / (inputJson.BPM * (float)note.LPB);
        float beatSec = kankaku * note.LPB;
        float time = (beatSec * note.num / (float)note.LPB) + inputJson.offset * 0.01f;

        NotesTime.Add(time);
        LaneNum.Add(note.block);
        NoteType.Add(note.type);

        float z = time * NotesSpeed;

        // 根據 type 生成不同物件
        GameObject prefabToSpawn = (note.type == 2) ? holdNoteObj : noteObj;
        NotesObj.Add(Instantiate(prefabToSpawn, new Vector3(note.block - 1.5f, 0.55f, z), Quaternion.identity));

        noteNum++;

        // 若是長按，遞迴解析內層 notes
        if (note.type == 2 && note.notes != null && note.notes.Length > 0)
        {
            foreach (var subNote in note.notes)
            {
                ParseNotes(subNote);
            }
        }
    }

    public float GetNoteSpeed() 
    {
        return NotesSpeed;
    }

    public void SetSongName(string name) 
    {
        songName = name;
    }
}