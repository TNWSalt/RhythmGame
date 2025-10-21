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

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner instance;
    public static NoteSpawner GetInstance() { return instance; }

    public int noteNum;// Note �`��    
    [SerializeField] private string songName;// �q���W��    
    public List<int> LaneNum = new List<int>();//�C�� Note ���y�D�s��    
    public List<int> NoteType = new List<int>();//�C�� Note ������   
    public List<float> NotesTime = new List<float>(); // �C�� Note �P�P�w�u���X���ɶ��]��^    
    public List<GameObject> NotesObj = new List<GameObject>();//�Ψ��x�s����ƥX�Ӫ� Note ����    
    [SerializeField] private float NotesSpeed;// Note �����ʳt��    
    [SerializeField] GameObject noteObj;// �Ψө� Note �� Prefab�]�w�s���^
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
        // Ū�� JSON �Э���
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

        // �ھ� type �ͦ����P����
        GameObject prefabToSpawn = (note.type == 2) ? holdNoteObj : noteObj;
        NotesObj.Add(Instantiate(prefabToSpawn, new Vector3(note.block - 1.5f, 0.55f, z), Quaternion.identity));

        noteNum++;

        // �Y�O�����A���j�ѪR���h notes
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