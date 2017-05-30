using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenuSpawner : MonoBehaviour
{
    public static ActionMenuSpawner instance = null;

    public RadialMenu radialMenuPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SpawnMenu(Transform targetPosition, List<Action> actions)
    {
        RadialMenu menu = Instantiate(radialMenuPrefab, transform);
        menu.transform.position = targetPosition.position;
    }
}
