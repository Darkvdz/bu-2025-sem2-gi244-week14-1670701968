using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    public ColorPicker ColorPicker;

    public void NewColorSelected(Color color)
    {
        // add code here to handle when a color is selected
        MainManager.GetInstance().TeamColor = color;
    }

    private void Start()
    {
        ColorPicker.Init();
        //this will call the NewColorSelected function when the color picker have a color button clicked.
        ColorPicker.onColorChanged += NewColorSelected;

        // 1.10 update color picker to show the current TeamColor in MainManager when load Menu scene
        ColorPicker.SelectColor(MainManager.GetInstance().TeamColor);
    }

    public void StartNew()
    {
        // 1.4 
        SceneManager.LoadScene("Main");
    }

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

    public void SaveColorClicked()
    {
        MainManager.GetInstance().SaveColor();
    }

    public void LoadColorClicked()
    {
        MainManager.GetInstance().LoadColor();
        ColorPicker.SelectColor(MainManager.GetInstance().TeamColor);
    }
}
