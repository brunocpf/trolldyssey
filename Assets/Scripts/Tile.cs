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


    public void MarkForMovement()
    {
        transform.Find("SelectionIndicator").gameObject.SetActive(true);
    }

    public void UnmarkForMovement()
    {
        transform.Find("SelectionIndicator").gameObject.SetActive(false);
    }
}