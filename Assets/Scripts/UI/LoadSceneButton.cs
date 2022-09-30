using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSceneButton : SceneLoader
{
    [SerializeField] private TextMeshProUGUI _text;
    private Button _button;

    protected override void ShowProgress(string progress)
    {
        _text.text = progress;
    }

    protected override void StartLoadScene()
    {
        base.StartLoadScene();

        _button.interactable = false;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartLoadScene);
    }
}
