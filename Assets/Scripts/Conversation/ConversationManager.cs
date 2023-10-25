using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Answer
{
    public string answerText;
}
[System.Serializable]
public class Question
{
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


    public int questionIndex = 0;
    private void Start()
    {
        LoadConversation(questionIndex);
    }
    public void LoadConversation(int index)
    {
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
                        firstAnswerButton.gameObject.SetActive(false);
                        break;
                    case 1:
                        answerButton = secondAnswerButton;
                        secondAnswerButton.gameObject.SetActive(false);
                        break;
                    case 2:
                        answerButton = thirdAnswerButton;
                        thirdAnswerButton.gameObject.SetActive(false);
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
        LoadConversation(questionIndex);
    }
}