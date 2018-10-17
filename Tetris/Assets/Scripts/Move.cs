using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour {

    [SerializeField]
    private GameObject[] tetrisObjects;
    private GameObject figure;

    public Text Score;

    void Start () {
        AppearanceRandome();
        //Debug.Log("2");
    }
	public void AppearanceRandome()
    {
        int index = Random.Range(0, tetrisObjects.Length);
        Instantiate(tetrisObjects[index], transform.position, Quaternion.identity);
        //figure = tetrisObjects[index];
       // Instantiate(figure, transform.position, Quaternion.identity);
    }
    public void Records(int score)
    {
        Score.text = score.ToString();
        FindObjectOfType<SaveScores>().Save();
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Figure"))
        {
            Application.LoadLevel(Application.loadedLevel);

        }
    }

    }
