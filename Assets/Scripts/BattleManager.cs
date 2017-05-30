using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    public Selection selection;
    public Alliance turnAlliance;
    public Action currentAction;

    private Tile origTile;
    private List<Unit> targets;
    private List<Tile> tilesList;
    private List<Tile> aoeTiles;


    public Unit currentUnit
    {
        get { return _currentUnit; }
        set {
            if (currentUnit != null)
                currentUnit.UnsetCurrent();
            _currentUnit = value;
            if (currentUnit != null)
                currentUnit.SetCurrent();
        }
    }
    private Unit _currentUnit;


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

    public List<Unit> AllUnits(Alliance alliance)
    {
        return GameMap.instance.GetAllContent().Where(unit => (unit.battler.alliance & alliance) == unit.battler.alliance).ToList();
    }

    public List<Unit> MovableUnits(Alliance alliance)
    {
        return AllUnits(alliance).Where(unit => unit.canAct).ToList();
    }

    private void OnStateExit(BattleState oldState, BattleState newState)
    {
        switch (oldState)
        {
            case BattleState.MoveTarget:
                GameMap.instance.UnmarkTiles(tilesList);
                tilesList = null;
                break;
            case BattleState.SelectActionTarget:
                GameMap.instance.UnmarkTiles(tilesList);
                GameMap.instance.UnmarkTiles(aoeTiles);
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
            case BattleState.Init:
                selection.UpdateSelection(GameMap.instance.GetAllySpawnLocations()[0]);
                turnAlliance = Alliance.Ally;
                state = BattleState.SelectUnit;
                break;
            case BattleState.SelectUnit:
                currentUnit = null;
                if (MovableUnits(turnAlliance).Count < 1)
                    state = BattleState.EndTurn;
                break;
            case BattleState.MoveTarget:
                if (currentUnit != null)
                {
                    origTile = currentUnit.tile;
                    tilesList = currentUnit.GetTilesInRange(GameMap.instance);
                    GameMap.instance.MarkTilesForMovement(tilesList);
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
                    tilesList = currentAction.GetTargetTiles(GameMap.instance, currentUnit.tile);
                    GameMap.instance.MarkTilesForActionTarget(tilesList);
                }
                break;
            case BattleState.ExecuteAction:
                currentUnit.UnsetCurrent();
                currentAction.Use(currentUnit, targets);
                state = BattleState.SelectUnit;
                break;
            case BattleState.EndTurn:
                List<Unit> units = AllUnits(turnAlliance);
                foreach (Unit unit in units)
                    unit.OnTurnEnd();
                turnAlliance = (turnAlliance == Alliance.Ally) ? Alliance.Enemy : Alliance.Ally;
                Debug.Log("Turn Ended!");
                state = BattleState.SelectUnit;
                break;
            default:
                break;
        }
    }

    public void UpdateSelections(Tile tile)
    {
        selection.UpdateSelection(tile);
        if (aoeTiles != null)
            GameMap.instance.UnmarkTiles(aoeTiles);
        aoeTiles = null;
        switch (state)
        {
            case BattleState.SelectActionTarget:
                GameMap.instance.MarkTilesForActionTarget(tilesList);
                aoeTiles = currentAction.GetAoeTiles(GameMap.instance, selection.selectedTile.GetComponent<Tile>(), currentUnit.tile);
                if (tilesList.Contains(selection.selectedTile.GetComponent<Tile>()))
                {
                    GameMap.instance.MarkTilesForAoe(aoeTiles);
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
                    if ((t.content.battler.alliance & turnAlliance) == t.content.battler.alliance && t.content.canAct) //t.content.battler.alliance == Alliance.Ally && t.content.canAct)
                    {
                        currentUnit = t.content;
                        state = BattleState.MoveTarget;
                    }
                }
                else
                {
                    OpenActionMenu();
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
                        targets = currentAction.GetValidTargets(aoeTiles);
                        if (targets.Count > 0)
                            state = BattleState.ExecuteAction;
                    }
                }
                break;
        }
    }

    private void OpenActionMenu()
    {
        throw new NotImplementedException();
    }

    public void OnCancel()
    {
        switch (state)
        {
            case BattleState.MoveTarget:
                state = BattleState.SelectUnit;
                break;
            case BattleState.SelectActionTarget:
                currentUnit.Place(origTile);
                currentUnit.Match();
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
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        state = BattleState.Init;
    }
}
