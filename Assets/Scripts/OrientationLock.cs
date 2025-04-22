using System.Collections;
using UnityEngine;

public class OrientationLock : MonoBehaviour
{
    private void Awake()
    {
        bool isTablet = GameHelper.IsTablet();

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