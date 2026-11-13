using UnityEngine;

public class GUIDonatPanel : MonoBehaviour
{
    [SerializeField] private GameObject animationPanel;
    
    private void Show() => animationPanel.SetActive(true);
    
    private void Hide() => animationPanel.SetActive(false);

    public void OnCloudTipsClick()
    {
#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_STANDALONE_OSX)
        if (AnalyticsManager.Instance != null)
            AnalyticsManager.Instance.LogEvent(AnalyticType.donation_cloudtips_click.ToString());
        Application.OpenURL("https://pay.cloudtips.ru/p/401f7806");
#endif
    }
    
    public void OnBoostyClick()
    {
#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_STANDALONE_OSX)
        if (AnalyticsManager.Instance != null)
            AnalyticsManager.Instance.LogEvent(AnalyticType.donation_boosty_click.ToString());
        Application.OpenURL("https://boosty.to/combysoft/donate");
#endif
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