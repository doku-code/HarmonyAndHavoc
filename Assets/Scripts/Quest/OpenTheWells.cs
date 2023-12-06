using AF;
using UnityEngine;

public class OpenTheWells : MonoBehaviour
{
    private static OpenTheWells instance;

    private bool wellsIsOpen = false;
    private GameObject wellsCollider;
    public static OpenTheWells Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        wellsCollider = GameObject.Find("QuestOpenWells");
    }
    public void OnAcceptQuest()
    {
        if (!wellsIsOpen)
        {
            SoundManager.Instance.PlayFxClip(4);
            wellsCollider.SetActive(false);
            wellsIsOpen = true;
        }
        else
        {
            return;
        }
    }
}
