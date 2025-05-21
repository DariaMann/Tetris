using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EducationHint : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float lineFillDuration = 1f;
    [SerializeField] private float textFadeDuration = 0.5f;

    private Coroutine animationCoroutine;
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameHelper.IsEdication)
        {
            ForceHintVisible();
        }
    }

    public void StartAnimation()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateHint());
    }
    
    public IEnumerator StartAnimationCoroutine()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        yield return animationCoroutine = StartCoroutine(AnimateHint());
    }

    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        // Сброс состояний
        if (image != null)
            image.fillAmount = 0f;

        if (text != null)
        {
            var color = text.color;
            color.a = 0f;
            text.color = color;
        }
    }

    private IEnumerator AnimateHint()
    {
        // 1. Анимация заливки линии
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / lineFillDuration;
            image.fillAmount = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        image.fillAmount = 1f;

        // 2. Анимация появления текста
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / textFadeDuration;
            var color = text.color;
            color.a = Mathf.Lerp(0f, 1f, t);
            text.color = color;
            yield return null;
        }

        animationCoroutine = null;
    }
    
    public void ForceHintVisible()
    {
        image.fillAmount = 1f;
        
        var color = text.color;
        color.a = 1f;
        text.color = color;
    }
}
