using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NumberManager : MonoBehaviour
{
    [SerializeField] List<NumberButton> numberButtons;
    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Color correctColor;
    [SerializeField] Color wrongColor;
    [SerializeField] Color neutralColor;

    float numberA;
    float numberB;
    float answer;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        SetAndDisableButtons();
    }

    private void Start()
    {
        SetTest(BossPhase.Hard);
    }

    public void SetTest(BossPhase phase) {
        SetManagerColor(neutralColor);
        switch (phase)
        {
            case BossPhase.Easy:
                SetFirstTest();
                break;
            case BossPhase.Average:
                SetSecondTest();
                break;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                SetThirdTest();
                break;
        }
    }

    private void SetAndDisableButtons()
    {
        foreach (var button in numberButtons)
        {
            button.SetColor(neutralColor);
            button.SetButtonManager(this);
            button.gameObject.SetActive(false);
        }
    }

    void SetFirstTest()
    {
        numberA = UnityEngine.Random.Range(1, 9);
        numberB = UnityEngine.Random.Range(1, 9);
        answer = numberA + numberB;
        textMeshPro.text = numberA + " + " + numberB;

        List<float> shuffledOptions = new List<float> { answer, answer + UnityEngine.Random.Range(1, 5) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1) };
        var result = shuffledOptions.OrderBy(a => Guid.NewGuid()).ToList();

        for (int i = 0; i < shuffledOptions.Count; i++)
        {
            numberButtons[i].gameObject.SetActive(true);
            numberButtons[i].SetValue(shuffledOptions[i]);
        }
    }

    void SetSecondTest()
    {
        numberA = UnityEngine.Random.Range(2, 5);
        numberB = UnityEngine.Random.Range(2, 5);
        answer = numberA * numberB;
        textMeshPro.text = numberA + " x " + numberB;

        List<float> shuffledOptions = new List<float> { 
            answer, 
            answer + UnityEngine.Random.Range(1, 3) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1),
            answer + UnityEngine.Random.Range(4, 5) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1)
        };
        var result = shuffledOptions.OrderBy(a => Guid.NewGuid()).ToList();

        for (int i = 0; i < shuffledOptions.Count; i++)
        {
            numberButtons[i].gameObject.SetActive(true);
            numberButtons[i].SetValue(shuffledOptions[i]);
        }
    }

    void SetThirdTest()
    {
        answer = 3.14f;
        textMeshPro.text = "pi?";

        List<float> shuffledOptions = new List<float> { 
            answer,
            answer + UnityEngine.Random.Range(1, 2) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1),
            answer + UnityEngine.Random.Range(3, 4) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1)};
        var result = shuffledOptions.OrderBy(a => Guid.NewGuid()).ToList();

        for (int i = 0; i < shuffledOptions.Count; i++)
        {
            numberButtons[i].gameObject.SetActive(true);
            numberButtons[i].SetValue(shuffledOptions[i]);
        }
    }

    public void OnHitButton(float value) {
        SetButtonsColor();
        if (value == answer) OnCorrectAnswer();
        else OnWrongAnswer();
    }

    private void OnWrongAnswer()
    {
        SetManagerColor(wrongColor);
    }

    private void OnCorrectAnswer()
    {
        SetManagerColor(correctColor);
    }

    void SetManagerColor(Color value)
    {
        sprite.color = value;
    }

    void SetButtonsColor()
    {
        foreach(NumberButton button in numberButtons) {
            if (button.Value == answer)
                button.SetColor(correctColor);
            else
                button.SetColor(wrongColor);
        }
    }
}
