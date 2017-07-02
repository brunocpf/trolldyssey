using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : BaseAction
{
    public Wait()
    {
        actionName = "Esperar";
        actionDescription = "Encerra a jogada do usuário.";
        baseNrgCost = 0;
        requireTarget = false;
        element = Element.None;
        targetAlliance = Alliance.None;
    }

    public override IEnumerator Use(Unit user, List<Unit> targets)
    {
        yield return new WaitForSeconds(1);
        user.hasActedThisTurn = true;
    }
}
