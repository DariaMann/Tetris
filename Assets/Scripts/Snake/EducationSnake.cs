using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EducationSnake : MonoBehaviour
{
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private EducationFinger finger;
    
    [SerializeField] private GameObject leftPos;
    [SerializeField] private GameObject rightPos;
    [SerializeField] private GameObject upPos;
    [SerializeField] private GameObject downPos;
    
    [SerializeField] private CanvasGroup playButton;
    [SerializeField] private GameObject backButton;
    
    private Coroutine _tutorialCoroutine;
    private bool _isFirstShow;
    private bool _buttonPlayShowed;
    
    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameHelper.IsEdication)
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
    }
    
    public void StopTutorial()
    {
        finger.IsTutorialRunning = false;

        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        finger.Stop();
    }
    
    public void Restart(int page)
    {
        StopTutorial();

        StartAnimationPage1();
    }

    private void StartAnimationPage1()
    {
        finger.IsTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayMoveStep(Vector2Int.right, true));
    }

    private IEnumerator PlayMoveStep(Vector2Int dir, bool speed = false)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 start1 = Vector3.zero;
            Vector3 end1 = Vector3.zero;

            if (dir == Vector2Int.right)
            {
                // первый клик и перемещение пальца
                start1 = leftPos.transform.position;
                end1 = rightPos.transform.position;
            }
            else if (dir == Vector2Int.left)
            {
                // первый клик и перемещение пальца
                start1 = rightPos.transform.position;
                end1 = leftPos.transform.position;
            }
            else if (dir == Vector2Int.down)
            {
                // первый клик и перемещение пальца
                start1 = upPos.transform.position;
                end1 = downPos.transform.position;
            }

            yield return StartCoroutine(finger.PlayFingerClickMove(start1, end1, speed));

            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public IEnumerator ShowPlayButton()
    {
        playButton.alpha = 0f;
        playButton.interactable = false;
        playButton.blocksRaycasts = false;

        // Анимация появления
        yield return playButton.DOFade(1f, 0.3f).WaitForCompletion();

        // Если тут ошибка — кнопка останется прозрачной, но видимой
        if (!finger.IsTutorialRunning)
        {
            ForcePlayButtonVisible();
            yield break;
        }

        playButton.interactable = true;
        playButton.blocksRaycasts = true;
    }
    
    private void ForcePlayButtonVisible()
    {
        // Сделать кнопку видимой, если анимация не успела завершиться
        playButton.alpha = 1f;
        playButton.interactable = true;
        playButton.blocksRaycasts = true;
        _buttonPlayShowed = true;
    }
}