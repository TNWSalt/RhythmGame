using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public static SceneController GetInstance() { return instance; }

    [SerializeField] private Image blackPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 切換場景也不銷毀
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 基本功能

    /// <summary>
    /// 加載指定場景 (立即切換)
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene(int sceneIndex, float time) 
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex, time));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex, float time)
    {
        FadeIn(time);
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(sceneIndex);
        //yield return new WaitForSeconds(time);
        FadeOut(time);
    }

    /// <summary>
    /// 加載指定場景 (可異步)
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    public void LoadSceneAsync(string sceneName, float time)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName,time));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            // 可加載進度顯示
            Debug.Log("Loading progress: " + asyncLoad.progress);
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName,float time)
    {
        FadeIn(time);
        yield return new WaitForSeconds(time);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            // 可加載進度顯示
            Debug.Log("Loading progress: " + asyncLoad.progress);
            yield return null;
        }
        FadeOut(time);
    }

    /// <summary>
    /// 重新加載當前場景
    /// </summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReloadCurrentScene(float time)
    {
        StartCoroutine(ReloadScene(time));
    }

    private IEnumerator ReloadScene(float time)
    {
        FadeIn(time);
        yield return new WaitForSeconds(time + .2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FadeOut(time);
    }

    /// <summary>
    /// 加載下一個場景（依Build Settings順序）
    /// </summary>
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextIndex);
    }

    public void FadeIn(float time = .5f) 
    {
        blackPanel.raycastTarget = true;
        blackPanel.DOFade(1, time);
    } 

    public void FadeOut(float time = .5f)
    {
        blackPanel.DOFade(0, time);
        blackPanel.raycastTarget = false;
    }

    #endregion
}
