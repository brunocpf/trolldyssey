using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MenuButton
{
    public override void Action()
    {
        base.Action();
        SceneManager.LoadScene("TitleScreen");
    }
}
