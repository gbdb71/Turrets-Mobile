using System;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;

[Serializable]
public class PoolInfo
{
    public GameObject Prefab;
    public int Count;
}

public class PoolInitializer : MonoBehaviour
{
    [SerializeField] private List<PoolInfo> objects = default;

    private void Start()
    {
        if (objects.Count == 0)
            return;

        for (int i = 0; i < objects.Count; i++)
            objects[i].Prefab.Populate(objects[i].Count);
    }
}
