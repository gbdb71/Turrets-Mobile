using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    public BoxCollider collider;
    public int ammoCount;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        collider.enabled = true;
    }
}
