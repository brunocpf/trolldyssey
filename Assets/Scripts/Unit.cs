using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    public int range = 3;
    public float moveTime = 0.25f;
    public Tile tile { get; protected set; }
    private List<Tile> path;

    public void Place(Tile target)
    {
        if (tile != null && tile.content == this)
            tile.content = null;
        tile = target;
        if (target != null)
            target.content = this;
    }

    public void Match()
    {
        transform.localPosition = tile.transform.localPosition;
    }

    public List<Tile> GetTilesInRange(GameMap map)
    {
        List<Tile> retValue = map.Search(tile, ExpandSearch);
        Filter(retValue);
        return retValue;
    }

    protected void Filter(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
            if (tiles[i].content != null)
                tiles.RemoveAt(i);
    }

    protected bool ExpandSearch(Tile from, Tile to)
    {
        if (to.content != null)
            return false;

        return (from.distance + 1) <= range;
    }

    public IEnumerator Traverse(Tile tile)
    {
        Place(tile);
        List<Tile> targets = new List<Tile>();
        while (tile != null)
        {
            targets.Insert(0, tile);
            tile = tile.prev;
        }
        for (int i = 1; i < targets.Count; ++i)
        {
            Debug.Log(i);
            Tile from = targets[i - 1];
            Tile to = targets[i];
            yield return StartCoroutine(Walk(to));
        }
        yield return null;
    }

    IEnumerator Walk(Tile target)
    {
        Tweener tweener = transform.DOLocalMove(target.transform.localPosition, moveTime, false).SetEase(Ease.Linear);
        yield return tweener.WaitForCompletion();
    }

}

