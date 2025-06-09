using UnityEngine;

public class OrientationManagerSnake : MonoBehaviour
{
    [SerializeField] private RectTransform topText;
    [SerializeField] private CameraRenderer cameraRenderer;
    
    void Update()
    {
        bool isTablet = GameHelper.IsTablet();
        if (Screen.width > Screen.height)
        {
            HorizontalOrientationTablet();
            
        }
        else
        {
            if (!isTablet)
            {
                VerticalOrientationPhone();
            }
            else
            {
                VerticalOrientationTablet();
            }
        }
    }
    
    private void VerticalOrientationTablet()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -113f);
        if (GameHelper.HaveAds)
        {
            cameraRenderer.zoomMultiplier = 1.1f;
        }
        else
        {
            cameraRenderer.zoomMultiplier = 1f;
        }
    }
    
    private void VerticalOrientationPhone()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -150f);
        cameraRenderer.zoomMultiplier = 1f;
    }

    private void HorizontalOrientationTablet()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -25f);
        if (GameHelper.HaveAds)
        {
            cameraRenderer.zoomMultiplier = 1.2f;
        }
        else
        {
            cameraRenderer.zoomMultiplier = 1f;
        }
    }
}
