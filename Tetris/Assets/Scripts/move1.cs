using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move1 : MonoBehaviour {

    [SerializeField]
    Transform figure;
    float speed = 10f;
    void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     mousePos.x = mousePos.x > 11.2f ? 11.2f : mousePos.x;//правая граница
     mousePos.x = mousePos.x < 0.7f ? 0.7f : mousePos.x;//левая граница
        figure.position = Vector2.MoveTowards(figure.position,
            new Vector2(mousePos.x, figure.position.y), speed * Time.deltaTime) ;
    }
}
