using Newtonsoft.Json;
using UnityEngine;


public class Data : MonoBehaviour
{
    [SerializeField] private UpgradesInfo _upgradesInfo;

    private const string UserKey = nameof(UserData);
    private UserData _userData = default(UserData);

    public UserData User => _userData;
    public UpgradesInfo UpgradesInfo => _upgradesInfo;

    [HideInInspector] public int CurrentLevel;

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

