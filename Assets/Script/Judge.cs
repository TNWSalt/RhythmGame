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
                     計算理論判定時間與實際敲擊時間的差距（誤差絕對值），
                       並傳送給 Judgement 函數進行判斷
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
            if (timer > notesManager.NotesTime[0] + 0.2f) //  超過應該敲擊 Note 的時間 0.2 秒還沒輸入，就視為 Miss
            {
                Message(3);
                deleteData();
                Debug.Log("Miss");
                //  Miss 判定
            }
        }

        void Judgement(float timeLag)
        {
            if (timeLag <= 0.10)

            // 若誤差在 0.1 秒以內 Perfect
            {
                Debug.Log("Perfect");
                Message(0);
                deleteData();
            }
            else
            {
                if (timeLag <= 0.15)
                // 誤差在 0.15 秒以內  Great
                {
                    Debug.Log("Great");
                    Message(1);
                    deleteData();
                }
                else
                {
                    if (timeLag <= 0.20)
                    // 誤差在 0.2 秒以內  Bad
                    {
                        Debug.Log("Bad");
                        Message(2);
                        deleteData();
                    }
                }
            }
        }

        float GetABS(float num)
        // 傳回參數的絕對值
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


        //  刪除已經敲擊過的 Note 資料
        void deleteData()
        {
            notesManager.NotesTime.RemoveAt(0);
            notesManager.LaneNum.RemoveAt(0);
            notesManager.NoteType.RemoveAt(0);
            notesManager.NotesObj.RemoveAt(0);
        }


        //  顯示對應的判定 UI（Perfect / Great / Bad / Miss）
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