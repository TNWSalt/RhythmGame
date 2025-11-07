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

    [SerializeField] private DifficultySelection difficulty;
    [SerializeField] private SwipeMenu swipeMenu;

    private List<SongDataGroup> items = new();

    public static SongSelectMenu instance;
    public static SongSelectMenu GetInstance() { return instance; }

	private void Awake()
	{
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;	
    }

	private void Start()
	{
        Populate();
        nameText.text = songs[swipeMenu.currentPosIndex].name;

        if (swipeMenu == null)
        {
            Debug.LogError("請在 SongSelectMenu 的 Inspector 中指定 SwipeMenu 物件！");
            return;
        }

        swipeMenu.InitializeSwipeMenu();
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
            nameText.text = songs[swipeMenu.currentPosIndex].name;
            nameText.DOFade(1, .2f);
            nameText.rectTransform.DOMoveY(100, .2f);
            nameText.rectTransform.DOLocalMoveY(-325, .2f);
        }
	}

	private void Populate()
    {
        foreach (Transform child in content) 
        {
            Destroy(child.gameObject);
        }
            
        items.Clear();
        for (int i = 0; i < songs.Count; i++)
        {
            var item = Instantiate(songDataPrefab, content);
            item.SetData(songs[i].sprite, i, this);
            items.Add(item);
        }
    }

    public void OpenDiffcultySelection(int index) 
    {
        difficulty.gameObject.SetActive(true);
        difficulty.Init(GetData(index));
    }

    public SongData GetData(int i) { return songs[i]; }
}
