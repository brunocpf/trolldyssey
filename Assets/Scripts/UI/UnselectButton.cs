using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnselectButton : MonoBehaviour
{
    public void Unselect()
    {
        Button button = GetComponent<Button>();
        button.enabled = false;
        button.enabled = true;
        //EventSystem.current.SetSelectedGameObject(null);
    }
}
