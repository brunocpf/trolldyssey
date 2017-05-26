using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Selection selection;
    public GameMap map;
    public Unit currentUnit;
    private List<Tile> tilesList;


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
            case BattleState.PlayerMoveTarget:
                map.UnmarkTiles(tilesList);
                tilesList = null;
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
            case BattleState.PlayerMoveTarget:
                if (currentUnit != null)
                {
                    tilesList = currentUnit.GetTilesInRange(map);
                    map.MarkTilesForMovement(tilesList);
                }
                break;
            case BattleState.PlayerMoveSequence:
                StartCoroutine("MoveSequence");
                break;
            default:
                break;
        }
    }

    public void OnMouseOverTile(Tile tile)
    {
        selection.CheckForNewTile(tile);
    }

    public void OnFire()
    {
        Tile t = null;
        if (selection.selectedTile != null)
            t = selection.selectedTile.GetComponent<Tile>();
        switch (state)
        {
            case BattleState.PlayerSelectUnit:
                if (t != null && t.content != null)
                    if (t.content is Unit)
                    {
                        currentUnit = t.content as Unit;
                        state = BattleState.PlayerMoveTarget;
                    }
                break;
            case BattleState.PlayerMoveTarget:
                if (t != null && currentUnit != null)
                {
                    if (tilesList.Contains(t))
                        state = BattleState.PlayerMoveSequence;
                }
                break;
        }
    }

    public void OnCancel()
    {

    }

    private IEnumerator MoveSequence()
    {
        Tile t = selection.selectedTile.GetComponent<Tile>();
        yield return StartCoroutine(currentUnit.Traverse(t));
        state = BattleState.PlayerSelectUnit;
    }

    private void Start()
    {
        state = BattleState.PlayerSelectUnit;
    }
}
