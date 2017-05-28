using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Learning {

    [Range(Constants.MIN_LEVEL, Constants.MAX_LEVEL)] public int level = 1;
    public GameObject action;

}
