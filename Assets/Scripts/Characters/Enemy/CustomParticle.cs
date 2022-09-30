using ToolBox.Pools;
using UnityEngine;

public class CustomParticle : MonoBehaviour, IPoolable
{
    [SerializeField] private ParticleSystem _particle;

    private float _timer = 0;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _particle.main.duration)
        {
            transform.parent.gameObject.Release();
        }
    }

    public void OnReuse()
    {
        _timer = 0;
    }

    public void OnRelease()
    {
    }
}
