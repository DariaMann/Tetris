using UnityEngine;

public class GUIDonatPanel : MonoBehaviour
{
    [SerializeField] private GameObject animationPanel;
    
    private void Show() => animationPanel.SetActive(true);
    
    private void Hide() => animationPanel.SetActive(false);

    public void OnCloudTipsClick()
    {
        Application.OpenURL("https://pay.cloudtips.ru/p/401f7806");
    }
    
    public void OnBoostyClick()
    {
        Application.OpenURL("https://boosty.to/combysoft/donate");
    }
    
    public void OnCloseClick()
    {
        Hide();
    }
    
    public void OnOpenClick()
    {
        Show();
    }
}