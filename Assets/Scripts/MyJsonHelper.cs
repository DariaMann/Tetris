using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public static class MyJsonHelper
{
    #region Tetris

    public static void SaveTetrisSettings(TetrisSettings data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonTetrisSettings(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("TetrisSettings", json);
        PlayerPrefs.Save();
    }
    
    public static TetrisSettings LoadTetrisSettings()
    {
        if (PlayerPrefs.HasKey("TetrisSettings"))
        {
            string json = PlayerPrefs.GetString("TetrisSettings");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            TetrisSettings data = DeserializeJsonTetrisSettings(json);
            return data;
        }
        return null;
    }
    
    private static TetrisSettings DeserializeJsonTetrisSettings(string jsonString)
    {
        TetrisSettings data = JsonConvert.DeserializeObject<TetrisSettings>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonTetrisSettings(TetrisSettings data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    public static void SaveTetris(SaveTetris data)
    {
        if (data == null)
        {
            data = new SaveTetris(0, null);
        }
        string json = SerializeJsonTetris(data);
        Debug.Log("SaveTetris: " + json);
        string path = Application.persistentDataPath + "/Tetris.json";
        File.WriteAllText(path, json);
    }
    
    public static SaveTetris LoadTetris()
    {
        string path = Application.persistentDataPath + "/Tetris.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            SaveTetris data = DeserializeJsonTetris(json);
            return data;
        }

        return new SaveTetris(0, null);
    }
    
    private static SaveTetris DeserializeJsonTetris(string jsonString)
    {
        SaveTetris data = JsonConvert.DeserializeObject<SaveTetris>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonTetris(SaveTetris data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    #region Snake

    public static void SaveSnakeSettings(SnakeSettings data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonSnakeSettings(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("SnakeSettings", json);
        PlayerPrefs.Save();
    }
    
    public static SnakeSettings LoadSnakeSettings()
    {
        if (PlayerPrefs.HasKey("SnakeSettings"))
        {
            string json = PlayerPrefs.GetString("SnakeSettings");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            SnakeSettings data = DeserializeJsonSnakeSettings(json);
            return data;
        }
        return null;
    }
    
    private static SnakeSettings DeserializeJsonSnakeSettings(string jsonString)
    {
        SnakeSettings data = JsonConvert.DeserializeObject<SnakeSettings>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSnakeSettings(SnakeSettings data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    public static void SaveSnake(SaveSnake data)
    {
        if (data == null)
        {
            data = new SaveSnake(0, null);
        }
        string json = SerializeJsonSnake(data);
        Debug.Log("SaveSnake: " + json);
        string path = Application.persistentDataPath + "/Snake.json";
        File.WriteAllText(path, json);
    }
    
    public static SaveSnake LoadSnake()
    {
        string path = Application.persistentDataPath + "/Snake.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            SaveSnake data = DeserializeJsonSnake(json);
            return data;
        }

        return new SaveSnake(0, null);
    }

    private static SaveSnake DeserializeJsonSnake(string jsonString)
    {
        SaveSnake data = JsonConvert.DeserializeObject<SaveSnake>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSnake(SaveSnake data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    #region 2048

    public static void Save2048(Save2048 data)
    {
        if (data == null)
        {
            data = new Save2048(0, 2, null);
        }
        string json = SerializeJson2048(data);
        Debug.Log("Save2048: " + json);
        string path = Application.persistentDataPath + "/2048.json";
        File.WriteAllText(path, json);
    }
    
    public static Save2048 Load2048()
    {
        string path = Application.persistentDataPath + "/2048.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            Save2048 data = DeserializeJson2048(json);
            return data;
        }

        return new Save2048(0,2,null);
    }
    
    private static Save2048 DeserializeJson2048(string jsonString)
    {
        Save2048 data = JsonConvert.DeserializeObject<Save2048>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJson2048(Save2048 data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }    
    
//    public static void Save2048Data(SaveData2048 data)
//    {
////        string json = JsonUtility.ToJson(data);
//        string json = SerializeJsonSaveData2048(data);
//        Debug.Log("Serialize: " + json);
//        PlayerPrefs.SetString("SaveData2048", json);
//        PlayerPrefs.Save();
//    }
//    
//    public static SaveData2048 Load2048Data()
//    {
//        if (PlayerPrefs.HasKey("SaveData2048"))
//        {
//            string json = PlayerPrefs.GetString("SaveData2048");
//            // Проверка на пустую строку
//            if (string.IsNullOrEmpty(json))
//            {
//                return null;
//            }
////            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
//            SaveData2048 data = DeserializeJsonSaveData2048(json);
//            return data;
//        }
//        return null;
//    }
//    
//    private static SaveData2048 DeserializeJsonSaveData2048(string jsonString)
//    {
//        SaveData2048 data = JsonConvert.DeserializeObject<SaveData2048>(jsonString, new JsonSerializerSettings 
//        { 
//            TypeNameHandling = TypeNameHandling.Auto,
//            //NullValueHandling = NullValueHandling.Ignore,
//        });
//        return data;
//    }
//    
//    private static string SerializeJsonSaveData2048(SaveData2048 data)
//    {
//        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
//        {
//            TypeNameHandling = TypeNameHandling.Auto
//        });
//        return jsonString;
//    }
    
    #endregion
    
    #region ChineseCheckers

    public static void SaveChineseCheckers(SaveChineseCheckers data)
    {
        if (data == null)
        {
            data = new SaveChineseCheckers(1000, null, new List<PlayerState>(){PlayerState.Player,
                PlayerState.Robot,PlayerState.Robot,PlayerState.Robot,PlayerState.Robot,PlayerState.Robot});
        }
        string json = SerializeJsonChineseCheckers(data);
        Debug.Log("SaveChineseCheckers: " + json);
        string path = Application.persistentDataPath + "/ChineseCheckers.json";
        File.WriteAllText(path, json);
    }
    
    public static SaveChineseCheckers LoadChineseCheckers()
    {
        string path = Application.persistentDataPath + "/ChineseCheckers.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            SaveChineseCheckers data = DeserializeJsonChineseCheckers(json);
            return data;
        }

        return new SaveChineseCheckers(1000, null, new List<PlayerState>(){PlayerState.Player,
            PlayerState.Robot,PlayerState.Robot,PlayerState.Robot,PlayerState.Robot,PlayerState.Robot});
    }
    
    private static SaveChineseCheckers DeserializeJsonChineseCheckers(string jsonString)
    {
        SaveChineseCheckers data = JsonConvert.DeserializeObject<SaveChineseCheckers>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonChineseCheckers(SaveChineseCheckers data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    #region Lines98

    public static void SaveLines98(SaveLines98 data)
    {
        if (data == null)
        {
            data = new SaveLines98(0, null);
        }
        string json = SerializeJsonLines98(data);
        Debug.Log("SaveLines98: " + json);
        string path = Application.persistentDataPath + "/Lines98.json";
        File.WriteAllText(path, json);
    }
    
    public static SaveLines98 LoadLines98()
    {
        string path = Application.persistentDataPath + "/Lines98.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            SaveLines98 data = DeserializeJsonLines98(json);
            return data;
        }

        return new SaveLines98(0, null);
    }
    
    private static SaveLines98 DeserializeJsonLines98(string jsonString)
    {
        SaveLines98 data = JsonConvert.DeserializeObject<SaveLines98>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonLines98(SaveLines98 data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion 
    
    #region Blocks

    public static void SaveBlocks(SaveBlocks data)
    {
        if (data == null)
        {
            data = new SaveBlocks(0, null);
        }
        string json = SerializeJsonBlocks(data);
        Debug.Log("SaveBlocks: " + json);
        string path = Application.persistentDataPath + "/Blocks.json";
        File.WriteAllText(path, json);
    }
    
    public static SaveBlocks LoadBlocks()
    {
        string path = Application.persistentDataPath + "/Blocks.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            SaveBlocks data = DeserializeJsonBlocks(json);
            return data;
        }

        return new SaveBlocks(0, null);
    }

    private static SaveBlocks DeserializeJsonBlocks(string jsonString)
    {
        SaveBlocks data = JsonConvert.DeserializeObject<SaveBlocks>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonBlocks(SaveBlocks data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    public static void DeleteAllSave()
    {
        DeleteSave(Application.persistentDataPath + "/Tetris.json");
        DeleteSave(Application.persistentDataPath + "/Snake.json");
        DeleteSave(Application.persistentDataPath + "/2048.json");
        DeleteSave(Application.persistentDataPath + "/ChineseCheckers.json");
        DeleteSave(Application.persistentDataPath + "/Lines98.json");
        DeleteSave(Application.persistentDataPath + "/Blocks.json");
    }
    
    public static void DeleteSave(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Сохранение удалено: " + path);
        }
        else
        {
            Debug.Log("Файл сохранения не найден: " + path);
        }
    }
}