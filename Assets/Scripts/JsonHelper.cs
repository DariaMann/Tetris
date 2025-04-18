using Newtonsoft.Json;
using UnityEngine;

public static class JsonHelper
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
    
    public static void SaveTetrisData(SaveDataTetris data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonSaveDataTetris(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("SaveDataTetris", json);
        PlayerPrefs.Save();
    }
    
    public static SaveDataTetris LoadTetrisData()
    {
        if (PlayerPrefs.HasKey("SaveDataTetris"))
        {
            string json = PlayerPrefs.GetString("SaveDataTetris");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            SaveDataTetris data = DeserializeJsonSaveDataTetris(json);
            return data;
        }
        return null;
    }
    
    private static SaveDataTetris DeserializeJsonSaveDataTetris(string jsonString)
    {
        SaveDataTetris data = JsonConvert.DeserializeObject<SaveDataTetris>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSaveDataTetris(SaveDataTetris data)
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
    
    public static void SaveSnakeData(SaveDataSnake data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonSaveDataSnake(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("SaveDataSnake", json);
        PlayerPrefs.Save();
    }
    
    public static SaveDataSnake LoadSnakeData()
    {
        if (PlayerPrefs.HasKey("SaveDataSnake"))
        {
            string json = PlayerPrefs.GetString("SaveDataSnake");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            SaveDataSnake data = DeserializeJsonSaveDataSnake(json);
            return data;
        }
        return null;
    }
    
    private static SaveDataSnake DeserializeJsonSaveDataSnake(string jsonString)
    {
        SaveDataSnake data = JsonConvert.DeserializeObject<SaveDataSnake>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSaveDataSnake(SaveDataSnake data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    #region 2048

    public static void Save2048Data(SaveData2048 data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonSaveData2048(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("SaveData2048", json);
        PlayerPrefs.Save();
    }
    
    public static SaveData2048 Load2048Data()
    {
        if (PlayerPrefs.HasKey("SaveData2048"))
        {
            string json = PlayerPrefs.GetString("SaveData2048");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            SaveData2048 data = DeserializeJsonSaveData2048(json);
            return data;
        }
        return null;
    }
    
    private static SaveData2048 DeserializeJsonSaveData2048(string jsonString)
    {
        SaveData2048 data = JsonConvert.DeserializeObject<SaveData2048>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSaveData2048(SaveData2048 data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    #region ChineseCheckers

    public static void SaveChineseCheckersData(SaveDataChineseCheckers data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonSaveDataChineseCheckers(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("SaveDataChineseCheckers", json);
        PlayerPrefs.Save();
    }
    
    public static SaveDataChineseCheckers LoadChineseCheckersData()
    {
        if (PlayerPrefs.HasKey("SaveDataChineseCheckers"))
        {
            string json = PlayerPrefs.GetString("SaveDataChineseCheckers");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            SaveDataChineseCheckers data = DeserializeJsonSaveDataChineseCheckers(json);
            return data;
        }
        return null;
    }
    
    private static SaveDataChineseCheckers DeserializeJsonSaveDataChineseCheckers(string jsonString)
    {
        SaveDataChineseCheckers data = JsonConvert.DeserializeObject<SaveDataChineseCheckers>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSaveDataChineseCheckers(SaveDataChineseCheckers data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
    
    #region Lines98

    public static void SaveLines98Data(SaveDataLines98 data)
    {
//        string json = JsonUtility.ToJson(data);
        string json = SerializeJsonSaveDataLines98(data);
        Debug.Log("Serialize: " + json);
        PlayerPrefs.SetString("SaveDataLines98", json);
        PlayerPrefs.Save();
    }
    
    public static SaveDataLines98 LoadLines98Data()
    {
        if (PlayerPrefs.HasKey("SaveDataLines98"))
        {
            string json = PlayerPrefs.GetString("SaveDataLines98");
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
//            SaveDataChineseCheckers data = JsonUtility.FromJson<SaveDataChineseCheckers>(json);
            SaveDataLines98 data = DeserializeJsonSaveDataLines98(json);
            return data;
        }
        return null;
    }
    
    private static SaveDataLines98 DeserializeJsonSaveDataLines98(string jsonString)
    {
        SaveDataLines98 data = JsonConvert.DeserializeObject<SaveDataLines98>(jsonString, new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            //NullValueHandling = NullValueHandling.Ignore,
        });
        return data;
    }
    
    private static string SerializeJsonSaveDataLines98(SaveDataLines98 data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return jsonString;
    }
    
    #endregion
}