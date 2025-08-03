using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Assets.SimpleLocalization;
using UnityEngine;

public static class GameHelper
{
        private static Themes _theme = Themes.Light;
        private static bool _isAutentificate = false;
        private static bool _sound = true;
        private static bool _music = true;
        private static bool _vibration = true;
        private static bool _haveAds = true;

        public static event Action<bool> OnAutentificateChanged;
        public static event Action<Themes> OnThemeChanged;
        public static event Action<bool> OnSoundChanged;
        public static event Action<bool> OnMusicChanged;
        public static event Action<bool> OnVibrationChanged;
        public static event Action<bool> OnHaveAdsChanged;
        
        public static Save2048 Save2048 { get; set; } = new Save2048(0,2,null);
        
        public static SaveTetris SaveTetris { get; set; } = new SaveTetris(0, null);
        
        public static SaveChineseCheckers SaveChineseCheckers { get; set; } = new SaveChineseCheckers(1000, null, new List<PlayerState>(){0,0,0,0,0,0});
        
        public static SaveSnake SaveSnake { get; set; } = new SaveSnake(0, null);
        
        public static SaveLines98 SaveLines98 { get; set; } = new SaveLines98(0, null);
        
        public static SaveBlocks SaveBlocks { get; set; } = new SaveBlocks(0, null);
        
        public static bool IdLoaded { get; set; } = false;
        
        public static bool IsGameOver { get; set; } = false;
        
        public static bool IsRevived { get; set; } = false;
        
        public static bool IsShowRevive { get; set; } = false;
        
        public static bool IsPause { get; set; } = false;
        
        public static bool IsEdication { get; set; } = false;
        
        public static bool IsUIEdication { get; set; } = false;
        
        public static bool IsDoScreenshot { get; set; } = false;

        public static SnakeSettings SnakeSettings { get; set; }
        
        public static TetrisSettings TetrisSettings { get; set; }
        
        public static MiniGameType GameType { get; set; }
        
        public static bool IsAutentificate 
        {
                get => _isAutentificate;
                set
                {
                        if (_isAutentificate != value) // Изменяем только если значение новое
                        {
                                _isAutentificate = value;
                                OnAutentificateChanged?.Invoke(_isAutentificate); // Вызываем событие
                        }
                }
        }

        public static Themes Theme
        {
                get => _theme;
                set
                {
                        if (_theme != value) // Изменяем только если значение новое
                        {
                                _theme = value;
                                InvokeThemeChange(); // Вызываем событие
                        }
                }
        }
        
        public static bool Sound
        {
                get => _sound;
                set
                {
                        if (_sound != value) // Изменяем только если значение новое
                        {
                                _sound = value;
                                OnSoundChanged?.Invoke(_sound); // Вызываем событие
                        }
                }
        }
        
        public static bool Music
        {
                get => _music;
                set
                {
                        if (_music != value) // Изменяем только если значение новое
                        {
                                _music = value;
                                OnMusicChanged?.Invoke(_music); // Вызываем событие
                        }
                }
        }   
        
        public static bool Vibration
        {
                get => _vibration;
                set
                {
                        if (_vibration != value) // Изменяем только если значение новое
                        {
                                _vibration = value;
                                OnVibrationChanged?.Invoke(_vibration); // Вызываем событие
                        }
                }
        }
        
        public static bool HaveAds
        {
                get => _haveAds;
                set
                {
                        if (_haveAds != value) // Изменяем только если значение новое
                        {
                                _haveAds = value;
                                OnHaveAdsChanged?.Invoke(_haveAds); // Вызываем событие
                        }
                }
        }
        
        public static Themes GetRealTheme()
        {
                switch (Theme)
                {
                        case Themes.Auto: return GetAutoTheme();
                        case Themes.Light: return Themes.Light;
                        case Themes.Night: return Themes.Night;
                }

                return Theme;
        }

        private static Themes GetAutoTheme()
        {
                bool isDark = ThemeManager.IsSystemDarkTheme();
                if (isDark)
                {
                        return Themes.Night;
                }
                else
                {
                        return Themes.Light;
                }
        }
        
        public static int GetTypeBySpeedTetris(float speedType)
        {
                float speed = Mathf.Round(speedType * 10f) / 10f;

                if (Mathf.Approximately(speed, 1.3f)) return 1;
                if (Mathf.Approximately(speed, 1f)) return 2;
                if (Mathf.Approximately(speed, 0.7f)) return 3;
                if (Mathf.Approximately(speed, 0.4f)) return 4;
                if (Mathf.Approximately(speed, 0.3f)) return 5;
                if (Mathf.Approximately(speed, 0.2f)) return 6;

                return 2;
        }
        
        public static float GetSpeedByTypeTetris(int speedType)
        {
                switch (speedType)
                {
                        case 1: return 1.3f;
                        case 2: return 1;
                        case 3: return 0.7f;
                        case 4: return 0.4f;
                        case 5: return 0.3f;
                        case 6: return 0.2f;
                        default: return 1;
                }
        }
        
        public static int GetTypeBySpeedSnake(float speedType)
        {
                int speed = Mathf.RoundToInt(speedType);
                switch (speed)
                {
                        case 5: return 1;
                        case 6: return 2;
                        case 7: return 3;
                        case 8: return 4;
                        case 10: return 5;
                        case 12: return 6;
                        default: return 2;
                }
        }
        
        public static int GetSpeedByTypeSnake(int speedType)
        {
                switch (speedType)
                {
                        case 1: return 5;
                        case 2: return 6;
                        case 3: return 7;
                        case 4: return 8;
                        case 5: return 10;
                        case 6: return 12;
                        default: return 6;
                }
        }

        public static void InvokeThemeChange()
        {
                OnThemeChanged?.Invoke(_theme);
        }
        
        private static bool IsTabletEditor()
        {
                float dpi = Screen.dpi;
                float width = Screen.width / dpi;
                float height = Screen.height / dpi;
                float diagonalInInches = Mathf.Sqrt(width * width + height * height);

                return diagonalInInches >= 6.5f; // Обычно планшеты > 6.5 дюймов
        }
    
        public static bool IsTablet()
        {
#if UNITY_EDITOR
                return IsTabletEditor(); // Флаг для тестирования в редакторе
#elif UNITY_IOS
        return IsIpad();
#elif UNITY_ANDROID
        return IsAndroidTablet();
#else
        return false;
#endif
        }
        
#if UNITY_IOS
    private static bool IsIpad()
    {
        return UnityEngine.iOS.Device.generation.ToString().Contains("iPad") ||
               SystemInfo.deviceModel.StartsWith("iPad");
    }
#endif
        
#if UNITY_ANDROID
        private static bool IsAndroidTablet()
        {
                try
                {
                        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                        using (var resources = activity.Call<AndroidJavaObject>("getResources"))
                        using (var config = resources.Call<AndroidJavaObject>("getConfiguration"))
                        {
                                // screenLayout & SCREENLAYOUT_SIZE_MASK
                                int screenLayout = config.Get<int>("screenLayout");
                                const int SCREENLAYOUT_SIZE_MASK = 0x0F;
                                const int SCREENLAYOUT_SIZE_LARGE = 3;
                                const int SCREENLAYOUT_SIZE_XLARGE = 4;

                                int layoutSize = screenLayout & SCREENLAYOUT_SIZE_MASK;
                                if (layoutSize >= SCREENLAYOUT_SIZE_LARGE)
                                {
                                        return true;
                                }

                                // Более точная проверка: smallestScreenWidthDp
                                int smallestWidthDp = config.Get<int>("smallestScreenWidthDp");
                                if (smallestWidthDp >= 600)
                                {
                                        return true;
                                }
                        }
                }
                catch (System.Exception e)
                {
                        Debug.LogWarning("IsAndroidTablet check failed: " + e.Message);
                }

                // Дополнительная проверка по диагонали, если выше не сработало
                float dpi = Screen.dpi;
                if (dpi == 0f) dpi = 160f;

                float widthInches = Screen.width / dpi;
                float heightInches = Screen.height / dpi;
                float diagonal = Mathf.Sqrt(widthInches * widthInches + heightInches * heightInches);

                return diagonal >= 7f;
        }
#endif

        public static void Loading()
        {
                Save2048 = MyJsonHelper.Load2048();
                SaveTetris = MyJsonHelper.LoadTetris();
                SaveSnake = MyJsonHelper.LoadSnake();
                SaveLines98 = MyJsonHelper.LoadLines98();
                SaveBlocks = MyJsonHelper.LoadBlocks();
                SaveChineseCheckers = MyJsonHelper.LoadChineseCheckers();
        }

        public static bool GetEducationState(MiniGameType type)
        {
                int state = 0;
                switch (type)
                {
                        case MiniGameType.Lines98: state = PlayerPrefs.GetInt("EduLines98"); break;
                        case MiniGameType.Tetris: state = PlayerPrefs.GetInt("EduTetris"); break;
                        case MiniGameType.Snake: state = PlayerPrefs.GetInt("EduSnake"); break;
                        case MiniGameType.ChineseCheckers: state = PlayerPrefs.GetInt("EduChineseCheckers"); break;
                        case MiniGameType.Blocks: state = PlayerPrefs.GetInt("EduBlocks"); break;
                        case MiniGameType.G2048: state = PlayerPrefs.GetInt("Edu2048"); break;
                }

                return state != 0;
        }
        
        public static void SetEducationState(MiniGameType type, bool stateBool)
        {
                int state = stateBool == false ? 0 : 1;
                switch (type)
                {
                        case MiniGameType.Lines98: 
                                PlayerPrefs.SetInt("EduLines98", state);
                                PlayerPrefs.Save(); break;
                        case MiniGameType.Tetris: 
                                PlayerPrefs.SetInt("EduTetris", state);
                                PlayerPrefs.Save(); break;
                        case MiniGameType.Snake: 
                                PlayerPrefs.SetInt("EduSnake", state);
                                PlayerPrefs.Save(); break;
                        case MiniGameType.ChineseCheckers: 
                                PlayerPrefs.SetInt("EduChineseCheckers", state);
                                PlayerPrefs.Save(); break;
                        case MiniGameType.Blocks:
                                PlayerPrefs.SetInt("EduBlocks", state);
                                PlayerPrefs.Save(); break;
                        case MiniGameType.G2048: 
                                PlayerPrefs.SetInt("Edu2048", state);
                                PlayerPrefs.Save(); break;
                }
        }
        
        public static void SetEducationStateFirst()
        {
                if (!PlayerPrefs.HasKey("EduLines98"))
                {
                        PlayerPrefs.SetInt("EduLines98", 0);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("EduTetris"))
                {
                        PlayerPrefs.SetInt("EduTetris", 0);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("EduSnake"))
                {
                        PlayerPrefs.SetInt("EduSnake", 0);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("EduChineseCheckers"))
                {
                        PlayerPrefs.SetInt("EduChineseCheckers", 0);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("EduBlocks"))
                {
                        PlayerPrefs.SetInt("EduBlocks", 0);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("Edu2048"))
                {
                        PlayerPrefs.SetInt("Edu2048", 0);
                        PlayerPrefs.Save();
                }
        }
        
        public static void SetFirstSettings()
        {
                if (!PlayerPrefs.HasKey("PlayerID"))
                {
                        SetPlayerID("");
                }
                if (!PlayerPrefs.HasKey("Language"))
                {
                        string language = "English";
                        if (Application.systemLanguage == SystemLanguage.Russian)
                        {
                                language = "Russian";
                        }

                        PlayerPrefs.SetString("Language", language);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("Theme"))
                {
                        PlayerPrefs.SetInt("Theme", (int) Theme);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("Sound"))
                {
                        int soundState = Sound ? 0 : 1;
                        PlayerPrefs.SetInt("Sound", soundState);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("Music"))
                {
                        int musicState = Music ? 0 : 1;
                        PlayerPrefs.SetInt("Music", (int) musicState);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("Vibration"))
                {
                        int vibrationState = Vibration ? 0 : 1;
                        PlayerPrefs.SetInt("Vibration", (int) vibrationState);
                        PlayerPrefs.Save();
                }
                if (!PlayerPrefs.HasKey("ActivatedAchievement"))
                {
                        GameAchievementServices.SaveList();
                }
                if (!PlayerPrefs.HasKey("HaveAds"))
                {
                        int haveAdsState = HaveAds ? 0 : 1;
                        PlayerPrefs.SetInt("HaveAds", haveAdsState);
                        PlayerPrefs.Save();
                }
//                if (!PlayerPrefs.HasKey("SaveDataChineseCheckers"))
//                {
//                        JsonHelper.SaveChineseCheckersData(null);
//                }
//                if (!PlayerPrefs.HasKey("SaveData2048"))
//                {
//                        JsonHelper.Save2048(null);
//                }   
//                if (!PlayerPrefs.HasKey("SaveDataLines98"))
//                {
//                        JsonHelper.SaveLines98Data(null);
//                } 
//                if (!PlayerPrefs.HasKey("SaveDataTetris"))
//                {
//                        JsonHelper.SaveTetrisData(null);
//                } 
//                if (!PlayerPrefs.HasKey("SaveDataSnake"))
//                {
//                        JsonHelper.SaveSnakeData(null);
//                }
//                if (!PlayerPrefs.HasKey("SaveDataBlocks"))
//                {
//                        JsonHelper.SaveBlocksData(null);
//                }
                
                if (!PlayerPrefs.HasKey("TetrisSettings"))
                {
                        TetrisSettings = new TetrisSettings();
                        MyJsonHelper.SaveTetrisSettings(TetrisSettings);
                } 
                
                if (!PlayerPrefs.HasKey("SnakeSettings"))
                {
                        SnakeSettings = new SnakeSettings();
                        MyJsonHelper.SaveSnakeSettings(SnakeSettings);
                }   
                
                if (!PlayerPrefs.HasKey("CountChangeBlocks"))
                {
                        PlayerPrefs.SetInt("CountChangeBlocks", 3);
                        PlayerPrefs.Save();
                } 
                if (!PlayerPrefs.HasKey("ChangeBlocksData"))
                {
                        DateTime now = DateTime.Now;
                        PlayerPrefs.SetString("ChangeBlocksData", now.ToString());
                        PlayerPrefs.Save();
                }

                SetEducationStateFirst();
        }

        public static void SetPlayerID(string playerId)
        {
                string id = GetPlayerID();
                if (id == playerId)
                {
                        return;
                }
                PlayerPrefs.SetString("PlayerID", playerId);
                PlayerPrefs.Save();
        }
        
        public static string GetPlayerID()
        {
                string id = PlayerPrefs.GetString("PlayerID");
                return id;
        }
        
        public static void ResetData()
        {
                MyJsonHelper.SaveChineseCheckers(null);
                SaveChineseCheckers = MyJsonHelper.LoadChineseCheckers();
                
                MyJsonHelper.Save2048(null);
                Save2048 = MyJsonHelper.Load2048();
                
                MyJsonHelper.SaveTetris(null);
                SaveTetris = MyJsonHelper.LoadTetris();
                
                MyJsonHelper.SaveSnake(null);
                SaveSnake = MyJsonHelper.LoadSnake();
                
                MyJsonHelper.SaveLines98(null);
                SaveLines98 = MyJsonHelper.LoadLines98();
                
                MyJsonHelper.SaveBlocks(null);
                SaveBlocks = MyJsonHelper.LoadBlocks();
                
                SetEducationState(MiniGameType.Lines98, false);
                SetEducationState(MiniGameType.Blocks, false);
                SetEducationState(MiniGameType.Tetris, false);
                SetEducationState(MiniGameType.Snake, false);
                SetEducationState(MiniGameType.ChineseCheckers, false);
                SetEducationState(MiniGameType.G2048, false);
                
//                string pathTetris = Application.persistentDataPath + "/ScoresTetris.xml";
//                string pathSnake = Application.persistentDataPath + "/ScoresSnake.xml";
//                string path2048 = Application.persistentDataPath + "/Scores2048.xml";
//                string pathLines98 = Application.persistentDataPath + "/ScoresLines98.xml";
//                string pathBlocks = Application.persistentDataPath + "/ScoresBlocks.xml";
                
//                SaveRecordData(pathTetris,0);
//                SaveRecordData(pathSnake,0);
//                SaveRecordData(path2048,0);

//                SaveRecordData(pathLines98,0);
//                SaveRecordData(pathBlocks,0);
        }

        public static void SaveRecordData(string path, int record)
        { 
                XElement root = new XElement("root");
                root.AddFirst(new XElement("score", record));
                XDocument saveDoc = new XDocument(root);
                File.WriteAllText(path, saveDoc.ToString());
        }

        public static int LoadRecordData(string path)
        {
                XElement root = null;
                if (File.Exists(path))
                {
                        root = XDocument.Parse(File.ReadAllText(path)).Element("root");
                        XElement T = root.Element("score");
                        int n = Convert.ToInt32(T.Value);
                        return (n);
                }
                // Если файла нет, создаем его с рекордом 0
                XDocument newDoc = new XDocument(new XElement("root", new XElement("score", 0)));
                File.WriteAllText(path, newDoc.ToString());
                return 0;
        }

        public static void SetTheme(Themes theme)
        {
                if (theme == Theme)
                {
                        return;
                }

                Theme = theme;
                PlayerPrefs.SetInt("Theme", (int) theme);
                PlayerPrefs.Save();
        }
        
        public static Themes GetTheme()
        {
                Theme = (Themes) PlayerPrefs.GetInt("Theme");
                Debug.Log("Theme: " + Theme.ToString());
                return Theme;
        }
        
        public static SnakeSettings GetSnakeSettings()
        {
                SnakeSettings = MyJsonHelper.LoadSnakeSettings();
                Debug.Log("SnakeSettings: " + SnakeSettings.ToString());
                return SnakeSettings;
        }   
        
        public static TetrisSettings GetTetrisSettings()
        {
                TetrisSettings = MyJsonHelper.LoadTetrisSettings();
                Debug.Log("TetrisSettings: " + TetrisSettings.ToString());
                return TetrisSettings;
        }

        public static void SetLanguage(int languageId)
        {
                string language = languageId == 0 ? "English" : "Russian";

                if (LocalizationManager.Language == language)
                {
                        return;
                }
                LocalizationManager.Language = language;
                PlayerPrefs.SetString("Language", language);
                PlayerPrefs.Save();
        }
        
        public static int GetLanguage()
        {
                LocalizationManager.Language = PlayerPrefs.GetString("Language") == "English" ? "English" : "Russian";
                int languageId = LocalizationManager.Language == "English" ? 0 : 1;
                Debug.Log("Language: " + LocalizationManager.Language);
                return languageId;
        }

        public static void SetSound(bool sound)
        {
                if (sound == Sound)
                {
                        return;
                }

                Sound = sound;
                int soundState = Sound ? 0 : 1;
                PlayerPrefs.SetInt("Sound", (int) soundState);
                PlayerPrefs.Save();
        }
        
        public static bool GetSound()
        {
                int soundState = PlayerPrefs.GetInt("Sound");
                Sound = soundState == 0 ? true : false;
                return Sound;
        }

        public static void SetMusic(bool music)
        {
                if (music == Music)
                {
                        return;
                }

                Music = music;
                int musicState = Music ? 0 : 1;
                PlayerPrefs.SetInt("Music", (int) musicState);
                PlayerPrefs.Save();
        }
        
        public static bool GetMusic()
        {
                int musicState = PlayerPrefs.GetInt("Music");
                Music = musicState == 0 ? true : false;
                return Music;
        }
        
        public static void SetVibration(bool vibration)
        {
                if (vibration == Vibration)
                {
                        return;
                }

                Vibration = vibration;
                int vibrationState = Vibration ? 0 : 1;
                PlayerPrefs.SetInt("Vibration", (int) vibrationState);
                PlayerPrefs.Save();
        }
        
        public static bool GetVibration()
        {
                int vibrationState = PlayerPrefs.GetInt("Vibration");
                Vibration = vibrationState == 0 ? true : false;
                return Vibration;
        }   
        
        public static void VibrationStart()
        {
                if (!Vibration)
                {
                        return;
                }
                Handheld.Vibrate();
        }
        
        public static void SetHaveAds(bool haveAds)
        {
                if (haveAds == HaveAds)
                {
                        return;
                }

                HaveAds = haveAds;
                int haveAdsState = HaveAds ? 0 : 1;
                PlayerPrefs.SetInt("HaveAds", (int) haveAdsState);
                PlayerPrefs.Save();
        }
        
        public static bool GetHaveAds()
        {
                int haveAdsState = PlayerPrefs.GetInt("HaveAds");
                HaveAds = haveAdsState == 0 ? true : false;
                return HaveAds;
        }

        public static void AdjustBoardSize(Camera cam)
        {
                float screenRatio = (float)Screen.width / Screen.height;
                float boardWidth = 22f; // Количество клеток в ширину
                float boardHeight = 22f; // Количество клеток в высоту

                if (screenRatio >= 1f) // Горизонтальная ориентация
                {
                        cam.orthographicSize = boardHeight / 2 + 1;
                }
                else // Вертикальная ориентация
                {
                        cam.orthographicSize = boardWidth / 2 + 1;
                }
        }
}