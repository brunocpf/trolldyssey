using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Action
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

    public override void Use(Unit user, List<Unit> targets)
    {
        user.hasActedThisTurn = true;
    }
}
