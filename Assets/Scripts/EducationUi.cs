using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EducationUi : MonoBehaviour
{
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private List<EducationHint> hints = new List<EducationHint>();
    
    private Coroutine _tutorialCoroutine;
    private Coroutine _hintCoroutine;
    private int _indexCurrentHint;
    private EducationHint _currentHint;
    private bool _allIsShowed;
    
    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameHelper.IsUIEdication)
        {
            ForceHintVisible();
        }
    }

    public void ShowEducation()
    {
        GameHelper.IsEdication = true;
        GameHelper.IsUIEdication = true;
        StopTutorial();
        educationPanel.SetActive(true);

        PlayTutorial();
    }

    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        GameHelper.IsUIEdication = false;
        StopTutorial();
        
        educationPanel.SetActive(false);
    }
    
    public void StopTutorial()
    {
        _allIsShowed = false;
        _indexCurrentHint = 0;
        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        foreach (var hint in hints)
        {
            hint.StopAnimation();
        }
    }

    private void PlayTutorial()
    {
        if (_tutorialCoroutine != null)
        {
            _currentHint.ForceHintVisible();
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }
        
        _tutorialCoroutine = StartCoroutine(PlayShowHint());
    }

    private IEnumerator PlayShowHint()
    {
        _currentHint = hints[_indexCurrentHint];
        yield return StartCoroutine(hints[_indexCurrentHint].StartAnimationCoroutine());
        
        hints[_indexCurrentHint].ForceHintVisible();
    }

    public void OnPanelClick()
    {
        _indexCurrentHint += 1;
        if (_indexCurrentHint >= hints.Count)
        {
            HideEducation();
        }
        else
        {
            PlayTutorial();
        }
    }
    
    
    public void ForceHintVisible()
    {
        if (_allIsShowed)
        {
            return;
        }
        
        foreach (var hint in hints)
        {
            hint.ForceHintVisible();
        }
        _allIsShowed = true;
        _indexCurrentHint = hints.Count-1;
    }
}