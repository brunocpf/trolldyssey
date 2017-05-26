using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Selection : MonoBehaviour
{
    public int gridX = 0;
    public int gridY = 0;
    public GameObject selectedTile = null;

    public void CheckForNewTile(Tile tile)
    {
        if (gridX != tile.gridX || gridY != tile.gridY)
        {
            selectedTile = tile.gameObject;
            UpdatePosition(tile.gridX, tile.gridY);
        }
    }

    void UpdatePosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        transform.DOLocalMove(new Vector2(x,y), 0.125f, false);
    }

    public void OnSelection()
    {
        if (selectedTile == null)
            return;
        //GameObject content = selectedTile.GetComponent<Tile>().content;
        //if ( != null)
    }
}
