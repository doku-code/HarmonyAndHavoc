using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using AF;
using JFM;

namespace charles
{
    [System.Serializable]
    public class Answer
    {
        [TextArea]
        public string answerText;
        public int price;
        public bool includePrice;
    }

    [System.Serializable]
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
        [SerializeField] private Canvas thankYouCanvas;
        [SerializeField] private Button firstAnswerButton;
        [SerializeField] private Button secondAnswerButton;
        [SerializeField] private int goldForHealing = 50;

        private TMP_Text textComponent;
        private bool buttonPressed = false;
        [SerializeField] private float typingSpeed = 0.08f;
        private int questionIndex = 0;
        private int currentUpgradePrice = 10;
        private bool repeatQuestion = true;
        private bool wellsIsOpen = false;

        private void Start()
        {
            textComponent = questionCanvas.GetComponentInChildren<TMP_Text>();
            LoadConversation(questionIndex);
        }

        public IEnumerator ShowText()
        {
            while (questionIndex < Conversations.Length)
            {
                string message = Conversations[questionIndex].questionText;

                yield return DisplayMessage(message);

                buttonPressed = false;
                yield return new WaitUntil(() => buttonPressed);

                currentUpgradePrice += 10;

                if (repeatQuestion)
                {
                    LoadConversation(questionIndex);
                }
                else
                {
                    questionIndex++;
                }
            }
        }

        private IEnumerator DisplayMessage(string message)
        {
            textComponent.text = "";

            for (int i = 0; i < message.Length; i++)
            {
                textComponent.text += message[i];
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(1.0f);
        }

        public void OnAnswerSubmitted()
        {
            Debug.Log("Answer submitted!");
            if (thankYouCanvas != null)
            {
                thankYouCanvas.gameObject.SetActive(true);
                questionCanvas.gameObject.SetActive(false);

            }
            else
            {
                questionIndex++;
            }
            currentUpgradePrice += 10;
            buttonPressed = true;
        }
        public void healThePlayer()
        {
            if (pData.Gold >= goldForHealing && pData.ActualOrder < pData.MaxOrder)
            {
                pData.HealPlayer(pData.MaxOrder);
                pData.Gold -= goldForHealing;

                //Juste le slider qui update pas malgrer qui est call dans healplayer de Player data ?  


                thankYouCanvas.gameObject.SetActive(true);
                questionCanvas.gameObject.SetActive(false);
            }
            else
            {
                questionIndex++;
                //Reussis pas a aller a la prochaine question dans le cas ou ta pas assez de cash 
            }
            buttonPressed = true;
        }
        public void LoadConversation(int index)
        {
            buttonPressed = false;
            if (index < Conversations.Length)
            {
                Answer[] answers = Conversations[index].answers;

                textComponent.text = Conversations[index].questionText;

                for (int i = 0; i < 2; i++)
                {
                    Button answerButton = i == 0 ? firstAnswerButton : secondAnswerButton;

                    if (i < answers.Length)
                    {
                        answerButton.interactable = true;
                        string answerText = string.IsNullOrEmpty(answers[i].answerText) ? "No Answer" : answers[i].answerText;

                        if (answers[i].includePrice)
                        {
                            answerText += " - Price: $" + currentUpgradePrice;
                        }

                        answerButton.GetComponentInChildren<TMP_Text>().text = answerText;
                    }
                }
            }
        }

        public void OnAcceptQuest()
        {
            if (!wellsIsOpen)
            {
                SoundManager.Instance.PlayFxClip(4);
                MapManager.Instance.UnlockDoor();
                wellsIsOpen = true;
                GameManager.Instance.player.GetComponent<PlayerController>().Data.CurrentPlayerMapProgression[GameManager.Instance.currentMap] = true;
            }
            else
            {
                return;
            }
        }
    }
}
