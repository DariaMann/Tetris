using UnityEngine;

public class CameraRenderer : MonoBehaviour
{
    public Vector2 DefaultResolution = new Vector2(720, 1280);
    [Range(0f, 1f)] public float WidthOrHeight = 0;

    private Camera componentCamera;

    private float initialSize;
    private float targetAspect;

    private float initialFov;
    private float horizontalFov = 120f;
    
    private bool orientationVertical;

    private void Start()
    {
        componentCamera = GetComponent<Camera>();
        initialSize = componentCamera.orthographicSize;

        targetAspect = DefaultResolution.x / DefaultResolution.y;

        initialFov = componentCamera.fieldOfView;
        horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);
        
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || 
             Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            WidthOrHeight = 1f;
            orientationVertical = false;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || 
                  Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            WidthOrHeight = 0f;
            orientationVertical = true;
        }
    }

    private void Update()
    {
//        bool isLandscape = Screen.width > Screen.height;
//        Debug.Log(isLandscape ? "Горизонтальная ориентация" : "Вертикальная ориентация");
        
        if ((Screen.orientation == ScreenOrientation.LandscapeLeft || 
            Screen.orientation == ScreenOrientation.LandscapeRight) && orientationVertical)
        {
            WidthOrHeight = 1f;
            orientationVertical = false;
        }
        else if ((Screen.orientation == ScreenOrientation.Portrait || 
                 Screen.orientation == ScreenOrientation.PortraitUpsideDown) && !orientationVertical)
        {
            WidthOrHeight = 0f;
            orientationVertical = true;
        }
        
        if (componentCamera.orthographic)
        {
            float constantWidthSize = initialSize * (targetAspect / componentCamera.aspect);
            componentCamera.orthographicSize = Mathf.Lerp(constantWidthSize, initialSize, WidthOrHeight);
        }
        else
        {
            float constantWidthFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);
            componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, WidthOrHeight);
        }
    }

    private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
    {
        float hFovInRads = hFovInDeg * Mathf.Deg2Rad;

        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

        return vFovInRads * Mathf.Rad2Deg;
    }
}