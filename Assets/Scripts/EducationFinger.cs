using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EducationFinger: MonoBehaviour
{
    [Header("Offsets & Animation")]
    [SerializeField] private float clickOffsetY = 0.5f;           // Насколько палец "нажимает" вниз
    [SerializeField] private float clickRotateZ = 10f;          // Угол наклона при нажатии
    [SerializeField] private float clickDuration = 0.15f;        // Длительность нажатия
    [SerializeField] private float moveDuration = 0.6f;          // Длительность перемещения от A до B
    [SerializeField] private float fingerXOffset = -0.2f;          // Смещение пальца вверх, чтобы "кликал" верхушкой
    [SerializeField] private float fingerYOffset = 1f; 
    
    [SerializeField] private RectTransform finger;
    [SerializeField] private CanvasGroup _fingerCanvasGroup;

    private Vector3 _startFingerPos = Vector3.zero;
    private Sequence _currentFingerTween;
    private Coroutine _currentRoutine;

    public bool IsTutorialRunning { get; set; }

    private void Awake()
    {
        _startFingerPos = finger.position;
    }

    public void ChangeByOrientation(Vector2 size, float yOffset)
    {
        finger.sizeDelta = size;
        fingerYOffset = yOffset;
    }
    
    public IEnumerator PlayFingerClickMove(Vector3 from, Vector3 to)
    {
        Stop();
        _currentRoutine = StartCoroutine(FingerClickMove(from, to));
        yield return _currentRoutine;
    }
    
    public IEnumerator PlayFingerClickMove(Vector3 from, Vector3 to, bool speed)
    {
        Stop();
        _currentRoutine = StartCoroutine(FingerClickMove(from, to, speed));
        yield return _currentRoutine;
    }
    
    public IEnumerator PlayFingerClickMove(Vector3 from, Vector3 to, Vector2Int ballPos, LineBoard lineBoard)
    {
        Stop();
        _currentRoutine = StartCoroutine(FingerClickMove(from, to, ballPos, lineBoard));
        yield return _currentRoutine;
    }   
    
    public IEnumerator PlayShowFinger()
    {
        Stop();
        _currentRoutine = StartCoroutine(ShowFinger());
        yield return _currentRoutine;
    }   
    
    public IEnumerator PlayHideFinger()
    {
        Stop();
        _currentRoutine = StartCoroutine(HideFinger());
        yield return _currentRoutine;
    } 
    
    public IEnumerator PlayFingerClickRotate()
    {
        Stop();
        _currentRoutine = StartCoroutine(FingerClickRotate());
        yield return _currentRoutine;
    }

    public void Stop()
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }

        if (_currentFingerTween != null && _currentFingerTween.IsActive())
        {
            _currentFingerTween.Kill();
            _currentFingerTween = null;
        }

        // Убиваем все твины, связанные с объектом
        DOTween.Kill(finger.transform);
        DOTween.Kill(_fingerCanvasGroup);

        finger.position = _startFingerPos;
        finger.rotation = Quaternion.identity;

        if (_fingerCanvasGroup != null)
            _fingerCanvasGroup.alpha = 0f;
    }

    private IEnumerator FingerClickMove(Vector3 from, Vector3 to)
    {
        Vector3 offset = new Vector3(-fingerXOffset, -fingerYOffset, 0f);
        Vector3 fromAdjusted = from + offset;
        Vector3 toAdjusted = to + offset;

        finger.position = fromAdjusted;
        finger.rotation = Quaternion.identity;
        _fingerCanvasGroup.alpha = 0f;
        
        // Появление
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(1f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(fromAdjusted.y - clickOffsetY, clickDuration).SetEase(Ease.InOutQuad).SetId(finger));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration).SetId(finger));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Перемещение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMove(toAdjusted, moveDuration).SetEase(Ease.InOutSine).SetId(finger));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(toAdjusted.y, clickDuration).SetEase(Ease.InOutQuad).SetId(finger));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration).SetId(finger));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Исчезновение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(0f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
    }

    private IEnumerator FingerClickMove(Vector3 from, Vector3 to, bool speed)
    {
        Vector3 offset = new Vector3(-fingerXOffset, -fingerYOffset, 0f);
        Vector3 fromAdjusted = from + offset;
        Vector3 toAdjusted = to + offset;

        finger.position = fromAdjusted;
        finger.rotation = Quaternion.identity;

        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 0f;

        float fingerSpeed = moveDuration;
        
        if (speed)
        {
            fingerSpeed = 0.1f;
        }

        // Плавное появление
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(1f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(fromAdjusted.y - clickOffsetY, clickDuration)
            .SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;
        
        // Перемещение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMove(toAdjusted, fingerSpeed).SetEase(Ease.InOutSine));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;
        
        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(toAdjusted.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Плавное исчезновение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(0f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
    }
    
    private IEnumerator FingerClickMove(Vector3 from, Vector3 to, Vector2Int ballPos, LineBoard lineBoard)
    {
        Vector3 offset = new Vector3(-fingerXOffset, -fingerYOffset, 0f);
        Vector3 fromAdjusted = from + offset;
        Vector3 toAdjusted = to + offset;

        finger.position = fromAdjusted;
        finger.rotation = Quaternion.identity;

        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 0f;

        // Плавное появление
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(1f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(fromAdjusted.y - clickOffsetY, clickDuration)
            .SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(fromAdjusted.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        Ball ball = lineBoard.GetBallByPos(ballPos);
        ball.SetSelection(true);
        
        yield return new WaitForSeconds(0.2f);
        if (!IsTutorialRunning) yield break;

        // Перемещение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMove(toAdjusted, moveDuration).SetEase(Ease.InOutSine));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(toAdjusted.y - clickOffsetY, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(toAdjusted.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;

        // Плавное исчезновение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(0f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
    }
    
    private IEnumerator ShowFinger()
    {
        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 0f;

        // Плавное появление
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(1f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
    }
    
    private IEnumerator HideFinger()
    {
        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 1f;

        // Плавное исчезновение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(_fingerCanvasGroup.DOFade(0f, 0.3f).SetId(_fingerCanvasGroup));
        yield return _currentFingerTween.WaitForCompletion();
    }

    private IEnumerator FingerClickRotate()
    {
        finger.position = _startFingerPos;
        finger.rotation = Quaternion.identity;

        // Сначала палец видим
        _fingerCanvasGroup.alpha = 1f;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(_startFingerPos.y - clickOffsetY, clickDuration)
            .SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;
        
        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(_startFingerPos.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!IsTutorialRunning) yield break;
    }
}