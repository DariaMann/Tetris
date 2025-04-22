using System;
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
        GameAchievementServices.LoadList();
        GameHelper.GetLanguage();
        GameHelper.GetTheme();
        GameHelper.GetTetrisSettings();
        GameHelper.GetSnakeSettings();
        GameServicesManager.AuthenticateUser();
        GameHelper.IdLoaded = true;
    }

    private void Start()
    {
        Application.targetFrameRate = 60; // или 120, если устройство поддерживает
        QualitySettings.vSyncCount = 0;   // отключить VSync
    }
}