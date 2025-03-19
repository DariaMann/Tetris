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

    public void SetData(Player player)
    {
        number.text = player.WinNumber.ToString();
        namePlayer.text = player.Name;
        steps.text = "Steps: " + player.WinSteps;
        chipsType.sprite = map.ChoseChipByType(player.ID);
    }
}