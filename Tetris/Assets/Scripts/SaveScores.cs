using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;

public class SaveScores : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI Record;
    [SerializeField] private TextMeshProUGUI InfAboutRecord;
    [SerializeField] private int scene;

    private string path;
    private int currentScore = 0;
    private int record = 0;

    public void Awake()
    {
        if (scene == 1)
        {
            path = Application.persistentDataPath + "/ScoresTetris.xml";
        }
        else if (scene == 2)
        {
            path = Application.persistentDataPath + "/ScoresSnake.xml";
        }
        else if (scene == 3)
        {
            path = Application.persistentDataPath + "/Scores2048.xml";
        }

        record = Load();
        ChangeScore(currentScore);
    }
    
    public void Save()
    {
        if (record < currentScore)
        {
            XElement root = new XElement("root");
            root.AddFirst(new XElement("score", currentScore));
            XDocument saveDoc = new XDocument(root);
            File.WriteAllText(path, saveDoc.ToString());
            record = currentScore;
            Record.text = record.ToString();
            InfAboutRecord.text = "Новый рекорд!";
        }
        else
        {
            Record.text = record.ToString();
        }
    }
    
    public int Load()//Предыдущий результат
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
    
    public void ChangeScore(int sc)
    {
        if (sc != 0)
        {
            sc = currentScore + sc;
        }
        currentScore = sc;
        ScoreText.text = currentScore.ToString();
        Save();
    }
}
