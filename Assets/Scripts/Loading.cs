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
        GameHelper.GetSnakeSettings();
        GameServicesManager.AuthenticateUser();
        GameHelper.IdLoaded = true;
    }
}