using System;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

public class ScreenshotTaker : MonoBehaviour
{
    [SerializeField, CanBeNull] private OrientationManagerSnake orientationManagerSnake;
    [SerializeField, CanBeNull] private OrientationManager2048 orientationManager2048;
    [SerializeField, CanBeNull] private OrientationManagerTetris orientationManagerTetris;
    [SerializeField, CanBeNull] private OrientationManagerLines98 orientationManagerLines98;
    [SerializeField, CanBeNull] private OrientationManagerChineseCheckers orientationManagerChineseCheckers;
    [SerializeField, CanBeNull] private OrientationManagerBlocks orientationManagerBlocks;
    [SerializeField, CanBeNull] private CameraRenderer cameraRenderer;
    [SerializeField, CanBeNull] private SquareUI squareUi;
    [SerializeField, CanBeNull] private SquareUIGrid squareUiGrid;
    
    public static ScreenshotTaker Instance;
    
    [System.Serializable]
    public struct ScreenshotSize
    {
        public string name;
        public int width;
        public int height;
        public bool isTablet;
        public bool isVertical;
    }

    public ScreenshotSize[] sizes = new ScreenshotSize[]
    {
        new ScreenshotSize { name = "iPhone_6.7", width = 1290, height = 2796, isTablet = false, isVertical = true},
        new ScreenshotSize { name = "iPhone_6.5", width = 1284, height = 2778, isTablet = false, isVertical = true },
        
        new ScreenshotSize { name = "iPad_12.9.ver", width = 2048, height = 2732, isTablet = true, isVertical = true },
        new ScreenshotSize { name = "iPad_12.9.hor", width = 2732, height = 2048, isTablet = true, isVertical = false },
        
        new ScreenshotSize { name = "Android_16_9", width = 1920, height = 1080, isTablet = true, isVertical = false },
        new ScreenshotSize { name = "Android_9_16", width = 1080, height = 1920, isTablet = false, isVertical = true}
    };
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
//            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            TakeScreenshots();
        }
    }
    
    [ContextMenu("Take All Screenshots")]
    public void TakeScreenshots()
    {
        GameHelper.IsDoScreenshot = true;
        StartCoroutine(TakeAllScreenshotsCoroutine());
    }

    private System.Collections.IEnumerator TakeAllScreenshotsCoroutine()
    {
        foreach (var size in sizes)
        {
            if (orientationManagerSnake != null)
            {
                orientationManagerSnake.SetCorrectUI(size.height, size.width, size.isTablet, size.isVertical);
            } 
            if (orientationManager2048 != null)
            {
                orientationManager2048.SetCorrectUI(size.height, size.width, size.isTablet, size.isVertical);
            }
            if (orientationManagerBlocks != null)
            {
                orientationManagerBlocks.SetCorrectUI(size.height, size.width, size.isTablet, size.isVertical);
            } 
            if (orientationManagerLines98 != null)
            {
                orientationManagerLines98.SetCorrectUI(size.height, size.width, size.isTablet, size.isVertical);
            }
            if (orientationManagerTetris != null)
            {
                orientationManagerTetris.SetCorrectUI(size.height, size.width, size.isTablet, size.isVertical);
            } 
            if (orientationManagerChineseCheckers != null)
            {
                orientationManagerChineseCheckers.SetCorrectUI(size.height, size.width, size.isTablet, size.isVertical);
            }
            
            Canvas.ForceUpdateCanvases();

            if (cameraRenderer != null)
            {
                cameraRenderer.ConfigureForTarget(size.width, size.height);
            } 
            if (squareUi != null)
            {
                squareUi.ResizeSquare();
            } 
            if (squareUiGrid != null)
            {
                squareUiGrid.ResizeSquare();
            }
            
            Canvas.ForceUpdateCanvases();
            
            yield return new WaitForEndOfFrame();

            var rt = new RenderTexture(size.width, size.height, 24);
            Camera.main.targetTexture = rt;
            var screenShot = new Texture2D(size.width, size.height, TextureFormat.RGB24, false);
            Camera.main.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, size.width, size.height), 0, 0);
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            byte[] bytes = screenShot.EncodeToPNG();
            string directory = Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string name = GameHelper.GameType.ToString() + "_" + size.width + "x" + size.height;
            string filename = Path.Combine(directory, name + ".png");
            File.WriteAllBytes(filename, bytes);
            Debug.Log("Saved screenshot to: " + filename);
        }
        
        GameHelper.IsDoScreenshot = false;
    }
}