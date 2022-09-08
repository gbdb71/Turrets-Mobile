using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public abstract class GameObjectFactory<T, T2> : ScriptableObject where T : MonoBehaviour
{
    [Inject] private DiContainer _container;
    private Scene scene;

    public abstract T Get(T2 args);

    protected T CreateGameObjectInstance(T prefab) 
    {
        if (!scene.isLoaded)
        {
            if (Application.isEditor)
            {
                scene = SceneManager.GetSceneByName(name);
                if (!scene.isLoaded)
                {
                    scene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                scene = SceneManager.CreateScene(name);
            }
        }
        T instance = _container.InstantiatePrefab(prefab).GetComponent<T>();
        SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
        return instance;
    }

    public abstract void Reclaim(T instance);
}

