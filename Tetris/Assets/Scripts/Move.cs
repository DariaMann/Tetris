using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour {

    [SerializeField]
    private GameObject[] tetrisObjects;
    private GameObject figure;
    [SerializeField]
    Text Score;

    void Start () {
        AppearanceRandome();
    }
	public void AppearanceRandome()
    {
        int index = Random.Range(0, tetrisObjects.Length-1);
        Instantiate(tetrisObjects[index], transform.position, Quaternion.identity);
    }
    public void Records(int score)
    {
        Score.text = score.ToString();
        FindObjectOfType<SaveScores>().Save();
    }
    }
