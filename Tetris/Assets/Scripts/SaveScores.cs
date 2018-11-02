using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;
using System;

public class SaveScores : MonoBehaviour {

    private string path;
    [SerializeField]
    Text ScoreText;
    [SerializeField]
    Text Record;
    [SerializeField]
    Text InfAboutRecord;
    [SerializeField]
    int D;
    void Start()
    {
        Save();
    }
    void Update()
    {

        Save();


    }
    public void Save()
    {
        
        int N = Load();
        D = Convert.ToInt32(ScoreText.text);
        if (N < D)
        {
            XElement root = new XElement("root");
            root.AddFirst(new XElement("score", D));
            XDocument saveDoc = new XDocument(root);
            File.WriteAllText(path, saveDoc.ToString());
            Record.text = D.ToString();
            InfAboutRecord.text = "Новый рекорд!";
        }
        else
        {


            Record.text = N.ToString();
        }
    }
    public void Awake()
    {
        path = Application.persistentDataPath + "/ScoresTetris.xml";
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
        return (0);
    }
}
