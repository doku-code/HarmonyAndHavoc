using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace charles
{
    [System.Serializable]
    public class Answer
    {
        [TextArea]
        public string answerText;
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
        [SerializeField] private Button thirdAnswerButton;

        private bool buttonPressed = false;
        [SerializeField] private float typingSpeed = 0.2f;
        public int questionIndex = 0;
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

                questionIndex++;
            }
        }
        public void LoadConversation(int index)
        {
            buttonPressed = false;
            if (index < Conversations.Length)
            {
                questionText.text = Conversations[index].questionText;
                Answer[] answers = Conversations[index].answers;

                for (int i = 0; i < 3; i++)
                {
                    Button answerButton = null;
                    switch (i)
                    {
                        case 0:
                            answerButton = firstAnswerButton;
                            break;
                        case 1:
                            answerButton = secondAnswerButton;
                            break;
                        case 2:
                            answerButton = thirdAnswerButton;
                            break;
                    }

                    if (i < answers.Length)
                    {
                        answerButton.interactable = true;
                        answerButton.GetComponentInChildren<TMP_Text>().text = answers[i].answerText;
                    }
                    else
                    {
                        answerButton.interactable = false;
                    }
                }
            }
        }
        public void OnAnswerSubmitted(int answerIndex)
        {
            questionIndex++;
            buttonPressed = true;
            LoadConversation(questionIndex);
        }
    }
}