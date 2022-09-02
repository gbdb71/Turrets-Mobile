using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryPlate : MonoBehaviour
{
    public Transform content;

    public bool CheckChild()
    {
        bool empty;
        if (content.transform.childCount > 0)
            empty = false;
        else
            empty = true;

        Debug.Log(empty);
        return empty;
    }
}
