using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateActionName : MonoBehaviour {
    private Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<Text>();
    }

    private void OnEnable()
    {
        textComponent.text = BattleManager.instance.currentAction.actionName;
    }
}
