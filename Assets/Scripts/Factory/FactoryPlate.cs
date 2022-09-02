using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryPlate : MonoBehaviour
{
    public Transform content;

    public bool HasChild()
    {
        return content.transform.childCount == 0;
    }
}
