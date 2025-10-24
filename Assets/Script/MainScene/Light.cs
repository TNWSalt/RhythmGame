using UnityEngine;
using DG.Tweening;

public class Light : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    [SerializeField] private Material mat;

    private Tween flashTween;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        SetAlpha(0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Debug.Log("Click " + key);

            // 如果有舊的 tween，先殺掉它（防止重疊）
            flashTween?.Kill();

            SetAlpha(1f);
            flashTween = mat.DOFade(0f, 0.2f).SetEase(Ease.OutQuad);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = mat.color;
        c.a = Mathf.Clamp01(alpha);
        mat.color = c;
    }
}
