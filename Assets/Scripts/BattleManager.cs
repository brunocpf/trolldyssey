using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public enum VictoryCondition
    {
        AllEnemiesDead,
        GetToPos
    }

    public VictoryCondition victoryCondition = VictoryCondition.AllEnemiesDead;
    public int goalPosX = 0;
    public int goalPosY = 0;


    public static BattleManager instance = null;

    public Animator turnEndAnimation;
    public Text turnText;
    public Selection selection;
    public Alliance turnAlliance;
    public BaseAction currentAction;
    public BaseAction waitAction;
    public Button endTurnButton;
    public GameObject actionNamePanel;
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

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

    private Tile currentMoveTarget;
    private Tile currentActionTarget;

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

    public bool canSelect = false;

    public List<Unit> AllUnits(Alliance alliance)
    {
        return GameMap.instance.GetAllContent().Where(unit => unit.battler.alliance.IsSameAlliance(alliance)).ToList();
    }

    public List<Unit> MovableUnits(Alliance alliance)
    {
        return AllUnits(alliance).Where(unit => unit.canAct).ToList();
    }

    public void TriggerGameOver()
    {
        foreach (Unit unit in AllUnits(Alliance.Ally))
        {
            unit.battler.Die();
        }
        state = BattleState.GameOver;
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
            case BattleState.ExecuteAction:
                actionNamePanel.SetActive(false);

                break;
            default:
                break;
        }
    }

    public void EndTurn()
    {
        endTurnButton.GetComponent<UnselectButton>().Unselect();
        state = BattleState.EndTurn;
    }

    private void OnStateEnter(BattleState oldState, BattleState newState)
    {
        _state = newState;
        switch (newState)
        {
            case BattleState.Init:
                selection.UpdateSelection(GameMap.instance.GetAllySpawnLocations()[0]);
                turnAlliance = Alliance.Ally;
                UpdateTurnText();
                state = BattleState.SelectUnit;
                break;
            case BattleState.SelectUnit:
                currentUnit = null;
                RemoveDeadUnits();
                if (CheckLossConditions())
                {
                    state = BattleState.GameOver;
                }
                else if (CheckVictoryConditions())
                {
                    state = BattleState.Victory;
                }
                else
                {
                    if (MovableUnits(turnAlliance).Count < 1)
                        state = BattleState.EndTurn;
                    else if (turnAlliance == Alliance.Enemy)
                    {
                        currentUnit = MovableUnits(turnAlliance)[0];
                        UpdateSelections(currentUnit.tile);
                        state = BattleState.MoveTarget;
                    }
                }
                break;
            case BattleState.MoveTarget:
                if (currentUnit != null)
                {
                    origTile = currentUnit.tile;
                    tilesList = currentUnit.GetTilesInRange(GameMap.instance);
                    GameMap.instance.MarkTilesForMovement(tilesList);
                    if (turnAlliance == Alliance.Enemy)
                        StartCoroutine(MakeAiMovement());
                }
                break;
            case BattleState.MoveSequence:
                StartCoroutine(MoveSequence());
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
                    if (turnAlliance == Alliance.Enemy)
                        StartCoroutine(AiActionTargetSequence());
                }
                break;
            case BattleState.ExecuteAction:
                StartCoroutine(ActionSequence());
                break;
            case BattleState.EndTurn:
                StartCoroutine(TurnEndSequence());
                break;
            case BattleState.Victory:
                ProcessVictory();
                break;
            case BattleState.GameOver:
                ProcessGameOver();
                break;
            default:
                break;
        }
    }

    private IEnumerator AiActionTargetSequence()
    {
        Tile t = MakeAiActionTarget(currentAction);
        if (t == null)
        {
            currentAction = waitAction;
        }
        else
        {
            UpdateSelections(t);
            yield return new WaitForSeconds(1.5f);
            targets = currentAction.GetValidTargets(aoeTiles);
        }
        if (targets == null || targets.Count < 1)
            currentAction = waitAction;
        state = BattleState.ExecuteAction;
    }

    private Tile MakeAiActionTarget(BaseAction action)
    {
        List<Tile> possibleTargets = action.GetTargetTiles(GameMap.instance, currentUnit.tile);
        List<int> advantages = new List<int>();
        foreach (Tile t in possibleTargets)
        {
            List<Unit> aiTargets = action.GetValidTargets(action.GetAoeTiles(GameMap.instance, t, currentUnit.tile));
            advantages.Add(action.CalcTotalAdvantage(currentUnit, aiTargets));
        }
        int maxAdvantage = 0;
        int maxAdvantageIndex = -1;
        for (int i = 0; i < advantages.Count; i++)
        {
            if (advantages[i] > maxAdvantage)
            {
                maxAdvantage = advantages[i];
                maxAdvantageIndex = i;
            }
        }
        if (maxAdvantageIndex > -1)
        {
            return possibleTargets[maxAdvantageIndex];
        }
        else
        {
            return null;
        }
    }

    private IEnumerator MakeAiMovement()
    {
        yield return new WaitForSeconds(1.5f);
        if (tilesList == null || tilesList.Count < 1)
        {
            currentMoveTarget = currentUnit.tile;
            state = BattleState.SelectAction;
            yield break;
        }
        List<Tile> possibleTiles = tilesList;
        int randomNumber = UnityEngine.Random.Range(0, tilesList.Count - 1);
        Tile randomTile = possibleTiles[randomNumber];
        if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
            randomTile = currentUnit.tile;
        if (randomTile == null)
        {
            currentMoveTarget = currentUnit.tile;
            state = BattleState.SelectAction;
        }
        else
        {
            currentMoveTarget = randomTile;
            UpdateSelections(currentMoveTarget);
            yield return new WaitForSeconds(0.5f);
            state = BattleState.MoveSequence;
        }
    }

    private IEnumerator TurnEndSequence()
    {
        List<Unit> units = AllUnits(turnAlliance);
        foreach (Unit unit in units)
            unit.OnTurnEnd();
        turnAlliance = (turnAlliance == Alliance.Ally) ? Alliance.Enemy : Alliance.Ally;
        Debug.Log("Turn Ended!");
        turnEndAnimation.SetTrigger("EndTurn");
        turnEndAnimation.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);
        UpdateTurnText();
        state = BattleState.SelectUnit;
    }

    private IEnumerator ActionSequence()
    {
        currentUnit.UnsetCurrent();
        actionNamePanel.SetActive(true);
        yield return StartCoroutine(currentAction.Use(currentUnit, targets));
        state = BattleState.SelectUnit;
    }

    public void ProcessGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    private void ProcessVictory()
    {
        victoryPanel.SetActive(true);
    }

    private bool CheckLossConditions()
    {
        if (AllUnits(Alliance.Ally).Count < 1)
            return true;
        return false;
    }

    private void RemoveDeadUnits()
    {
        List<Unit> allies = AllUnits(Alliance.Ally).ToList();
        List<Unit> enemies = AllUnits(Alliance.Enemy).ToList();
        List<Unit> others = AllUnits(Alliance.None).ToList();
        foreach (Unit unit in allies)
        {
            if (unit.battler.IsDead())
            {
                unit.Place(null);
                unit.battler.RecoverAll();
                //unit.transform.SetParent(GameManager.instance.transform);
                unit.gameObject.SetActive(false);
            }
        }
        foreach (Unit unit in enemies)
        {
            if (unit.battler.IsDead())
            {
                unit.Place(null);
                unit.gameObject.SetActive(false);
            }
        }
        foreach (Unit unit in others)
        {
            if (unit.battler.IsDead())
            {
                unit.Place(null);
                unit.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateTurnText()
    {
        switch (turnAlliance)
        {
            case Alliance.Ally:
                turnText.text = "Jogador";
                turnText.color = Constants.ALLY_COLOR;
                break;
            case Alliance.Enemy:
                turnText.text = "Inimigos";
                turnText.color = Constants.ENEMY_COLOR;
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
        if (turnAlliance == Alliance.Enemy)
            return;
        Tile t = null;
        if (selection.selectedTile != null)
            t = selection.selectedTile.GetComponent<Tile>();
        switch (state)
        {
            case BattleState.SelectUnit:
                if (t != null && t.content != null)
                {
                    if (t.content.battler.alliance.IsSameAlliance(turnAlliance) && t.content.canAct) //t.content.battler.alliance == Alliance.Ally && t.content.canAct)
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
                    {
                        currentMoveTarget = t;
                        state = BattleState.MoveSequence;
                    }
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
                        {
                            currentActionTarget = t;
                            state = BattleState.ExecuteAction;
                        }
                    }
                    else
                    {
                        currentAction = waitAction;
                        state = BattleState.ExecuteAction;
                    }
                }
                break;
        }
    }

    private void OpenActionMenu()
    {
        //throw new NotImplementedException();
    }

    private bool CheckVictoryConditions()
    {
        if (AllUnits(Alliance.Enemy).Count < 1)
            if (victoryCondition == VictoryCondition.AllEnemiesDead)
                return true;
        return false;
    }

    public void OnCancel()
    {
        if (turnAlliance == Alliance.Enemy)
            return;
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
        Tile t = currentMoveTarget;
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

    private void Update()
    {
        if (turnAlliance != Alliance.Ally || state == BattleState.MoveTarget || state == BattleState.SelectActionTarget)
            endTurnButton.interactable = false;
        else
            endTurnButton.interactable = true;

        if (turnAlliance != Alliance.Ally)
            canSelect = false;
        else
            canSelect = true;
    }

    private void OnEnable()
    {
        EventManager.StartListening("Fire", OnFire);
        EventManager.StartListening("Cancel", OnCancel);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Fire", OnFire);
        EventManager.StopListening("Cancel", OnCancel);
    }
}
