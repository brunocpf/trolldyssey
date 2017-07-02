using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MenuButton
{
    public string sceneName;

    public override void Action()
    {
        base.Action();
        SceneManager.LoadScene(sceneName);
    }
}
