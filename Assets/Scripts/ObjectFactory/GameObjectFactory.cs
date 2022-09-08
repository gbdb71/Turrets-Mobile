using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class GameObjectFactory<T, T2> : ScriptableObject where T : MonoBehaviour
{
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
        T instance = Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
        return instance;
    }

    public abstract void Reclaim(T instance);
}

