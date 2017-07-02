using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Battler))]
public class Unit : MonoBehaviour
{
    public Color cantActColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

    [HideInInspector] public Battler battler;

    public bool canAct { get { return !hasActedThisTurn && battler.canMove && gameObject.activeSelf; } }
    public bool hasActedThisTurn
    {
        get
        { return _hasActedThisTurn; }
        set
        {
            _hasActedThisTurn = value;
            RefreshColor();
        }
    }


    public bool isCurrent = false;
    private Tween colorAnimation;

    private void RefreshColor()
    {
        if (!canAct)
            sRenderer.color = cantActColor;
        else
            sRenderer.color = Color.white;
    }

    public void UnsetCurrent()
    {
        if (colorAnimation != null)
            colorAnimation.Kill();
        RefreshColor();
    }

    public void SetCurrent()
    {
        colorAnimation = sRenderer.DOColor(Color.blue, 0.5f).From().SetEase(Ease.OutFlash).SetLoops(-1);
    }

    public int range { get { return battler.mov; } }
    public float moveTime = 0.25f;
    public Tile tile { get; protected set; }

    private bool _hasActedThisTurn = false;
    private SpriteRenderer sRenderer;


    public void Place(Tile target)
    {
        if (target == null)
        {
            tile.content = null;
            tile = null;
            return;
        }
        if (tile != null && tile.content == this)
            tile.content = null;
        tile = target;
        if (target != null)
            target.content = this;
    }

    public BaseAction SelectAction()
    {
        BaseAction[] actions = GetComponentsInChildren<BaseAction>();
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

    private void Filter(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
            if (tiles[i].content != null)
                tiles.RemoveAt(i);
    }

    private bool ExpandSearch(Tile from, Tile to)
    {
        if (to.content != null && !to.content.battler.alliance.IsSameAlliance(battler.alliance))
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

    private IEnumerator Walk(Tile target)
    {
        Tweener tweener = transform.DOLocalMove(target.transform.localPosition, moveTime, false).SetEase(Ease.Linear);
        yield return tweener.WaitForCompletion();
    }

    public void OnTurnEnd()
    {
        hasActedThisTurn = false;
    }

    private void ShowHPGauge()
    {
        OverlayHPGaugeSpawner.instance.CreateGauge(this);
    }

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        battler = GetComponent<Battler>();
    }

    private void Start()
    {
        ShowHPGauge();
    }

    private void Update()
    {
        //if (isCurrent)
            //StartCoroutine("Flicker");
    }

}