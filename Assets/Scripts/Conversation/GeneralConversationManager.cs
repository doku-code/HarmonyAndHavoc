using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using AF;
using JFM;
using System;
using System.Text;
using UnityEngine.Events;

namespace charles
{       
    public class GeneralConversationManager : MonoBehaviour
    {
        [SerializeField] public ConversationMessage[] messages;
        
        [Header("UI")]
        [SerializeField] protected Canvas questionCanvas;
        [SerializeField] protected PlayerData pData;
        [SerializeField] protected Button[] answerButtons;        
        
        [SerializeField] protected float typingSpeed = 0.08f;

        protected TMP_Text textComponent;
        protected bool isDisplayingMessage = false;
        protected float timeSinceTypingEnded = 0f;
        protected Coroutine displayCoroutine;
        
        private void Start()
        {
            textComponent = questionCanvas.GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            if (isDisplayingMessage)
            {
                timeSinceTypingEnded += Time.deltaTime;
            }
        }

        public void StopMessage()
        {
            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }
        }

        public IEnumerator DisplayMessage(string message)
        {            
            isDisplayingMessage = true;

            StringBuilder stringBuilder = new StringBuilder();         
            for (int i = 0; i < message.Length; i++)
            {
                stringBuilder.Append(message[i]);
                textComponent.text = stringBuilder.ToString();
                
                yield return new WaitForSeconds(typingSpeed);
            }
            textComponent.text = message;

            yield return new WaitForSeconds(1.0f);

            isDisplayingMessage = false;
            timeSinceTypingEnded = 0f;
        }

        public void DisplayConversationMessage()
        {
            DisplayConversationMessage(messages[0]);
        }
        
        public void DisplayConversationMessage(int messageIndex)
        {
            DisplayConversationMessage(messages[messageIndex]);
        }

        public void DisplayConversationMessage(ConversationMessage message)
        {                       
            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            displayCoroutine = StartCoroutine(DisplayMessage(message.text));
            
            DeactivateUnusedButtons(message);

            if (message.type == ConversationMessageType.NOBUTTONS
                || message.type == ConversationMessageType.SHOWITEMS)
            {
                return;
            }

            for (int i = 0; i < message.buttons.Length; i++)
            {
                if(i >= answerButtons.Length)
                {
                    break;
                }

                Button answerButton = answerButtons[i];                

                answerButton.gameObject.SetActive(true);
                answerButton.interactable = true;

                string label = "";

                switch (message.type)
                {
                    case ConversationMessageType.SHOWLABELS:
                        label = message.buttons[i].label;
                        break;
                    case ConversationMessageType.OKONLY:
                        if (i == 0)
                        {
                            label = "Ok";
                        }
                        break;
                    case ConversationMessageType.YESNO:
                        label = i % 2 == 0 ? "Yes" : "No";
                        break;                    
                }
                
                int value = message.buttons[i].evt.value;
                MyIntEvent evt = message.buttons[i].evt;

                answerButton.GetComponentInChildren<TMP_Text>().text = label;
                //answerButton.onClick.AddListener(message.buttons[i].);
                answerButton.onClick.RemoveAllListeners();
                answerButton.onClick.AddListener(() => 
                    {
                        //Debug.Log($"Lambda called with arg {i} {value} {message.buttons.Length}");
                        evt.Invoke(value); 
                    }
                );
            }                        
        }

        protected void DeactivateUnusedButtons(ConversationMessage message)
        {
            int length = message.type == ConversationMessageType.NOBUTTONS ? 0 : message.buttons.Length;
            if (length < answerButtons.Length)
            {
                for (int i = message.buttons.Length; i < answerButtons.Length; i++)
                {
                    Button button = answerButtons[i];
                    button.gameObject.SetActive(false);
                }
            }
        }

        public virtual void DoEffect() {}
    }
}
