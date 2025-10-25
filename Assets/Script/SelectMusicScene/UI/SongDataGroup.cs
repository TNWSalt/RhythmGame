using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class SongData 
{
	public string name;
	public AudioClip music;
	public Sprite sprite;
	public TextAsset musicChart;
}

public class SongDataGroup : MonoBehaviour
{
    [SerializeField] private Image coverImage;
    private CanvasGroup canvasGroup;
    public int index;

    private SongSelectMenu menu;

    public void SetData(Sprite sprite, int idx, SongSelectMenu menu)
    {
        coverImage.sprite = sprite;
        index = idx;
        this.menu = menu;
    }

    public void ClickButton() 
    {
        GameManager.GetInstance().SetSongData(menu.GetData(index));
        SceneController.GetInstance().LoadNextScene();
    }
}
