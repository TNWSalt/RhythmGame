using System;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class JudgeTxetMessage
    {
        public string judgeText;
        public Color textColor;
    }

    public class Judge : MonoBehaviour
    {
        //[SerializeField] private GameObject[] MessageObj;
        [SerializeField] NoteSpawner notesManager;
        //[SerializeField] Camera mainCamera;
        [SerializeField] float timer;
        [SerializeField] JudgeTxetMessage[] judgeMessage;
        [SerializeField] GameObject messagePrefab;

        private ObjectPoolManager poolManager;

        private void Start()
        {
            notesManager = NoteSpawner.GetInstance();
            poolManager = ObjectPoolManager.GetInstance();
            timer = 0;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (notesManager.LaneNum.Count <= 0) { return; }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (notesManager.LaneNum[0] == 0)
                {
                    Judgement(GetABS(timer - notesManager.NotesTime[0]));
                    /*               
                     �p��z�קP�w�ɶ��P��ںV���ɶ����t�Z�]�~�t����ȡ^�A
                       �öǰe�� Judgement ��ƶi��P�_
                    */
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (notesManager.LaneNum[0] == 1)
                {
                    Judgement(GetABS(timer - notesManager.NotesTime[0]));
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (notesManager.LaneNum[0] == 2)
                {
                    Judgement(GetABS(timer - notesManager.NotesTime[0]));
                }
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (notesManager.LaneNum[0] == 3)
                {
                    Judgement(GetABS(timer - notesManager.NotesTime[0]));
                }
            }

            if (notesManager.NotesObj.Count <= 0) { return; }
            if (timer > notesManager.NotesTime[0] + 0.2f) //  �W�L���ӺV�� Note ���ɶ� 0.2 ���٨S��J�A�N���� Miss
            {
                Message(3);
                deleteData();
                Debug.Log("Miss");
                //  Miss �P�w
            }
        }

        void Judgement(float timeLag)
        {
            if (timeLag <= 0.10)

            // �Y�~�t�b 0.1 ��H�� Perfect
            {
                Debug.Log("Perfect");
                Message(0);
                deleteData();
            }
            else
            {
                if (timeLag <= 0.15)
                // �~�t�b 0.15 ��H��  Great
                {
                    Debug.Log("Great");
                    Message(1);
                    deleteData();
                }
                else
                {
                    if (timeLag <= 0.20)
                    // �~�t�b 0.2 ��H��  Bad
                    {
                        Debug.Log("Bad");
                        Message(2);
                        deleteData();
                    }
                }
            }
        }

        float GetABS(float num)
        // �Ǧ^�Ѽƪ������
        {
            if (num >= 0)
            {
                return num;
            }
            else
            {
                return -num;
            }
        }


        //  �R���w�g�V���L�� Note ���
        void deleteData()
        {
            notesManager.NotesTime.RemoveAt(0);
            notesManager.LaneNum.RemoveAt(0);
            notesManager.NoteType.RemoveAt(0);
            notesManager.NotesObj.RemoveAt(0);
        }


        //  ��ܹ������P�w UI�]Perfect / Great / Bad / Miss�^
        void Message(int judge)
        {
            /*Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[0] - 1.5f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0))
                .GetComponent<Canvas>().worldCamera = mainCamera;*/
            var text = poolManager.SpwanFromPool(messagePrefab.name,
                new Vector3(notesManager.LaneNum[0] - 1.5f, 0.76f, 0.15f),
                Quaternion.Euler(45, 0, 0)).GetComponent<JudgeText>();
            text.SetText(judgeMessage[judge]);
        }
    }