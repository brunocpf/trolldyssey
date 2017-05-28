using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionCoord {
    public int x, y;

    public ActionCoord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public ActionCoord GetFlipped()
    {
        return new ActionCoord(-y, -x);
    }


    public static ActionCoord[] Adjacent()
    {
        ActionCoord[] a = {
                            new ActionCoord(-1, 0),
                            new ActionCoord( 1, 0),
                            new ActionCoord( 0,-1),
                            new ActionCoord( 0, 1),
                          };
        return a;
    }
}