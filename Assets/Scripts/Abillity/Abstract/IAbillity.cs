using UnityEngine;

public interface IAbillity
{
    bool CanActivate();
    void Activate();
    Transform GetTransform();
}
