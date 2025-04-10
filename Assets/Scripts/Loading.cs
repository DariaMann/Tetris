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
        GameServicesManager.AuthenticateUser();
        GameHelper.IdLoaded = true;
    }
}