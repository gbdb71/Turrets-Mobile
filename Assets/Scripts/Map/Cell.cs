using UnityEngine;
using System;

[Serializable, SelectionBase]
public class Cell : MonoBehaviour
{
    private CellType _type;
    private GameObject _object;
    private Vector2Int _position;

    public Vector2Int Position => _position;
    public CellType Type => _type;
    public GameObject Object => _object;
    public bool IsBusy => Object != null;

    public void Initialize(CellType type, Vector2Int pos)
    {
        _type = type;
        _position = pos;
    }
    public void SetObject(GameObject obj)
    {
        _object = obj;
    }
    public void ClearCell()
    {
        if(Object != null)
        {
            Object.transform.SetParent(null);
        }

        _object = null;
    }
}
