using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        private string UIGamePanelRoot = "Prefabs/UI/GamePanel/";

        public GameObject mainCanvasRoot;
        public Dictionary<string, GameObject> gamePanelDictionary;

        [SerializeField] private List<string> openingPanelList;

        [SerializeField] private string hiddingPanelName;
        [SerializeField] private List<string> hiddingPanelList;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            gamePanelDictionary = new Dictionary<string, GameObject>();
            hiddingPanelName = "";
        }

        public static UIManager GetInstance()
        {
            return instance;
        }

        private bool CheckCanvasRootIsNull()
        {
            if (mainCanvasRoot == null)
            {
                Debug.LogError("mainCanvasRoot is null. Pleas add UIManagerHandler.cs.");
                return true;
            }
            return false;
        }

        private bool PanelIsOpen(string name)
        {
            return gamePanelDictionary.ContainsKey(name);
        }

        public GameObject ShowPanel(string name)
        {
            if (CheckCanvasRootIsNull()) { return null; }
            if (PanelIsOpen(name))
            {
                Debug.LogWarning("is Opened");
                return null;
            }

            GameObject loadGo = Utility.AssetRelate.ResourcesLoadCheckNull<GameObject>(UIGamePanelRoot + name);
            if (loadGo == null) { return null; }

            GameObject panel = Utility.GameObjectRelate.InstantiateGameObject(mainCanvasRoot, loadGo);
            panel.name = name;

            gamePanelDictionary.Add(name, panel);
            openingPanelList.Add(name);
            return panel;
        }

        public GameObject OpenPanel(string name)
        {
            if (CheckCanvasRootIsNull()) { return null; }
            if (PanelIsOpen(name))
            {
                Debug.LogWarning("is Opened");
                return null;
            }

            GameObject loadGo = Utility.AssetRelate.ResourcesLoadCheckNull<GameObject>(UIGamePanelRoot + name);
            if (loadGo == null) { return null; }

            GameObject panel = Utility.GameObjectRelate.InstantiateGameObject(mainCanvasRoot, loadGo);
            panel.name = name;

            gamePanelDictionary.Add(name, panel);
            openingPanelList.Add(name);
            return panel;
        }

        public void TogglePanel(string name, bool isOn)
        {
            if (PanelIsOpen(name))
            {
                if (gamePanelDictionary[name] != null)
                {
                    gamePanelDictionary[name].SetActive(isOn);
                    if (!isOn)
                    {
                        hiddingPanelName = gamePanelDictionary[name].name;
                    }
                }
            }
            else { Debug.LogWarning("Toggle panel fail"); }

            if (!isOn)
            {
                hiddingPanelList.Add(name);
                hiddingPanelName = hiddingPanelList[hiddingPanelList.Count - 1];
            }
        }

        public void ClosePanel(string name)
        {
            if (PanelIsOpen(name))
            {
                if (gamePanelDictionary[name] != null)
                {
                    Destroy(gamePanelDictionary[name]);
                    gamePanelDictionary.Remove(name);
                }
                hiddingPanelList.Remove(name);
                if (hiddingPanelList.Count == 0)
                {
                    hiddingPanelName = "";
                    return;
                }
                hiddingPanelName = hiddingPanelList[hiddingPanelList.Count - 1];
            }
            else { Debug.LogWarning("Close fail"); }
        }

        public void ClosePanel(string name, float time) 
        {
            StartCoroutine(DelayClosePanel(name, time));
        }

        private IEnumerator DelayClosePanel(string name, float time)
        {
            yield return new WaitForSeconds(time);

            if (PanelIsOpen(name))
            {
                if (gamePanelDictionary[name] != null)
                {
                    Destroy(gamePanelDictionary[name]);
                    gamePanelDictionary.Remove(name);
                }
                hiddingPanelList.Remove(name);
                if (hiddingPanelList.Count == 0)
                {
                    hiddingPanelName = "";
                    yield return null;
                }
                if (hiddingPanelList.Count > 0) 
                {
                    hiddingPanelName = hiddingPanelList[hiddingPanelList.Count - 1];
                }
                
            }
            else { Debug.LogWarning("Close fail"); }
        }

        public void CloseAllPanel()
        {
            foreach (var item in gamePanelDictionary)
            {
                if (item.Value != null) { Destroy(item.Value); }
            }

            gamePanelDictionary.Clear();
        }

        public Vector2 GetCanvasSize()
        {
            if (CheckCanvasRootIsNull())
                return Vector2.one * -1;

            RectTransform trans = mainCanvasRoot.transform as RectTransform;
            return trans.sizeDelta;
        }

        public bool GetTogglingPanel()
        {
            if (gamePanelDictionary.Count >= 1)
            {
                return true;
            }
            return false;
        }

        public string GetCurrentHiddingPanelName()
        {
            return hiddingPanelName;
        }

        public bool isHiddingListNull()
        {
            return hiddingPanelList.Count > 0 ? false : true;
        }

        public void AddInOpenList(string name) 
        {
            openingPanelList.Add(name);
        }
    }