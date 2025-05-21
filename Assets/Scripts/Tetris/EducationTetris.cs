using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EducationTetris : MonoBehaviour
{
    [SerializeField] private EducationUi educationUi;
    
    [SerializeField] private GameObject leftText;
    [SerializeField] private GameObject rightText;
    
    [SerializeField] private GameObject leftPos;
    [SerializeField] private GameObject rightPos;
    [SerializeField] private GameObject upPos;
    [SerializeField] private GameObject downPos;
    
    [SerializeField] private Board board;
    [SerializeField] private Ghost ghost;
    
    [SerializeField] private List<GameObject> objForDeactivate = new List<GameObject>();
    
    [SerializeField] private EducationFinger finger;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private CanvasGroup finishEducationPanel;
    [SerializeField] private GameObject backButton;
    
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    
    private Coroutine _tutorialCoroutine;
    private bool _isStartShowFinish;
    
    private bool _isFirstShow;
    private int _step;
    
    public Vector3Int EnableFinishPosition { get; private set; }
    
    public Vector2Int EnableDirection { get; private set; }
    
    public bool EnableCanRotate { get; private set; }
    
    public bool EnableCanHardDrop { get; private set; }
    
    public bool EducationIsOver { get; private set; } = false;
    
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
                StopTutorial();
                ForceFinishEducation();
                return;
            }

            if (_step == 5 && !_isStartShowFinish)
            {
                return;
            }
            
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
        yield return new WaitForSeconds(0.2f);

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
        Restart(0);
    }
    
    public void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        GameHelper.IsEdication = true;

        foreach (var obj in objForDeactivate)
        {
            obj.SetActive(false);
        }
        
        board.gameObject.SetActive(true);
        ghost.gameObject.SetActive(true);
        
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
        EducationIsOver = false;
        
        _step = 0;
        StartPlay();
    }
    
    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        StopTutorial();
        HideFinishEducation();
        
        GameManagerTetris.Instance.ResetAllBoardEducation();
        
        foreach (var obj in objForDeactivate)
        {
            obj.SetActive(true);
        }

        board.gameObject.SetActive(false);
        ghost.gameObject.SetActive(false);
        backButton.SetActive(false);
        educationPanel.SetActive(false);

        if (_isFirstShow)
        {
            educationUi.ShowEducation();
        }
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

    public void Restart(int step)
    {
        _step = step;
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
            EducationIsOver = false;
            GameManagerTetris.Instance.ResetAllBoardEducation();
            SaveDataTetris saveData = GetFirstSaveData();
            GameManagerTetris.Instance.LoadEducation(saveData);
            EnableDirection = Vector2Int.right;
            EnableFinishPosition = new Vector3Int(3,8,0);
            EnableCanRotate = false;
            EnableCanHardDrop = false;
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
            
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(EnableDirection));
        }
        else if (_step == 1)
        {
            EducationIsOver = false;
            EnableDirection = Vector2Int.down;
            EnableFinishPosition = new Vector3Int(3,-10,0);
            EnableCanRotate = false;
            EnableCanHardDrop = false;
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
            
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(EnableDirection));
        }
        else if (_step == 2)
        {
            EducationIsOver = false;
            GameManagerTetris.Instance.ResetAllBoardEducation();
            SaveDataTetris saveData = GetSecondSaveData();
            GameManagerTetris.Instance.LoadEducation(saveData);
            EnableDirection = Vector2Int.zero;
            EnableFinishPosition = new Vector3Int(0,0,0);
            EnableCanRotate = true;
            EnableCanHardDrop = false;
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

            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayRotateStep(true));
        }   
        else if (_step == 3)
        {
            EducationIsOver = false;
            EnableDirection = Vector2Int.left;
            EnableFinishPosition = new Vector3Int(-5,8,0);
            EnableCanRotate = false;
            EnableCanHardDrop = false;
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
            
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(EnableDirection));
        }    
        else if (_step == 4)
        {
            EducationIsOver = false;
            EnableDirection = Vector2Int.down;
            EnableFinishPosition = new Vector3Int(-5,-9,0);
            EnableCanRotate = false;
            EnableCanHardDrop = true;
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
            
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayMoveStep(EnableDirection, true));
        }
        else if (_step == 5)
        {
            leftText.SetActive(false);
            rightText.SetActive(false);
            EducationIsOver = true;
            GameManagerTetris.Instance.ResetAllBoardEducation();
            _isStartShowFinish = true;
            finger.IsTutorialRunning = false;
            _tutorialCoroutine = StartCoroutine(ShowFinishEducation());
        }
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

    private IEnumerator PlayRotateStep(bool isFirstShow = false)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (isFirstShow)
            {
                yield return StartCoroutine(finger.PlayShowFinger());
                isFirstShow = false;
            }

            yield return StartCoroutine(finger.PlayFingerClickRotate());

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
        finishEducationPanel.alpha = 1f;
        finishEducationPanel.interactable = true;
        finishEducationPanel.blocksRaycasts = true;
        _isStartShowFinish = false;
    }
}