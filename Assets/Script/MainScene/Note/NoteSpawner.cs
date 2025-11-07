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
    public NoteDefinition[] notes; //  Note（音符）資訊的列表
}

[Serializable]
public class NoteDefinition
{
    public int type;  //  Note 的類型（例如普通音符、長按音符等）
    public int num;   //  被放在第幾拍的位置
    public int block; //  被放在哪一個軌道（Lane）
    public int LPB;   //  每一拍被細分成幾個等份（tick 數）
    public NoteDefinition[] notes;
    public int direction;
}

[Serializable]
public class NoteEvent
{
    public NoteDefinition note;
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
    [SerializeField] private GameObject noteObj;// 用來放 Note 的 Prefab（預製物）
    [SerializeField] private GameObject holdNoteObj;
    [SerializeField] private GameObject swipeNotePrefab;

    private float currentTime;
    private bool isPlaying;
    [SerializeField] private List<NoteEvent> upcomingNotes = new List<NoteEvent>();
    private Data inputJson;
    private GameManager gameManager;
    private Judge judge;
    private ObjectPoolManager objectPool;

    private void Awake()
	{
        if (instance != null) { Debug.LogError("More than one NoteSpawner!"); Destroy(gameObject); return; }

        instance = this;    
	}

	private void Start()
	{
        gameManager = GameManager.GetInstance();
        judge = Judge.GetInstance();
        objectPool = ObjectPoolManager.GetInstance();
        objectPool.CreatePool("SwipeNote", swipeNotePrefab, 10);

        noteNum = 0;
        if (gameManager.GetSongData() != null) 
        { 
            Load(gameManager.GetSongData().difficultyDatas[gameManager.GetDifficulty()].chart);
        }
        else { Load(songName); }
        isPlaying = true;
        currentTime = 0;
        NotesTime.Clear();
        LaneNum.Clear();
        NoteType.Clear();
        judge.CalculateTotalScore(noteNum);
    }

	private void Update()
	{
        if (!isPlaying) { return; }

        currentTime = judge.timer;

        while (upcomingNotes.Count > 0)
        {
            var noteData = upcomingNotes[0]; // 看第一個元素
            if (noteData.time - currentTime <= spawnLeadTime)
            {
                SpawnNote(noteData.note, noteData.time);
                upcomingNotes.RemoveAt(0); // 移除第一個
            }
            else
            {
                break;
            }
        }
    }

    /*private void Load(string SongName)
    {
        #region 
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
    }*/

    private void Load(string songName)
    {
        // 讀取 JSON 譜面檔
        string inputString = Resources.Load<TextAsset>(songName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);
        noteNum = 0;

        // 遞迴解析整個 notes 陣列
        foreach (var note in inputJson.notes)
        {
            ParseNotes(note);
        }

        Debug.Log($"Loaded {noteNum} notes");
    }

    private void Load(TextAsset musicChart)
    {                
        string inputString = musicChart.ToString();
        inputJson = JsonUtility.FromJson<Data>(inputString);
        noteNum = 0;

        // 遞迴解析整個 notes 陣列
        foreach (var note in inputJson.notes)
        {
            ParseNotes(note);
        }

        Debug.Log($"Loaded {noteNum} notes");
    }

    private void ParseNotes(NoteDefinition note, bool addToUpcoming = true)
    {
        float kankaku = 60f / (inputJson.BPM * (float)note.LPB);
        float beatSec = kankaku * note.LPB;
        float time = (beatSec * note.num / (float)note.LPB) + inputJson.offset * 0.01f;

        if (addToUpcoming) 
        {
            upcomingNotes.Add(new NoteEvent { note = note, time = time });
        }
        
        noteNum++;

        if (note.type == 2 && note.notes != null && note.notes.Length > 0)
        {
            foreach (var subNote in note.notes)
            {
                ParseNotes(subNote, false);
            }
        }
    }

    private void SpawnNote(NoteDefinition note, float time) 
    {
        NotesTime.Add(time);
        LaneNum.Add(note.block);
        NoteType.Add(note.type);
       
        float z = spawnLeadTime * notesSpeed;
        if (time <= spawnLeadTime) { z = time * notesSpeed; }

        // 根據 type 生成不同物件
        string prefabToSpawn = "Note";
        switch (note.type)
        {
            case 1:
                prefabToSpawn = "Note";
                break;

            case 2:
                prefabToSpawn = "HoldNote";
                break;
            case 3:
                prefabToSpawn = "SwipeNote";
                break;
        }
        var noteScript = objectPool.SpwanFromPool(prefabToSpawn, new Vector3(note.block - 1.5f, 0.55f, z), Quaternion.identity);
        NotesObj.Add(noteScript);
        noteScript.GetComponent<Note>().InitNote(notesSpeed, time, note.block);

        //noteNum++;

        if (note.type == 2 && note.notes != null && note.notes.Length > 0)
        {
            var tail = note.notes[0];

            float tailKankaku = 60f / inputJson.BPM ;
            float tailTime = (tailKankaku * tail.num / (float)tail.LPB) + inputJson.offset * 0.01f;

            //float tailZ = tailTime * notesSpeed;
            float tailZ = (tailTime - time + spawnLeadTime) * notesSpeed;

            var tailScript = ObjectPoolManager.GetInstance().
                SpwanFromPool("HoldNote", new Vector3(tail.block - 1.5f, 0.55f, tailZ), Quaternion.identity);
            NotesObj.Add(tailScript);

            tailScript.GetComponent<Note>().InitNote(notesSpeed, tailTime, tail.block);
            noteScript.GetComponent<HoldNote>().SetTail(tailScript.transform);
        }

        if(note.type == 3) { }
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