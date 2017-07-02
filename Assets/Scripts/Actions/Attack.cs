using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseAction
{
    public enum ModifierStat
    {
        Atk,
        Def,
        Mat
    }

    public int basePower = 10;
    public float baseMultiplier = 1.0f;
    public float variance = 0.1f;
    public ModifierStat modifier = ModifierStat.Atk;
    public bool ignoreDef = false;
    public bool canBeNegative = false;
    public GameObject instantiatedObject;

    public Point[] aoe = { new Point(0, 0) };

    public Attack()
    {
        actionName = "Ataque";
        actionDescription = "Ataque simples no alvo.";
        baseNrgCost = 0;
        canTargetDead = false;
        requireTarget = true;
        element = Element.Physical;
        targetAlliance = Alliance.Ally | Alliance.Enemy;
        range = Point.Adjacent();
    }


    public override IEnumerator Use(Unit user, List<Unit> targets)
    {
        Animator userAnim = user.GetComponent<Animator>();
        userAnim.SetBool("attack", true);
        List<GameObject> instantiatedObjects = new List<GameObject>();
        GameObject obj;
        Animator targetAnim;
        foreach (Unit target in targets)
        {
            int oldHp = target.battler.hp;
            if (instantiatedObject != null)
            {
                obj = Instantiate(instantiatedObject, target.transform);
                instantiatedObjects.Add(obj);
            }
            ApplyEffects(user, target);
            targetAnim = target.GetComponent<Animator>();
            if (target.battler.hp < oldHp)
                targetAnim.SetBool("hurt", true);
            yield return new WaitForSeconds(0.3f);
            targetAnim.SetBool("hurt", false);
        }
        yield return new WaitForSeconds(1);
        userAnim.SetBool("attack", false);
        user.hasActedThisTurn = true;
        yield return null;
    }

    public override bool IsUsable()
    {
        return true;
    }


    public override List<Tile> GetAoeTiles(GameMap map, Tile tile, Tile orig)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Point c in aoe)
        {
            Point coord = new Point(c.x, c.y);
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
        target.battler.DealDamage(CalcDamage(user, target));
    }

    public override int CalcAdvantage(Unit user, Unit target)
    {
        int damage = CalcDamage(user, target);
        if (target.battler.alliance.IsSameAlliance(user.battler.alliance))
        {
            return -damage;
        }
        else
        {
            return damage;
        }
    }

    private int CalcDamage(Unit user, Unit target)
    {
        float damage = 0f;
        switch (modifier)
        {
            case ModifierStat.Atk:
                damage = basePower + user.battler.atk;
                break;
            case ModifierStat.Def:
                damage = basePower + user.battler.def;
                break;
            case ModifierStat.Mat:
                damage = basePower + user.battler.mat;
                break;
        }
        damage *= baseMultiplier;
        float mitigation = modifier == ModifierStat.Mat ? (target.battler.def) / 2 : (target.battler.def);
        if (!ignoreDef)
            damage -= mitigation * 1.5f;
        damage += UnityEngine.Random.Range(-variance, variance) * damage;
        if (!canBeNegative && damage < 0f)
            damage = 0f;
        return Mathf.RoundToInt(damage);
    }
}