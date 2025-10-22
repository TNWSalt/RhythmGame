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


[Serializable]
public class NoteData
{
    public NoteInfo note;
    public float time;
}

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner instance;
    public static NoteSpawner GetInstance() { return instance; }

    [SerializeField] private string songName;// 歌曲名稱
    [SerializeField] private int noteNum;// Note 總數    
    [SerializeField] private float notesSpeed;// Note 的移動速度
    [SerializeField] private float spawnLeadTime = 2f;

    [Space(10)]
    public List<int> LaneNum = new List<int>();//每個 Note 的軌道編號    
    public List<int> NoteType = new List<int>();//每個 Note 的種類   
    public List<float> NotesTime = new List<float>(); // 每個 Note 與判定線重合的時間（秒）    
    public List<GameObject> NotesObj = new List<GameObject>();//用來儲存實體化出來的 Note 物件                                                                  
    

    [Header("Prefab")]
    [SerializeField] GameObject noteObj;// 用來放 Note 的 Prefab（預製物）
    [SerializeField] GameObject holdNoteObj;

    private float currentTime;
    private bool isPlaying;
    [SerializeField] private Queue<NoteData> upcomingNotes = new Queue<NoteData>();
    private Data inputJson;

    private void Awake()
	{
        if (instance != null) { Debug.LogError("More than one NoteSpawner!"); Destroy(gameObject); return; }

        instance = this;    
	}

	private void Start()
	{
        noteNum = 0;
        Load(songName);
        isPlaying = true;
        currentTime = 0;
        NotesTime.Clear();
        LaneNum.Clear();
        NoteType.Clear();
    }

	private void Update()
	{
        if (!isPlaying) { return; }

        currentTime = Judge.GetInstance().GetCurrentTime();

        while (upcomingNotes.Count > 0)
        {
            var noteData = upcomingNotes.Peek(); // 只看第一個
            if (noteData.time - currentTime <= spawnLeadTime)
            {
                SpawnNote(noteData.note, noteData.time);
                upcomingNotes.Dequeue(); // 從佇列中移除
            }
            else
            {
                break; // 還沒到時間，後面的也一定還沒到
            }
        }
    }

	private void Load(string SongName)
    {
        #region 
        /*// 讀取 JSON 譜面檔
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
        }*/
        #endregion
        string inputString = Resources.Load<TextAsset>(SongName).ToString();
        inputJson = JsonUtility.FromJson<Data>(inputString);
        noteNum = 0;

        // 遞迴解析整個 notes 陣列
        foreach (var note in inputJson.notes)
        {
            ParseNotes(note);
        }

        Debug.Log($"Loaded {noteNum} notes");
    }

	private void ParseNotes(NoteInfo note)
    {
        float kankaku = 60f / (inputJson.BPM * (float)note.LPB);
        float beatSec = kankaku * note.LPB;
        float time = (beatSec * note.num / (float)note.LPB) + inputJson.offset * 0.01f;

        upcomingNotes.Enqueue(new NoteData { note = note, time = time });
        noteNum++;

        if (note.type == 2 && note.notes != null && note.notes.Length > 0)
        {
            foreach (var subNote in note.notes)
            {
                ParseNotes(subNote);
            }
        }
    }

    private void SpawnNote(NoteInfo note, float time) 
    {
        NotesTime.Add(time);
        LaneNum.Add(note.block);
        NoteType.Add(note.type);

        float z = spawnLeadTime * notesSpeed;

        // 根據 type 生成不同物件
        //GameObject prefabToSpawn = (note.type == 2) ? holdNoteObj : noteObj;
        string prefabToSpawn = "Note";
        switch (note.type)
        {
            case 1:
                prefabToSpawn = "Note";
                break;

            case 2:
                prefabToSpawn = "HoldNote";
                break;
        }
        var noteScript = ObjectPoolManager.GetInstance().
            SpwanFromPool(prefabToSpawn, new Vector3(note.block - 1.5f, 0.55f, z), Quaternion.identity);
        Debug.Log(noteScript);
        NotesObj.Add(noteScript);
        noteScript.GetComponent<Note>().InitNote(notesSpeed, time, note.block);

        noteNum++;

        if (note.type == 2 && note.notes != null && note.notes.Length > 0)
        {
            var tail = note.notes[0];

            float tailKankaku = 60f / (inputJson.BPM * (float)tail.LPB);
            float tailTime = (tailKankaku * tail.num / (float)tail.LPB) + inputJson.offset * 0.01f;
            float tailZ = tailTime * notesSpeed;

            var tailScript = ObjectPoolManager.GetInstance().
                SpwanFromPool("HoldNote", new Vector3(tail.block - 1.5f, 0.55f, tailZ), Quaternion.identity);
            NotesObj.Add(tailScript);

            tailScript.GetComponent<Note>().InitNote(notesSpeed, tailTime, tail.block);

            noteScript.GetComponent<HoldNote>().SetTail(tailScript.transform);
        }
    }

    public float GetNoteSpeed() 
    {
        return notesSpeed;
    }

    public void SetSongName(string name) 
    {
        songName = name;
    }
}