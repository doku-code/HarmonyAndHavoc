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
    public enum MerchantMessageType
    {
        NOBUTTONS,
        SHOWITEMS,
        OKONLY,
        YESNO
    }

    [System.Serializable]
    public class MyIntEvent : UnityEvent<int>
    {
        public int value;
    }

    [Serializable]
    public class MerchantItem
    {
        [TextArea]
        public string label;
        public int price;        
    }
    
    [Serializable]
    public class MerchantMessage
    {
        [TextArea]
        public string text; 
        public MyIntEvent[] events;
        public MerchantMessageType type;
    }   

    public abstract class MerchantConversationManager : MonoBehaviour
    {
        [SerializeField] public MerchantMessage SellPitchMessage;
        [SerializeField] public MerchantMessage NotEnoughMoneyMessage;
        [SerializeField] public MerchantMessage AlreadyFullMessage;
        [SerializeField] public MerchantMessage ThankYouMessage;
        [SerializeField] public MerchantItem[] Items;

        [Header("UI")]
        [SerializeField] private Canvas questionCanvas;
        [SerializeField] protected PlayerData pData;
        [SerializeField] private Button[] answerButtons;        

        [SerializeField] private int priceIncrease = 10;
        [SerializeField] private float typingSpeed = 0.08f;
        
        private TMP_Text textComponent;
        private bool buttonPressed = false;
        private bool isDisplayingMessage = false;
        private float timeSinceTypingEnded = 0f;
        private Coroutine displayCoroutine;
        
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

        public Coroutine DisplayMerchantMessage(MerchantMessage message)
        {
            Coroutine coroutine = null;

            if(message.type == MerchantMessageType.SHOWITEMS
                && (message.events.Length != answerButtons.Length 
                || answerButtons.Length != Items.Length))
            {
                Debug.LogError("Number of events, answer buttons and items must be equal for a message of type 'SHOWITEMS'!");
                return coroutine;
            }

            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            buttonPressed = false;

            //textComponent.text = message.text;

            displayCoroutine = StartCoroutine(DisplayMessage(message.text));
            coroutine = displayCoroutine;

            DeactivateUnusedButtons(message);          

            for (int i = 0; i < message.events.Length; i++)
            {
                if(i >= answerButtons.Length)
                {
                    break;
                }

                Button answerButton = answerButtons[i];
                if (message.type == MerchantMessageType.NOBUTTONS)
                {
                    break;
                }

                answerButton.gameObject.SetActive(true);
                answerButton.interactable = true;

                string label = "";

                switch (message.type)
                {
                    case MerchantMessageType.SHOWITEMS:
                        if (i < Items.Length)
                        {
                            MerchantItem item = Items[i];
                            label = string.IsNullOrEmpty(Items[i].label) ? "No MerchantItem" : Items[i].label;
                            label += " - Price: $" + Items[i].price;
                        }
                        break;
                    case MerchantMessageType.OKONLY:
                        if (i == 0)
                        {
                            label = "Ok";
                        }
                        break;
                    case MerchantMessageType.YESNO:
                        label = i % 2 == 0 ? "Yes" : "No";
                        break;                    
                }
                
                Debug.Log($"i={i}");
                int value = message.events[i].value;
                MyIntEvent evt = message.events[i];

                answerButton.GetComponentInChildren<TMP_Text>().text = label;
                //answerButton.onClick.AddListener(message.events[i].);
                answerButton.onClick.RemoveAllListeners();
                answerButton.onClick.AddListener(() => 
                    {
                        Debug.Log($"Lambda called with arg {i} {value} {message.events.Length}");
                        evt.Invoke(value); 
                    }
                );
            }            

            return coroutine;
        }

        private void DeactivateUnusedButtons(MerchantMessage message)
        {
            int length = message.type == MerchantMessageType.NOBUTTONS ? 0 : message.events.Length;
            if (length < answerButtons.Length)
            {
                for (int i = message.events.Length; i < answerButtons.Length; i++)
                {
                    Button button = answerButtons[i];
                    button.gameObject.SetActive(false);
                }
            }
        }

        public void SellItem(int itemIndex)
        {
            Debug.Log($"SellItem() called with arg {itemIndex} Items[itemIndex].price={Items[itemIndex].price}");

            if(CheckIfFull())
            {
                DisplayMerchantMessage(AlreadyFullMessage);
            }
            else if (pData.Gold >= Items[itemIndex].price)
            {
                pData.Gold -= Items[itemIndex].price;
                ItemEffect(itemIndex);
                Items[itemIndex].price += priceIncrease;

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
    }
}
