using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool canFire = true;
    private bool canCancel = true;

    private void CheckMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            canFire = true;
            if(hit.collider.gameObject.CompareTag("Tile"))
            {
                if (BattleManager.instance.canSelect)
                    BattleManager.instance.UpdateSelections(hit.collider.transform.GetComponent<Tile>());
            }
        }
        else
        {
            canFire = false;
        }
    }

    private void Update()
    {
        CheckMousePosition();
        if (Input.GetMouseButtonDown(0) && canFire)
            EventManager.TriggerEvent("Fire");
        else if (Input.GetMouseButtonDown(1) && canCancel)
            EventManager.TriggerEvent("Cancel");
    }
    
}
