using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace charles
{
    public class DebugController : MonoBehaviour
    {
        private bool showConsole;
        private bool showHelp;
        private string input;
        private Vector2 scroll;
        private List<string> commandHistory;
        private int historyIndex;

        [Header("CommandName")]
        public static DebugCommand HELP;
        public static DebugCommand<float> TIMESCALE;

        [Header("Script Reference")]
        public DebugFunction debugFunc;

        [Header("Command List")]
        public List<object> commandList;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            CallTheCheatHere();
        }

        private void CallTheCheatHere()
        {
            TIMESCALE = new DebugCommand<float>("timescale", "Reduce game time to debug", "timescale", (x) =>
            {
                debugFunc.ModifyTimeScale(x);
            });

            HELP = new DebugCommand("help", "Show the list of available commands", "help", () =>
            {
                showHelp = true;
            });

            commandList = new List<object>
            {
                HELP,
                TIMESCALE
            };

            commandHistory = new List<string>();
            historyIndex = 0;
        }

        public void OnToggleDebug(InputAction.CallbackContext value)
        {
            showConsole = !showConsole;
        }

        public void OnReturn(InputAction.CallbackContext value)
        {
            if (showConsole)
            {
                HandleInput();
                input = "";
            }
        }

        public void OnTextInput(InputAction.CallbackContext value)
        {
            if (showConsole)
            {
                object inputObject = value.ReadValueAsObject();

                if (inputObject is string)
                {
                    string currentInput = (string)inputObject;

                    if (currentInput == "\n")
                    {
                        HandleInput();
                        input = "";
                    }
                    else if (currentInput == "\b" && input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                    }
                    else
                    {
                        input += currentInput;
                    }
                }
            }
        }
        public void OnArrowUp(InputAction.CallbackContext value)
        {
            if (showConsole)
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    input = commandHistory[historyIndex];
                }
            }
        }
        public void OnArrowDown(InputAction.CallbackContext value)
        {
            if (showConsole)
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    input = commandHistory[historyIndex];
                }
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
                    if (commandList[i] is DebugCommand)
                    {
                        (commandList[i] as DebugCommand).Invoke();
                    }
                    else if (commandList[i] is DebugCommand<int>)
                    {
                        if (properties.Length > 1 && int.TryParse(properties[1], out int intValue))
                        {
                            (commandList[i] as DebugCommand<int>).Invoke(intValue);
                        }
                    }
                    else if (commandList[i] is DebugCommand<float>)
                    {
                        if (properties.Length > 1 && float.TryParse(properties[1], out float floatValue))
                        {
                            (commandList[i] as DebugCommand<float>).Invoke(floatValue);
                        }
                    }
                }
            }

            // Add the command to history
            commandHistory.Add(input);
            historyIndex = commandHistory.Count;
        }
    }
}
