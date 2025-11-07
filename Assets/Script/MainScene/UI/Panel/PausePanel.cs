using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : BasePanel
{
    public void Continue() 
    {
        ClosePanel();
        PauseManager.GetInstance().Continue();
    }

    public void Restart() 
    {
        SceneController.GetInstance().ReloadCurrentScene(.5f);
    }

    public void ClickBakButton()
    {
        //SceneController.GetInstance().LoadScene(SceneManager.GetActiveScene().buildIndex - 1, .5f);
        SceneController.GetInstance().LoadSceneAsync("SelectMusicScene", .5f);
    }
}
