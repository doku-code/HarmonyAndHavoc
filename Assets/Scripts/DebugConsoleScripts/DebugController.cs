using AF;
using JFM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        public static DebugCommand<int> KNOWLEDGESLOT;
        public static DebugCommand<int> ACTUALORDER;
        public static DebugCommand<int> ACTUALCHAOS;
        public static DebugCommand<int> MAXORDER;
        public static DebugCommand<int> GOLD;
        public static DebugCommand<int> ORDERFRAGMENTS;
        public static DebugCommand<int> WEAPONUPGRADE;
        public static DebugCommand<int> ARMORUPGRADE;
        public static DebugCommand<int> HITPLAYER;
        public static DebugCommand<int> CHANGECHAOSAMOUNT;        
        public static DebugCommand<string> SCENE;
        public static DebugCommand<int> SETKNOWLEDGE;
        public static DebugCommand<int> UNSETKNOWLEDGE;

        [Header("Script Reference")]
        public DebugFunction debugFunc;
        public PlayerData playerData;

        [Header("Command List")]
        public List<object> commandList;
        private static DebugController instance;

        private bool showErrorMessage;
        private float showErrorStartTime;
        [SerializeField] private float errorMessageDuration = 3.0f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                CallTheCheatHere();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void CallTheCheatHere()
        {
            HELP = new DebugCommand("help", "Show the list of available commands", "help", () =>
            {
                showHelp = true;
            });
            GOLD = new DebugCommand<int>("gold", "Add gold", "gold", (x) =>
            {
                playerData.Gold = x;
            });
            TIMESCALE = new DebugCommand<float>("timescale", "Reduce game time to debug", "timescale", (x) =>
            {
                debugFunc.ModifyTimeScale(x);
            });

            KNOWLEDGESLOT = new DebugCommand<int>("knowledgeslot", "Add knowledge slot", "knowledgeslot", (x) =>
            {
                playerData.KnowledgeSlots = x;
            });
            ACTUALORDER = new DebugCommand<int>("actualorder", "Add your Actual Order", "actualorder", (x) =>
            {
                playerData.ActualOrder = x;
            });
            ACTUALCHAOS = new DebugCommand<int>("actualchaos", "Add your Actual Chaos", "actualchaos", (x) =>
            {
                playerData.ActualChaos = x;
            });
            MAXORDER = new DebugCommand<int>("maxorder", "Add max Order", "maxorder", (x) =>
            {
                playerData.MaxOrder = x;
            });
            ORDERFRAGMENTS = new DebugCommand<int>("orderfragments", "Add order Fragments", "orderfragments", (x) =>
            {
                playerData.OrderFragments = x;
            });
            WEAPONUPGRADE = new DebugCommand<int>("weaponupgrade", "Upgrade weapon FREE OF CHARGE", "weaponupgrade", (x) =>
            {
                playerData.WeaponUpgrade = x;
            });
            ARMORUPGRADE = new DebugCommand<int>("armorupgrade", "Upgrade Armor FREE OF CHARGE", "armorupgrade", (x) =>
            {
                playerData.WeaponUpgrade = x;
            });
            SCENE = new DebugCommand<string>("scene", "Change Scene in build", "scene", (x) =>
            {
                debugFunc.ChangeScene(x);
            });
            HITPLAYER = new DebugCommand<int>("hitplayer", "Give damage to player", "hitplayer", (x) =>
            {
                debugFunc.HitPlayer(x);
            });
            CHANGECHAOSAMOUNT = new DebugCommand<int>("villagechaos", "Change the world state of chaos", "villagechaos", (x) =>
            {
                debugFunc.ChangeChaos(x);
            });
            SETKNOWLEDGE = new DebugCommand<int>("setknowledge", "Learn a knowledge", "setknowledge", (x) =>
            {
                debugFunc.SetKnowldege((KnowledgeID)x);
            });
            UNSETKNOWLEDGE = new DebugCommand<int>("unsetknowledge", "Unlearn a knowledge", "unsetknowledge", (x) =>
            {
                debugFunc.UnsetKnowldege((KnowledgeID)x);
            });

            commandList = new List<object>
            {
                HELP,
                TIMESCALE,
                KNOWLEDGESLOT,
                ACTUALORDER,
                ACTUALCHAOS,
                MAXORDER,                
                GOLD,
                ORDERFRAGMENTS,
                WEAPONUPGRADE,
                ARMORUPGRADE,
                SCENE,
                HITPLAYER,
                CHANGECHAOSAMOUNT,
                SETKNOWLEDGE,
                UNSETKNOWLEDGE
            };

            commandHistory = new List<string>();
            historyIndex = 0;
        }

        public void OnToggleDebug(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Performed)
            {
                showConsole = !showConsole;
                Time.timeScale = showConsole ? 0 : 1;

                // Let error message's duration start from now
                if (showErrorMessage)
                {
                    showErrorStartTime = Time.time;
                }
            }
        }

        public void OnReturn(InputAction.CallbackContext value)
        {
            if (showConsole && value.phase == InputActionPhase.Performed)
            {
                HandleInput();
                input = "";
            }
        }
        public void OnTextInput(InputAction.CallbackContext value)
        {
            if (showConsole && value.phase == InputActionPhase.Performed)
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
            if (!showConsole)
            {
                //Time.timeScale = 1f;
                return;
            }
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float fontSize = Mathf.Min(screenWidth, screenHeight) * 0.03f;
            float y = 0f;

            GUI.skin.textField.fontSize = (int)fontSize;
            GUI.skin.label.fontSize = (int)fontSize;
            if(showConsole)
            {
                //Time.timeScale = 0f;
            }
            int i = 0;
            if (showHelp)
            {
                GUI.Box(new Rect(0f, y, screenWidth, screenHeight * 0.2f), "");

                Rect viewport = new Rect(0, 0, screenWidth - 30f, screenHeight * 0.1f * commandList.Count);

                scroll = GUI.BeginScrollView(new Rect(0, y + screenHeight * 0.05f, screenWidth, screenHeight * 0.18f), scroll, viewport);

                for (; i < commandList.Count; i++)
                {
                    DebugCommandBase command = commandList[i] as DebugCommandBase;

                    string label = $"{command.commandFormat} - {command.commandDescription}";

                    Rect labelRect = new Rect(5f, screenHeight * 0.05f * i, viewport.width - 100f, screenHeight * 0.1f);

                    GUI.Label(labelRect, label);
                }
                GUI.EndScrollView();

                y += screenHeight * 0.2f;            
            }

            if (showErrorMessage)
            {
                Debug.Log($"{Time.time} - {showErrorStartTime} < {errorMessageDuration}");
                if (Time.time - showErrorStartTime < errorMessageDuration)
                {
                    Color oldColor = GUI.color;
                    GUI.color = Color.red;
                    GUI.Label(new Rect(5f, 0, screenWidth - 30f - 100f, screenHeight * 0.1f), "Command error!!!");
                    GUI.color = oldColor;
                }
                else
                {
                    showErrorMessage = false;
                }
            }

            GUI.Box(new Rect(0, y, screenWidth, screenHeight * 0.05f), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.SetNextControlName("DebugInput");
            input = GUI.TextField(new Rect(10f, y + screenHeight * 0.010f, screenWidth - 20f, screenHeight * 0.04f), input);
            if (showConsole)
            {
                GUI.FocusControl("DebugInput");
            }
        }

        private void HandleInput()
        {
            string[] properties = input.Split(' ');
            bool commandFound = false;
            for (int i = 0; i < commandList.Count && !commandFound; i++)
            {
                DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

                if (input.Contains(commandBase.commandId))
                {
                    if (commandList[i] is DebugCommand)
                    {
                        (commandList[i] as DebugCommand).Invoke();
                        commandFound = true;
                    }
                    else if (commandList[i] is DebugCommand<int>)
                    {
                        if (properties.Length > 1 && int.TryParse(properties[1], out int intValue))
                        {
                            (commandList[i] as DebugCommand<int>).Invoke(intValue);
                            commandFound = true;
                        }
                    }
                    else if (commandList[i] is DebugCommand<float>)
                    {
                        if (properties.Length > 1 && float.TryParse(properties[1], out float floatValue))
                        {
                            (commandList[i] as DebugCommand<float>).Invoke(floatValue);
                            commandFound = true;
                        }
                    }
                    else if (commandList[i] is DebugCommand<string>)
                    {
                        if (properties.Length > 1)
                        {
                            (commandList[i] as DebugCommand<string>).Invoke(properties[1]);
                            commandFound = true;
                        }
                    }
                }
            }
            if(commandFound)
            {
                commandHistory.Add(input);
                historyIndex = commandHistory.Count;
            }
            else
            {
                ShowErrorMessage();
            }
        }

        private void ShowErrorMessage()
        {
            showErrorMessage = true;
            showErrorStartTime = Time.time;
            Debug.Log("Called!!!");
        }
    }
}
