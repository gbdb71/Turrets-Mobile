using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SceneLoader : MonoBehaviour
{
    [SerializeField] private SerializedScene _targetScene;

    private IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(_targetScene.BuildIndex, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.progress >= 0.9f)
        {
            ShowProgress(async.progress.ToString());    
            yield return null;
        }

        async.allowSceneActivation = true;
    }

    protected abstract void ShowProgress(string progress);

    protected virtual void StartLoadScene()
    {
        StartCoroutine(LoadScene());
    }
}