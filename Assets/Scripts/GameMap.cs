using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMap : MonoBehaviour
{

    public static GameMap instance = null;

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

    public GameObject tilePrefab;
    public Transform grid;
    public Transform units;

    [SerializeField] private Unit[] enemies = new Unit[mapX * mapY];

    [SerializeField] private GridTile[] mapArray = new GridTile[mapX * mapY];
    
    public void SetEnemySpawnLocation(int x, int y, Unit content)
    {
        enemies[Get2DIndex(x, y)] = content;
    }

    public Unit GetInitialTileContent(int x, int y)
    {
        return enemies[Get2DIndex(x, y)];
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
                enemies[Get2DIndex(x, y)] = null;
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
            foreach (Transform child in grid)
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
                        GameObject tObject = Instantiate(tilePrefab, grid) as GameObject;
                        mapArray[Get2DIndex(x, y)].tile = tObject.GetComponent<Tile>();
                        mapArray[Get2DIndex(x, y)].tile.gridX = x;
                        mapArray[Get2DIndex(x, y)].tile.gridY = y;
                        mapArray[Get2DIndex(x, y)].tile.gameObject.transform.localPosition = new Vector3(x, y, 0);
                        break;
                }
            }
        }
    }

    private void SpawnAllies()
    {
        List<Tile> allySpawnLocations = GetAllySpawnLocations();
        for (int i = 0; i < Mathf.Min(allySpawnLocations.Count, GameManager.instance.partyMembers.Count); i++)
        {
            GameObject ally = GameManager.instance.partyMembers[i];
            ally.SetActive(true);
            SpawnUnit(allySpawnLocations[i], ally.GetComponent<Unit>());
        }
    }

    public List<Tile> GetAllySpawnLocations()
    {
        List<Tile> locations = new List<Tile>();
        for (int x = 0; x < mapX; x++)
            for (int y = 0; y < mapY; y++)
                if (this[x, y] == TileType.A)
                    locations.Add(mapArray[Get2DIndex(x, y)].tile);
        return locations;
    }

    private void SpawnEnemies()
    {
        for (int x = 0; x < mapX; x++)
            for (int y = 0; y < mapY; y++)
                if (this[x, y] == TileType.E)
                    SpawnUnit(mapArray[Get2DIndex(x, y)].tile, enemies[Get2DIndex(x, y)], true);
    }


    private void SpawnUnit(Tile tile, Unit unit, bool instantiate = false)
    {
        if (tile == null || unit == null)
            return;
        if (instantiate)
        {
            GameObject uObject = Instantiate(unit.gameObject);
            unit = uObject.GetComponent<Unit>();
        }
        unit.transform.SetParent(units, false);
        unit.Place(tile);
        unit.Match();
    }


    /*
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                Unit c = GetInitialTileContent(x, y);
                if (c != null)
                {
                    GameObject cObject = Instantiate(c.gameObject, units) as GameObject;
                    cObject.GetComponent<Unit>().Place(mapArray[Get2DIndex(x, y)].tile);
                    cObject.GetComponent<Unit>().Match();
                }
            }
        }
    }*/

    public void MarkTilesForMovement(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            tiles[i].MarkForMovement();
        }
    }

    public void MarkTilesForActionTarget(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            tiles[i].MarkForActionTarget();
        }
    }

    public void MarkTilesForAoe(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            tiles[i].MarkForAoe();
        }
    }

    public void UnmarkTiles(List<Tile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            tiles[i].Unmark();
        }
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


    public Tile GetTile(int x, int y)
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

    public List<Unit> GetAllContent()
    {
        List<Unit> contents = new List<Unit>();
        foreach (GridTile gTile in mapArray)
            if (gTile.tile != null && gTile.tile.content != null)
                contents.Add(gTile.tile.content);
        return contents;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        units = transform.Find("Units");
    }

    private void Start()
    {
        PopulateGrid();
        SpawnEnemies();
        SpawnAllies();
    }
}
