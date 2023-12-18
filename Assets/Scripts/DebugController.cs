using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    bool showConsole;
    string input;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnToggleDebug();
        }
    }
    public void OnToggleDebug()
    {
        showConsole = !showConsole;
    }
    private void OnGUI()
    {
        if(!showConsole) { return; }
        float y = 0;
        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f),input);
    }
}
