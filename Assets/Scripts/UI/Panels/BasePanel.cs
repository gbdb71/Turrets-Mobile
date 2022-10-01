using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
    [SerializeField] protected Transform _content;

    protected abstract void Show();
}

