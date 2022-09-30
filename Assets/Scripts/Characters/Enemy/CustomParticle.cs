using System.Collections;
using ToolBox.Pools;
using UnityEngine;

public class CustomParticle : MonoBehaviour, IPoolable
{
    [SerializeField] private ParticleSystem _particle;

    private IEnumerator DisableParticle()
    {
        Debug.Log("Play");
        yield return new WaitForSeconds(_particle.main.duration);
        gameObject.Release();
    }

    public void OnReuse()
    {
        Debug.Log("Enable");
        _particle.Play();
        StartCoroutine(DisableParticle());
    }
   
    public void OnRelease()
    {
        Debug.Log("Disable");
    }
}
