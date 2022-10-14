using Newtonsoft.Json;
using UnityEngine;


[DefaultExecutionOrder(-1)]
public class Data : MonoBehaviour
{
    private const string UserKey = nameof(UserData);
    private UserData _userData = default(UserData);

    public UserData User => _userData;

    private void Awake()
    {
        LoadData();
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

    [ContextMenu("Clear Data")]
    private void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }
}

