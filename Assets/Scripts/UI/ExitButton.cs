using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MenuButton
{
    public override void Action()
    {
        base.Action();
        Application.Quit();
    }
}
