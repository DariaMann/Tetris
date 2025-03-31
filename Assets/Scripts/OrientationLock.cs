using System.Collections;
using UnityEngine;

public class OrientationLock : MonoBehaviour
{
    private void Awake()
    {
        bool isTablet = IsTablet();

        if (!isTablet)
        {
            StartCoroutine(SetPortrait());
        }
    }
    
    private IEnumerator SetPortrait()
    {
        yield return null;
        Screen.orientation = ScreenOrientation.Portrait;
        OnUpdateSaveArea();
    }

    private void OnDestroy()
    {
        // Восстанавливаем автоориентацию при выходе из сцены
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    
    private bool IsTablet()
    {
        float dpi = Screen.dpi;
        float width = Screen.width / dpi;
        float height = Screen.height / dpi;
        float diagonalInInches = Mathf.Sqrt(width * width + height * height);

        return diagonalInInches >= 6.5f; // Обычно планшеты > 6.5 дюймов
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