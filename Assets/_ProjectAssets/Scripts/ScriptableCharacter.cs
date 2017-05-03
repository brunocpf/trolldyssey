using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCharacter : ScriptableObject
{
    public string characterName = "Character Name";
    [Range(1,99)] public int initialLevel = 1;

    [Tooltip("Stats no level inicial")]
    public Statistics initialStats = new Statistics(true);

}