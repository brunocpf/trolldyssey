using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Selection selection;
    public Alliance turnAlliance;
    public Unit currentUnit;
    public Action currentAction;
    private List<Tile> tilesList;
    private List<Tile> aoeTiles;

    [HideInInspector] public GameMap map;

    public BattleState state
    {
        get
        {
            return _state;
        }
        private set
        {
            OnStateExit(_state, value);
            OnStateEnter(_state, value);
        }
    }
    private BattleState _state;

    private void OnStateExit(BattleState oldState, BattleState newState)
    {
        switch (oldState)
        {
            case BattleState.MoveTarget:
                map.UnmarkTiles(tilesList);
                tilesList = null;
                break;
            case BattleState.SelectActionTarget:
                map.UnmarkTiles(tilesList);
                map.UnmarkTiles(aoeTiles);
                tilesList = null;
                aoeTiles = null;
                break;
            default:
                break;
        }
        
    }

    private void OnStateEnter(BattleState oldState, BattleState newState)
    {
        _state = newState;
        switch (newState)
        {
            case BattleState.MoveTarget:
                if (currentUnit != null)
                {
                    tilesList = currentUnit.GetTilesInRange(map);
                    map.MarkTilesForMovement(tilesList);
                }
                break;
            case BattleState.MoveSequence:
                StartCoroutine("MoveSequence");
                break;
            case BattleState.SelectAction:
                currentAction = currentUnit.SelectAction();
                state = BattleState.SelectActionTarget;
                break;
            case BattleState.SelectActionTarget:
                if (currentAction != null)
                {
                    tilesList = currentAction.GetTargetTiles(map, currentUnit.tile);
                    map.MarkTilesForActionTarget(tilesList);
                }
                break;
            default:
                break;
        }
    }

    public void OnMouseOverTile(Tile tile)
    {
        selection.CheckForNewTile(tile);
        if (aoeTiles != null)
            map.UnmarkTiles(aoeTiles);
        aoeTiles = null;
        switch (state)
        {
            case BattleState.SelectActionTarget:
                map.MarkTilesForActionTarget(tilesList);
                aoeTiles = currentAction.GetAoeTiles(map, selection.selectedTile.GetComponent<Tile>(), currentUnit.tile);
                if (tilesList.Contains(selection.selectedTile.GetComponent<Tile>()))
                {
                    map.MarkTilesForAoe(aoeTiles);
                }
                break;
        }
    }

    public void OnFire()
    {
        Tile t = null;
        if (selection.selectedTile != null)
            t = selection.selectedTile.GetComponent<Tile>();
        switch (state)
        {
            case BattleState.SelectUnit:
                if (t != null && t.content != null)
                {
                    if (t.content is Unit)
                    {
                        currentUnit = t.content as Unit;
                        state = BattleState.MoveTarget;
                    }
                }
                break;
            case BattleState.MoveTarget:
                if (currentUnit != null)
                {
                    if (tilesList.Contains(t))
                        state = BattleState.MoveSequence;
                    else
                        state = BattleState.SelectAction;
                }
                break;
            case BattleState.SelectActionTarget:
                if (t != null && currentUnit != null)
                {
                    if (tilesList.Contains(t))
                    {
                        List<Unit> targets = currentAction.GetValidTargets(aoeTiles);
                        if (targets.Count > 0)
                        {
                            currentAction.Use(currentUnit, targets);
                            state = BattleState.SelectUnit;
                        }
                    }
                }
                break;
        }
    }

    public void OnCancel()
    {
        switch (state)
        {
            case BattleState.MoveTarget:
                state = BattleState.SelectUnit;
                break;
            case BattleState.SelectActionTarget:
                state = BattleState.SelectUnit;
                break;
        }
    }

    private IEnumerator MoveSequence()
    {
        Tile t = selection.selectedTile.GetComponent<Tile>();
        yield return StartCoroutine(currentUnit.Traverse(t));
        state = BattleState.SelectAction;
    }

    private void Awake()
    {
        map = GameObject.Find("Map").GetComponent<GameMap>();
    }

    private void Start()
    {
        turnAlliance = Alliance.Ally;
        state = BattleState.SelectUnit;
    }
}
