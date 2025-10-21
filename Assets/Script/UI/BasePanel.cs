using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TNWSalt
{
    public class BasePanel : MonoBehaviour
    {
        [SerializeField] protected float fadeTime = .2f;
		[SerializeField] private KeyCode closeKey = KeyCode.Escape;

		protected RectTransform rectTransform;
		protected UIManager uIManager;

		public virtual void Start()
		{
			uIManager = UIManager.GetInstance();
			rectTransform = GetComponent<RectTransform>();

			OpenPanel();
		}

		public virtual void Update() 
		{
			if (Input.GetKeyDown(closeKey)) { ClosePanel(); }
		}

		public virtual void OpenPanel() 
		{
			rectTransform.localScale = Vector3.zero;
			rectTransform.DOScale(1, fadeTime).SetEase(Ease.OutQuad);
		}

		public virtual void ClosePanel() 
		{
			rectTransform.DOScale(0, 0.2f).SetEase(Ease.InQuad);
			uIManager.ClosePanel(name, fadeTime);
		}

		public virtual void OpenOtherPanel(string name) 
		{
			uIManager.TogglePanel(name, false);
			uIManager.ShowPanel(name);
		}

		/*public void PlayButtonAudio() { AudioManager.GetInstance().PlayUISxf("Click"); }
		public void PlayButtonEnterAudio() { AudioManager.GetInstance().PlayUISxf("Enter"); }*/
	}
}