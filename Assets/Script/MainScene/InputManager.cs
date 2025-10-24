using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public static InputManager instance;
	public static InputManager GetInstance() { return instance; }

    private float[] holdTime = new float[4]; // 對應 D, F, J, K 四個鍵
    private bool[] startHolding = new bool[4];
    private bool[] isHolding = new bool[4];
    [SerializeField] private float longPressThreshold = 0.05f; // 長按門檻

    //[SerializeField] JudgeTxetMessage[] judgeMessage;
    //[SerializeField] GameObject messagePrefab;

    private Judge judge;

	private void Awake()
	{
        if (instance != null) { return; }
        instance = this;
	}

	private void Start()
	{
        judge = Judge.GetInstance();
    }

    void Update()
    {
        CheckKey(KeyCode.D, 0);
        CheckKey(KeyCode.F, 1);
        CheckKey(KeyCode.J, 2);
        CheckKey(KeyCode.K, 3);
    }

    void CheckKey(KeyCode key, int index)
    {
        // 按下當下觸發（只觸發一次）
        if (Input.GetKeyDown(key))
        {
            holdTime[index] = 0f;
            startHolding[index] = true;
            judge.JudgementNote(index, startHolding[index]); // 瞬間按下時的行為
        }

        // 持續按著的時候計時
        if (Input.GetKey(key) && startHolding[index])
        {
            holdTime[index] += Time.deltaTime;

            if (holdTime[index] >= longPressThreshold)
            {
                isHolding[index] = true;
                Debug.Log("isHolding");
                //isHolding[index] = false; // 只觸發一次長按事件
                //judge.Hold(index);  // 你需要在 judge 中實作 LongPress(int index)
            }
        }

        // 鍵放開時重設
        if (Input.GetKeyUp(key))
        {
            holdTime[index] = 0f;            
            isHolding[index] = false;
            judge.JudgementNote(index, isHolding[index]);
            startHolding[index] = false;
        }
    }
}
