using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Education2048 : MonoBehaviour
{
    [SerializeField] private EducationUi educationUi;
    
    [SerializeField] private EducationFinger finger;
    [SerializeField] private RectTransform leftPos;
    [SerializeField] private RectTransform rightPos;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private CanvasGroup finishEducationPanel;
    [SerializeField] private GameObject backButton;
    
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image plusImage;
    [SerializeField] private Image equalsImage;
    [SerializeField] private CanvasGroup futureTileImage;
    
    [SerializeField] private float waitingTime = 0.3f;
    [SerializeField] private float speedShowImage = 0.3f;
    
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

    private Coroutine _tutorialCoroutine;
    private bool _isFirstShow;
    private bool _isStartShowFinish;
    private bool _isStartShowTwoStep;
    private bool _isReshow;
    
    private int _step;
    
    public Vector2Int EnableMoveDirection { get; set; }
    
    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameHelper.IsEdication && !GameHelper.IsUIEdication)
        {
            if (_isStartShowFinish)
            {
                StopTutorial();
                ForceFinishEducation();
                return;
            }
            
            if (_step == 2 && !_isStartShowFinish)
            {
                return;
            }

            _isReshow = true;
            Restart(_step);
        }
    }

    public void HideFinishEducation()
    {
        finishEducationPanel.alpha = 0f;
        finishEducationPanel.interactable = false;
        finishEducationPanel.blocksRaycasts = false;
    }
    
    public IEnumerator ShowFinishEducation()
    {
        finishEducationPanel.alpha = 0f;
        finishEducationPanel.interactable = false;
        finishEducationPanel.blocksRaycasts = false;

        // Анимация появления
        yield return finishEducationPanel.DOFade(1f, 0.3f).WaitForCompletion();

        ForceFinishEducation();

        finishEducationPanel.interactable = true;
        finishEducationPanel.blocksRaycasts = true;
    }
    
    public IEnumerator ShowTwoStep()
    {
        _isStartShowTwoStep = true;
        // Устанавливаем полностью прозрачный цвет
        var color = arrowImage.color;
        color.a = 0f;
        arrowImage.color = color;
        
        var color1 = plusImage.color;
        color1.a = 0f;
        plusImage.color = color1;
        
        var color2 = equalsImage.color;
        color2.a = 0f;
        equalsImage.color = color2;
        
        futureTileImage.alpha = 0f;

        // Анимация появления
        yield return arrowImage
            .DOFade(1f, speedShowImage) // Requires DOTween's DOFade for Image
            .WaitForCompletion();
        
        // Анимация появления
        yield return plusImage
            .DOFade(1f, speedShowImage) // Requires DOTween's DOFade for Image
            .WaitForCompletion();
        
        // Анимация появления
        yield return equalsImage
            .DOFade(1f, speedShowImage) // Requires DOTween's DOFade for Image
            .WaitForCompletion();
        
        // Анимация появления
        yield return futureTileImage.
            DOFade(0.5f, speedShowImage/2).
            WaitForCompletion();
        
        ForceTwoStep();
    }
    
    public void HideTwoStep()
    {
        var color = arrowImage.color;
        color.a = 0f;
        arrowImage.color = color;
        
        var color1 = plusImage.color;
        color1.a = 0f;
        plusImage.color = color1;
        
        var color2 = equalsImage.color;
        color2.a = 0f;
        equalsImage.color = color2;
        
        futureTileImage.alpha = 0f;
    }
    
    private void ForceTwoStep()
    {
        var color = arrowImage.color;
        color.a = 1f;
        arrowImage.color = color;
        
        var color1 = plusImage.color;
        color1.a = 1f;
        plusImage.color = color1;
        
        var color2 = equalsImage.color;
        color2.a = 1f;
        equalsImage.color = color2;
        
        futureTileImage.alpha = 0.5f;
        
        _isStartShowTwoStep = false;
    }
    
    public void RepeatEducation()
    {
        HideFinishEducation();
        Restart(0);
    }
    
    public void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        GameHelper.IsEdication = true;
        ShowView(isFirstEducation);
        ShowEducation();
    }
    
    public void ShowView(bool isFirstEducation)
    {
        if (isFirstEducation)
        {
            backButton.SetActive(false);
        }
        else
        {
            backButton.SetActive(true);
        }
    }

    public void ShowEducation()
    {
        educationPanel.SetActive(true);
        GameHelper.IsEdication = true;

        _step = 0;
        StartPlay();
    }
    
    public void HideEducation()
    {
        StopTutorial();
        HideFinishEducation();
        HideTwoStep();
        GameManager2048.Instance.ResetAllBoardEducation();
        educationPanel.SetActive(false);
        GameHelper.IsEdication = false;
        
        if (_isFirstShow)
        {
            educationUi.ShowEducation();
        }
    }
    
    public void ShowBanner(bool isShow)
    {
        if (_isFirstShow)
        {
            AppodealManager.Instance.HideBottomBanner();
            return;
        }
        if (isShow)
        {
            AppodealManager.Instance.ShowBottomBanner();
        }
        else
        {
            AppodealManager.Instance.HideBottomBanner();
        }
    }

    private SaveData2048 GetFirstSaveData()
    {
        List<SaveTile2024> tiles = new List<SaveTile2024>();
        SaveTile2024 tile1 = new SaveTile2024(1, 3, 1);
        SaveTile2024 tile2 = new SaveTile2024(0, 2, 2);
        SaveTile2024 tile3 = new SaveTile2024(0, 3, 3);
        
        tiles.Add(tile1);
        tiles.Add(tile2);
        tiles.Add(tile3);
        
        SaveData2048 saveData = new SaveData2048();

        saveData.SaveTiles = tiles;

        return saveData;
    } 
    
    private SaveData2048 GetSecondSaveData()
    {
        List<SaveTile2024> tiles = new List<SaveTile2024>();
        SaveTile2024 tile1 = new SaveTile2024(1, 0, 1);
        SaveTile2024 tile2 = new SaveTile2024(0, 0, 2);
        SaveTile2024 tile4 = new SaveTile2024(0, 2, 2);
        SaveTile2024 tile3 = new SaveTile2024(0, 0, 3);
        
        tiles.Add(tile1);
        tiles.Add(tile2);
        tiles.Add(tile4);
        tiles.Add(tile3);
        
        SaveData2048 saveData = new SaveData2048();

        saveData.SaveTiles = tiles;

        return saveData;
    }

    public bool IsCreateNewTile()
    {
        if (_step == 0)
        {
            return true;
        }
        return false;
    }
    
    public void Restart(int step)
    {
        _step = step;
        StartPlay();
    }

    public void ChangeStepAfterTouch()
    {
        if (_step == 1)
        {
            StopTutorial();
            _tutorialCoroutine = StartCoroutine(WaitForFinal());
            return;
        }

        ChangeStep();
    }

    public void ChangeStep()
    {
        _step += 1;
        if (_step > 2)
        {
            _step = 0;
        }

        StartPlay();
    }

    public void SetEnableMoveDirection(Vector2Int move)
    {
        EnableMoveDirection = move;
    }

    public void StartPlay()
    {
        StopTutorial();
        if (_step == 0)
        {
            GameManager2048.Instance.LoadEducation(GetFirstSaveData());
            SetEnableMoveDirection(Vector2Int.zero);
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("2048.edutext1");
            }
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        }
        else if (_step == 1)
        {
            if (_isReshow)
            {
                GameManager2048.Instance.LoadEducation(GetSecondSaveData());
                _isReshow = false;
            }
            SetEnableMoveDirection(Vector2Int.zero);
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("2048.edutext2");
            }
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayTwoStep());
        }
        else if (_step == 2)
        {
            _isStartShowFinish = true;
            finger.IsTutorialRunning = false;
            HideTwoStep();
            _tutorialCoroutine = StartCoroutine(ShowFinishEducation());
        }
    }

    private IEnumerator WaitForFinal()
    {
        yield return new WaitForSeconds(waitingTime);
        ChangeStep();
    } 
    
    private IEnumerator PlayTwoStep()
    {
        yield return StartCoroutine(ShowTwoStep());
        yield return StartCoroutine(PlayFirstStep(false));
    }

    private IEnumerator PlayFirstStep(bool toLeft = true)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 start1 = Vector3.zero;
            Vector3 end1 = Vector3.zero;

            if (toLeft)
            {
                start1 = rightPos.position;
                end1 = leftPos.position;
            }
            else
            {
                start1 = leftPos.position;
                end1 = rightPos.position;
            }

            if (toLeft)
            {
                SetEnableMoveDirection(Vector2Int.left);
            }
            else
            {
                SetEnableMoveDirection(Vector2Int.right);
            }

            yield return StartCoroutine(finger.PlayFingerClickMove(start1, end1));

            yield return new WaitForSeconds(0.2f);

        }
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

    private void ForceFinishEducation()
    {
        // Сделать кнопку видимой, если анимация не успела завершиться
        finishEducationPanel.alpha = 1f;
        finishEducationPanel.interactable = true;
        finishEducationPanel.blocksRaycasts = true;
        _isStartShowFinish = false;
    }
}