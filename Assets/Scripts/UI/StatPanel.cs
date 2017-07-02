using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class StatPanel : MonoBehaviour
{

    private Selection selection;
    private RectTransform rectTransform;
    private Unit selectedUnit;
    private bool right = true;
    private bool hidden = true;

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

    private Text atkValue;
    private Text defValue;
    private Text matValue;
    private Text agiValue;
    private Text movValue;
    private Text hpValues;
    private Image namePanel;
    private Image hpFill;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        selection = GameObject.Find("Selection").GetComponent<Selection>();
        coordText = GameObject.Find("CoordText").GetComponent<Text>();
        nameText = GameObject.Find("NameText").GetComponent<Text>();
        atkValue = GameObject.Find("AtkValue").GetComponent<Text>();
        defValue = GameObject.Find("DefValue").GetComponent<Text>();
        matValue = GameObject.Find("MatValue").GetComponent<Text>();
        //agiValue = GameObject.Find("AgiValue").GetComponent<Text>();
        movValue = GameObject.Find("MovValue").GetComponent<Text>();
        hpValues = GameObject.Find("HPValues").GetComponent<Text>();
        namePanel = GameObject.Find("NameTextPanel").GetComponent<Image>();
        hpFill = GameObject.Find("PanelHPFill").GetComponent<Image>();
        selectedUnit = null;
    }


    void Start()
    {
        UpdateStats();
    }


    void Update()
    {
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
        UpdateColor(selectedUnit == null ? Alliance.None : selectedUnit.battler.alliance);
        if (right != oldRight || hidden != oldHidden)
            StartCoroutine("AdjustPosition");
    }

    private void UpdateColor(Alliance alliance)
    {
        Color color = Constants.GetAllianceColor(alliance);
        namePanel.color = new Color(color.r, color.g, color.b);
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
            //agiValue.text = "--";
            movValue.text = "--";
            hpValues.text = "--/--";
            return;
        }
        nameText.text = selectedUnit.battler.battlerName;
        atkValue.text = selectedUnit.battler.atk.ToString().PadLeft(2, '0');
        defValue.text = selectedUnit.battler.def.ToString().PadLeft(2, '0');
        matValue.text = selectedUnit.battler.mat.ToString().PadLeft(2, '0');
        //agiValue.text = selectedUnit.battler.agi.ToString().PadLeft(2, '0');
        movValue.text = selectedUnit.battler.mov.ToString().PadLeft(2, '0');
        hpValues.text = selectedUnit.battler.hp.ToString() + "/" + selectedUnit.battler.mhp.ToString();
        hpFill.fillAmount = selectedUnit.battler.HpRate();
    }
}
