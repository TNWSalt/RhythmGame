using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    [SerializeField] private Material mat;

    private Coroutine flashCoroutine;

    void Start()
    {
        gameObject.SetActive(true);
        mat = GetComponent<Renderer>().material;
        SetAlpha(0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Debug.Log("Click " + key);

            // 若已經在閃爍，先停止之前的 Coroutine
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }

            flashCoroutine = StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        SetAlpha(1f);

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        flashCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = mat.color;
        c.a = Mathf.Clamp01(alpha);
        mat.color = c;
    }
}
