using UnityEngine;

public interface IAbillity
{
    bool HasDelay();
    bool CanActivate();
    void Activate();
    Transform GetTransform();
}
