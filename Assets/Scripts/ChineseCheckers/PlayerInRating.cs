using Assets.SimpleLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInRating :MonoBehaviour
{
    [SerializeField] private HexMap map;
    
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private TextMeshProUGUI namePlayer;
    [SerializeField] private TextMeshProUGUI steps;
    [SerializeField] private Image chipsType;
    
    public PlayerState State { get; set; }

    public void SetData(Player player)
    {
        number.text = player.WinNumber.ToString();
        State = player.State;
        switch (State)
        {
            case PlayerState.Robot:
                namePlayer.text = LocalizationManager.Localize("Сheckers.robot");
                break;
            case PlayerState.Player:
                namePlayer.text = LocalizationManager.Localize("Сheckers.player");
                break;
        }
        steps.text = LocalizationManager.Localize("Сheckers.steps") + ": " + player.WinSteps;
        chipsType.sprite = map.ChoseChipByType(player.ID);
    }
}