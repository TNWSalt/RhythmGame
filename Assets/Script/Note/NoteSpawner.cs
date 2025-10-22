using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public string name;  // ���W
    public int maxBlock; // �̤j�y�D�ơ]Lane �ơ^
    public int BPM;      //  BPM�]�q���`���t�ס^
    public int offset;   //  �}�l�ɶ���������
    public NoteInfo[] notes; //  Note�]���š^��T���C��
}

[Serializable]
public class NoteInfo
{
    public int type;  //  Note �������]�Ҧp���q���šB�������ŵ��^
    public int num;   //  �Q��b�ĴX�窺��m
    public int block; //  �Q��b���@�ӭy�D�]Lane�^
    public int LPB;   //  �C�@��Q�Ӥ����X�ӵ����]tick �ơ^
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

    [SerializeField] private string songName;// �q���W��
    [SerializeField] private int noteNum;// Note �`��    
    [SerializeField] private float notesSpeed;// Note �����ʳt��
    [SerializeField] private float spawnLeadTime = 2f;

    [Space(10)]
    public List<int> LaneNum = new List<int>();//�C�� Note ���y�D�s��    
    public List<int> NoteType = new List<int>();//�C�� Note ������   
    public List<float> NotesTime = new List<float>(); // �C�� Note �P�P�w�u���X���ɶ��]��^    
    public List<GameObject> NotesObj = new List<GameObject>();//�Ψ��x�s����ƥX�Ӫ� Note ����                                                                  
    

    [Header("Prefab")]
    [SerializeField] GameObject noteObj;// �Ψө� Note �� Prefab�]�w�s���^
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
            var noteData = upcomingNotes.Peek(); // �u�ݲĤ@��
            if (noteData.time - currentTime <= spawnLeadTime)
            {
                SpawnNote(noteData.note, noteData.time);
                upcomingNotes.Dequeue(); // �q��C������
            }
            else
            {
                break; // �٨S��ɶ��A�᭱���]�@�w�٨S��
            }
        }
    }

	private void Load(string SongName)
    {
        #region 
        /*// Ū�� JSON �Э���
        string inputString = Resources.Load<TextAsset>(SongName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);

        //�]�w Note ���`��
        noteNum = inputJson.notes.Length;

        for (int i = 0; i < inputJson.notes.Length; i++)
        {
        //�p��C�� Note ���ӥX�{���ɶ��I
        float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
        float beatSec = kankaku * (float)inputJson.notes[i].LPB;
        float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.01f;

        //���ƥ[�J������ List
        NotesTime.Add(time);
        LaneNum.Add(inputJson.notes[i].block);
        NoteType.Add(inputJson.notes[i].type);

        float z = NotesTime[i] * NotesSpeed;
        //����� Note ����
        NotesObj.Add(Instantiate(noteObj, new Vector3(inputJson.notes[i].block - 1.5f, 0.55f, z), Quaternion.identity));
        }*/
        #endregion
        string inputString = Resources.Load<TextAsset>(SongName).ToString();
        inputJson = JsonUtility.FromJson<Data>(inputString);
        noteNum = 0;

        // ���j�ѪR��� notes �}�C
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

        // �ھ� type �ͦ����P����
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