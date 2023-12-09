using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeInFadeOutScreen : MonoBehaviour
{
    [SerializeField] private float fadeDelay = 2f;
    [SerializeField] private float FadeInFadeOutDuration;
    
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
        
        loadingCanvas.SetActive(false);
        
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
    }

    public void FadeInFadeOut()
    {
        StartCoroutine(FadeInFadeOutCoroutine());
    }

    private IEnumerator FadeInFadeOutCoroutine()
    {
        StartCoroutine(FadeInCoroutine());
        yield return new WaitForSeconds(FadeInFadeOutDuration * 0.5f);
        StartCoroutine(FadeOutCoroutine());
    }
    
    private IEnumerator FadeInCoroutine()
    {
        float timeLeft = fadeDelay;
        loadingCanvas.SetActive(true);
        while (timeLeft >= 0f)
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
    }    
    
    private IEnumerator FadeOutCoroutine()
    {
        float timeLeft = fadeDelay;
        
        while (timeLeft >= 0f)
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
