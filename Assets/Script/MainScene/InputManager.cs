using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public static InputManager instance;
	public static InputManager GetInstance() { return instance; }

    private float[] holdTime = new float[4]; // ���� D, F, J, K �|����
    private bool[] startHolding = new bool[4];
    private bool[] isHolding = new bool[4];
    [SerializeField] private float longPressThreshold = 0.05f; // �������e

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
        // ���U��UĲ�o�]�uĲ�o�@���^
        if (Input.GetKeyDown(key))
        {
            holdTime[index] = 0f;
            startHolding[index] = true;
            judge.JudgementNote(index, startHolding[index]); // �������U�ɪ��欰
        }

        // ������۪��ɭԭp��
        if (Input.GetKey(key) && startHolding[index])
        {
            holdTime[index] += Time.deltaTime;

            if (holdTime[index] >= longPressThreshold)
            {
                isHolding[index] = true;
                Debug.Log("isHolding");
                //isHolding[index] = false; // �uĲ�o�@�������ƥ�
                //judge.Hold(index);  // �A�ݭn�b judge ����@ LongPress(int index)
            }
        }

        // ���}�ɭ��]
        if (Input.GetKeyUp(key))
        {
            holdTime[index] = 0f;            
            isHolding[index] = false;
            judge.JudgementNote(index, isHolding[index]);
            startHolding[index] = false;
        }
    }
}
