using UnityEngine;
using DG.Tweening;

public class Light : MonoBehaviour
{
    [SerializeField] private Material mat;

    private Tween flashTween;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        SetAlpha(0f);
    }

    public void Flash() 
    {
        flashTween?.Kill();

        SetAlpha(1f);
        flashTween = mat.DOFade(0f, 0.2f).SetEase(Ease.OutQuad);
    }

    private void SetAlpha(float alpha)
    {
        Color c = mat.color;
        c.a = Mathf.Clamp01(alpha);
        mat.color = c;
    }
}
