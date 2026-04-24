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
        //3
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;
        string j = JsonUtility.ToJson(data);
        Debug.Log(j);

        string p = Application.persistentDataPath; //"C:\\Users\\Administrator";
        string fileName = "saveData.json";
        string fullPath = Path.Combine(p , fileName);

        File.WriteAllText(fullPath, j);
        Debug.Log(fullPath);

        //2
        //SaveData data = new SaveData();
        //data.TeamColor = TeamColor;
        //string j = JsonUtility.ToJson(data);
        //Debug.Log(j);
        //PlayerPrefs.SetString("saveData", j);

        //1
        //PlayerPrefs.SetFloat("teamColor.a", TeamColor.a);
        //PlayerPrefs.SetFloat("teamColor.r", TeamColor.r);
        //PlayerPrefs.SetFloat("teamColor.g", TeamColor.g);
        //PlayerPrefs.SetFloat("teamColor.b", TeamColor.b);

        //C:\Users\Administrator
    }

    public void LoadColor()
    {
        string p = Application.persistentDataPath; //"C:\\Users\\Administrator";
        string fileName = "saveData.json";
        string fullPath = Path.Combine(p, fileName);

        string j = File.ReadAllText(fullPath);
        Debug.Log(j);
        SaveData data = JsonUtility.FromJson<SaveData>(j);
        TeamColor = data.TeamColor;

        //string j = PlayerPrefs.GetString("saveData");
        //Debug.Log(j);
        //SaveData data = JsonUtility.FromJson<SaveData>(j);
        //TeamColor = data.TeamColor;

        //TeamColor.a = PlayerPrefs.GetFloat("teamColor.a", 1f);
        //TeamColor.r = PlayerPrefs.GetFloat("teamColor.r", 1f);
        //TeamColor.g = PlayerPrefs.GetFloat("teamColor.g", 1f);
        //TeamColor.b = PlayerPrefs.GetFloat("teamColor.b", 1f);

    }
}
