using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using AF;
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
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private Button firstAnswerButton;
        [SerializeField] private Button secondAnswerButton;

        private bool buttonPressed = false;
        [SerializeField] private float typingSpeed = 0.2f;
        private int questionIndex = 0;
        private int currentUpgradePrice = 10;
        private bool repeatQuestion = true;
        private bool wellsIsOpen = false;

        private void Start()
        {
            LoadConversation(questionIndex);
        }

        public IEnumerator ShowText()
        {
            while (questionIndex < Conversations.Length)
            {
                string question = Conversations[questionIndex].questionText;

                questionText.text = "";
                for (int i = 0; i < question.Length; i++)
                {
                    questionText.text += question[i];
                    yield return new WaitForSeconds(typingSpeed);
                }
                yield return new WaitForSeconds(1.0f);

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

        public void LoadConversation(int index)
        {
            buttonPressed = false;
            if (index < Conversations.Length)
            {
                questionText.text = Conversations[index].questionText;
                Answer[] answers = Conversations[index].answers;

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

        public void OnAnswerSubmitted(int answerIndex)
        {
            buttonPressed = true;
            currentUpgradePrice += 10;

            if (!repeatQuestion)
            {
                questionIndex++;
            }
            LoadConversation(questionIndex);
        }
        
        public void OnAcceptQuest()
        {
            if (!wellsIsOpen)
            {
                SoundManager.Instance.PlayFxClip(4);
                MapManager.Instance.UnlockDoor();
                wellsIsOpen = true;
            }
            else
            {
                return;
            }
        }
    }
}
