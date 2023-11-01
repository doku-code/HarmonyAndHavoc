using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace charles
{
    public class DebugController : MonoBehaviour
    {
        private bool showConsole;
        private bool showHelp;
        private string input;
        private Vector2 scroll;

        [Header("CommandName")]
        public static DebugCommand TEST_DEBUG;
        public static DebugCommand HELP;

        [Header("Script Reference")]
        public Test test;

        [Header("Command List")]
        public List<object> commandList;


        private void Awake()
        {
            DontDestroyOnLoad(this);
            CallTheCheatHere();
        }

        private void CallTheCheatHere()
        {

            TEST_DEBUG = new DebugCommand("test_debug", "Make a debug in the console", "test_debug", () =>
            {
                test.testdebug();
            });

            HELP = new DebugCommand("help", "Show the list of available commands", "help", () =>
            {
                showHelp = true;
            });

            commandList = new List<object>
            {
                TEST_DEBUG,
                HELP
            };
        }
        public void OnToggleDebug(CallbackContext value)
        {
            showConsole = !showConsole;
        }

        public void OnReturn(CallbackContext value)
        {
            if (showConsole)
            {
                HandleInput();
                input = "";
            }
        }

        private void OnGUI()
        {
            if (!showConsole) return;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float fontSize = Mathf.Min(screenWidth, screenHeight) * 0.03f;
            float y = 0f;

            GUI.skin.textField.fontSize = (int)fontSize;
            GUI.skin.label.fontSize = (int)fontSize;


            if (showHelp)
            {
                GUI.Box(new Rect(0f, y, screenWidth, screenHeight * 0.2f), "");

                Rect viewport = new Rect(0, 0, screenWidth - 30f, screenHeight * 0.1f * commandList.Count);

                scroll = GUI.BeginScrollView(new Rect(0, y + screenHeight * 0.05f, screenWidth, screenHeight * 0.18f), scroll, viewport);

                for (int i = 0; i < commandList.Count; i++)
                {
                    DebugCommandBase command = commandList[i] as DebugCommandBase;

                    string label = $"{command.commandFormat} - {command.commandDescription}";

                    Rect labelRect = new Rect(5f, screenHeight * 0.1f * i, viewport.width - 100f, screenHeight * 0.1f);

                    GUI.Label(labelRect, label);
                }
                GUI.EndScrollView();

                y += screenHeight * 0.2f;
            }

            GUI.Box(new Rect(0, y, screenWidth, screenHeight * 0.05f), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            input = GUI.TextField(new Rect(10f, y + screenHeight * 0.025f, screenWidth - 20f, screenHeight * 0.04f), input);
        }

        private void HandleInput()
        {
            string[] properties = input.Split(' ');

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

                if (input.Contains(commandBase.commandId))
                {
                    if (commandList[i] as DebugCommand != null)
                    {
                        (commandList[i] as DebugCommand).Invoke();
                    }
                    else if (commandList[i] as DebugCommand<int> != null)
                    {
                        (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    }
                }
            }
        }
    }
}