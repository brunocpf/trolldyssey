using System;
using UnityEngine;

public static class Constants {

    public static readonly Color COLOR_MOVEMENT = Color.blue;
    public static readonly Color COLOR_ACTION_TARGET = Color.red;
    public static readonly Color COLOR_AOE = Color.yellow;

    public static readonly Color ALLY_COLOR = Color.blue;//new Color(0.4f, 0.6f, 0.6f);
    public static readonly Color ENEMY_COLOR = Color.red;// new Color(0.6f, 0.2f, 0.2f);
    public static readonly Color NEUTRAL_COLOR = new Color(1.0f, 0.85f, 0.75f);


    public const int DEATH_STATE_ID = 0;

    public const float CRIT_HP_RATE = 0.25f;
    public const float CRIT_NRG_RATE = CRIT_HP_RATE;

    public const int MIN_LEVEL = 1;
    public const int MIN_EXP = 0;

    public const int MIN_HP = 0;
    public const int MIN_MHP = 1;
    public const int MIN_NRG = 0;
    public const int MIN_MNRG = 1;

    public const int MIN_ATK = 0;
    public const int MIN_DEF = 0;
    public const int MIN_MAT = 0;
    public const int MIN_AGI = 0;
    public const int MIN_MOV = 0;

    public const int MAX_LEVEL = 99;
    public const int MAX_EXP = 999999;

    public const int MAX_HP = 999;
    public const int MAX_MHP = 999;
    public const int MAX_MP = 999;
    public const int MAX_MNRG = 999;

    public const int MAX_ATK = 99;
    public const int MAX_DEF = 99;
    public const int MAX_MAT = 99;
    public const int MAX_AGI = 99;
    public const int MAX_MOV = 50;



    public static readonly int[] MIN_STATS = {
                                      MIN_MHP, // MHP
                                      MIN_MNRG, // MMP
                                      MIN_ATK, // ATK
                                      MIN_DEF, // DEF
                                      MIN_MAT, // MAT
                                      MIN_AGI, // AGI
                                      MIN_MOV, // MOV
    };

    public static readonly int[] MAX_STATS = {
                                      MAX_MHP, // MHP
                                      MAX_MNRG, // MMP
                                      MAX_ATK, // ATK
                                      MAX_DEF, // DEF
                                      MAX_MAT, // MAT
                                      MAX_AGI, // AGI
                                      MAX_MOV, // MOV
    };

    public static Color GetAllianceColor(Alliance alliance)
    {
        switch (alliance)
        {
            case Alliance.Ally:
                return ALLY_COLOR;
            case Alliance.Enemy:
                return ENEMY_COLOR;
            default:
                return NEUTRAL_COLOR;
        }
    }
}