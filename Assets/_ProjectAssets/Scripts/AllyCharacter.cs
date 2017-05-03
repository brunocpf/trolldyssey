using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ally", menuName = "Characters/Ally")]
public class AllyCharacter : ScriptableCharacter {
    public AllyCharacter()
    {
        initialStats = null;
        initialStats = new Statistics(false);
    }
}