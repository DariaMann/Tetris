using System;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class AnalyticsManager : MonoBehaviour, IGameAnalyticsATTListener
{
    public static AnalyticsManager Instance;

    private DateTime _pauseTime;
    private bool _isInitialized = false;
    
    public bool IsInitialized => _isInitialized;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_isInitialized) return;
        
        InitializeAnalytics();
        LogEvent(AnalyticType.session_start.ToString());
    }

    void OnApplicationPause(bool pause)
    {
        if (!_isInitialized) return;

        if (pause)
        {
            _pauseTime = DateTime.UtcNow;
            LogEvent(AnalyticType.app_paused.ToString());
        }
        else
        {
            TimeSpan timeAway = DateTime.UtcNow - _pauseTime;
            LogEvent(AnalyticType.app_resumed.ToString());

            if (timeAway.TotalSeconds > 300)
            {
                LogEvent(AnalyticType.session_restart.ToString());
            }
        }
    }

    private void InitializeAnalytics()
    {
        if (_isInitialized) return;
        
#if UNITY_IOS
        GameAnalytics.RequestTrackingAuthorization(this);
#else
        if (GameHelper.IsAutentificate)
        {
            GameAnalytics.SetCustomId(GameHelper.GetPlayerID());
        }
        GameAnalytics.Initialize();
        _isInitialized = true;
        Debug.Log("[Analytics] GameAnalytics initialized.");
#endif
    }

    public void GameAnalyticsATTListenerAuthorized()
    {
        if (GameHelper.IsAutentificate)
        {
            GameAnalytics.SetCustomId(GameHelper.GetPlayerID());
        }
        GameAnalytics.Initialize();
        _isInitialized = true;
        Debug.Log("[Analytics] GA authorized and initialized.");
    }

    public void GameAnalyticsATTListenerDenied()        => GameAnalyticsATTListenerAuthorized();
    public void GameAnalyticsATTListenerRestricted()    => GameAnalyticsATTListenerAuthorized();
    public void GameAnalyticsATTListenerNotDetermined() => GameAnalyticsATTListenerAuthorized();

//    public void LogEvent(string eventName)
//    {
//        if (!_isInitialized) return;
//
//        try
//        {
//            GameAnalytics.NewDesignEvent(eventName);
//#if UNITY_EDITOR
//            Debug.Log($"[Analytics] Event: {eventName}");
//#endif
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"[Analytics] Failed to log event '{eventName}': {ex.Message}");
//        }
//    }
    
    public void LogEvent(string eventId, float? value = null)
    {
        if (!_isInitialized) return;
        
        try
        {
            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogWarning("GA eventId is null or empty");
                return;
            }

            if (value.HasValue)
            {
                GameAnalytics.NewDesignEvent(eventId, value.Value);
#if UNITY_EDITOR
                Debug.Log($"[Analytics] Event: {eventId}:{value.Value}");
#endif
            }
            else
            {
                GameAnalytics.NewDesignEvent(eventId);
#if UNITY_EDITOR
                Debug.Log($"[Analytics] Event: {eventId}");
#endif
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Analytics] Failed to log event '{eventId}': {ex.Message}");
        }
    }
}