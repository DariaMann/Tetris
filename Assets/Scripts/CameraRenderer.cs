using UnityEngine;

public class CameraRenderer : MonoBehaviour
{
    public Vector2 DefaultResolution = new Vector2(720, 1280);
    [Range(0f, 1f)] public float horizontal = 1;
    [Range(0f, 1f)] public float vertical = 0;
    
    [Range(0f, 1f)] public float WidthOrHeight = 0;
    [Range(0.5f, 3f)] public float zoomMultiplier = 1f;

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
            WidthOrHeight = horizontal;
            orientationVertical = false;
            OnUpdateSaveArea();
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || 
                  Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            WidthOrHeight = vertical;
            orientationVertical = true;
            OnUpdateSaveArea();
        }
    }

    private void Update()
    {
        if (GameHelper.IsDoScreenshot)
        {
            return;
        }
        
//        bool isLandscape = Screen.width > Screen.height;
//        Debug.Log(isLandscape ? "Горизонтальная ориентация" : "Вертикальная ориентация");
        
        if ((Screen.orientation == ScreenOrientation.LandscapeLeft || 
            Screen.orientation == ScreenOrientation.LandscapeRight) && orientationVertical)
        {
            WidthOrHeight = horizontal;
            orientationVertical = false;
            OnUpdateSaveArea();
        }
        else if ((Screen.orientation == ScreenOrientation.Portrait || 
                 Screen.orientation == ScreenOrientation.PortraitUpsideDown) && !orientationVertical)
        {
            WidthOrHeight = vertical;
            orientationVertical = true;
            OnUpdateSaveArea();
        }
        
        if (componentCamera.orthographic)
        {
//            float constantWidthSize = initialSize * (targetAspect / componentCamera.aspect);
//            componentCamera.orthographicSize = Mathf.Lerp(constantWidthSize, initialSize, WidthOrHeight);
            
            float constantWidthSize = initialSize * (targetAspect / componentCamera.aspect);
            float size = Mathf.Lerp(constantWidthSize, initialSize, WidthOrHeight);
            componentCamera.orthographicSize = size * zoomMultiplier;
        }
        else
        {
            float constantWidthFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);
            componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, WidthOrHeight);
        }
    }
    
    public void ConfigureForTarget(int targetWidth, int targetHeight)
    {
        bool isLandscape = targetWidth > targetHeight;

        // Эмулируем поведение переключения Orientation, но вручную
        if (isLandscape)
        {
            WidthOrHeight = horizontal;
            orientationVertical = false;
        }
        else
        {
            WidthOrHeight = vertical;
            orientationVertical = true;
        }

        // Применяем сохранённые области вручную
//        OnUpdateSaveArea();

        // Принудительно обновим камеры/параметры сразу (как если бы Update уже прошёл)
        if (componentCamera == null)
            componentCamera = GetComponent<Camera>();
        
        // Эмулируем нужное соотношение сторон для цели
        float targetAspectRatio = (float)targetWidth / targetHeight;
//        componentCamera.aspect = targetAspectRatio;

        OnUpdateSaveArea();

        if (componentCamera.orthographic)
        {
            float constantWidthSize = initialSize * (targetAspect / targetAspectRatio);
            float size = Mathf.Lerp(constantWidthSize, initialSize, WidthOrHeight);
            componentCamera.orthographicSize = size * zoomMultiplier;
            Debug.Log(targetWidth + "x" + targetHeight + ": Camera size = " + size);
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
    
    public void OnUpdateSaveArea()
    {
        //todo: метод для отладки временный
        GUISaveArea[] allSaveAreas = FindObjectsOfType<GUISaveArea>();
        
        foreach (var area in allSaveAreas)
        {
            area.ApplySafeArea();
        }
    }
}