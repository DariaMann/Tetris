using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManagerSnake: MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private FoodController foodController;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private EducationSnake education;
    
    public static GameManagerSnake Instance { get; private set; }
    
    public GameOver GameOverPanel
    {
        get => gameOver;
        set => gameOver = value;
    }
    
    public SaveScores SaveScores
    {
        get => saveScores;
        set => saveScores = value;
    }
    
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }
    
    private void Start()
    {
        LoadLastPlay();
        
        if (!GameHelper.GetEducationState(MiniGameType.Snake))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.Snake, true);
        }
    }
    
    void OnApplicationQuit()
    {
        SaveLastPlay();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLastPlay();
        }
    }
    
    private void OnDestroy()
    {
        SaveLastPlay();
    }

    public void LoadLastPlay()
    {
        SaveDataSnake saveData = GameHelper.SaveSnake.SaveDataSnake;
        if (saveData == null)
        {
            snake.ResetState();
            return;
        }

        snake.LoadSave(saveData);
    }
    
    public void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveSnake.SaveDataSnake = null;
            JsonHelper.SaveSnake(GameHelper.SaveSnake);
            return;
        }

        SaveDataSnake data = new SaveDataSnake(saveScores.IsWin, transform.position, foodController.Foods, snake.Direction, saveScores.CurrentScore);
        GameHelper.SaveSnake.SaveDataSnake = data;
        JsonHelper.SaveSnake(GameHelper.SaveSnake);
    }

    public void GameOver()
    {
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }
    
    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        
        snake.ResetState();
    }
}