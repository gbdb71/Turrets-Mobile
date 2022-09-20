using UnityEngine;
using Zenject;

public class ScriptablesInstaller : MonoInstaller
{
    [SerializeField] private SerializedDirectory _directory;

    public override void InstallBindings()
    {
        string directory = _directory.Path.Replace("Assets/Resources/", "");
        ScriptableObject[] scriptObjects = Resources.LoadAll<ScriptableObject>(directory);
        foreach (ScriptableObject scriptObject in scriptObjects)
        {
            Container.QueueForInject(scriptObject);
        }
    }
}
