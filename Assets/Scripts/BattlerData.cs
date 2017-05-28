using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "BattlerData", menuName = "Battler")]
public class BattlerData : ScriptableObject
{
    public string battlerName = "Battler";
    [Range(Constants.MIN_LEVEL, Constants.MAX_LEVEL)] public int initialLevel = 1;
    [Header("MHP  MMP  ATK  DEF  MAT  AGI  MOV")]
    public int[] initialStats = new int[(int)StatType.Count];
    public float[] growthRate = new float[(int)StatType.Count];
    public List<Learning> learnings = new List<Learning>();

    private void OnValidate()
    {
        for (StatType i = 0; i < StatType.Count; i++)
        {
            initialStats[(int)i] = Mathf.Clamp(initialStats[(int)i], Constants.MIN_STATS[(int)i], Constants.MAX_STATS[(int)i]);
        }
    }

}