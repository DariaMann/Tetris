using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move1 : MonoBehaviour {

    public Transform figure;
    public Transform[] figures;
    void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = mousePos.x > 11.2f ? 11.2f : mousePos.x;//правая граница
        mousePos.x = mousePos.x < 0.7f ? 0.7f : mousePos.x;//левая граница
        figure.position = new Vector2(mousePos.x, figure.position.y);
        //this.figures[].position = new Vector2(mousePos.x, figure.position.y);
    }
    //void OnTriggerEnter2D(Collider2D other)
    //{

    //    if (other.CompareTag("Figure"))
    //    {
    //        Debug.Log("ура");
    //        //Application.LoadLevel(Application.loadedLevel);

    //    }
    //}
}
