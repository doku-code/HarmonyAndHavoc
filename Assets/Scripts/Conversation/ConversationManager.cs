using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using AF;
using JFM;
using System;
using System.Text;

namespace charles
{
    [Serializable]
    public class Answer
    {
        [TextArea]
        public string answerText;
        public int price;
        public bool includePrice;
    }

    [Serializable]
    public class Question
    {
        [TextArea]
        public string questionText;
        public Answer[] answers;
    }

    public class ConversationManager : MonoBehaviour
    {
        [SerializeField] public Question[] Conversations;

        [Header("UI")]
        [SerializeField] private Canvas questionCanvas;
        [SerializeField] private PlayerData pData;
        [SerializeField] private Button firstAnswerButton;
        [SerializeField] private Button secondAnswerButton;
        [SerializeField] private int goldForHealing = 50;
        [SerializeField] private int goldForArmor = 10;
        [SerializeField] private int goldForWeapon = 10;
        [SerializeField] private int priceIncrease = 10;

        public int questionIndex = 0;
        [SerializeField] private float typingSpeed = 0.08f;
        private TMP_Text textComponent;
        private bool buttonPressed = false;
        private int currentUpgradePrice = 10;
        private bool wellsIsOpen = false;
        private bool isDisplayingMessage = false;
        private float timeSinceTypingEnded = 0f;

        private void Start()
        {
            textComponent = questionCanvas.GetComponentInChildren<TMP_Text>();

            // Why here? This method displays all the messages in the conversation at once in their
            // respective TMP text property!?
            // DisplayConversation(questionIndex);
        }

        private void Update()
        {
            if (isDisplayingMessage)
            {
                timeSinceTypingEnded += Time.deltaTime;
            }
        }

        private IEnumerator TellAStory() //  When a NPC want to only tell a story or say a message on multiple Question 
        {
            while (questionIndex < Conversations.Length)
            {
                string message = Conversations[questionIndex].questionText;

                yield return DisplayMessage(message);

                buttonPressed = false;
                yield return new WaitUntil(() => buttonPressed);

                questionIndex++;
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

        public Coroutine DisplayConversation(int index)
        {
            Coroutine coroutine = null;

            buttonPressed = false;
            if (index < Conversations.Length)
            {
                Answer[] answers = Conversations[index].answers;

                textComponent.text = Conversations[index].questionText;

                coroutine = StartCoroutine(DisplayMessage(Conversations[index].questionText));

                for (int i = 0; i < 2; i++)
                {
                    Button answerButton = i == 0 ? firstAnswerButton : secondAnswerButton;

                    if (i < answers.Length)
                    {
                        answerButton.interactable = true;
                        string answerText = string.IsNullOrEmpty(answers[i].answerText) ? "No Answer" : answers[i].answerText;

                        if (answers[i].includePrice)
                        {
                            answerText += " - Price: $" + answers[i].price;
                        }

                        answerButton.GetComponentInChildren<TMP_Text>().text = answerText;
                    }
                }
            }

            return coroutine;
        }

        public void BlackSmithArmor()
        {
            if (pData.Gold >= goldForArmor)
            {
                pData.Gold -= goldForArmor;
                pData.ArmorUpgrade += 1;
                goldForArmor += priceIncrease;

                DisplayConversation(2);
            }
            else
            {
                DisplayConversation(1);
            }
            buttonPressed = true;
        }

        public void BlackSmithWeapon()
        {
            if (pData.Gold >= goldForWeapon)
            {
                pData.Gold -= goldForWeapon;
                pData.WeaponUpgrade += 1;
                goldForWeapon += priceIncrease;

                DisplayConversation(2);
            }
            else
            {
                DisplayConversation(1);
            }
            buttonPressed = true;
        }

        public void healThePlayer()
        {            
            if (!isDisplayingMessage)
            {
                if (pData.Gold >= goldForHealing && pData.ActualOrder < pData.MaxOrder)
                {
                    Debug.Log("HealThePlayer() actually healed the player.");
                    pData.HealPlayer(pData.MaxOrder);
                    pData.Gold -= goldForHealing;                    
                    DisplayConversation(2);
                }
                else if (pData.ActualOrder == pData.MaxOrder)
                {
                    Debug.Log("HealThePlayer() pData.ActualOrder == pData.MaxOrder.");
                    DisplayConversation(3);
                }
                else
                {
                    Debug.Log("HealThePlayer() ...else");
                    DisplayConversation(1);
                }

                buttonPressed = true;
            }
        }
        public void GraveDigger()
        {
            /*if (!wellsIsOpen)
            {*/
                SoundManager.Instance.PlayFxClip(4);
                MapManager.Instance.UnlockDoor();
                /*wellsIsOpen = true;                
            }
            else
            {
                return;
            }*/
        }

    }
}
