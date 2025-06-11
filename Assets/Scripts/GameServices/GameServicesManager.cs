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
                Debug.Log(achivement.ToString());
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
            case AchivementServices.FirstLine: return ServicesIds.achievement_first_line;
            case AchivementServices.Tetris10Points: return ServicesIds.achievement_tetris_10_points;
            case AchivementServices.Tetris50Points: return ServicesIds.achievement_tetris_50_points;
            case AchivementServices.Tetris100Points: return ServicesIds.achievement_tetris_100_points;
            case AchivementServices.Tetris500Points: return ServicesIds.achievement_tetris_500_points;
            case AchivementServices.Tetris1000Points: return ServicesIds.achievement_tetris_1000_points;
            case AchivementServices.Tetris1500Points: return ServicesIds.achievement_tetris_1500_points;
            case AchivementServices.Tetris2000Points: return ServicesIds.achievement_tetris_2000_points;
            case AchivementServices.Snake10Points: return ServicesIds.achievement_snake_10_points;
            case AchivementServices.Snake50Points: return ServicesIds.achievement_snake_50_points;
            case AchivementServices.Snake100Points: return ServicesIds.achievement_snake_100_points;
            case AchivementServices.Snake200Points: return ServicesIds.achievement_snake_200_points;
            case AchivementServices.Snake250Points: return ServicesIds.achievement_snake_250_points;
            case AchivementServices.Tile128: return ServicesIds.achievement_128_tile;
            case AchivementServices.Tile256: return ServicesIds.achievement_256_tile;
            case AchivementServices.Tile512: return ServicesIds.achievement_512_tile;
            case AchivementServices.Tile1024: return ServicesIds.achievement_1024_tile;
            case AchivementServices.Tile2048: return ServicesIds.achievement_2048_tile;
            case AchivementServices.Tile4096: return ServicesIds.achievement_4096_tile;
            case AchivementServices.Tile8192: return ServicesIds.achievement_8192_tile;
            case AchivementServices.Tile16384: return ServicesIds.achievement_16384_tile;
            case AchivementServices.G1000Points: return ServicesIds.achievement_2048_1000_points;
            case AchivementServices.G5000Points: return ServicesIds.achievement_2048_5000_points;
            case AchivementServices.G10000Points: return ServicesIds.achievement_2048_10000_points;
            case AchivementServices.G50000Points: return ServicesIds.achievement_2048_50000_points;
            case AchivementServices.G100000Points: return ServicesIds.achievement_2048_100000_points;
            case AchivementServices.G150000Points: return ServicesIds.achievement_2048_150000_points;
            case AchivementServices.G200000Points: return ServicesIds.achievement_2048_200000_points;
            case AchivementServices.Lines98100Points: return ServicesIds.achievement_lines_98_100_points;
            case AchivementServices.Lines98200Points: return ServicesIds.achievement_lines_98_200_points;
            case AchivementServices.Lines98300Points: return ServicesIds.achievement_lines_98_300_points;
            case AchivementServices.Lines98400Points: return ServicesIds.achievement_lines_98_400_points;
            case AchivementServices.Lines98500Points: return ServicesIds.achievement_lines_98_500_points;
            case AchivementServices.Lines98700Points: return ServicesIds.achievement_lines_98_700_points;
            case AchivementServices.Lines98900Points: return ServicesIds.achievement_lines_98_900_points;
            case AchivementServices.Lines981000Points: return ServicesIds.achievement_lines_98_1000_points;
            case AchivementServices.Lines982000Points: return ServicesIds.achievement_lines_98_2000_points;
            case AchivementServices.Lines983000Points: return ServicesIds.achievement_lines_98_3000_points;
            case AchivementServices.Lines984000Points: return ServicesIds.achievement_lines_98_4000_points;
            case AchivementServices.Lines985000Points: return ServicesIds.achievement_lines_98_5000_points;
            case AchivementServices.Lines986000Points: return ServicesIds.achievement_lines_98_6000_points;
            case AchivementServices.Lines987000Points: return ServicesIds.achievement_lines_98_7000_points;
            case AchivementServices.Blocks100Points: return ServicesIds.achievement_blocks_100_points;
            case AchivementServices.Blocks300Points: return ServicesIds.achievement_blocks_300_points;
            case AchivementServices.Blocks500Points: return ServicesIds.achievement_blocks_500_points;
            case AchivementServices.Blocks700Points: return ServicesIds.achievement_blocks_700_points;
            case AchivementServices.Blocks900Points: return ServicesIds.achievement_blocks_900_points;
            case AchivementServices.Blocks1000Points: return ServicesIds.achievement_blocks_1000_points;
            case AchivementServices.Blocks1500Points: return ServicesIds.achievement_blocks_1500_points;
            case AchivementServices.Blocks2000Points: return ServicesIds.achievement_blocks_2000_points;
            case AchivementServices.Blocks3000Points: return ServicesIds.achievement_blocks_3000_points;
            case AchivementServices.Blocks4000Points: return ServicesIds.achievement_blocks_4000_points;
            case AchivementServices.Blocks5000Points: return ServicesIds.achievement_blocks_5000_points;
            case AchivementServices.Blocks6000Points: return ServicesIds.achievement_blocks_6000_points;
            case AchivementServices.ChineseCheckersPlay10Games: return ServicesIds.achievement_chinese_checkers_play_10_games;
            case AchivementServices.ChineseCheckersPlay50Games: return ServicesIds.achievement_chinese_checkers_play_50_games;
            case AchivementServices.ChineseCheckersPlay100Games: return ServicesIds.achievement_chinese_checkers_play_100_games;
            case AchivementServices.ChineseCheckersPlay300Games: return ServicesIds.achievement_chinese_checkers_play_300_games;
            case AchivementServices.ChineseCheckersPlay500Games: return ServicesIds.achievement_chinese_checkers_play_500_games;
            case AchivementServices.ChineseCheckersPlay1000Games: return ServicesIds.achievement_chinese_checkers_play_1000_games;
            default: return "";
        }
    }
    
    //Получение ID достижения
    private static string GetLeaderBoardId(MiniGameType type)
    {
        Debug.Log("Id таблицы: " + type);
        switch (type)
        {
            case MiniGameType.Tetris: return ServicesIds.leaderboard_tetris;
            case MiniGameType.Snake: return ServicesIds.leaderboard_snake;
            case MiniGameType.G2048: return ServicesIds.leaderboard_2048;
            case MiniGameType.Lines98: return ServicesIds.leaderboard_lines_98;
            case MiniGameType.Blocks: return ServicesIds.leaderboard_blocks;
            case MiniGameType.ChineseCheckers: return ServicesIds.leaderboard_chinese_checkers;
            default: return ServicesIds.leaderboard_tetris;
        }
    }

    #endregion
}