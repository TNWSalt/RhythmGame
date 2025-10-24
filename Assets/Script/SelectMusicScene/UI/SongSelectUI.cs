using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SongSelectUI : MonoBehaviour
{
    [Header("References")]
    public List<SongData> songs;
    public ScrollRect scrollRect;
    public RectTransform content;
    public SongDataGroup songItemPrefab;
    public TextMeshProUGUI songNameText;
    public Button startButton;

    private List<SongDataGroup> spawnedItems = new();
    private int currentIndex = 0;
    private bool isDragging = false;

    void Start()
    {
        Populate();
        scrollRect.onValueChanged.AddListener(_ => OnScrollChanged());
        startButton.onClick.AddListener(OnStartClicked);
        UpdateSelection();
    }

    void Populate()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        spawnedItems.Clear();
        for (int i = 0; i < songs.Count; i++)
        {
            var item = Instantiate(songItemPrefab, content);
            item.SetData(songs[i].sprite, i);
            spawnedItems.Add(item);
        }
    }

    void OnScrollChanged()
    {
        // 自動偵測中心項
        float centerPos = scrollRect.viewport.rect.width / 2f;
        float minDist = float.MaxValue;
        int nearestIndex = 0;

        for (int i = 0; i < spawnedItems.Count; i++)
        {
            var item = spawnedItems[i];
            float dist = Mathf.Abs(centerPos - GetViewportX(item.transform.position));
            if (dist < minDist)
            {
                minDist = dist;
                nearestIndex = i;
            }

            // 放大中心項目
            float scale = Mathf.Lerp(1f, 1.3f, 1f - Mathf.Clamp01(dist / 300f));
            item.transform.localScale = Vector3.one * scale;
        }

        if (nearestIndex != currentIndex)
        {
            currentIndex = nearestIndex;
            UpdateSelection();
        }
    }

    float GetViewportX(Vector3 worldPos)
    {
        Vector3 localPos = scrollRect.viewport.InverseTransformPoint(worldPos);
        return localPos.x + (scrollRect.viewport.rect.width / 2f);
    }

    void UpdateSelection()
    {
        songNameText.text = songs[currentIndex].name;
    }

    void OnStartClicked()
    {
        GameManager.GetInstance().SetSongData(songs[currentIndex]);
    }
}
