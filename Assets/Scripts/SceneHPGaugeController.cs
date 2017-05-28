using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHPGaugeController : MonoBehaviour
{
    public GameObject gaugePrefab;
    public Dictionary<GameObject, Unit> hpGauges = new Dictionary<GameObject, Unit>();

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RefreshGauges();
    }


    public void RefreshGauges()
    {
        foreach (KeyValuePair<GameObject, Unit> entry in hpGauges)
        {
            RectTransform rTransform = entry.Key.GetComponent<RectTransform>();
            Transform fill = rTransform.Find("HPFill");
            fill.GetComponent<Image>().fillAmount = entry.Value.battler.HpRate();
            rTransform.position = new Vector3(entry.Value.transform.position.x, entry.Value.transform.position.y - 0.5f);
        }
    }

    public void ShowHPGauge(Unit unit)
    {
        GameObject hpGauge = Instantiate(gaugePrefab, rectTransform);
        hpGauges.Add(hpGauge, unit);
    }
}
