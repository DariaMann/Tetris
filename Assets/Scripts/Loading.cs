using System;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using UnityEngine;

public class Loading: MonoBehaviour
{
    private void Awake()
    {
        if (GameHelper.IdLoaded)
        {
            return;
        }
        GameHelper.SetFirstSettings();
        GameHelper.Loading();
        GameAchievementServices.LoadList();
        GameHelper.GetLanguage();
        GameHelper.GetTheme();
        GameHelper.GetTetrisSettings();
        GameHelper.GetSnakeSettings();
        GameServicesManager.AuthenticateUser();
        GameHelper.IdLoaded = true;
        AnalyticsManager.Instance.LogEvent(AnalyticType.session_start.ToString(), new Dictionary<string, object>
        {
            { AnalyticType.timestamp.ToString(), DateTime.UtcNow.ToString("o") }
        });
    }

    private void Start()
    {
//        Application.targetFrameRate = 60; // или 120, если устройство поддерживает
        QualitySettings.vSyncCount = 0;   // отключить VSync
    }
}