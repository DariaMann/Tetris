using System;
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

        public static Themes Theme
        {
                get => _theme;
                set
                {
                        if (_theme != value) // Изменяем только если значение новое
                        {
                                _theme = value;
                                OnThemeChanged?.Invoke(_theme); // Вызываем событие
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
                return Theme;
        }    
        
        public static void SetLanguage(int languageId)
        {
                string language = languageId == 0 ? "English" : "Russian";

                if (LocalizationManager.Language == language)
                {
                        return;
                }
        
                PlayerPrefs.SetString("Language", language);
                PlayerPrefs.Save();
        }
        
        public static int GetLanguage()
        {
                LocalizationManager.Language = PlayerPrefs.GetString("Language") == "English" ? "English" : "Russian";
                int languageId = LocalizationManager.Language == "English" ? 0 : 1;
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