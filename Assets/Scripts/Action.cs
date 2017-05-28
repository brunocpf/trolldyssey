using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    public string actionName = "Action";
    public string actionDescription = "-";
    public Alliance targetAlliance = Alliance.None;
    public bool canTargetDead = false;
    public int baseNrgCost = 0;
    public Element element = Element.None;
    public ActionCoord[] range = ActionCoord.Adjacent();
    public bool canFlip = true;

    public abstract bool IsUsable();

    public virtual void Use(Unit user, List<Unit> targets)
    {
        foreach (Unit target in targets)
            ApplyEffects(user, target);
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
        foreach (ActionCoord coord in range)
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