using System;
using UnityEngine;

public class BannerAds : MonoBehaviour
{
    [SerializeField] private RectTransform banner;
    
    private RectTransform _canvasRect;

    private void Awake()
    {
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        _canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        SetSize();
    }

    private void SetSize()
    {
        // Получаем высоту канваса
        float canvasHeight = _canvasRect.rect.height;

//        float desiredHeight = Mathf.Max(canvasHeight * 0.1f, 100f);
        float desiredHeight = canvasHeight * 0.1f;
        
        banner.offsetMin = new Vector2(banner.offsetMin.x, 0);  
        banner.offsetMax = new Vector2(banner.offsetMax.x, desiredHeight);
    }
}