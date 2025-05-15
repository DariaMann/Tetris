using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EducationLines98 : MonoBehaviour
{
    [Header("Offsets & Animation")]
    [SerializeField] private float clickOffsetY = 10f;           // Насколько палец "нажимает" вниз
    [SerializeField] private float clickRotateZ = -10f;          // Угол наклона при нажатии
    [SerializeField] private float clickDuration = 0.15f;        // Длительность нажатия
    [SerializeField] private float moveDuration = 0.6f;          // Длительность перемещения от A до B
    [SerializeField] private float fingerXOffset = 1f;          // Смещение пальца вверх, чтобы "кликал" верхушкой
    [SerializeField] private float fingerYOffset = 1f;          // Смещение пальца вверх, чтобы "кликал" верхушкой
    
    [SerializeField] private RectTransform finger;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private GameManagerLines98 gameManager;
    [SerializeField] private LineBoard lineBoard;
    
    [SerializeField] private CanvasGroup playButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject topText;
    [SerializeField] private GameObject topBoardText;
    [SerializeField] private GameObject bottomText;
    
    private CanvasGroup _fingerCanvasGroup;
    private SquareUIGrid _squareUi;
    private RectTransform _rectBoard;

    private Vector3 _startFingerPos;
    
    private Vector2Int _bluePos;
    private Vector2Int _yellowPos;
    private Vector2Int _redPos;
    
    private Vector2Int _futureBluePos;
    private Vector2Int _futureYellowPos;
    private Vector2Int _futureRedPos;
    
    private Coroutine _tutorialCoroutine;
    private Sequence _currentFingerTween;
    private bool _isTutorialRunning;
    
    private bool _isFirstShow;
    private bool _buttonPlayShowed;
    
    public void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        ShowView(isFirstEducation);
        ShowEducation();
    }

    private void ShowEducation()
    {
        educationPanel.SetActive(true);
        
        _fingerCanvasGroup = finger.gameObject.GetComponent<CanvasGroup>();

        _startFingerPos = finger.transform.position;
        
        _futureBluePos = new Vector2Int(1,0);
        _futureYellowPos = new Vector2Int(3,2);
        _futureRedPos = new Vector2Int(8,3);
        
        _bluePos = new Vector2Int(3,1);
        _yellowPos = new Vector2Int(2,8);
        _redPos = new Vector2Int(7,7);

        SaveDataLines98 saveData = GetSaveData();
        
        gameManager.LoadEducation(saveData);

        _isTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayTutorial());
    }
    
    private void ShowView(bool isFirstEducation)
    {
        bool isTablet = GameHelper.IsTablet();
        if (isFirstEducation)
        {
            playButton.gameObject.SetActive(true);
            if (isTablet)
            {
                topBoardText.SetActive(false);
                topText.SetActive(true);
            }
            else
            {
                topBoardText.SetActive(true);
                topText.SetActive(false);
            }
            backButton.SetActive(false);
            bottomText.SetActive(false);
        }
        else
        {
            playButton.gameObject.SetActive(false);
            topText.SetActive(false);
            backButton.SetActive(true);
            bottomText.SetActive(true);
            
            if (isTablet)
            {
                topBoardText.SetActive(false);
                bottomText.SetActive(true);
            }
            else
            {
                topBoardText.SetActive(true);
                bottomText.SetActive(false);
            }
        }
    }
    
    public void SetOrientation(bool isVertical)
    {
        bool isTablet = GameHelper.IsTablet();
        if (_squareUi == null)
        {
            _squareUi = lineBoard.GetComponent<SquareUIGrid>();
        }  
        if (_rectBoard == null)
        {
            _rectBoard = lineBoard.GetComponent<RectTransform>();
        }
        if (isVertical)
        {
            _squareUi.Padding = 67;
            if (isTablet)
            {
                finger.sizeDelta = new Vector2(80f, 80f);
                fingerYOffset = 1f;
            }
            else
            {
                finger.sizeDelta = new Vector2(60f, 60f);
                fingerYOffset = 0.5f;
            }
            
            Vector2 pos = _rectBoard.anchoredPosition;
            pos.y = 0f;
            _rectBoard.anchoredPosition = pos;
        }
        else
        {
            if (_isFirstShow)
            {
                _squareUi.Padding = 148;
                finger.sizeDelta = new Vector2(50f, 50f);
                fingerYOffset = 1f;
                
                Vector2 pos = _rectBoard.anchoredPosition;
                pos.y = 0f;
                _rectBoard.anchoredPosition = pos;
            }
            else
            {
                _squareUi.Padding = 113;
                finger.sizeDelta = new Vector2(70f, 70f);
                fingerYOffset = 1f;
                
                Vector2 pos = _rectBoard.anchoredPosition;
                pos.y = 50f;
                _rectBoard.anchoredPosition = pos;
            }
        }

        ShowView(_isFirstShow);
    }
    
    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && educationPanel.activeSelf)
        {
            Restart();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && educationPanel.activeSelf)
        {
            Restart();
        }
        
        if (pauseStatus)
        {
            ForcePlayButtonVisible();
        }
    }

    public void HideEducation()
    {
        StopTutorial();
        
        gameManager.ResetAllBoardEducation();
        educationPanel.SetActive(false);
    }
    
    private void StopTutorial()
    {
        _isTutorialRunning = false;

        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        if (_currentFingerTween != null && _currentFingerTween.IsActive())
        {
            _currentFingerTween.Kill();
            _currentFingerTween = null;
        }

        DOTween.Kill(_fingerCanvasGroup);
        
        finger.transform.position = _startFingerPos;
        finger.rotation = Quaternion.identity;
        if (_fingerCanvasGroup != null)
        {
            _fingerCanvasGroup.alpha = 0f;
        }
    }

    private SaveDataLines98 GetSaveData()
    {
        SaveBall saveBall = new SaveBall(0,3,1);
        SaveBall saveBall1 = new SaveBall(0,1,1);
        SaveBall saveBall2 = new SaveBall(0,1,2);
        SaveBall saveBall3 = new SaveBall(0,1,3);
        SaveBall saveBall4 = new SaveBall(0,1,4);
        
        SaveBall save2Ball = new SaveBall(6,2,8);
        SaveBall save2Ball1 = new SaveBall(6,4,2);
        SaveBall save2Ball2 = new SaveBall(6,5,2);
        SaveBall save2Ball3 = new SaveBall(6,6,2);
        SaveBall save2Ball4 = new SaveBall(6,7,2);
        
        SaveBall save3Ball = new SaveBall(5,7,7);
        SaveBall save3Ball1 = new SaveBall(5,4,7);
        SaveBall save3Ball2 = new SaveBall(5,5,6);
        SaveBall save3Ball3 = new SaveBall(5,6,5);
        SaveBall save3Ball4 = new SaveBall(5,7,4);
        SaveDataLines98 saveData = new SaveDataLines98();
        saveData.SaveBalls = new List<SaveBall>()
        {
            saveBall, saveBall1, saveBall2, saveBall3, saveBall4,
            save2Ball, save2Ball1, save2Ball2, save2Ball3, save2Ball4,
            save3Ball, save3Ball1, save3Ball2, save3Ball3, save3Ball4
        };
        return saveData;
    }

    private void Restart()
    {
        StopTutorial();

        SaveDataLines98 saveData = GetSaveData();
        
        gameManager.ReloadEducation(saveData);
        
        _isTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayTutorial());
    }

    private IEnumerator PlayTutorial()
    {
        yield return new WaitForSeconds(0.1f);

        // первый клик и перемещение пальца
        Vector3 start1 = lineBoard.GetLineTileByPos(_bluePos).transform.position;
        Vector3 end1 = lineBoard.GetLineTileByPos(_futureBluePos).transform.position;

        yield return StartCoroutine(FingerClickMove(start1, end1, _bluePos));

//        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(lineBoard.GetLineTileByPos(_futureBluePos).PlayerMoveCoroutine());

        // второй клик и перемещение пальца
        Vector3 start2 = lineBoard.GetLineTileByPos(_yellowPos).transform.position;
        Vector3 end2 = lineBoard.GetLineTileByPos(_futureYellowPos).transform.position;
        yield return StartCoroutine(FingerClickMove(start2, end2, _yellowPos));

//        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(lineBoard.GetLineTileByPos(_futureYellowPos).PlayerMoveCoroutine());

        // третий клик и перемещение пальца
        Vector3 start3 = lineBoard.GetLineTileByPos(_redPos).transform.position;
        Vector3 end3 = lineBoard.GetLineTileByPos(_futureRedPos).transform.position;
        yield return StartCoroutine(FingerClickMove(start3, end3, _redPos));

//        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(lineBoard.GetLineTileByPos(_futureRedPos).PlayerMoveCoroutine());

        yield return new WaitForSeconds(0.2f);

        if (_isFirstShow && !_buttonPlayShowed)
        {
            yield return StartCoroutine(ShowPlayButton());
            _buttonPlayShowed = true;
        }
        
        Restart();
    }

    public IEnumerator ShowPlayButton()
    {
        playButton.alpha = 0f;
        playButton.interactable = false;
        playButton.blocksRaycasts = false;

        // Анимация появления
        yield return playButton.DOFade(1f, 0.3f).WaitForCompletion();

        // Если тут ошибка — кнопка останется прозрачной, но видимой
        if (!_isTutorialRunning)
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
    }

    public IEnumerator FingerClickMove(Vector3 from, Vector3 to, Vector2Int ballPos)
    {
        Vector3 offset = new Vector3(-fingerXOffset, -fingerYOffset, 0f);
        Vector3 fromAdjusted = from + offset;
        Vector3 toAdjusted = to + offset;

        finger.position = fromAdjusted;
        finger.rotation = Quaternion.identity;

        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 0f;

        // Плавное появление
        yield return _fingerCanvasGroup.DOFade(1f, 0.3f).WaitForCompletion();
        if (!_isTutorialRunning) yield break;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(fromAdjusted.y - clickOffsetY, clickDuration)
            .SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!_isTutorialRunning) yield break;

        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(fromAdjusted.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!_isTutorialRunning) yield break;

        Ball yellow = lineBoard.GetBallByPos(ballPos);
        yellow.SetSelection(true);
        yield return new WaitForSeconds(0.2f);
        if (!_isTutorialRunning) yield break;

        // Перемещение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMove(toAdjusted, moveDuration).SetEase(Ease.InOutSine));
        yield return _currentFingerTween.WaitForCompletion();
        if (!_isTutorialRunning) yield break;

        // Клик вниз
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(toAdjusted.y - clickOffsetY, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(new Vector3(0, 0, clickRotateZ), clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!_isTutorialRunning) yield break;

        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(toAdjusted.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!_isTutorialRunning) yield break;

        // Плавное исчезновение
        yield return _fingerCanvasGroup.DOFade(0f, 0.3f).WaitForCompletion();
        if (!_isTutorialRunning) yield break;
    }

}