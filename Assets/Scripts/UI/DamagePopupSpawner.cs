using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner instance = null;

    public GameObject popupPrefab;

    public void CreatePopup(Unit unit, int value)
    {
        GameObject popup = Instantiate(popupPrefab, transform, false);
        RectTransform rTransform = popup.GetComponent<RectTransform>();
        rTransform.position = new Vector3(unit.transform.position.x, unit.transform.position.y);
        DamagePopupAnimation popupAnimation = popup.GetComponent<DamagePopupAnimation>();
        popupAnimation.SetValue(value);
        StartCoroutine(popupAnimation.Animate());
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
