using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private static MainManager instance;
    public static MainManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadColor();
    }

    public Color TeamColor = Color.white;

    [System.Serializable]
    class SaveData
    {
        public Color TeamColor;
    }

    public void SaveColor()
    {

        // 2.3
        // PlayerPrefs.SetFloat("TeamColor.R", TeamColor.r);
        // PlayerPrefs.SetFloat("TeamColor.G", TeamColor.g);
        // PlayerPrefs.SetFloat("TeamColor.B", TeamColor.b);
        // PlayerPrefs.SetFloat("TeamColor.A", TeamColor.a);

        // 3.3
        // SaveData data = new SaveData();
        // data.TeamColor = TeamColor;
        // string json = JsonUtility.ToJson(data);
        // Debug.Log($"Saving color {TeamColor} as json: {json}");
        // PlayerPrefs.SetString("SaveData", json);

        // 4.2
        // SaveData data = new SaveData();
        // data.TeamColor = TeamColor;

        // string json = JsonUtility.ToJson(data);
        // string path = "C:\\Users\\chany\\Desktop\\savefile.json";
        // File.WriteAllText(path, json);

        // 4.3
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        File.WriteAllText(path, json);
        Debug.Log("Save file path: " + path);
    }

    public void LoadColor()
    {
        // 2.3
        // TeamColor.r = PlayerPrefs.GetFloat("TeamColor.R", 1f);
        // TeamColor.g = PlayerPrefs.GetFloat("TeamColor.G", 1f);
        // TeamColor.b = PlayerPrefs.GetFloat("TeamColor.B", 1f);
        // TeamColor.a = PlayerPrefs.GetFloat("TeamColor.A", 1f);

        // 3.3
        // string json = PlayerPrefs.GetString("SaveData", "");
        // if (!string.IsNullOrEmpty(json))
        // {
        //     SaveData data = JsonUtility.FromJson<SaveData>(json);
        //     TeamColor = data.TeamColor;
        // }

        // 4.2
        // string path = "C:\\Users\\chany\\Desktop\\savefile.json";
        // if (File.Exists(path))
        // {
        //     string json = File.ReadAllText(path);
        //     SaveData data = JsonUtility.FromJson<SaveData>(json);
        //     TeamColor = data.TeamColor;
        // }

        // 4.3
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            TeamColor = data.TeamColor;
        }
    }
}
