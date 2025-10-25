using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SongSelectMenu : MonoBehaviour
{
    [SerializeField] private List<SongData> songs;

    [SerializeField] private SongDataGroup songDataPrefab;
    [SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private RectTransform content;

    private List<SongDataGroup> items = new();

    private void Start()
	{
        Populate();
        nameText.text = songs[content.GetComponent<SwipeMenu>().GetFocuseIndex()].name;
    }

	public void Update()
	{
        if (Input.GetMouseButtonDown(0)) 
        {
            nameText.DOFade(0, .2f);
            nameText.rectTransform.DOLocalMoveY(-600, .2f);
        }
        if (Input.GetMouseButtonUp(0)) 
        {
            nameText.text = songs[content.GetComponent<SwipeMenu>().GetFocuseIndex()].name;
            nameText.DOFade(1, .2f);
            nameText.rectTransform.DOMoveY(100, .2f);
            nameText.rectTransform.DOLocalMoveY(-325, .2f);
        }
	}

	private void Populate()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        items.Clear();
        for (int i = 0; i < songs.Count; i++)
        {
            var item = Instantiate(songDataPrefab, content);
            item.SetData(songs[i].sprite, i, this);
            items.Add(item);
        }
    }

    public SongData GetData(int i) { return songs[i]; }
}
