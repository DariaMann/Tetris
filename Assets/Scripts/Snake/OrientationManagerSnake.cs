using UnityEngine;

public class OrientationManagerSnake : MonoBehaviour
{
    [SerializeField] private RectTransform topText;
    [SerializeField] private Transform cameraRect;
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
            cameraRect.position = new Vector3(cameraRect.position.x, 0.8f, cameraRect.position.z);
            cameraRenderer.zoomMultiplier = 1.1f;
        }
        else
        {
            cameraRect.position = new Vector3(cameraRect.position.x, 1.8f, cameraRect.position.z);
            cameraRenderer.zoomMultiplier = 1f;
        }
    }
    
    private void VerticalOrientationPhone()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -150f);
        cameraRect.position = new Vector3(cameraRect.position.x, 1.8f, cameraRect.position.z);
        cameraRenderer.zoomMultiplier = 1f;
    }

    private void HorizontalOrientationTablet()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -25f);
        if (GameHelper.HaveAds)
        {
            cameraRect.position = new Vector3(cameraRect.position.x, 1.2f, cameraRect.position.z);
            cameraRenderer.zoomMultiplier = 1.2f;
        }
        else
        {
            cameraRect.position = new Vector3(cameraRect.position.x, 1.8f, cameraRect.position.z);
            cameraRenderer.zoomMultiplier = 1f;
        }
    }
}
