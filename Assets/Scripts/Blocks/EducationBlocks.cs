using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EducationBlocks : Education
{
    [SerializeField] private EducationFinger finger;
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private CanvasGroup finishEducationPanel;
    [SerializeField] private GameObject backButton;
    [SerializeField] private BlocksBoard boardEdu;
    
    [SerializeField] private BlockShape blockOne;
    
    private Coroutine _tutorialCoroutine;
    private bool _isStartShowFinish;
    
    private int _step;
    
    public BlockTile EnableTile { get; set; }
    
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

            if (_step == 3 && !_isStartShowFinish)
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
    
    public override void ShowEducation(bool isFirstEducation)
    {
        GameHelper.IsEdication = true;
        
        ShowView(isFirstEducation);
        ShowEducation();
    }
    
    public override void ShowView(bool isFirstEducation)
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

    public override void ShowEducation()
    {
        educationPanel.SetActive(true);
        
        GameManagerBlocks.Instance.LoadStartEducation();

        _step = 0;
        StartPlay();
    }
    
    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        StopTutorial();
        HideFinishEducation();
        
        GameManagerBlocks.Instance.ResetAllBoardEducation();
        educationPanel.SetActive(false);
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

    public override void Restart(int step)
    {
        _step = step;
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
        GameManagerBlocks.Instance.ResetGridEdu();
        if (_step == 0)
        {
            SaveDataBlocks saveData = GetFirstSaveData();
            GameManagerBlocks.Instance.LoadEducation(saveData);
            EnableTile = boardEdu.Tiles[40];
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        }
        else if (_step == 1)
        {
            SaveDataBlocks saveData = GetSecondSaveData();
            GameManagerBlocks.Instance.LoadEducation(saveData);
            EnableTile = boardEdu.Tiles[40];
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        }
        else if (_step == 2)
        {
            SaveDataBlocks saveData = GetThirdSaveData();
            GameManagerBlocks.Instance.LoadEducation(saveData);
            EnableTile = boardEdu.Tiles[40];
            finger.IsTutorialRunning = true;
            _tutorialCoroutine = StartCoroutine(PlayFirstStep());
        } 
        else if (_step == 3)
        {
            _isStartShowFinish = true;
            finger.IsTutorialRunning = false;
            _tutorialCoroutine = StartCoroutine(ShowFinishEducation());
        }
    }

    private IEnumerator PlayFirstStep()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            // первый клик и перемещение пальца
            Vector3 start1 = boardEdu.Blocks[1].transform.position;
            Vector3 end1 = boardEdu.Tiles[40].transform.position;

            yield return StartCoroutine(finger.PlayFingerClickMove(start1, end1));

            yield return new WaitForSeconds(0.2f);
        }
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

    private void ForceFinishEducation()
    {
        finishEducationPanel.alpha = 1f;
        finishEducationPanel.interactable = true;
        finishEducationPanel.blocksRaycasts = true;
        _isStartShowFinish = false;
    }

}