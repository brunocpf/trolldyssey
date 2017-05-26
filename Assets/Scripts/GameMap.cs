using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMap : MonoBehaviour
{

    public TileType this[int i, int j]
    {
        get { return mapArray[Get2DIndex(i, j)].type; }
        private set { mapArray[Get2DIndex(i, j)].type = value; }
    }

    [System.Serializable]
    public class GridTile
    {
        public TileType type;
        public Tile tile;
    }

    public static int mapX = 10;
    public static int mapY = 10;

    public GameObject grid;
    public GameObject tilePrefab;
    public GameObject units;

    [SerializeField] private Unit[] initialContents = new Unit[mapX * mapY];

    [SerializeField] private GridTile[] mapArray = new GridTile[mapX * mapY];
    
    public void SetInitialTileContent(int x, int y, Unit content)
    {
        initialContents[Get2DIndex(x, y)] = content;
    }

    public Unit GetInitialTileContent(int x, int y)
    {
        return initialContents[Get2DIndex(x, y)];
    }


    public void ToggleTile(int x, int y)
    {
        this[x, y] = (TileType)((int)(this[x, y] + 1) % (int)(TileType.Count));
    }

    public void ResetMap()
    {
        for (int x = 0; x < mapX; x++)
            for (int y = 0; y < mapY; y++)
            {
                mapArray[Get2DIndex(x, y)].type = TileType.P;
                mapArray[Get2DIndex(x, y)].tile = null;
                initialContents[Get2DIndex(x, y)] = null;
            }
    }

    public void ClearGrid()
    {
        if (grid == null) return;
        for (int x = 0; x < mapX; x++)
            for (int y = 0; y < mapY; y++)
                mapArray[Get2DIndex(x, y)].tile = null;
        int n = 0;
        for (int i = 0; i < mapX; i++)
            foreach (Transform child in grid.transform)
            {
                n++;
                DestroyImmediate(child.gameObject);
            }
    }


    public void PopulateGrid()
    {
        ClearGrid();
        if (tilePrefab == null || grid == null) return;
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                switch (this[x, y])
                {
                    case TileType.X:
                        break;
                    default:
                        GameObject instance = Instantiate(tilePrefab, grid.transform) as GameObject;
                        mapArray[Get2DIndex(x, y)].tile = instance.GetComponent<Tile>();
                        mapArray[Get2DIndex(x, y)].tile.gridX = x;
                        mapArray[Get2DIndex(x, y)].tile.gridY = y;
                        mapArray[Get2DIndex(x, y)].tile.gameObject.transform.localPosition = new Vector3(x, y, 0);
                        break;
                }
            }
        }
    }

    private void SpawnIntialContent()
    {
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                Unit c = GetInitialTileContent(x, y);
                if (c != null)
                {
                    GameObject instance = Instantiate(c.gameObject, units.transform) as GameObject;
                    instance.GetComponent<Unit>().Place(mapArray[Get2DIndex(x, y)].tile);
                    instance.GetComponent<Unit>().Match();
                }
            }
        }
    }

    public void MarkTilesForMovement(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            tiles[i].MarkForMovement();
        }
            //tiles[i].GetComponent<Renderer>().material.SetColor("_Color", selectedTileColor);
    }

    public void UnmarkTiles(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            tiles[i].UnmarkForMovement();
        }
            //tiles[i].GetComponent<Renderer>().material.SetColor("_Color", defaultTileColor);
    }

    public List<Tile> Search(Tile start, Func<Tile, Tile, bool> addTile)
    {
        List<Tile> retValue = new List<Tile>();
        retValue.Add(start);
        ClearSearch();
        Queue<Tile> checkNext = new Queue<Tile>();
        Queue<Tile> checkNow = new Queue<Tile>();
        start.distance = 0;
        checkNow.Enqueue(start);
        while (checkNow.Count > 0)
        {
            Tile t = checkNow.Dequeue();
            for (int x = -1; x <= 1; x++)
                for(int y = -1; y <= 1; y++)
                {
                    if (Mathf.Abs(x) == Mathf.Abs(y))
                        continue;
                    Tile next = GetTile(t.gridX + x, t.gridY + y);
                    if (next == null || next.distance <= t.distance + 1)
                        continue;
                    if (addTile(t, next))
                    {
                        next.distance = t.distance + 1;
                        next.prev = t;
                        checkNext.Enqueue(next);
                        retValue.Add(next);
                    }
                }
            if (checkNow.Count == 0)
                SwapReference(ref checkNow, ref checkNext);
        }
        return retValue;
    }


    private Tile GetTile(int x, int y)
    {
        if ((x < 0 || x >= mapX) || (y < 0 || y >= mapY))
            return null;
        return mapArray[Get2DIndex(x, y)].tile;
    }

    private void ClearSearch()
    {
        foreach (GridTile t in mapArray)
        {
            if (t.tile == null)
                continue;
            t.tile.prev = null;
            t.tile.distance = int.MaxValue;
        }
    }

    private void SwapReference(ref Queue<Tile> a, ref Queue<Tile> b)
    {
        Queue<Tile> temp = a;
        a = b;
        b = temp;
    }

    private int Get2DIndex(int x, int y)
    {
        return y * mapX + x;
    }

    void Awake()
    {
        PopulateGrid();
        SpawnIntialContent();
    }
}
