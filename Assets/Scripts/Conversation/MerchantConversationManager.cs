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
    public enum ConversationMessageType
    {
        NOBUTTONS,
        SHOWITEMS,
        SHOWLABELS,
        OKONLY,
        YESNO
    }

    [System.Serializable]
    public class MyIntEvent : UnityEvent<int>
    {
        public int value;        
    }

    [Serializable]
    public class ConversationButton
    {
        public MyIntEvent evt;
        public string label;
    }

    [Serializable]
    public class MerchantItem
    {
        [TextArea]
        public string label;
        public int price;
    }

    [Serializable]
    public class ConversationMessage
    {
        [TextArea]
        public string text;
        public ConversationButton[] buttons;
        public ConversationMessageType type;
    }

    public abstract class MerchantConversationManager : MonoBehaviour
    {
        [SerializeField] public ConversationMessage SellPitchMessage;
        [SerializeField] public ConversationMessage NotEnoughMoneyMessage;
        [SerializeField] public ConversationMessage AlreadyFullMessage;
        [SerializeField] public ConversationMessage ThankYouMessage;
        [SerializeField] public MerchantItem[] Items;

        [Header("UI")]
        [SerializeField] protected Canvas questionCanvas;
        [SerializeField] protected PlayerData pData;
        [SerializeField] protected Button[] answerButtons;        

        [SerializeField] protected int priceIncrease = 10;
        [SerializeField] protected float typingSpeed = 0.08f;
        
        protected TMP_Text textComponent;
        protected bool buttonPressed = false;
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

        public void DisplayMerchantMessage(ConversationMessage message)
        {           
            if(message.type == ConversationMessageType.SHOWITEMS
                && (message.buttons.Length != answerButtons.Length 
                || answerButtons.Length != Items.Length))
            {
                Debug.LogError("Number of conversation buttons, answer buttons and items must be equal for a message of type 'SHOWITEMS'!");
                return;
            }

            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            buttonPressed = false;

            //textComponent.text = message.text;

            displayCoroutine = StartCoroutine(DisplayMessage(message.text));
            
            DeactivateUnusedButtons(message);

            if (message.type == ConversationMessageType.NOBUTTONS)
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
                    case ConversationMessageType.SHOWITEMS:
                        if (i < Items.Length)
                        {
                            int newPrice = GetItemInflatedPrice(i);
                            MerchantItem item = Items[i];
                            label = string.IsNullOrEmpty(Items[i].label) ? "No MerchantItem" : Items[i].label;
                            label += " - Price: $" + newPrice;
                        }
                        break;
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

        private void DeactivateUnusedButtons(ConversationMessage message)
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

        public void SellItem(int itemIndex)
        {
            //Debug.Log($"SellItem() called with arg {itemIndex} Items[itemIndex].price={Items[itemIndex].price}");
            int newPrice = GetItemInflatedPrice(itemIndex);

            if(CheckIfFull())
            {
                DisplayMerchantMessage(AlreadyFullMessage);
            }
            else if (pData.Gold >= newPrice)
            {
                pData.Gold -= newPrice;
                ItemEffect(itemIndex);                

                DisplayMerchantMessage(ThankYouMessage);
            }
            else
            {
                DisplayMerchantMessage(NotEnoughMoneyMessage);
            }
            buttonPressed = true;
        }

        public abstract void ItemEffect(int itemIndex);

        public abstract bool CheckIfFull();

        public abstract int GetItemInflatedPrice(int itemIndex);
    }
}
