using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private SerializedScene _targetScene;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartLoadScene);
    }

    private void StartLoadScene()
    {
        _button.interactable = false;
        StartCoroutine(LoadScene());
    }
    private IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(_targetScene.BuildIndex, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.progress <= 0.8f)
        {
            Debug.Log(async.progress);
            yield return null;
        }

        async.allowSceneActivation = true;
    }
}
