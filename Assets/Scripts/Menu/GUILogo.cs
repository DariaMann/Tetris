using System;
using System.Collections;
using UnityEngine;

public class GUILogo : MonoBehaviour
{
    private static bool _introShownThisSession;

    [Header("Intro animation")]
    [SerializeField] private GameObject logoPanel;
    [SerializeField] private Animator logoAnimator;
    [SerializeField, Min(0f)] private float startDelay = 0.35f;
    private bool _introRequested;
    private bool _animationStarted;
    private bool _applicationPaused;
    private bool _hasFocus = true;
    private bool _paused;
    private bool _completed;
    
    public event Action LogoAnimationComplete = () => { };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetIntroSession()
    {
        _introShownThisSession = false;
    }

    private void Start()
    {
        ShowLogo();
    }

    public void ShowLogo()
    {
        if (_introRequested || _completed) return;
        _introRequested = true;

        if (_introShownThisSession)
        {
            OnLogoAnimationComplete();
            return;
        }

        _introShownThisSession = true;
        AudioManager.Instance.StopMusic();

        if (logoPanel != null && logoAnimator != null && logoAnimator.runtimeAnimatorController != null)
        {
            logoPanel.SetActive(true);
            logoAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            logoAnimator.enabled = true;
            logoAnimator.speed = 0f;
            logoAnimator.Play(0, 0, 0f);
            logoAnimator.Update(0f);
            StartCoroutine(WaitForLogoAnimation());
        }
        else
        {
            OnLogoAnimationComplete();
        }
    }
    
    private IEnumerator WaitForLogoAnimation()
    {
        yield return null;
        yield return new WaitForSecondsRealtime(startDelay);
        yield return new WaitUntil(() => !_paused);

        if (_completed) yield break;
        _animationStarted = true;
        logoAnimator.speed = 1f;

        yield return new WaitUntil(() =>
            !_paused && logoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        OnLogoAnimationComplete();
    }
    
    public void OnLogoAnimationComplete()
    {
        if (_completed) return;
        _completed = true;
        if (logoAnimator != null) logoAnimator.enabled = false;
        if (logoPanel != null) logoPanel.SetActive(false);

        AudioManager.Instance.StartMusic();
        LogoAnimationComplete();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        _applicationPaused = pauseStatus;
        UpdatePauseState();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _hasFocus = hasFocus;
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        _paused = _applicationPaused || !_hasFocus;
        if (!_completed && logoAnimator != null)
            logoAnimator.speed = !_paused && _animationStarted ? 1f : 0f;
    }
    
    public void EndSnakeSound()
    {
        AudioManager.Instance.StartLogoSound();
    }

    public void PlayShowTextSound()
    {
        AudioManager.Instance.EndLogoSound();
    }
}
