using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EducationLines98 : Education
{
    [SerializeField] private EducationFinger finger;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private LineBoard lineBoard;
    
    [SerializeField] private CanvasGroup playButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject topText;
    [SerializeField] private GameObject topBoardText;
    [SerializeField] private GameObject bottomText;
    
    private SquareUIGrid _squareUi;
    private RectTransform _rectBoard;

    private Vector2Int _bluePos;
    private Vector2Int _yellowPos;
    private Vector2Int _redPos;
    
    private Vector2Int _futureBluePos;
    private Vector2Int _futureYellowPos;
    private Vector2Int _futureRedPos;
    
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
            ForcePlayButtonVisible();
        }
    }
    
    public override void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        GameHelper.IsEdication = true;
        ShowView(isFirstEducation);
        ShowEducation();
    }

    public override void ShowEducation()
    {
        educationPanel.SetActive(true);

        _futureBluePos = new Vector2Int(1,0);
        _futureYellowPos = new Vector2Int(3,2);
        _futureRedPos = new Vector2Int(8,3);
        
        _bluePos = new Vector2Int(3,1);
        _yellowPos = new Vector2Int(2,8);
        _redPos = new Vector2Int(7,7);

        SaveDataLines98 saveData = GetSaveData();
        
        GameManagerLines98.Instance.LoadEducation(saveData);

        finger.IsTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayTutorial());
    }
    
    public override void ShowView(bool isFirstEducation)
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
    
    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        StopTutorial();
        
        GameManagerLines98.Instance.ResetAllBoardEducation();
        educationPanel.SetActive(false);
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
                finger.ChangeByOrientation(new Vector2(80f, 80f), 1f);
            }
            else
            {
                finger.ChangeByOrientation(new Vector2(60f, 60f), 0.5f);
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
                finger.ChangeByOrientation(new Vector2(50f, 50f), 1f);
                
                Vector2 pos = _rectBoard.anchoredPosition;
                pos.y = 0f;
                _rectBoard.anchoredPosition = pos;
            }
            else
            {
                _squareUi.Padding = 113;
                finger.ChangeByOrientation(new Vector2(70f, 70f), 1f);
                
                Vector2 pos = _rectBoard.anchoredPosition;
                pos.y = 50f;
                _rectBoard.anchoredPosition = pos;
            }
        }

        ShowView(_isFirstShow);
    }

    public override void StopTutorial()
    {
        finger.IsTutorialRunning = false;

        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        finger.Stop();
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

    public override void Restart(int step)
    {
        StopTutorial();

        SaveDataLines98 saveData = GetSaveData();
        
        GameManagerLines98.Instance.ReloadEducation(saveData);
        
        finger.IsTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayTutorial());
    }

    private IEnumerator PlayTutorial()
    {
        yield return new WaitForSeconds(0.1f);

        // первый клик и перемещение пальца
        Vector3 start1 = lineBoard.GetLineTileByPos(_bluePos).transform.position;
        Vector3 end1 = lineBoard.GetLineTileByPos(_futureBluePos).transform.position;

        yield return StartCoroutine(finger.PlayFingerClickMove(start1, end1, _bluePos, lineBoard));

        yield return StartCoroutine(lineBoard.GetLineTileByPos(_futureBluePos).PlayerMoveCoroutine());

        // второй клик и перемещение пальца
        Vector3 start2 = lineBoard.GetLineTileByPos(_yellowPos).transform.position;
        Vector3 end2 = lineBoard.GetLineTileByPos(_futureYellowPos).transform.position;
        yield return StartCoroutine(finger.PlayFingerClickMove(start2, end2, _yellowPos, lineBoard));

        yield return StartCoroutine(lineBoard.GetLineTileByPos(_futureYellowPos).PlayerMoveCoroutine());

        // третий клик и перемещение пальца
        Vector3 start3 = lineBoard.GetLineTileByPos(_redPos).transform.position;
        Vector3 end3 = lineBoard.GetLineTileByPos(_futureRedPos).transform.position;
        yield return StartCoroutine(finger.PlayFingerClickMove(start3, end3, _redPos, lineBoard));

        yield return StartCoroutine(lineBoard.GetLineTileByPos(_futureRedPos).PlayerMoveCoroutine());

        yield return new WaitForSeconds(0.2f);

        if (_isFirstShow && !_buttonPlayShowed)
        {
            yield return StartCoroutine(ShowPlayButton());
            _buttonPlayShowed = true;
        }
        
        Restart(0);
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