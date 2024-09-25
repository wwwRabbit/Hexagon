using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TestMain : MonoBehaviour
{
    public DialogData data;
    string nameTest;

    private void Start()
    {
        Dialogue.Instance.RefreshDialogData(data);
    }

    private void Update()
    {
        InputSystem.onAnyButtonPress.Call((control) =>
        {
            nameTest = control.name;
        });
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 35;
        GUI.Label(new Rect(0, 0, 400, 200), name + "键按下", style);
    }
}
