using UnityEngine;

public class GUIDonatPanel : MonoBehaviour
{
    [SerializeField] private GameObject animationPanel;
    
    private void Show() => animationPanel.SetActive(true);
    
    private void Hide() => animationPanel.SetActive(false);

    public void OnCloudTipsClick()
    {
        if (AnalyticsManager.Instance != null)
            AnalyticsManager.Instance.LogEvent(AnalyticType.donation_cloudtips_click.ToString());
        Application.OpenURL("https://pay.cloudtips.ru/p/401f7806");
    }
    
    public void OnBoostyClick()
    {
        if (AnalyticsManager.Instance != null)
            AnalyticsManager.Instance.LogEvent(AnalyticType.donation_boosty_click.ToString());
        Application.OpenURL("https://boosty.to/combysoft/donate");
    }
    
    public void OnCloseClick()
    {
        Hide();
    }
    
    public void OnOpenClick()
    {
        Show();
        if (AnalyticsManager.Instance != null)
            AnalyticsManager.Instance.LogEvent(AnalyticType.donation_panel_open.ToString());
    }
}