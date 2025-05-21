using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EducationChineseCheckers : MonoBehaviour
{
    [SerializeField] private EducationUi educationUi;
    
    [SerializeField] private float waitingTime = 0.2f;
    [SerializeField] private GameObject educationPanel;

    [SerializeField] private Image image1Page1;
    [SerializeField] private Image image2Page1;
    
    [SerializeField] private Image image1Page2;
    [SerializeField] private Image image2Page2;
    [SerializeField] private Image image3Page2;
    [SerializeField] private Image image4Page2;
    
    [SerializeField] private CanvasGroup playButton;
    [SerializeField] private GameObject backButton;

    private List<Coroutine> _tutorialCoroutines = new List<Coroutine>();
    
    private bool _isFirstShow;
    private bool _buttonPlayShowed;
    
    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameHelper.IsEdication && !GameHelper.IsUIEdication)
        {
            Restart(0);
        }
    }
    
    public void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        GameHelper.IsEdication = true;
        StopTutorial();
        ShowView(isFirstEducation);
        ShowEducation();
    }

    public void ShowEducation()
    {
        educationPanel.SetActive(true);

        StartAnimationPage1();
        StartAnimationPage2();
    }
    
    public void ShowView(bool isFirstEducation)
    {
        if (isFirstEducation)
        {
            playButton.gameObject.SetActive(true);
            backButton.SetActive(false);
        }
        else
        {
            playButton.gameObject.SetActive(false);
            backButton.SetActive(true);
        }
    }
    
    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        StopTutorial();
        
        educationPanel.SetActive(false);
        
        if (_isFirstShow)
        {
            educationUi.ShowEducation();
        }
    }
    
    public void StopTutorial()
    {
        foreach (var tutorial in _tutorialCoroutines)
        {
            if (tutorial != null)
            {
                StopCoroutine(tutorial);
            }
        }
        
        _tutorialCoroutines.Clear();
        
        
        image1Page1.DOKill();
        image2Page1.DOKill();
        image1Page2.DOKill();
        image2Page2.DOKill(); 
        image3Page2.DOKill();
        image4Page2.DOKill();
    }
    
    public void Restart(int page)
    {
        StopTutorial();

        StartAnimationPage1();
        StartAnimationPage2();
    }

    private void StartAnimationPage1()
    {
        Coroutine coroutine = StartCoroutine(PlayTutorialPage1());
        _tutorialCoroutines.Add(coroutine);
    }

    private IEnumerator PlayTutorialPage1()
    {
        Color c;
        while (true)
        {
            c = image1Page1.color;
            c.a = 0f;
            image1Page1.color = c;

            c = image2Page1.color;
            c.a = 0f;
            image2Page1.color = c;

            yield return image1Page1.DOFade(1f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image2Page1.DOFade(1f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image1Page1.DOFade(0f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image2Page1.DOFade(0f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);
        }
    }

    private void StartAnimationPage2()
    {
        Coroutine coroutine = StartCoroutine(PlayTutorialPage2());
        _tutorialCoroutines.Add(coroutine);  
        coroutine = StartCoroutine(PlayTutorial2Page2());
        _tutorialCoroutines.Add(coroutine);
    }

    private IEnumerator PlayTutorialPage2()
    {
        Color c;
        while (true)
        {
            c = image1Page2.color;
            c.a = 0f;
            image1Page2.color = c;

            c = image2Page2.color;
            c.a = 0f;
            image2Page2.color = c;

            yield return image1Page2.DOFade(1f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image2Page2.DOFade(1f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image2Page2.DOFade(0f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image1Page2.DOFade(0f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

        }
    }

    private IEnumerator PlayTutorial2Page2()
    {
        Color c;
        while (true)
        {
            c = image3Page2.color;
            c.a = 0f;
            image3Page2.color = c;

            c = image4Page2.color;
            c.a = 0f;
            image4Page2.color = c;

            yield return image3Page2.DOFade(1f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image4Page2.DOFade(1f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image4Page2.DOFade(0f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

            yield return image3Page2.DOFade(0f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(waitingTime);

        }
    }

}