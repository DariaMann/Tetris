using System;
using System.IO;
using System.Xml.Linq;
using Assets.SimpleLocalization;
using UnityEngine;

public static class GameHelper
{
        private static Themes _theme = Themes.Light;
        private static bool _sound = true;
        private static bool _music = true;
        private static bool _vibration = true;

        public static event Action<Themes> OnThemeChanged;
        public static event Action<bool> OnSoundChanged;
        public static event Action<bool> OnMusicChanged;
        public static event Action<bool> OnVibrationChanged;
        
        public static bool IdLoaded { get; set; } = false;
        
        public static SnakeSettings SnakeSettings { get; set; }
        
        public static MiniGameType GameType { get; set; }
        
        public static bool IsAutentificate { get; set; } = false;

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

        public static void InvokeThemeChange()
        {
                OnThemeChanged?.Invoke(_theme);
        }

        public static void SetFirstSettings()
        {
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
                if (!PlayerPrefs.HasKey("SaveDataChineseCheckers"))
                {
                        JsonHelper.SaveChineseCheckersData(null);
                }
                if (!PlayerPrefs.HasKey("SaveData2048"))
                {
                        JsonHelper.Save2048Data(null);
                } 
                if (!PlayerPrefs.HasKey("SaveDataTetris"))
                {
                        JsonHelper.SaveTetrisData(null);
                } 
                if (!PlayerPrefs.HasKey("SaveDataSnake"))
                {
                        JsonHelper.SaveSnakeData(null);
                }
                
                if (!PlayerPrefs.HasKey("SnakeSettings"))
                {
                        SnakeSettings = new SnakeSettings();
                        JsonHelper.SaveSnakeSettings(SnakeSettings);
                }
        }
        
        public static void ResetData()
        {
                JsonHelper.SaveChineseCheckersData(null);
                JsonHelper.Save2048Data(null);
                JsonHelper.SaveTetrisData(null);
                JsonHelper.SaveSnakeData(null);
                string pathTetris = Application.persistentDataPath + "/ScoresTetris.xml";
                string pathSnake = Application.persistentDataPath + "/ScoresSnake.xml";
                string path2048 = Application.persistentDataPath + "/Scores2048.xml";
                
                SaveRecordData(pathTetris,0);
                SaveRecordData(pathSnake,0);
                SaveRecordData(path2048,0);
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
                SnakeSettings = JsonHelper.LoadSnakeSettings();
                Debug.Log("SnakeSettings: " + SnakeSettings.ToString());
                return SnakeSettings;
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