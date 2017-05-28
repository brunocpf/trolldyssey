using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour {

    public int stateId;
    public int priority = 0;
    public bool removeOnDeath = true;


    protected bool isPermanent;
    protected int turns;
    //protected List<Trait> traits;


    public virtual void OnAllianceTurnStart()
    {

    }

    public virtual void OnAllianceTurnEnd()
    {

    }

    public virtual bool IsDeathState()
    {
        return stateId == Constants.DEATH_STATE_ID;
    }
}