using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Battler))]
public class Unit : MonoBehaviour
{

    [HideInInspector] public Battler battler;

    public int range { get { return battler.mov; } }
    public float moveTime = 0.25f;
    public Tile tile { get; protected set; }

    public void Place(Tile target)
    {
        if (tile != null && tile.content == this)
            tile.content = null;
        tile = target;
        if (target != null)
            target.content = this;
    }

    public Action SelectAction()
    {
        Action[] actions = GetComponentsInChildren<Action>();
        return actions[0];
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

    private void Awake()
    {
        battler = GetComponent<Battler>();
        GameObject.Find("Popups Canvas").GetComponent<SceneHPGaugeController>().ShowHPGauge(this);
    }

    private void Start()
    {
        
    }

}