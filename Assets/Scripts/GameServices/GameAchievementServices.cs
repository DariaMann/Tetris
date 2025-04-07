using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameAchievementServices
{
    public static List<AchivementServices> ActivatedAchievementIds { get; set; } = new List<AchivementServices>();

    public static void Reset()
    {
        ActivatedAchievementIds = new List<AchivementServices>();
    }

    public static void SaveList()
    {
        AchivementListWrapper wrapper = new AchivementListWrapper { list = ActivatedAchievementIds };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("ActivatedAchievement", json);
        PlayerPrefs.Save();
    }

    public static List<AchivementServices> LoadList()
    {
        if (PlayerPrefs.HasKey("ActivatedAchievement"))
        {
            string json = PlayerPrefs.GetString("ActivatedAchievement");
            AchivementListWrapper wrapper = JsonUtility.FromJson<AchivementListWrapper>(json);
            ActivatedAchievementIds = wrapper.list;
            return ActivatedAchievementIds;
        }
        return new List<AchivementServices>();
    }
}


[Serializable]
public class AchivementListWrapper
{
    public List<AchivementServices> list = new List<AchivementServices>();
}