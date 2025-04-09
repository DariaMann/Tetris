using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile2024 : MonoBehaviour
{
    public TileState State { get; private set; }
    public TileCell Cell { get; private set; }
    
    public Transform Grid { get; private set; }
    public bool Locked { get; set; }

    private Image _background;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state)
    {
        this.State = state;

        _background.color = state.backgroundColor;
        _text.color = state.textColor;
        _text.text = state.number.ToString();

        int currentScore = state.number;
        switch (currentScore)
        {
            case 128: GameServicesManager.UnlockAchieve(AchivementServices.Tile128); break;
            case 256: GameServicesManager.UnlockAchieve(AchivementServices.Tile256); break;
            case 512: GameServicesManager.UnlockAchieve(AchivementServices.Tile512); break;
            case 1024: GameServicesManager.UnlockAchieve(AchivementServices.Tile1024); break;
            case 2048: GameServicesManager.UnlockAchieve(AchivementServices.Tile2048); break;
            case 4096: GameServicesManager.UnlockAchieve(AchivementServices.Tile4096); break;
            case 8192: GameServicesManager.UnlockAchieve(AchivementServices.Tile8192); break;
            case 16384: GameServicesManager.UnlockAchieve(AchivementServices.Tile16384); break;
        }
    }

    public void Spawn(TileCell cell, Transform grid)
    {
        if (this.Cell != null) {
            this.Cell.Tile = null;
        }

        this.Cell = cell;
        this.Cell.Tile = this;
        Grid = grid;

        transform.position = cell.transform.position;


        if (cell != null)
        {
            transform.SetParent(cell.transform);
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0,0);
            rectTransform.anchorMax = new Vector2(1f,1f);
            
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }


    }

    public void MoveTo(TileCell cell)
    {
        if (this.Cell != null) {
            this.Cell.Tile = null;
        }

        this.Cell = cell;
        this.Cell.Tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        if (this.Cell != null) {
            this.Cell.Tile = null;
        }

        this.Cell = null;
        cell.Tile.Locked = true;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        transform.SetParent(Grid);
        
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
        
        
        
        
        
        if (Cell != null)
        {
            transform.SetParent(Cell.transform);
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0,0);
            rectTransform.anchorMax = new Vector2(1f,1f);
            
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        
        
        
        

        if (merging) {
            Destroy(gameObject);
        }
    }

}