using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Unit content = null;
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;
    [HideInInspector] public Tile prev;
    [HideInInspector] public int distance;

    private void SetIndicatorColor(Transform indicator, Color color, float alpha)
    {
        Color c = new Color(color.r, color.g, color.b, alpha);
        indicator.GetComponent<SpriteRenderer>().color = c;
    }

    public void MarkForMovement()
    {
        Transform selectionIndicator = transform.Find("SelectionIndicator");
        selectionIndicator.gameObject.SetActive(true);
        SetIndicatorColor(selectionIndicator, Constants.COLOR_MOVEMENT, 0.5f);
    }

    public void MarkForActionTarget()
    {
        Transform selectionIndicator = transform.Find("SelectionIndicator");
        selectionIndicator.gameObject.SetActive(true);
        SetIndicatorColor(selectionIndicator, Constants.COLOR_ACTION_TARGET, 0.5f);
    }

    public void MarkForAoe()
    {
        Transform selectionIndicator = transform.Find("SelectionIndicator");
        selectionIndicator.gameObject.SetActive(true);
        SetIndicatorColor(selectionIndicator, Constants.COLOR_AOE, 0.5f);
    }

    public void Unmark()
    {
        transform.Find("SelectionIndicator").gameObject.SetActive(false);
    }
}