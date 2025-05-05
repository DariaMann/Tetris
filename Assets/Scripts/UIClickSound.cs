using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIClickSound : MonoBehaviour
{
    void Awake()
    {
        if (TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(PlayClick);
        }
        else if (TryGetComponent<Toggle>(out var toggle))
        {
            toggle.onValueChanged.AddListener(_ => PlayClick());
        }
        else if (TryGetComponent<Slider>(out var slider))
        {
            slider.onValueChanged.AddListener(_ => PlayClick());
        }
    }

    private void PlayClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
    }
}