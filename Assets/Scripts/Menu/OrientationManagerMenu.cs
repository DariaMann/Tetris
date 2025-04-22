using UnityEngine;
using UnityEngine.UI;

public class OrientationManagerMenu: MonoBehaviour
{
    [SerializeField] private RectTransform buttonsPanel;
    [SerializeField] private VerticalLayoutGroup buttonslLayoutGroup;
    
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private ScrollRect scrollRect;

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
            
//            panel.anchoredPosition = landscapePosition;
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
//            panel.anchoredPosition = portraitPosition;
        }
    }
    
    private void VerticalOrientationTablet()
    {
        buttonsPanel.offsetMax = new Vector2(0, -160);
        buttonslLayoutGroup.padding.top = 0;
        RefreshUI();
    }
    
    private void VerticalOrientationPhone()
    {
        buttonsPanel.offsetMax= new Vector2(0, -160);
        buttonslLayoutGroup.padding.top = 0;
        RefreshUI();
    }

    private void HorizontalOrientationTablet()
    {
        buttonsPanel.offsetMax = new Vector2(0, 0);
        buttonslLayoutGroup.padding.top = 60;
        RefreshUI();
    }
    
    public void RefreshUI()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupParent);
        scrollRect.verticalNormalizedPosition = 1f; // Обновление скролла
    }
}