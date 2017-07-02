using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battler : MonoBehaviour
{

    public BattlerData data;
    public bool canObtainExp = true;
    public bool scaleInitialStats = true;

    public Alliance alliance;

    public string battlerName { get { return _name; } }

    public int level { get { return _level; } }
    public int exp { get { return _exp; } }

    public int hp { get { return _hp; } }
    public int nrg { get { return _nrg; } }

    public int mhp { get { return Param(StatType.MHP); } }
    public int mnrg { get { return Param(StatType.MNRG); } }
    public int atk { get { return Param(StatType.ATK); } }
    public int def { get { return Param(StatType.DEF); } }
    public int mat { get { return Param(StatType.MAT); } }
    public int agi { get { return Param(StatType.AGI); } }
    public int mov { get { return Param(StatType.MOV); } }

    public State[] states { get { return GetComponentsInChildren<State>(); } }
    public BaseAction[] learnedActions { get { return GetComponentsInChildren<BaseAction>(); } }

    public bool isMaxLevel { get { return level >= Constants.MAX_LEVEL; } }

    public bool canMove = true;

    private string _name;
    private int _hp;
    private int _nrg;
    private int _level;
    private int _exp;
    private Unit unit;

    public void DealDamage(int v)
    {
        _hp -= v;
        DamagePopupSpawner.instance.CreatePopup(unit, v);
        Refresh();
    }

    private int[] _paramsPlus = new int[(int)StatType.Count];
    private float[] _paramsRate = new float[(int)StatType.Count];

    private void Awake()
    {
        unit = GetComponent<Unit>();
        InitializeBattler();
    }

    private void InitializeBattler()
    {
        _hp = 1;
        _nrg = 0;
        ClearStates();
        ClearParams();
        Setup();
        Refresh();
    }

    private void Setup()
    {
        _name = data.battlerName;
        _level = data.initialLevel;
        RecoverAll();
        InitActions();
    }

    private void ClearParams()
    {
        for (StatType stat = 0; stat < StatType.Count; stat++)
        {
            _paramsPlus[(int)stat] = 0;
            _paramsRate[(int)stat] = 1.0f;
        }
    }

    public int ExpForLevel(int lv)
    {
        return Mathf.Clamp(2 * lv * lv, Constants.MIN_EXP, Constants.MAX_EXP);
    }

    public void InitExp()
    {
        _exp = ExpForCurrentLevel();
    }

    public int ExpForCurrentLevel()
    {
        return ExpForLevel(level);
    }

    public int ExpForNextLevel()
    {
        return ExpForLevel(level + 1);
    }

    public int ExpForLevelUp()
    {
        return ExpForNextLevel() - exp;
    }

    public void InitActions()
    {
        foreach (Learning learning in data.learnings)
        {
            if (learning.level <= level)
                LearnAction(learning.action);
        }
    }

    public void LearnAction(BaseAction action)
    {
        Instantiate(action.gameObject, transform.Find("Actions"));
    }

    private void ChangeExp(int value)
    {
        if (value < exp)
            return;
        _exp = Mathf.Clamp(Mathf.Max(value, 0), Constants.MIN_EXP, Constants.MAX_EXP);
        int oldLevel = level;
        while (!isMaxLevel && exp >= ExpForNextLevel())
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _level++;
        foreach (Learning learning in data.learnings)
        {
            if (learning.level == level)
                LearnAction(learning.action);
        }
    }

    public void GainExp(int value)
    {
        if (!canObtainExp)
            return;
        ChangeExp(value + exp);
    }

    void ClearStates()
    {
        foreach (State state in states)
            Destroy(state.gameObject);
    }

    void EraseState(int stateId)
    {
        foreach (State state in states)
        {
            if (stateId == state.stateId)
                Destroy(state.gameObject);
        }
    }

    bool IsStateAffected(int stateId)
    {
        foreach (State state in states)
        {
            if (state.stateId == stateId)
                return true;
        }
        return false;
    }

    bool IsDeathStateAffected()
    {
        return IsStateAffected(Constants.DEATH_STATE_ID);
    }

    void OnDeathClearStates()
    {
        foreach (State state in states)
        {
            if (state.removeOnDeath)
                Destroy(state.gameObject);
        }
    }

    public void Die()
    {
        _hp = 0;
        OnDeathClearStates();
    }

    void Revive()
    {
        if (_hp == 0)
        {
            _hp = 1;
        }
    }

    int ParamBase(StatType stat)
    {
        float modifier = scaleInitialStats ? (level * data.growthRate[(int)stat]) : 0f;
        return Mathf.Clamp(Mathf.FloorToInt(data.initialStats[(int)stat] + modifier), Constants.MIN_STATS[(int) stat], Constants.MAX_STATS[(int)stat]);
    }

    int ParamPlus(StatType stat)
    {
        return _paramsPlus[(int)stat];
    }

    float ParamRate(StatType stat)
    {
        return _paramsRate[(int)stat];
    }

    int Param(StatType stat)
    {
        float value = ParamBase(stat) + ParamPlus(stat);
        value *= ParamRate(stat);
        return Mathf.RoundToInt(Mathf.Clamp(value, Constants.MIN_STATS[(int)stat], Constants.MAX_STATS[(int)stat]));
    }

    void AddParam(StatType stat, int value)
    {
        _paramsPlus[(int)stat] += value;
        Refresh();
    }

    void SetHp(int value)
    {
        _hp = value;
        Refresh();
    }

    void SetNrg(int value)
    {
        _nrg = value;
        Refresh();
    }

    void Refresh()
    {
        _hp = Mathf.Clamp(_hp, 0, mhp);
        _nrg = Mathf.Clamp(_nrg, 0, mnrg);
        if (IsDead())
            GetComponent<Animator>().SetBool("dead", true);
        else
            GetComponent<Animator>().SetBool("dead", false);
    }

    public void RecoverAll()
    {
        ClearStates();
        _hp = mhp;
        _nrg = mnrg;
    }

    public float HpRate()
    {
        return (float) hp / (float) mhp;
    }

    float NrgRate()
    {
        return mnrg > 0 ? nrg / mnrg : 0;
    }

    public bool IsDead()
    {
        return (hp <= 0);
        return IsDeathStateAffected();
    }

    public bool IsAlive()
    {
        return !IsDead();
    }

    bool IsHPCritical()
    {
        return (IsAlive() && HpRate() < Constants.CRIT_HP_RATE);
    }

    bool IsNRGCritial()
    {
        return (IsAlive() && NrgRate() < Constants.CRIT_NRG_RATE);
    }

    bool CanMove()
    {
        return IsAlive();
    }

    void SortStates()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}