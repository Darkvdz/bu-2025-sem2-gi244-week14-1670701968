# Week 14 - Persistent data

# 1. Overview

1.1 in this week, we have 2 scene Menu and Main. We sill start with Menu scene. You will see UI allows user to choose colors, save/load colors, and Start/Exit game

---

1.2 we will introduce 2 type of data persistence:

- between scene (using static or singleton) - this approach we are already done last week, we can use static and singleton pattern.
- between session (open-close game, using PlayerPrefs or JSON file)

---

1.3 next, take a look at MainManager which manage and hold TeamColor data. And also MenuUIHandler which handle UI in Menu scene. All UI actions are handled in MenuUIHandler, and MainManager will be responsible for hold data and pass data to other scene.

Choose `StartButton`, and see its inspector. you will see its OnClick is linked to `MenuUIHandler.StartNew` function. So when user click StartButton, it will call `StartNew` function in MenuUIHandler.

---

1.4 Open MenuUIHandler, we will Implement scene loading to see MainManager is alive through scene loading. We will use `SceneManager.LoadScene` to load Main scene, and we will use `DontDestroyOnLoad` in MainManager to make sure it is not destroyed when load new scene.

```c# (MenuUIHandler.cs)

    public void StartNew()
    {
        SceneManager.LoadScene("Main");
    }

```

---

1.5 And then Exit game in function Exit()

```c# (MenuUIHandler.cs)

    public void Exit()
    {
        Application.Quit();
    }
```

but with this code, wehn you run in Editor, it will not quit the game, but if you build and run the game, it will work. So for testing purpose, to mimic application quit while running in Editor, you can add `#if UNITY_EDITOR` preprocessor directive to make it work in Editor.

```c# (MenuUIHandler.cs)
    public void Exit()
    {
        // 1.5 (1)
        // Application.Quit();

        // 1.5 (2)
#if UNITY_EDITOR
        //NOTE: this is very important since this line does not cover with #if UNITY_EDITOR
        // it causes build errors, since the EditorApplication class is not available in builds
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
```

1.6 Let see `ColorPicker` UI Component in `Scripts/Helpers/ColorPicker.cs`. the colorpicker is a custom UI component, it is not a built-in UI component. It handles color picking. @INSTRUCTOR introduce event action onColorChanged. that make it easy to subscribe to color change event, and update color in MainManager when color changed. as you can see in `MenuUIHandler`

```c#
    private void Start()
    {
        ColorPicker.Init();
        //this will call the NewColorSelected function when the color picker have a color button clicked.
        ColorPicker.onColorChanged += NewColorSelected;
    }
```

and when color changed, it will call `NewColorSelected` function, and we will update TeamColor in MainManager.

```c#
    public void NewColorSelected(Color color)
    {
        // add code here to handle when a color is selected
        MainManager.GetInstance().TeamColor = color;
    }
```

1.7 now when you change color in ColorPicker, the MainManager's teamColor also change as well. This approach usually used for listen to events, and update data when event happen. In this case, we listen to color change event, and update TeamColor in MainManager when color changed.

---

1.8 Let move to Main scene, and see how data persist between scene. In Main scene, we have Transporter uints, we will make its color changed to TeamColor in MainManager. The script, which manage Transporter Unit is `Unit.cs` let set color in Start function, so when the unit is spawned, it will set color to TeamColor in MainManager.

```c#
    private void Start()
    {
        SetColor(MainManager.GetInstance().TeamColor);
    }
```

actually the script attached transport unit is `TransportUnit.cs`, but it is inherited from `Unit.cs`, so it will also call Start function in `Unit.cs`, so it can get TeamColor from MainManager and set color when spawned.

Now when we choose color in Menu scene, and click start game, the unit in Main scene will have the color we choose in Menu scene. This is how we persist data between scene using singleton pattern.

---

1.9 let wire up "Back to Menu" button in Main scene, so we can go back to Menu scene. and see the color is still there when we go back to Menu scene. which is in `UIMainScene.cs`. now we will link click event programmatically instead of using the Inspector. in Awake function, and load Menu scene when click "Back to Menu" button.

```c#
    public Button backToMenuButton;


    private void Awake()
    {
        Instance = this;
        InfoPopup.gameObject.SetActive(false);
        ResourceDB.Init();

        backToMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Menu");
        });
    }
```

then assign `BackToMenuButton` in inspector, and now when you click "Back to Menu" button, it will load Menu scene, and the color is still there since MainManager is not destroyed when load new scene.

---

1.10 But when you get back to Menu scene, the chosen colotr in color picker is reset to default color, which is white. This is because we did not update color picker to show the current TeamColor in MainManager when load Menu scene. So we need to update color picker to show the current TeamColor in MainManager when load Menu scene. we can do this in `MenuUIHandler` Start function, so when Menu scene is loaded, it will update color picker to show the current TeamColor in MainManager.

```c#
    private void Start()
    {
        ColorPicker.Init();
        //this will call the NewColorSelected function when the color picker have a color button clicked.
        ColorPicker.onColorChanged += NewColorSelected;

        // 1.10 update color picker to show the current TeamColor in MainManager when load Menu scene
        ColorPicker.SelectColor(MainManager.GetInstance().TeamColor);
    }
```

# 2. Persistent data using PlayerPrefs

2.1 Now we have persistent data between scene, but when we close the game and open again, the data will be lost. To persist data between session, we can use PlayerPrefs or JSON file. In this example, we will use PlayerPrefs to save and load color data.

---

2.2 In MenuUIHandler, we will work with MainManager to save and load color data. when user click "Save Color" or "Load Color" button. now we do not link click event programmatically but using Inspector, so we can directly call function in MenuUIHandler when click the button. so we will add two functions in MenuUIHandler, one for save color and one for load color.

```c#
    public void SaveColorClicked()
    {
        MainManager.GetInstance().SaveColor();
    }

    public void LoadColorClicked()
    {
        MainManager.GetInstance().LoadColor();
        ColorPicker.SelectColor(MainManager.GetInstance().TeamColor);
    }
```

in next step, we will implement SaveColor and LoadColor function in MainManager to save and load color data using PlayerPrefs.

---

2.3 Let implement SaveColor and LoadColor function in MainManager. we will save each color channel (R, G, B, A) as a separate float value in PlayerPrefs, and load them back when load color.

```c#
    public void SaveColor()
    {
        PlayerPrefs.SetFloat("TeamColor.R", TeamColor.r);
        PlayerPrefs.SetFloat("TeamColor.G", TeamColor.g);
        PlayerPrefs.SetFloat("TeamColor.B", TeamColor.b);
        PlayerPrefs.SetFloat("TeamColor.A", TeamColor.a);
    }

    public void LoadColor()
    {
        TeamColor.r = PlayerPrefs.GetFloat("TeamColor.R", 1f);
        TeamColor.g = PlayerPrefs.GetFloat("TeamColor.G", 1f);
        TeamColor.b = PlayerPrefs.GetFloat("TeamColor.B", 1f);
        TeamColor.a = PlayerPrefs.GetFloat("TeamColor.A", 1f);
    }
```

in PlayerPrefs, we have various data type we can save, such as SetInt, SetFloat, SetString, SetBool. ANd we can find the save value in RegistryEditor in Windows by path `Computer\HKEY_CURRENT_USER\Software\Unity\Unity\Junior Programmer Pathway`

---

# 3. Persistent data using JSON file

3.1 In case of our save data structure is more complex, such as we have multiple data to save, or we have nested data, it is better to use JSON file to save and load data. JSON file is a text file that can store data in a structured way, and it is easy to read and write. We can use `JsonUtility` class in Unity to serialize and deserialize data to and from JSON format.

---

3.2 You will see a class `SaveData` to hold the data we want to save, and make it serializable. In the future, to make better file organization, you can create a new file by right click in Scripts folder, Create > Scripting > Empty C# Script, name it `SaveData.cs`, and add the following code:

```c#
[Serializable]
class SaveData
{
    public Color TeamColor;
}
```

---

3.3 then we will rewrite SaveColor and LoadColor function in MainManager to use JSON file to save and load data. we will save the data to a file in Application.persistentDataPath, which is a special folder that is persistent between sessions, and it is different for each platform.

```c#
    public void SaveColor()
    {
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", json);
    }

    public void LoadColor()
    {
        string json = PlayerPrefs.GetString("SaveData", "");
        if (!string.IsNullOrEmpty(json))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            TeamColor = data.TeamColor;
        }
    }
```

---

# 4. Using file system

4.1 In case of we want to save data to a file in file system instead of PlayerPrefs. with limitation of PlayerPrefs, such as you could not specfic the file name and file path. then you are lacking of file backing up, remote syncing save file.

we can use `System.IO` namespace to read and write files. we can use `File.WriteAllText` to write data to a file, and `File.ReadAllText` to read data from a file. we can save the file in Application.persistentDataPath, which is a special folder that is persistent between sessions, and it is different for each platform.

4.2 But the thing to consider that where to save the file. for our example, we will save it onto window desktop. To find Desktop path, right click on Desktop, and choose Properties, then you will see the path in Location. for example, `C:\Users\<user-name>\Desktop`. then we can combine this path with file name to get the full path of the file.

```c#
    public void SaveColor()
    {
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;

        string json = JsonUtility.ToJson(data);
        string path = "C:\\Users\\chany\\Desktop\\savefile.json";
        File.WriteAllText(path, json);
    }

    public void LoadColor()
    {
        string path = "C:\\Users\\chany\\Desktop\\savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            TeamColor = data.TeamColor;
        }
    }
```

4.3 but saving file to desktop, it is quite dangerous, since user can delete the file by mistake, or the file can be corrupted. so it is better to save the file in a special folder that is not easily accessible by user, such as Application.persistentDataPath. you can also create a subfolder in Application.persistentDataPath to organize your save files.

```c#
    public void SaveColor()
    {
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        File.WriteAllText(path, json);
        Debug.Log("Save file path: " + path);
    }

    public void LoadColor()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            TeamColor = data.TeamColor;
        }
    }
```

---

4.4 Conclusion: in this week, we have learned how to persist data between scene using singleton pattern, and how to persist data between session using PlayerPrefs and JSON file. we also learn how to use file system to read and write files. with these knowledge, you can now save and load data in your game, and make your game more user-friendly.

---

# 5. Challenge

5.1 Add LastTimePlayed into SaveData, and save the last time the user played the game. you can use `System.DateTime` to get the current time, and save it as a string in JSON file. then when load the game, you can parse the string back to DateTime, and show the last time played in UI.

```c#
[Serializable]
class SaveData
{
    public Color TeamColor;
    public string LastTimePlayed;
}
```

```c#
    public void SaveColor()
    {
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;
        data.LastTimePlayed = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        File.WriteAllText(path, json);
        Debug.Log("Save file path: " + path);
    }

    public void LoadColor()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            TeamColor = data.TeamColor;
        }
    }
```

and send saveFile.json file to instructor, and show the last time played in UI. you can create a new UI Text in Menu scene to show the last time played. and update the text when load the game.

---

5.2 Thinking question: Lets see the real world game save system, it need to manage versioning. the SaveData structure may change when you update your game, and you need to make sure that the old save data can still be loaded in the new version of the game. how do you manage versioning in your save system? how do you handle the case when the save data structure is changed, and the old save data cannot be loaded in the new version of the game?
