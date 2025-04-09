using Newtonsoft.Json;
using UnityEngine;

public static class JsonHelper
{
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
}