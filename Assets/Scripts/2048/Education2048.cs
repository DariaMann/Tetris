using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Education2048 : MonoBehaviour
{
    [Header("Offsets & Animation")]
    [SerializeField] private float clickOffsetY = 0.5f;           // Насколько палец "нажимает" вниз
    [SerializeField] private float clickRotateZ = 10f;          // Угол наклона при нажатии
    [SerializeField] private float clickDuration = 0.15f;        // Длительность нажатия
    [SerializeField] private float moveDuration = 0.6f;          // Длительность перемещения от A до B
    [SerializeField] private float fingerXOffset = -0.2f;          // Смещение пальца вверх, чтобы "кликал" верхушкой
    [SerializeField] private float fingerYOffset = 1f; 
    
    [SerializeField] private RectTransform finger;
    [SerializeField] private RectTransform leftPos;
    [SerializeField] private RectTransform rightPos;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private GameManager board;
    [SerializeField] private CanvasGroup finishEducationPanel;
    [SerializeField] private GameObject backButton;
    
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image plusImage;
    [SerializeField] private Image equalsImage;
    [SerializeField] private CanvasGroup futureTileImage;
    
    [SerializeField] private float waitingTime = 0.3f;
    [SerializeField] private float speedShowImage = 0.3f;
    
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    
    private CanvasGroup _fingerCanvasGroup;
    
    private Vector3 _startFingerPos;
    
    private Coroutine _tutorialCoroutine;
    private Sequence _currentFingerTween;
    private bool _isTutorialRunning;
    private bool _isStartShowFinish;
    private bool _isStartShowTwoStep;
    
    private bool _isFirstShow;
    private int _step;
    
    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && educationPanel.activeSelf)
        {
            Restart(); // <-- это должно запустить StartPlay, а значит и PlayFirstStep
        }

        if (pauseStatus)
        {
            StopTutorial(); // Добавь это, чтобы всё явно остановилось при паузе

            if (_isStartShowFinish)
            {
                ForceFinishEducation();
                return;
            }

            if (_isStartShowTwoStep)
            {
                ForceTwoStep();
                return;
            }     
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
        Restart();
    }
    
    public void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        ShowView(isFirstEducation);
        ShowEducation();
    }
    
    private void ShowView(bool isFirstEducation)
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

    private void ShowEducation()
    {
        educationPanel.SetActive(true);
        GameHelper.IsEdication = true;
        _fingerCanvasGroup = finger.gameObject.GetComponent<CanvasGroup>();
        _startFingerPos = finger.transform.position;
        
//        board.LoadEducation(GetFirstSaveData());

        _step = 0;
        StartPlay();
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
    
//    private SaveDataBlocks GetSecondSaveData()
//    {
//        SaveBlock block1 = new SaveBlock(blockOne, false);
//        SaveBlock block2 = new SaveBlock(blockOne, true);
//        SaveBlock block3 = new SaveBlock(blockOne, false);
//        List<SaveBlock> blocks = new List<SaveBlock>(){block1, block2, block3};
//        
//        List<SaveBlocksTile> blocksTiles = new List<SaveBlocksTile>();
//        int countTiles = 81;
//        while (countTiles > 0)
//        {
//            blocksTiles.Add(new SaveBlocksTile());
//            countTiles -= 1;
//        }
//
//        blocksTiles[4].IsFull = true;
//        blocksTiles[13].IsFull = true;
//        blocksTiles[22].IsFull = true;
//        blocksTiles[31].IsFull = true;
//        blocksTiles[49].IsFull = true;
//        blocksTiles[58].IsFull = true;
//        blocksTiles[67].IsFull = true;
//        blocksTiles[76].IsFull = true;
//        
//        SaveDataBlocks saveData = new SaveDataBlocks();
//
//        saveData.SaveBlocksTile = blocksTiles;
//        saveData.Blocks = blocks;
//
//        return saveData;
//    }
//    
//    private SaveDataBlocks GetThirdSaveData()
//    {
//        SaveBlock block1 = new SaveBlock(blockOne, false);
//        SaveBlock block2 = new SaveBlock(blockOne, true);
//        SaveBlock block3 = new SaveBlock(blockOne, false);
//        List<SaveBlock> blocks = new List<SaveBlock>(){block1, block2, block3};
//        
//        List<SaveBlocksTile> blocksTiles = new List<SaveBlocksTile>();
//        int countTiles = 81;
//        while (countTiles > 0)
//        {
//            blocksTiles.Add(new SaveBlocksTile());
//            countTiles -= 1;
//        }
//
//        blocksTiles[30].IsFull = true;
//        blocksTiles[31].IsFull = true;
//        blocksTiles[32].IsFull = true;
//        blocksTiles[39].IsFull = true;
//        blocksTiles[41].IsFull = true;
//        blocksTiles[48].IsFull = true;
//        blocksTiles[49].IsFull = true;
//        blocksTiles[50].IsFull = true;
//        
//        SaveDataBlocks saveData = new SaveDataBlocks();
//
//        saveData.SaveBlocksTile = blocksTiles;
//        saveData.Blocks = blocks;
//
//        return saveData;
//    }

    public void OnPlayClick()
    {
        HideEducation(true);
    }
    
    public void HideEducation(bool isDelay = false)
    {
        StopTutorial();
        HideFinishEducation();
        HideTwoStep();
        board.ResetAll();
        educationPanel.SetActive(false);
        if (isDelay)
        {
            StartCoroutine(DelayStopEducation());
        }
        else
        {
            GameHelper.IsEdication = false;
        }
    }  
    
    public IEnumerator DelayStopEducation()
    {
        yield return new WaitForSeconds(0.1f);
        GameHelper.IsEdication = false;
    }
    
    public void Restart()
    {
        _step = 0;
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
        board.EnableMoveDirection = move;
    }

    public void StartPlay()
    {
        StopTutorial();
        if (_step == 0)
        {
            board.LoadEducation(GetFirstSaveData());
            SetEnableMoveDirection(Vector2Int.zero);
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("2048.edutext1");
            }
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        }
        else if (_step == 1)
        {
            SetEnableMoveDirection(Vector2Int.zero);
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("2048.edutext2");
            }
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayTwoStep());
        }
//        else if (_step == 2)
//        {
//            SaveDataBlocks saveData = GetThirdSaveData();
//            board.LoadEducation(saveData);
//            board.EnableTile = board.Tiles[40];
//            _isTutorialRunning = true;
//            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
//        } 
        else if (_step == 2)
        {
            _isStartShowFinish = true;
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
        SetEnableMoveDirection(Vector2Int.zero);
        yield return StartCoroutine(PlayFirstStep(false));
    }
    
    private IEnumerator PlayFirstStep(bool toLeft = true)
    {
        yield return new WaitForSeconds(0.1f);

        // первый клик и перемещение пальца
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

        yield return StartCoroutine(FingerClickMove(start1, end1));
        
        if (toLeft)
        {
            SetEnableMoveDirection(Vector2Int.left);
        }
        else
        {
            SetEnableMoveDirection(Vector2Int.right);
        }

        yield return new WaitForSeconds(0.2f);

        _isTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayFirstStep(toLeft));
    }

    public void StopTutorial()
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

        DOTween.Kill(_fingerCanvasGroup); // Чтобы остановить DOFade
        
        finger.transform.position = _startFingerPos;
        finger.rotation = Quaternion.identity;
        _fingerCanvasGroup.alpha = 0f;
    }
    
    private void ForcePlayButtonVisible()
    {
        // Сделать кнопку видимой, если анимация не успела завершиться
//        playButton.alpha = 1f;
//        playButton.interactable = true;
//        playButton.blocksRaycasts = true;
    }   
    
    private void ForceFinishEducation()
    {
        // Сделать кнопку видимой, если анимация не успела завершиться
        finishEducationPanel.alpha = 1f;
        finishEducationPanel.interactable = true;
        finishEducationPanel.blocksRaycasts = true;
        _isStartShowFinish = false;
    }
    
    public IEnumerator FingerClickMove(Vector3 from, Vector3 to)
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

        
        
        // Перемещение
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMove(toAdjusted, moveDuration).SetEase(Ease.InOutSine));
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