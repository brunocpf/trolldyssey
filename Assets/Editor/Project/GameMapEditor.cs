using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GameMap))]
public class GameMapEditor : Editor
{

    public override void OnInspectorGUI()
    {

        GameMap map = (GameMap)target;
        int mapX = GameMap.mapX;
        int mapY = GameMap.mapY;
        map.tilePrefab = EditorGUILayout.ObjectField("Tile Prefab", map.tilePrefab, typeof(GameObject), false) as GameObject;

        EditorGUILayout.BeginHorizontal();
        for (int x = 0; x < mapX; x++)
        {
            EditorGUILayout.BeginVertical();
            for (int y = mapY - 1; y >= 0; y--)
            {
                switch (map[x, y])
                {
                    case TileType.X:
                        GUI.color = Color.black;
                        break;
                    case TileType.A:
                        GUI.color = Color.blue;
                        break;
                    case TileType.E:
                        GUI.color = Color.red;
                        break;
                    default:
                        GUI.color = Color.white;
                        break;
                }
                if (GUILayout.Button(map[x, y].ToString(), GUILayout.Width(25), GUILayout.Height(25)))
                {
                    map.ToggleTile(x, y);
                    map.UpdateMap();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();


        GUI.color = Color.white;

        if (GUILayout.Button("Force Update"))
        {
            map.UpdateMap();
        }

        if (GUILayout.Button("Reset"))
        {
            map.ResetMap();
        }

        if (GUILayout.Button("Force Clear"))
        {
            map.ClearMap();
        }
    }
}
