using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EducationTetris : MonoBehaviour
{
    [Header("Offsets & Animation")]
    [SerializeField] private float clickOffsetY = 0.5f;           // Насколько палец "нажимает" вниз
    [SerializeField] private float clickRotateZ = 10f;          // Угол наклона при нажатии
    [SerializeField] private float clickDuration = 0.15f;        // Длительность нажатия
    [SerializeField] private float moveDuration = 0.6f;          // Длительность перемещения от A до B
    [SerializeField] private float fingerXOffset = -0.2f;          // Смещение пальца вверх, чтобы "кликал" верхушкой
    [SerializeField] private float fingerYOffset = 1f; 
    
    [SerializeField] private GameObject leftText;
    [SerializeField] private GameObject rightText;
    
    [SerializeField] private GameObject leftPos;
    [SerializeField] private GameObject rightPos;
    [SerializeField] private GameObject upPos;
    [SerializeField] private GameObject downPos;
    
    [SerializeField] private GameManagerTetris gameManager;
    [SerializeField] private Board board;
    [SerializeField] private Ghost ghost;
    
    [SerializeField] private List<GameObject> objForDeactivate = new List<GameObject>();
    
    [SerializeField] private RectTransform finger;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private CanvasGroup finishEducationPanel;
    [SerializeField] private GameObject backButton;
    
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    
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
        if (!pauseStatus && GameHelper.IsEdication)
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
        yield return new WaitForSeconds(0.5f);

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
        GameHelper.IsEdication = true;
        _isFirstShow = isFirstEducation;

        foreach (var obj in objForDeactivate)
        {
            obj.SetActive(false);
        }
        
        board.gameObject.SetActive(true);
        ghost.gameObject.SetActive(true);
        
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
        gameManager.EducationIsOver = false;
        _fingerCanvasGroup = finger.gameObject.GetComponent<CanvasGroup>();
        _startFingerPos = finger.transform.position;
        
        _step = 0;
        StartPlay();
    }

    private SaveDataTetris GetFirstSaveData()
    {
        List<SaveTetramino> tetraminos = new List<SaveTetramino>();
        
        SaveTetramino tetramino = new SaveTetramino(Tetromino.O, -5, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -4, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -3, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -2, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -1, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, 0, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, 1, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, 2, -10, 0);
        
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -5, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -4, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -3, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -2, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, -1, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, 0, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, 1, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.O, 2, -9, 0);
        tetraminos.Add(tetramino);
        
        SaveDataTetris saveData = new SaveDataTetris();

        saveData.CurrentTetromino = Tetromino.O;
        saveData.NextTetromino = Tetromino.J;
        saveData.SaveTetrominos = tetraminos;

        return saveData;
    }
    
    private SaveDataTetris GetSecondSaveData()
    {
        List<SaveTetramino> tetraminos = new List<SaveTetramino>();
        
        SaveTetramino tetramino = new SaveTetramino(Tetromino.J, -4, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -3, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -2, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -1, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 0, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 1, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 2, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 3, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 4, -10, 0);
        tetraminos.Add(tetramino);
        
        tetramino = new SaveTetramino(Tetromino.J, -4, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -3, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -2, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -1, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 0, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 1, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 2, -9, 0);
        tetraminos.Add(tetramino);   
        tetramino = new SaveTetramino(Tetromino.J, 3, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 4, -9, 0);
        tetraminos.Add(tetramino);
        
        tetramino = new SaveTetramino(Tetromino.J, -3, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -2, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, -1, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 0, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 1, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 2, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 3, -8, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.J, 4, -8, 0);
        tetraminos.Add(tetramino);
        
        SaveDataTetris saveData = new SaveDataTetris();

        saveData.CurrentTetromino = Tetromino.J;
        saveData.NextTetromino = Tetromino.O;
        saveData.SaveTetrominos = tetraminos;

        return saveData;
    }
    
    private SaveDataTetris GetThirdSaveData()
    {
        List<SaveTetramino> tetraminos = new List<SaveTetramino>();
        
        SaveTetramino tetramino = new SaveTetramino(Tetromino.T, -5, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -4, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -3, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -2, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -1, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, 0, -10, 0);
        tetraminos.Add(tetramino);
//        tetramino = new SaveTetramino(Tetromino.T, 1, -10, 0);
//        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, 2, -10, 0);
        tetraminos.Add(tetramino);     
        tetramino = new SaveTetramino(Tetromino.T, 3, -10, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, 4, -10, 0);
        tetraminos.Add(tetramino);
        
        
        tetramino = new SaveTetramino(Tetromino.T, -5, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -4, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -3, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -2, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, -1, -9, 0);
        tetraminos.Add(tetramino);
//        tetramino = new SaveTetramino(Tetromino.T, 0, -9, 0);
//        tetraminos.Add(tetramino);
//        tetramino = new SaveTetramino(Tetromino.T, 1, -9, 0);
//        tetraminos.Add(tetramino);
//        tetramino = new SaveTetramino(Tetromino.T, 2, -9, 0);
//        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, 3, -9, 0);
        tetraminos.Add(tetramino);
        tetramino = new SaveTetramino(Tetromino.T, 4, -9, 0);
        tetraminos.Add(tetramino);
        
        SaveDataTetris saveData = new SaveDataTetris();

        saveData.CurrentTetromino = Tetromino.T;
        saveData.NextTetromino = Tetromino.O;
        saveData.SaveTetrominos = tetraminos;

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
    
    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        StopTutorial();
        
        gameManager.ResetAllBoardEducation();
        
        foreach (var obj in objForDeactivate)
        {
            obj.SetActive(true);
        }
        
        board.gameObject.SetActive(false);
        ghost.gameObject.SetActive(false);
        backButton.SetActive(false);
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
        if (_step > 5)
        {
            _step = 0;
        }

        StartPlay();
    }

    public void StartPlay()
    {
        StopTutorial();
        if (_step == 0)
        {
            gameManager.EducationIsOver = false;
            gameManager.ResetAllBoardEducation();
            SaveDataTetris saveData = GetFirstSaveData();
            gameManager.LoadEducation(saveData);
            gameManager.EnableDirection = Vector2Int.right;
            gameManager.EnableFinishPosition = new Vector3Int(3,8,0);
            gameManager.EnableCanRotate = false;
            gameManager.EnableCanHardDrop = false;
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("Tetris.edutext1");
            }
            
            if (GameHelper.IsTablet())
            {
                leftText.SetActive(true);
                rightText.SetActive(false);
            }
            else
            {
                leftText.SetActive(false);
                rightText.SetActive(false);
            }
            
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(gameManager.EnableDirection));
        }
        else if (_step == 1)
        {
            gameManager.EducationIsOver = false;
            gameManager.EnableDirection = Vector2Int.down;
            gameManager.EnableFinishPosition = new Vector3Int(3,-10,0);
            gameManager.EnableCanRotate = false;
            gameManager.EnableCanHardDrop = false;
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("Tetris.edutext4");
            }
            
            if (GameHelper.IsTablet())
            {
                leftText.SetActive(true);
                rightText.SetActive(false);
            }
            else
            {
                leftText.SetActive(false);
                rightText.SetActive(false);
            }
            
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(gameManager.EnableDirection));
        }
        else if (_step == 2)
        {
            gameManager.EducationIsOver = false;
            gameManager.ResetAllBoardEducation();
            SaveDataTetris saveData = GetSecondSaveData();
            gameManager.LoadEducation(saveData);
            gameManager.EnableDirection = Vector2Int.zero;
            gameManager.EnableFinishPosition = new Vector3Int(0,0,0);
            gameManager.EnableCanRotate = true;
            gameManager.EnableCanHardDrop = false;
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("Tetris.edutext3");
            }

            if (GameHelper.IsTablet())
            {
                leftText.SetActive(false);
                rightText.SetActive(true);
            }
            else
            {
                leftText.SetActive(false);
                rightText.SetActive(false);
            }

            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayRotateStep(true));
        }   
        else if (_step == 3)
        {
            gameManager.EducationIsOver = false;
            gameManager.EnableDirection = Vector2Int.left;
            gameManager.EnableFinishPosition = new Vector3Int(-5,8,0);
            gameManager.EnableCanRotate = false;
            gameManager.EnableCanHardDrop = false;
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("Tetris.edutext2");
            }
            
            if (GameHelper.IsTablet())
            {
                leftText.SetActive(false);
                rightText.SetActive(true);
            }
            else
            {
                leftText.SetActive(false);
                rightText.SetActive(false);
            }
            
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(gameManager.EnableDirection));
        }    
        else if (_step == 4)
        {
            gameManager.EducationIsOver = false;
            gameManager.EnableDirection = Vector2Int.down;
            gameManager.EnableFinishPosition = new Vector3Int(-5,-9,0);
            gameManager.EnableCanRotate = false;
            gameManager.EnableCanHardDrop = true;
            foreach (var text in texts)
            {
                text.text = LocalizationManager.Localize("Tetris.edutext5");
            }
            
            if (GameHelper.IsTablet())
            {
                leftText.SetActive(false);
                rightText.SetActive(true);
            }
            else
            {
                leftText.SetActive(false);
                rightText.SetActive(false);
            }
            
            _isTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(gameManager.EnableDirection, true));
        }
        else if (_step == 5)
        {
            leftText.SetActive(false);
            rightText.SetActive(false);
            gameManager.EducationIsOver = true;
            gameManager.ResetAllBoardEducation();
            _isStartShowFinish = true;
            _tutorialCoroutine = StartCoroutine(ShowFinishEducation());
        }
    }
    
    private IEnumerator PlayMoveStep(Vector2Int dir, bool speed = false)
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

        yield return StartCoroutine(FingerClickMove(start1, end1, speed));

        yield return new WaitForSeconds(0.2f);

        _isTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayMoveStep(dir, speed));
    }  
    
    private IEnumerator PlayRotateStep(bool isFirstShow = false)
    {
        yield return new WaitForSeconds(0.1f);
        
        if (isFirstShow)
        {
            yield return StartCoroutine(ShowFinger());
        }

        yield return StartCoroutine(FingerClickRotate());

        yield return new WaitForSeconds(0.2f);

        _isTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayRotateStep());
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
        if (_fingerCanvasGroup != null)
        {
            _fingerCanvasGroup.alpha = 0f;
        }
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
    
    public IEnumerator ShowFinger()
    {
        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 0f;

        // Плавное появление
        yield return _fingerCanvasGroup.DOFade(1f, 0.3f).WaitForCompletion();
        if (!_isTutorialRunning) yield break;
    }
    
    public IEnumerator HideFinger()
    {
        // Сначала палец невидим
        _fingerCanvasGroup.alpha = 1f;

        // Плавное исчезновение
        yield return _fingerCanvasGroup.DOFade(0f, 0.3f).WaitForCompletion();
        if (!_isTutorialRunning) yield break;
    }

    public IEnumerator FingerClickRotate()
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
        if (!_isTutorialRunning) yield break;
        
        // Клик вверх
        _currentFingerTween = DOTween.Sequence();
        _currentFingerTween.Append(finger.DOMoveY(_startFingerPos.y, clickDuration).SetEase(Ease.InOutQuad));
        _currentFingerTween.Join(finger.DORotate(Vector3.zero, clickDuration));
        yield return _currentFingerTween.WaitForCompletion();
        if (!_isTutorialRunning) yield break;
    }  
    
    public IEnumerator FingerClickMove(Vector3 from, Vector3 to, bool speed)
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
        _currentFingerTween.Append(finger.DOMove(toAdjusted, fingerSpeed).SetEase(Ease.InOutSine));
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