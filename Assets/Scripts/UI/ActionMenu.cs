using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{

    public float radius = 100.0f;
    public RadialButton buttonPrefab;
    public RadialButton selected;
    
    private void Start()
    {
        RadialButton newButton = Instantiate(buttonPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
