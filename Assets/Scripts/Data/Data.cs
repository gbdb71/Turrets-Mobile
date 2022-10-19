using Newtonsoft.Json;
using UnityEngine;
using Zenject;

[DefaultExecutionOrder(-1)]
public class Data : MonoBehaviour, IInitializable
{
    private const string UserKey = nameof(UserData);
    private UserData _userData = new UserData();

    public UserData User => _userData;

    private void Start()
    {
        User.PropertyChanged += delegate { SaveData(); };
    }
    private void OnDisable()
    {
        SaveData();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetString(UserKey, JsonConvert.SerializeObject(User));
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        try
        {
            _userData = PlayerPrefs.HasKey(UserKey) ? JsonConvert.DeserializeObject<UserData>(PlayerPrefs.GetString(UserKey)) : new UserData();
        }
        catch
        {
            _userData = new UserData();
        }
    }

    public void Initialize()
    {
        LoadData();
    }
}

