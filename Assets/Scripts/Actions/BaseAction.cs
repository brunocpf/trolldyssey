using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public string actionName = "Action";
    public string actionDescription = "-";
    public Alliance targetAlliance = Alliance.None;
    public bool canTargetDead = false;
    public bool requireTarget = true;
    public int baseNrgCost = 0;
    public Element element = Element.None;
    public Point[] range = Point.Adjacent();
    public bool canFlip = true;

    public virtual bool IsUsable()
    {
        return true;
    }

    public int CalcTotalAdvantage(Unit user, List<Unit> targets)
    {
        int totalAdvantage = 0;
        foreach (Unit target in targets)
        {
            totalAdvantage += CalcAdvantage(user, target);
        }
        return totalAdvantage;
    }

    public virtual int CalcAdvantage(Unit user, Unit targets)
    {
        return 0;
    }

    public virtual IEnumerator Use(Unit user, List<Unit> targets)
    {
        yield return new WaitForSeconds(3);
        foreach (Unit target in targets)
            ApplyEffects(user, target);
        user.hasActedThisTurn = true;
        yield return null;
    }

    public virtual void ApplyEffects(Unit user, Unit target)
    {
        if (user == null || target == null)
            return;
        Debug.Log(user.battler.battlerName + " usou " + actionName + " em " + target.battler.battlerName + "!");
    }

    public virtual List<Tile> GetTargetTiles(GameMap map, Tile tile)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Point coord in range)
        {
            Tile t = map.GetTile(tile.gridX + coord.x, tile.gridY + coord.y);
            if (t != null)
                tiles.Add(t);
        }
        return tiles;
    }

    public virtual List<Tile> GetAoeTiles(GameMap map, Tile tile, Tile orig)
    {
        return null;
    }

    public virtual List<Unit> GetValidTargets(List<Tile> tiles)
    {
        if (tiles == null)
            return null;
        List<Unit> targets = new List<Unit>();
        
        foreach (Tile t in tiles)
        {
            if (t.content == null)
                continue;
            if ((t.content.battler.alliance & targetAlliance) == t.content.battler.alliance)
            {
                targets.Add(t.content);
            }
        }
        return targets;
    }
}