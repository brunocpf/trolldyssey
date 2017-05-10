using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMap : MonoBehaviour
{

    public TileType this[int i, int j]
    {
        get { return mapArray[i, j].type; }
        private set { mapArray[i, j].type = value; }
    }


    public struct Tile
    {
        public TileType type;
        public GameObject tileObject;
    }


    public static int mapX = 10;
    public static int mapY = 10;
    public GameObject tilePrefab;

    private Tile[,] mapArray = new Tile[mapX, mapY];

    public void ToggleTile(int x, int y)
    {
        this[x, y] = (TileType)((int)(this[x, y] + 1) % (int)(TileType.Count));
    }

    public void ResetMap()
    {
        for (int x = 0; x < mapArray.GetLength(0); x++)
            for (int y = 0; y < mapArray.GetLength(1); y++)
            {
                mapArray[x, y].type = TileType.P;
                DestroyImmediate(mapArray[x, y].tileObject);
                mapArray[x, y].tileObject = null;
            }
        UpdateMap();
    }

    public void ClearMap()
    {
        for (int x = 0; x < mapArray.GetLength(0); x++)
            for (int y = 0; y < mapArray.GetLength(1); y++)
            {
                mapArray[x, y].type = TileType.X;
                DestroyImmediate(mapArray[x, y].tileObject);
                mapArray[x, y].tileObject = null;
            }
        UpdateMap();
    }

    public void SetTileObject(int x, int y, GameObject tileObject)
    {
        mapArray[x, y].tileObject = tileObject;
    }

    public void UpdateMap()
    {
        if (tilePrefab == null) return;
        for (int x = 0; x < mapArray.GetLength(0); x++)
        {
            for (int y = 0; y < mapArray.GetLength(1); y++)
            {
                switch (this[x, y])
                {
                    case TileType.X:
                        if (mapArray[x, y].tileObject != null)
                        {
                            DestroyImmediate(mapArray[x, y].tileObject);
                            mapArray[x, y].tileObject = null;
                        }
                        break;
                    default:
                        if (mapArray[x, y].tileObject == null)
                        {
                            GameObject instance = Instantiate(tilePrefab, transform) as GameObject;
                            mapArray[x, y].tileObject = instance;
                        }
                        mapArray[x, y].tileObject.transform.localPosition = new Vector3(x, y, 0);
                        break;
                }
            }
        }
    }

    void Awake()
    {
        UpdateMap();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
