using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayHPGaugeSpawner : MonoBehaviour
{
    public static OverlayHPGaugeSpawner instance = null;

    public GameObject gaugePrefab;

    public float yOffset = -0.5f;

    private Dictionary<GameObject, Unit> hpGauges = new Dictionary<GameObject, Unit>();

    public void RefreshGauges()
    {
        foreach (KeyValuePair<GameObject, Unit> entry in hpGauges)
        {
            if (entry.Value == null || entry.Value.gameObject == null || !entry.Value.gameObject.activeSelf)
            {
                entry.Key.SetActive(false);
                continue;
            }
            RectTransform rTransform = entry.Key.GetComponent<RectTransform>();
            Transform fill = rTransform.Find("HPFill");
            fill.GetComponent<Image>().fillAmount = entry.Value.battler.HpRate();
            rTransform.position = new Vector3(entry.Value.transform.position.x, entry.Value.transform.position.y + yOffset);
        }
    }

    public void CreateGauge(Unit unit)
    {
        GameObject hpGauge = Instantiate(gaugePrefab, transform, false);
        Transform fill = hpGauge.transform.Find("HPFill");
        Color color = Constants.GetAllianceColor(unit.battler.alliance);
        fill.GetComponent<Image>().color = color;
        hpGauges.Add(hpGauge, unit);
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        RefreshGauges();
    }
}
