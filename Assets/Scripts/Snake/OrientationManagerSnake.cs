using UnityEngine;

public class OrientationManagerSnake : MonoBehaviour
{
    [SerializeField] private RectTransform topText;
    
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
    }
    
    private void VerticalOrientationPhone()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -150f);
    }

    private void HorizontalOrientationTablet()
    {
        topText.anchoredPosition = new Vector2(topText.anchoredPosition.x, -25f);
    }
}
