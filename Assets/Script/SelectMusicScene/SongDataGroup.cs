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
	[SerializeField] private SongData data;

	[SerializeField] private Image image;
	[HideInInspector] public int index;

	public void SetData(Sprite sprite, int idx)
	{
		image.sprite = sprite;
		index = idx;
	}
}
