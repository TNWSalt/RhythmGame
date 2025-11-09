using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (新) 定義滑動方向
public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static InputManager GetInstance() { return instance; }

    private float[] holdTime = new float[4];
    private bool[] startHolding = new bool[4];
    private bool[] isHolding = new bool[4];
    [SerializeField] private float longPressThreshold = 0.05f;

    [SerializeField] private Light[] lights;

    [SerializeField] private float swipeMinDistance = 50f;
    [SerializeField] private float swipeDominantAxisThreshold = 30f;

    private bool[] isKeyboardHolding = new bool[4];

    private Judge judge;
    private KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

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
        CheckKeyboardInput(); // 檢查鍵盤
        CheckHoldingStatus(); // 檢查長按
    }

    void CheckKeyboardInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                isKeyboardHolding[i] = true;
                OnLanePress(i);
            }
            else if (isKeyboardHolding[i] && !Input.GetKey(keys[i]))
            {

                isKeyboardHolding[i] = false;

                OnLaneRelease(i);
            }
        }
#endif
    }

    void CheckHoldingStatus()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!startHolding[i])  { continue;}

            holdTime[i] += Time.deltaTime;
            if (!isHolding[i] && holdTime[i] >= longPressThreshold)
            {
                isHolding[i] = true;
                Debug.Log("Lane " + i + " isHolding");
            }
        }
    }

    public void OnLanePress(int index)
    {
        if (PauseManager.GetInstance().isPause) { return; }
        if (index < 0 || index >= 4) { return; }
        if (startHolding[index]) { return; }

        Debug.Log("InputManager: OnLanePress (Lane " + index + ")");
        lights[index].Flash();
        holdTime[index] = 0f;
        startHolding[index] = true;
        isHolding[index] = false;

        judge.JudgementNote(index, true);
    }

    public void OnLaneRelease(int index)
    {
        if (PauseManager.GetInstance().isPause) { return; }
        if (index < 0 || index >= 4) { return; }
        if (!startHolding[index]) { return; }

        Debug.Log("InputManager: OnLaneRelease (Lane " + index + ")");
        holdTime[index] = 0f;
        isHolding[index] = false;

        isKeyboardHolding[index] = false;
        startHolding[index] = false;

        judge.JudgementNote(index, false);
    }

    public void OnLaneDragStart(int index)
    {
        if (PauseManager.GetInstance().isPause) { return; }
        if (index < 0 || index >= 4) { return; };
        if (!startHolding[index]) { return; }

        Debug.Log("InputManager: OnLaneDragStart (Lane " + index + ") - Canceling Press.");

        //startHolding[index] = false;
        isKeyboardHolding[index] = false;

        //isHolding[index] = false;
        //holdTime[index] = 0f;

        judge.CancelPress(index);
    }

    public void OnLaneSwipeEnd(int index, Vector2 startPos, Vector2 endPos)
    {
        if (PauseManager.GetInstance().isPause) { return; }
        if (index < 0 || index >= 4) { return; }

        Vector2 delta = endPos - startPos;
        float distance = delta.magnitude;
        
        if (distance < swipeMinDistance)
        {
            Debug.Log("InputManager: Swipe Ignored (Too Short)");
            judge.JudgementNote(index, false);
            return;
        }

        float deltaX = delta.x;
        float deltaY = delta.y;
        float absDeltaX = Mathf.Abs(deltaX);
        float absDeltaY = Mathf.Abs(deltaY);

        SwipeDirection dir;

        if (absDeltaX > absDeltaY)
        {
            dir = deltaX > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else 
        {
            dir = deltaY > 0 ? SwipeDirection.Up : SwipeDirection.Down;
        }
            
        Debug.Log($"InputManager: Swipe {dir} on Lane {index}");

        //保留判定呼叫
        judge.JudgementSwipeNote(index, dir);

        startHolding[index] = false;
        isHolding[index] = false;
        holdTime[index] = 0f;
    }
}