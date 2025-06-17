using System.Collections.Generic;
using UnityEngine;

public class GameplayTimeTracker : MonoBehaviour
{
    public static GameplayTimeTracker Instance;
    
    private float totalPlayTime = 0f;
    private float lastResumeTime = 0f;
    private bool isPlaying = false;
    
    private float sessionStartTime;
    private float accumulatedPlayTime;
    
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetFirstSettings();
            ChangeTimer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResumeTimer(); // начало игры
    }

    public void ResumeTimer()
    {
        if (!isPlaying)
        {
            sessionStartTime = Time.realtimeSinceStartup;
            
            lastResumeTime = Time.realtimeSinceStartup;
            isPlaying = true;
        }
    }

    public void PauseTimer()
    {
        if (isPlaying)
        {
            float sessionPlayTime = Time.realtimeSinceStartup - sessionStartTime;
            accumulatedPlayTime += sessionPlayTime;
            
            var parameters = new Dictionary<string, object>()
            {
                { AnalyticType.play_time_sec.ToString(), Mathf.RoundToInt(accumulatedPlayTime) }
            };
            
            AnalyticsManager.Instance.LogEvent(AnalyticType.session_play_time.ToString(), parameters);
            
            totalPlayTime += Time.realtimeSinceStartup - lastResumeTime;
            isPlaying = false;
        }
        SetTimer();
    }

    public float OnGameOver()
    {
        if (isPlaying)
        {
            PauseTimer(); // зафиксировать финальный кусок времени
        }

        Debug.Log($"Total active play time: {totalPlayTime} seconds");

        float finalTime = totalPlayTime;

        ResetTimer();

        SetTimer();

        return finalTime;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            PauseTimer();
        else
            ResumeTimer();
    }

    private void OnApplicationQuit()
    {
        PauseTimer(); // зафиксировать если игра закрыта совсем
    }

    public void ChangeTimer()
    {
        totalPlayTime = GetTimer();
    }
    
    public void ResetTimer()
    {
        totalPlayTime = 0;
        accumulatedPlayTime = 0;
        lastResumeTime = Time.realtimeSinceStartup;
    }
    
    public void RestartTimer()
    {
        Debug.Log("RestartTimer");
        ResetTimer();
        ChangeTimer();
        isPlaying = false;
        ResumeTimer();
    }
    
    public float GetTimer()
    {
        switch (GameHelper.GameType)
        {
            case MiniGameType.Tetris: return PlayerPrefs.GetFloat("TotalActiveTetrisTime");
            case MiniGameType.Snake: return PlayerPrefs.GetFloat("TotalActiveSnakeTime");
            case MiniGameType.Lines98: return PlayerPrefs.GetFloat("TotalActiveLines98Time");
            case MiniGameType.ChineseCheckers: return PlayerPrefs.GetFloat("TotalActiveChineseCheckersTime");
            case MiniGameType.G2048: return PlayerPrefs.GetFloat("TotalActiveG2048Time");
            case MiniGameType.Blocks: return PlayerPrefs.GetFloat("TotalActiveBlocksTime");
        }

        return 0;
    }
    
    public void SetTimer()
    {
        switch (GameHelper.GameType)
        {
            case MiniGameType.None: return;
            case MiniGameType.Tetris: PlayerPrefs.SetFloat("TotalActiveTetrisTime", totalPlayTime); break;
            case MiniGameType.Snake: PlayerPrefs.SetFloat("TotalActiveSnakeTime", totalPlayTime); break;
            case MiniGameType.Lines98: PlayerPrefs.SetFloat("TotalActiveLines98Time", totalPlayTime); break;
            case MiniGameType.ChineseCheckers: PlayerPrefs.SetFloat("TotalActiveChineseCheckersTime", totalPlayTime); break;
            case MiniGameType.G2048: PlayerPrefs.SetFloat("TotalActiveG2048Time", totalPlayTime); break;
            case MiniGameType.Blocks: PlayerPrefs.SetFloat("TotalActiveBlocksTime", totalPlayTime); break;
        }
        
        PlayerPrefs.Save();
    }

    public void SetFirstSettings()
    {
        foreach (MiniGameType game in System.Enum.GetValues(typeof(MiniGameType)))
        {
            if (game == MiniGameType.None) continue;

            string key = $"TotalActive{game}Time";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetFloat(key, 0f);
            }
        }
        PlayerPrefs.Save();
    }
}