#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;

using GooglePlayGames.BasicApi.SavedGame;
#endif

using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public static class GameServicesManager
{
    #region Authenticate

    public static void AuthenticateUser()
    {
        Debug.Log("Аутентификайия игрока");
#if UNITY_EDITOR
        AuthenticateUnity("1", "admin");
#elif UNITY_ANDROID
        AuthenticateGooglePlay();
#elif UNITY_IOS
        AuthenticateGameCenter();
#endif
    }
    
    public static void AuthenticateUnity(string idStr, string name)
    {
        Debug.Log("Тестирование в редакторе: симуляция успешного подключения");
        Debug.Log("Успешная симуляция подключения к игровым сервисам");
        GameHelper.IsAutentificate = true;
    }
    
    private static void AuthenticateGooglePlay()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();

        if (!Social.localUser.authenticated)
        {
            // Аутентификация игрока
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Успешная аутентификация в Google Play Games");
                    Debug.Log("Имя игрока: " + Social.localUser.userName);
                    GameHelper.IsAutentificate = true;
                    SyncAllAchievements();
                }
                else
                {
                    Debug.Log("Не удалось аутентифицироваться в Google Play Games");
                    GameHelper.IsAutentificate = false;
                }
            });
        }
        else
        {
            Debug.Log("Пользователь уже аутентифицирован в Google Play Games");
            GameHelper.IsAutentificate = true;
        }
#endif
    }
    
    private static void AuthenticateGameCenter()
    {
        // Инициализация Game Center
//        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    Debug.Log("Успешная аутентификация в Game Center");
                    string playerId = Social.localUser.id;
                    string nameId = Social.localUser.userName;
                    Debug.Log("Player ID: " + playerId);
                    GameHelper.IsAutentificate = true;
                    SyncAllAchievements();
                }
                else
                {
                    Debug.Log("Не удалось аутентифицироваться в Game Center");
                    GameHelper.IsAutentificate = false;
                }
            });
        }
        else
        {
            Debug.Log("Пользователь уже аутентифицирован в Game Center");
            GameHelper.IsAutentificate = true;
        }
    }
    
    #endregion

    #region LeaderBoard
    
    // Отправка результата в таблицу лидеров
    public static void ReportScore(long score, MiniGameType type)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!Social.localUser.authenticated)
        {
            Debug.Log("Нет регистрации. Не отправить результат в таблицу лидеров");
            return;
        }
        // Отправка результата в таблицу лидеров
        Social.ReportScore(score, GetLeaderBoardId(type), (bool success) => {
            if (success)
            {
                Debug.Log("Успешно отправлен результат в таблицу лидеров");
            }
            else
            {
                Debug.Log("Ошибка при отправке результата в таблицу лидеров");
            }
        });
#endif
    }

    // Открытие таблицы лидеров
    public static void ShowLeaderboardUI(MiniGameType type)
    {
#if UNITY_ANDROID
        // Открытие UI лидеров в Google Play Games
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GetLeaderBoardId(type));
#elif UNITY_IOS
        // Открытие UI лидеров в Apple Game Center
        Social.ShowLeaderboardUI();
#endif
    }
    
    #endregion

    #region Achievements
    
    // Открытие достижений
    public static void ShowAchievementsUI()
    {
#if UNITY_ANDROID
        // Открытие UI достижений в Google Play Games
        PlayGamesPlatform.Instance.ShowAchievementsUI();
#elif UNITY_IOS
        // Открытие UI достижений в Apple Game Center
        Social.ShowAchievementsUI();
#endif
    }
    
    //Разблокировка достижения
    public static void UnlockAchieve(AchivementServices type)
    {
        if (!GameAchievementServices.ActivatedAchievementIds.Contains(type))
        {
            Debug.Log("Разблокировка достижения разблокированного достижения : " + type);
            GameAchievementServices.ActivatedAchievementIds.Add(type);
            UnlockAchievement(GetAchievementId(type));
        }
    }
    
    // Разблокировка достижения
    private static void UnlockAchievement(string achievementID)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!Social.localUser.authenticated)
        {
            Debug.Log("Нет регистрации. Не разблокировать достижение");
            return;
        }
        // Разблокировка достижения
        Social.ReportProgress(achievementID, 100.0f, (bool success) => {
            if (success)
            {
                Debug.Log("Достижение разблокировано");
            }
            else
            {
                Debug.Log("Ошибка при разблокировке достижения");
            }
        });
#endif
    }

    //Синхронизация всех достижений
    private static void SyncAllAchievements()
    {
        foreach (AchivementServices achivement in Enum.GetValues(typeof(AchivementServices)))
        {
            if (GameAchievementServices.ActivatedAchievementIds.Contains(achivement))
            {
                UnlockAchievement(GetAchievementId(achivement));
            }
        }
    }

    //Получение ID достижения
    private static string GetAchievementId(AchivementServices type)
    {
        Debug.Log("Id достижения: " + type);
        switch (type)
        {
            case AchivementServices.FirstLine: return "CgkImZqemP4FEAIQEA";
            case AchivementServices.Tetris10Points: return "CgkImZqemP4FEAIQEg";
            case AchivementServices.Tetris50Points: return "CgkImZqemP4FEAIQEw";
            case AchivementServices.Tetris100Points: return "CgkImZqemP4FEAIQGg";
            case AchivementServices.Tetris500Points: return "CgkImZqemP4FEAIQFA";
            case AchivementServices.Tetris1000Points: return "CgkImZqemP4FEAIQEQ";
            case AchivementServices.Snake10Points: return "CgkImZqemP4FEAIQFQ";
            case AchivementServices.Snake50Points: return "CgkImZqemP4FEAIQFg";
            case AchivementServices.Snake100Points: return "CgkImZqemP4FEAIQFw";
            case AchivementServices.Snake200Points: return "CgkImZqemP4FEAIQGA";
            case AchivementServices.Snake300Points: return "CgkImZqemP4FEAIQGQ";
            case AchivementServices.Tile128: return "CgkImZqemP4FEAIQAQ";
            case AchivementServices.Tile256: return "CgkImZqemP4FEAIQAg";
            case AchivementServices.Tile512: return "CgkImZqemP4FEAIQAw";
            case AchivementServices.Tile1024: return "CgkImZqemP4FEAIQBA";
            case AchivementServices.Tile2048: return "CgkImZqemP4FEAIQBQ";
            case AchivementServices.Tile4096: return "CgkImZqemP4FEAIQBg";
            case AchivementServices.Tile8192: return "CgkImZqemP4FEAIQBw";
            case AchivementServices.Tile16384: return "CgkImZqemP4FEAIQCA";
            case AchivementServices.G1000Points: return "CgkImZqemP4FEAIQDQ";
            case AchivementServices.G5000Points: return "CgkImZqemP4FEAIQCQ";
            case AchivementServices.G10000Points: return "CgkImZqemP4FEAIQCg";
            case AchivementServices.G50000Points: return "CgkImZqemP4FEAIQCw";
            case AchivementServices.G100000Points: return "CgkImZqemP4FEAIQDA";
            case AchivementServices.G150000Points: return "CgkImZqemP4FEAIQDg";
            case AchivementServices.G200000Points: return "CgkImZqemP4FEAIQDw";
            default: return "";
        }
    }
    
    //Получение ID достижения
    private static string GetLeaderBoardId(MiniGameType type)
    {
        Debug.Log("Id таблицы: " + type);
        switch (type)
        {
            case MiniGameType.Tetris: return "CgkImZqemP4FEAIQGw";
            case MiniGameType.Snake: return "CgkImZqemP4FEAIQHA";
            case MiniGameType.G2048: return "CgkImZqemP4FEAIQHQ";
            default: return "";
        }
    }

    #endregion
}