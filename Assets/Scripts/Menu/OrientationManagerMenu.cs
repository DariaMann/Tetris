using UnityEngine;
using UnityEngine.UI;

public class OrientationManagerMenu: MonoBehaviour
{
    [SerializeField] private GameObject settingsUpButton;
    [SerializeField] private GameObject settingsCornerUpButton;
    
    private int _type = -1; //0 - планшет горизонт, 1 - планшет вертикаль, 2 - телефон вертикаль
    void Update()
    {
        bool isTablet = GameHelper.IsTablet();
        if (Screen.width > Screen.height) // Горизонтальная ориентация
        {
            if (_type == 0)
            {
                return;
            }

            _type = 0;
            HorizontalOrientationTablet();
        }
        else // Вертикальная ориентация
        {
            if (!isTablet)
            {
                if (_type == 2)
                {
                    return;
                }

                _type = 2;
                VerticalOrientationPhone();
            }
            else
            {
                if (_type == 1)
                {
                    return;
                }

                _type = 1;
                VerticalOrientationTablet();
            }
        }
    }
    
    private void VerticalOrientationTablet()
    {
        settingsUpButton.SetActive(true);
        settingsCornerUpButton.SetActive(false);
    }
    
    private void VerticalOrientationPhone()
    {
        settingsUpButton.SetActive(true);
        settingsCornerUpButton.SetActive(false);
    }

    private void HorizontalOrientationTablet()
    {
        settingsUpButton.SetActive(false);
        settingsCornerUpButton.SetActive(true);
    }
}