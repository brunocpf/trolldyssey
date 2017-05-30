using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private void CheckMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            if(hit.collider.gameObject.CompareTag("Tile"))
            {
                BattleManager.instance.UpdateSelections(hit.collider.transform.GetComponent<Tile>());
            }
        }
    }

    void Update()
    {
        CheckMousePosition();
        if (Input.GetMouseButtonDown(0))
            BattleManager.instance.OnFire();
        else if (Input.GetMouseButtonDown(1))
            BattleManager.instance.OnCancel();
    }
}
