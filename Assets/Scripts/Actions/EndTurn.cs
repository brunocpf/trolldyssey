using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : BaseAction
{
    public EndTurn()
    {
        actionName = "Passar Turno";
        actionDescription = "Passa o turno para o oponente.";
        baseNrgCost = 0;
        requireTarget = false;
        element = Element.None;
        targetAlliance = Alliance.None;
    }

    public override IEnumerator Use(Unit user, List<Unit> targets)
    {
        List<Unit> units = BattleManager.instance.AllUnits(user.battler.alliance);
        foreach (Unit unit in units)
        {
            unit.hasActedThisTurn = true;
        }
        yield return null;
    }
}
