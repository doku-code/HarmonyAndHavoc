using JFM;

using UnityEngine;

public class OnDeathEnding : MonoBehaviour
{
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject win;

    private void Start()
    {
        if(ChaosOrderSystem.Instance.ChaosAmount == ChaosOrderSystem.maxChaosAmount - 1)
        {
            gameOver.SetActive(true);
        }
        else
        {
            win.SetActive(true);
        }

    }
}
