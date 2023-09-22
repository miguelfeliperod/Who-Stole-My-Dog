using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
    [SerializeField] List<CreditNode> creditNodes;
    [SerializeField] float fadeInNodeTime;
    [SerializeField] float fadeOutNodeTime;

    void Start()
    {
        foreach (var creditNode in creditNodes)
        {
            foreach (Image image in creditNode.images)
                image.color = Color.clear;
            foreach (TextMeshProUGUI text in creditNode.texts)
                text.color = Color.clear;
        }

        StartCoroutine(PlayCreditNodes());
    }

    IEnumerator PlayCreditNodes()
    {
        yield return new WaitForSeconds(3);

        foreach (CreditNode creditNode in creditNodes)
        {
            foreach (Image image in creditNode.images)
                StartCoroutine(ShowAndHideImage(image, 5, Color.white));
            foreach (TextMeshProUGUI text in creditNode.texts)
                StartCoroutine(ShowAndHideText(text, 5, Color.white));
            yield return new WaitForSeconds(fadeInNodeTime + fadeOutNodeTime + creditNode.timeToWait + 1);
        }

        yield return new WaitForSeconds(3);
    }

    IEnumerator ShowAndHideImage(Image image, float timeToWait, Color targetColor ) {
        float timer = 0;

        while (timer < fadeInNodeTime)
        {
            timer += Time.deltaTime;
            image.color = Color.Lerp(Color.clear, targetColor, timer / fadeInNodeTime);
            yield return null;
        }
        image.color = targetColor;


        yield return new WaitForSeconds(timeToWait);

        timer = 0;

        while (timer < fadeOutNodeTime)
        {
            timer += Time.deltaTime;
            image.color = Color.Lerp(targetColor, Color.clear, timer / fadeOutNodeTime);
            yield return null;
        }
        image.color = Color.clear;
    }

    IEnumerator ShowAndHideText(TextMeshProUGUI text, float timeToWait, Color targetColor)
    {
        float timer = 0;

        while (timer < fadeInNodeTime)
        {
            timer += Time.deltaTime;
            text.color = Color.Lerp(Color.clear, targetColor, timer / (fadeInNodeTime));
            yield return null;
        }
        text.color = targetColor;


        yield return new WaitForSeconds(timeToWait);

        timer = 0;

        while (timer < fadeOutNodeTime)
        {
            timer += Time.deltaTime;
            text.color = Color.Lerp(targetColor, Color.clear, timer / fadeOutNodeTime);
            yield return null;
        }
        text.color = Color.clear;
    }
}

[Serializable]
public class CreditNode
{
    public List<Image> images;
    public List<TextMeshProUGUI> texts;
    public float timeToWait;
}
