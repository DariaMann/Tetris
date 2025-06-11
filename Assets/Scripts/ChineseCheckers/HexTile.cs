using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexTile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Vector2 position;
    [SerializeField] private float speedSelectionRotate = 4f; // Время одного оборота
    [SerializeField] private SpriteRenderer selection;
    [SerializeField] private TextMeshPro numberText;
    
    private CheckersManager _checkersManager;
    private SpriteRenderer _spriteRenderer;

    public int Priority { get; set; }
    
    public int Row { get; private set; }

    public int Col { get; private set; }

    public bool IsOccupied { get; private set; }
    
    public Chip Chip { get; private set; }
    
    public bool IsSelected { get; private set; }
    
    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    void Start()
    {
        float angle = 360f; // Определяем направление вращения
        selection.transform.DORotate(new Vector3(0, 0, angle), speedSelectionRotate, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart); // Бесконечный цикл
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Row = " + Row + ", Col = " + Col);
        if (_checkersManager.SelectedChip != null && IsSelected)
        {
            StartCoroutine(PlayerMoveCoroutine());
        }
    }
    
    public IEnumerator PlayerMoveCoroutine()
    {
        yield return StartCoroutine(_checkersManager.SelectedChip.MoveCoroutine(this));
        _checkersManager.StartNextTurn();
    }

    public void SetData(Vector2 pos, int row, int col, CheckersManager checkersManager)
    {
        Position = pos;
        Row = row;
        Col = col;
        numberText.text = row + ";" + Col;
        _checkersManager = checkersManager;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        SetOccupied(false);
    }

    public void Reposition(Vector2 pos)
    {
        Position = pos;
    }
    
    public void SetSelection(bool selected, bool showSelection = true)
    {
        IsSelected = selected;
        selection.gameObject.SetActive(showSelection && selected);
    }
    
    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }
    
    public void SetChip(Chip chip)
    {
        Chip = chip;
        SetOccupied(true);
    }
    
    public void SetTheme(Color colorTile, Color colorSelection)
    {
        _spriteRenderer.color = colorTile;
        selection.color = colorSelection;
    }
    
    public void RemoveChip()
    {
        Chip = null;
        SetOccupied(false);
    }
}