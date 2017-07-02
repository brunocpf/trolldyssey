using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestruct : MonoBehaviour
{
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
