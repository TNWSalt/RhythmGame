using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static InputManager GetInstance() { return instance; }

    private float[] holdTime = new float[4]; // 對應 4 個軌道
    private bool[] startHolding = new bool[4];
    private bool[] isHolding = new bool[4];
    [SerializeField] private float longPressThreshold = 0.05f; // 長按門檻

    [SerializeField] private Light[] lights;

    private Judge judge;
    private KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K }; // 鍵盤按鍵

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
        if (PauseManager.GetInstance().isPause) { return; }

        // 1. 持續偵測鍵盤輸入 (僅在 Editor 或 PC 平台)
        CheckKeyboardInput();

        // 2. 持續檢查所有 "startHolding" 的軌道，以處理長按 (Hold) 邏輯
        // 這個邏輯對於鍵盤和觸控是共用的
        CheckHoldingStatus();
    }

    // (僅在 Editor/PC 執行)
    void CheckKeyboardInput()
    {
        // 偵測鍵盤輸入只在 Editor 或 PC 平台執行
#if UNITY_EDITOR || UNITY_STANDALONE
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                OnLanePress(i);
            }

            if (Input.GetKeyUp(keys[i]))
            {
                OnLaneRelease(i);
            }
        }
#endif
    }

    // (鍵盤和觸控共用)
    void CheckHoldingStatus()
    {
        for (int i = 0; i < 4; i++)
        {
            // 如果軌道處於 "按下" 狀態
            if (startHolding[i])
            {
                holdTime[i] += Time.deltaTime;

                // 如果滿足了長按門檻，且尚未標記為 "isHolding"
                if (!isHolding[i] && holdTime[i] >= longPressThreshold)
                {
                    isHolding[i] = true;
                    Debug.Log("Lane " + i + " isHolding");
                    // 這裡可以觸發 Judge 的長按開始事件 (如果你需要)
                    // judge.HoldStart(i); 
                }
            }
        }
    }

    // --- 給 EventTrigger 呼叫的 Public 函數 ---

    /// <summary>
    /// 處理 "按下" 事件 (由 EventTrigger 的 PointerDown 或 鍵盤 GetKeyDown 觸發)
    /// </summary>
    public void OnLanePress(int index)
    {
        if (PauseManager.GetInstance().isPause) { return; }
        if (index < 0 || index >= 4) { return; }

        // 防止重複觸發 (例如鍵盤和滑鼠同時點)
        if (startHolding[index]) { return; }

        lights[index].Flash();
        holdTime[index] = 0f;
        startHolding[index] = true;
        isHolding[index] = false; // 剛按下時，尚未滿足 long press

        // 呼叫 Judge，第二個參數 true 代表 "Press" 事件
        judge.JudgementNote(index, true);
    }

    /// <summary>
    /// 處理 "放開" 事件 (由 EventTrigger 的 PointerUp 或 鍵盤 GetKeyUp 觸發)
    /// </summary>
    public void OnLaneRelease(int index)
    {
        if (PauseManager.GetInstance().isPause) { return; }
        if (index < 0 || index >= 4) { return; }

        // 必須是 "按下" 狀態才能 "放開"
        if (!startHolding[index]) { return; }

        // 記錄在放開前是否為長按狀態 (你的 Judge 可能需要這個資訊)
        // bool wasHolding = isHolding[index];

        holdTime[index] = 0f;
        isHolding[index] = false;
        startHolding[index] = false;

        // 呼叫 Judge，第二個參數 false 代表 "Release" 事件
        // 這遵循你原始碼在 GetKeyUp 時的邏GIN
        judge.JudgementNote(index, false);
    }
}