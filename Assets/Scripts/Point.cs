using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x, y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Flip()
    {
        x = -y;
        y = -x;
    }

    public Point GetFlipped()
    {
        return new Point(-y, -x);
    }

    public static Point[] Adjacent()
    {
        Point[] a = {
                        new Point(-1, 0),
                        new Point( 1, 0),
                        new Point( 0,-1),
                        new Point( 0, 1),
                    };
        return a;
    }
}