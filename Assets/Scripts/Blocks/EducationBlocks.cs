using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EducationBlocks : MonoBehaviour
{
    [Header("Offsets & Animation")]
    [SerializeField] private float clickOffsetY = 0.5f;           // Насколько палец "нажимает" вниз
    [SerializeField] private float clickRotateZ = 10f;          // Угол наклона при нажатии
    [SerializeField] private float clickDuration = 0.15f;        // Длительность нажатия
    [SerializeField] private float moveDuration = 0.6f;          // Длительность перемещения от A до B
    [SerializeField] private float fingerXOffset = -0.2f;          // Смещение пальца вверх, чтобы "кликал" верхушкой
    [SerializeField] private float fingerYOffset = 1f; 
    
    [SerializeField] private RectTransform finger;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private BlocksBoard board;
    [SerializeField] private CanvasGroup finishEducationPanel;
    [SerializeField] private GameObject backButton;
    
    [SerializeField] private BlockShape blockOne;
    
    private CanvasGroup _fingerCanvasGroup;
    
    private Vector3 _startFingerPos;
    
    private Coroutine _tutorialCoroutine;
    private Sequence _currentFingerTween;
    private bool _isTutorialRunning;
    private bool _isStartShowFinish;
    
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
            if (_isStartShowFinish)
            {
                // Завершить обучение вручную, если это был последний шаг
                ForceFinishEducation();
            }
            else
            {
                Restart();
            }
        }

        if (pauseStatus && _isStartShowFinish)
        {
            // Если свернули на шаге 3 — сразу показать финал
            ForceFinishEducation();
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

        // Если тут ошибка — кнопка останется прозрачной, но видимой
        if (!_isStartShowFinish)
        {
            ForceFinishEducation();
            yield break;
        }

        finishEducationPanel.interactable = true;
        finishEducationPanel.blocksRaycasts = true;
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
        _fingerCanvasGroup = finger.gameObject.GetComponent<CanvasGroup>();
        _startFingerPos = finger.transform.position;
        
        board.LoadStartEducation();

        _step = 0;
        StartPlay();
    }

    private SaveDataBlocks GetFirstSaveData()
    {
        SaveBlock block1 = new SaveBlock(blockOne, false);
        SaveBlock block2 = new SaveBlock(blockOne, true);
        SaveBlock block3 = new SaveBlock(blockOne, false);
        List<SaveBlock> blocks = new List<SaveBlock>(){block1, block2, block3};
        
        List<SaveBlocksTile> blocksTiles = new List<SaveBlocksTile>();
        int countTiles = 81;
        while (countTiles > 0)
        {
            blocksTiles.Add(new SaveBlocksTile());
            countTiles -= 1;
        }

        blocksTiles[36].IsFull = true;
        blocksTiles[37].IsFull = true;
        blocksTiles[38].IsFull = true;
        blocksTiles[39].IsFull = true;
        blocksTiles[41].IsFull = true;
        blocksTiles[42].IsFull = true;
        blocksTiles[43].IsFull = true;
        blocksTiles[44].IsFull = true;
        
        SaveDataBlocks saveData = new SaveDataBlocks();

        saveData.SaveBlocksTile = blocksTiles;
        saveData.Blocks = blocks;

        return saveData;
    }
    
    private SaveDataBlocks GetSecondSaveData()
    {
        SaveBlock block1 = new SaveBlock(blockOne, false);
        SaveBlock block2 = new SaveBlock(blockOne, true);
        SaveBlock block3 = new SaveBlock(blockOne, false);
        List<SaveBlock> blocks = new List<SaveBlock>(){block1, block2, block3};
        
        List<SaveBlocksTile> blocksTiles = new List<SaveBlocksTile>();
        int countTiles = 81;
        while (countTiles > 0)
        {
            blocksTiles.Add(new SaveBlocksTile());
            countTiles -= 1;
        }

        blocksTiles[4].IsFull = true;
        blocksTiles[13].IsFull = true;
        blocksTiles[22].IsFull = true;
        blocksTiles[31].IsFull = true;
        blocksTiles[49].IsFull = true;
        blocksTiles[58].IsFull = true;
        blocksTiles[67].IsFull = true;
        blocksTiles[76].IsFull = true;
        
        SaveDataBlocks saveData = new SaveDataBlocks();

        saveData.SaveBlocksTile = blocksTiles;
        saveData.Blocks = blocks;

        return saveData;
    }
    
    private SaveDataBlocks GetThirdSaveData()
    {
        SaveBlock block1 = new SaveBlock(blockOne, false);
        SaveBlock block2 = new SaveBlock(blockOne, true);
        SaveBlock block3 = new SaveBlock(blockOne, false);
        List<SaveBlock> blocks = new List<SaveBlock>(){block1, block2, block3};
        
        List<SaveBlocksTile> blocksTiles = new List<SaveBlocksTile>();
        int countTiles = 81;
        while (countTiles > 0)
        {
            blocksTiles.Add(new SaveBlocksTile());
            countTiles -= 1;
        }

        blocksTiles[30].IsFull = true;
        blocksTiles[31].IsFull = true;
        blocksTiles[32].IsFull = true;
        blocksTiles[39].IsFull = true;
        blocksTiles[41].IsFull = true;
        blocksTiles[48].IsFull = true;
        blocksTiles[49].IsFull = true;
        blocksTiles[50].IsFull = true;
        
        SaveDataBlocks saveData = new SaveDataBlocks();

        saveData.SaveBlocksTile = blocksTiles;
        saveData.Blocks = blocks;

        return saveData;
    }
    
    public void HideEducation()
    {
        StopTutorial();
        
        board.ResetAllAll();
        educationPanel.SetActive(false);
    }  
    
    public void Restart()
    {
        _step = 0;
        StartPlay();
    }

    public void ChangeStep()
    {
        _step += 1;
        if (_step > 3)
        {
            _step = 0;
        }

        StartPlay();
    }

    public void StartPlay()
    {
        StopTutorial();
        board.ResetGrid();
        if (_step == 0)
        {
            SaveDataBlocks saveData = GetFirstSaveData();
            board.LoadEducation(saveData);
            board.EnableTile = board.Tiles[40];
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        }
        else if (_step == 1)
        {
            SaveDataBlocks saveData = GetSecondSaveData();
            board.LoadEducation(saveData);
            board.EnableTile = board.Tiles[40];
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        }
        else if (_step == 2)
        {
            SaveDataBlocks saveData = GetThirdSaveData();
            board.LoadEducation(saveData);
            board.EnableTile = board.Tiles[40];
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        } 
        else if (_step == 3)
        {
            _isStartShowFinish = true;
            _tutorialCoroutine = StartCoroutine(ShowFinishEducation());
        }
    }
    
    private IEnumerator PlayFirstStep()
    {
        yield return new WaitForSeconds(0.1f);

        // первый клик и перемещение пальца
        Vector3 start1 = board.Blocks[1].transform.position;
        Vector3 end1 = board.Tiles[40].transform.position;

        yield return StartCoroutine(FingerClickMove(start1, end1));

        yield return new WaitForSeconds(0.2f);

        _isTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayFirstStep());
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