using AF;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FadeInFadeOutScreen : MonoBehaviour
{
    [SerializeField] private float fadeDelay;
    [SerializeField] private float fadeInFadeOutDuration;
    
    private GameObject loadingCanvas;
    
    private Image[] loadingScreenImages;
    private RawImage[] loadingScreenRawImages;
    private TMP_Text[] loadingScreenTexts;
    
    void Awake()
    {
        loadingCanvas = GetComponentInChildren<Canvas>().gameObject;
        loadingScreenImages = GetComponentsInChildren<Image>();
        loadingScreenTexts = GetComponentsInChildren<TMP_Text>();
        loadingScreenRawImages = GetComponentsInChildren<RawImage>();
        
        foreach(var text in loadingScreenTexts)
        {
            text.alpha = 0f;
        }

        foreach (var image in loadingScreenImages)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }

        foreach (var raw in loadingScreenRawImages)
        {
            raw.color = new Color(raw.color.r, raw.color.g, raw.color.b, 0);
        }
        
        loadingCanvas.SetActive(false);
    }

    public void FadeInFadeOut()
    {
        StartCoroutine(FadeInFadeOutCoroutine());
    }

    private IEnumerator FadeInFadeOutCoroutine()
    {
        StartCoroutine(FadeInCoroutine());
        yield return new WaitForSeconds(fadeInFadeOutDuration * 0.5f);
        
        StartCoroutine(FadeOutCoroutine());
    }
    
    private IEnumerator FadeInCoroutine()
    {
        float timeLeft = fadeDelay;
        loadingCanvas.SetActive(true);
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            foreach (var text in loadingScreenTexts)
            {
                text.alpha += 1 / fadeDelay * Time.deltaTime;
            }

            foreach (var image in loadingScreenImages)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b,
                    (image.color.a + 1 / fadeDelay * Time.deltaTime));
            }

            foreach (var raw in loadingScreenRawImages)
            {
                raw.color = new Color(raw.color.r, raw.color.g, raw.color.b,
                    (raw.color.a + 1 / fadeDelay * Time.deltaTime));
            }

            yield return null;
        }

        GameManager.Instance.OnReadyToLoadMapDelegate();
    }    
    
    private IEnumerator FadeOutCoroutine()
    {
        float timeLeft = fadeDelay;
        
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            
            foreach (var text in loadingScreenTexts)
            {
                text.alpha -= 1 / fadeDelay * Time.deltaTime;
            }            
            
            foreach (var image in loadingScreenImages)
            {
                image.color = new Color(image.color.r,image.color.g,image.color.b,(image.color.a - 1 / fadeDelay * Time.deltaTime));
            }            
            
            foreach (var raw in loadingScreenRawImages)
            {
                raw.color = new Color(raw.color.r,raw.color.g,raw.color.b,(raw.color.a - 1 / fadeDelay * Time.deltaTime));
            }
            yield return null;
        }
        loadingCanvas.SetActive(false);
    }
}
