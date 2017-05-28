﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class StatPanel : MonoBehaviour {

    private Selection selection;
    private RectTransform rectTransform;
    private Unit selectedUnit;
    private bool right = true;
    private bool hidden = true;

    private Vector2 rightMin = new Vector2(0.5f, 0.0f);
    private Vector2 rightMax = new Vector2(1.0f, 0.3f);
    private Vector2 leftMin = new Vector2(0.0f, 0.0f);
    private Vector2 leftMax = new Vector2(0.5f, 0.3f);

    private float minHiddenY = -0.25f;
    private float maxHiddenY = 0.05f;
    private float minShownY = 0.0f;
    private float maxShownY = 0.3f;

    private float minRightX = 0.5f;
    private float maxRightX = 1.0f;
    private float minLeftX = 0.0f;
    private float maxLeftX = 0.5f;

    private Text coordText;
    private Text nameText;

    private TextMeshProUGUI atkValue;
    private TextMeshProUGUI defValue;
    private TextMeshProUGUI matValue;
    private TextMeshProUGUI agiValue;
    private TextMeshProUGUI movValue;

    private void Awake()
    {
        selection = GameObject.Find("Selection").GetComponent<Selection>();
        rectTransform = GetComponent<RectTransform>();
        coordText = GameObject.Find("CoordText").GetComponent<Text>();
        nameText = GameObject.Find("NameText").GetComponent<Text>();
        atkValue = GameObject.Find("AtkValue").GetComponent<TextMeshProUGUI>();
        defValue = GameObject.Find("DefValue").GetComponent<TextMeshProUGUI>();
        matValue = GameObject.Find("MatValue").GetComponent<TextMeshProUGUI>();
        agiValue = GameObject.Find("AgiValue").GetComponent<TextMeshProUGUI>();
        movValue = GameObject.Find("MovValue").GetComponent<TextMeshProUGUI>();
        selectedUnit = null;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bool oldRight = right;
        bool oldHidden = hidden;
        if (selection == null || selection.selectedTile == null)
            return;
        coordText.text = "<" + selection.gridX + "," + selection.gridY + ">";
        selectedUnit = selection.selectedTile.GetComponent<Tile>().content;
        if (selectedUnit != null)
        {
            UpdateStats();
            if (hidden)
                hidden = false;
        }
        else if (selectedUnit == null && !hidden)
            hidden = true;
        if (selection.gridX >= 5 && right)
            right = false;
        else if (selection.gridX < 5 && !right)
            right = true;
        if (right != oldRight || hidden != oldHidden)
            StartCoroutine("AdjustPosition");
	}

    private IEnumerator AdjustPosition()
    {
        Vector2 newMaxAnchor = new Vector2(right ? maxRightX : maxLeftX, hidden ? maxHiddenY : maxShownY);
        Vector2 newMinAnchor = new Vector2(right ? minRightX : minLeftX, hidden ? minHiddenY : minShownY);
        Tweener tweener = rectTransform.DOAnchorMin(newMinAnchor, 0.25f, false).SetEase(Ease.OutBack);
        rectTransform.DOAnchorMax(newMaxAnchor, 0.25f, false).SetEase(Ease.OutBack);
        yield return tweener.WaitForCompletion();
    }


    private void UpdateStats()
    {
        if (selectedUnit == null)
        {
            nameText.text = "";
            atkValue.text = "--";
            defValue.text = "--";
            matValue.text = "--";
            agiValue.text = "--";
            movValue.text = "--";
            return;
        }
        nameText.text = selectedUnit.battler.battlerName;
        atkValue.text = "<mspace=2.75em>" + selectedUnit.battler.atk.ToString().PadLeft(2, '0');
        defValue.text = "<mspace=2.75em>" + selectedUnit.battler.def.ToString().PadLeft(2, '0');
        matValue.text = "<mspace=2.75em>" + selectedUnit.battler.mat.ToString().PadLeft(2, '0');
        agiValue.text = "<mspace=2.75em>" + selectedUnit.battler.agi.ToString().PadLeft(2, '0');
        movValue.text = "<mspace=2.75em>" + selectedUnit.battler.mov.ToString().PadLeft(2, '0');
    }
}
