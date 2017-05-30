using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    public ActionCoord[] aoe = { new ActionCoord(0,0) };

    public Attack()
    {
        actionName = "Ataque";
        actionDescription = "Ataque simples no alvo.";
        baseNrgCost = 0;
        canTargetDead = false;
        requireTarget = true;
        element = Element.Physical;
        targetAlliance = Alliance.Ally | Alliance.Enemy;
        range = ActionCoord.Adjacent();
    }
    
    public override bool IsUsable()
    {
        return true;
    }

    private IEnumerator ActionSequence()
    {
        yield return 0;
    }


    public override List<Tile> GetAoeTiles(GameMap map, Tile tile, Tile orig)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (ActionCoord c in aoe)
        {
            ActionCoord coord = new ActionCoord(c.x, c.y);
            if (canFlip && (tile.gridX != orig.gridX))
                coord = c.GetFlipped();
            Tile t = map.GetTile(tile.gridX + coord.x, tile.gridY + coord.y);
            if (t != null)
                tiles.Add(t);
        }
        return tiles;
    }

    public override void ApplyEffects(Unit user, Unit target)
    {
        base.ApplyEffects(user, target);
        target.battler.DealDamage(10);
    }
}