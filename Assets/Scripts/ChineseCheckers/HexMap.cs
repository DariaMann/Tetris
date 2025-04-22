using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    [SerializeField] private CheckersManager checkersManager;
    
    [SerializeField] private GameObject hexPrefab; // Префаб гексагональной клетки
    [SerializeField] private float yOffset = -0.5f; // Горизонтальный шаг между клетками
    [SerializeField] private float hexXOffset = 0.5f; // Горизонтальный шаг между клетками

    [SerializeField] private float hexYOffset = 0.4325f; // Вертикальный шаг между рядами

    [SerializeField] private GameObject prefabChip;

    public int[] RowSizes { get; set; } = { 1, 2, 3, 4, 13, 12, 11, 10, 9, 10, 11, 12, 13, 4, 3, 2, 1 };
    
    public static readonly Dictionary<int, Vector2Int[]> PriorityOneCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(13, 0), new Vector2Int( 13, 1), new Vector2Int( 13, 2), new Vector2Int( 13, 3)} },
        { 2, new Vector2Int[] {
            new Vector2Int(14, 0), new Vector2Int(14, 1), new Vector2Int( 14, 2)} },
        { 3, new Vector2Int[] { 
            new Vector2Int( 15, 0), new Vector2Int(15, 1)} },
        { 4, new Vector2Int[] { new Vector2Int( 16, 0)} }
    };
    
    public static readonly Dictionary<int, Vector2Int[]> PriorityTwoCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(9, 0), new Vector2Int( 10, 1), new Vector2Int( 11, 2), new Vector2Int( 12, 3)} },
        { 2, new Vector2Int[] {
            new Vector2Int( 10, 0), new Vector2Int( 11, 1), new Vector2Int( 12, 2)} },
        { 3, new Vector2Int[] { 
            new Vector2Int( 11, 0), new Vector2Int( 12, 1)} },
        { 4, new Vector2Int[] { new Vector2Int( 12, 0)} }
    };
    
    public static readonly Dictionary<int, Vector2Int[]> PriorityThreeCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(7, 0), new Vector2Int( 6, 1), new Vector2Int( 5, 2), new Vector2Int( 4, 3)} },
        { 2, new Vector2Int[] { 
            new Vector2Int( 6, 0), new Vector2Int( 5, 1), new Vector2Int( 4, 2)} },
        { 3, new Vector2Int[] { 
            new Vector2Int( 5, 0), new Vector2Int( 4, 1)} },
        { 4, new Vector2Int[] { new Vector2Int( 4, 0)} }
    };
    
    public static readonly Dictionary<int, Vector2Int[]> PriorityFourCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(3, 0), new Vector2Int( 3, 1), new Vector2Int( 3, 2), new Vector2Int( 3, 3)} },
        { 2, new Vector2Int[] {
            new Vector2Int( 2, 0), new Vector2Int( 2, 1), new Vector2Int( 2, 2)} },
        { 3, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 1, 1)} },
        { 4, new Vector2Int[] { new Vector2Int( 0, 0)} }
    };
    
    public static readonly Dictionary<int, Vector2Int[]> PriorityFiveCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(4, 9), new Vector2Int( 5, 9), new Vector2Int( 6, 9), new Vector2Int( 7, 9)} },
        { 2, new Vector2Int[] {
            new Vector2Int( 4, 10), new Vector2Int( 5, 10), new Vector2Int( 6, 10)} },
        { 3, new Vector2Int[] {
            new Vector2Int( 4, 11), new Vector2Int( 5, 11)} },
        { 4, new Vector2Int[] { new Vector2Int( 4, 12)} }
    };
    
    public static readonly Dictionary<int, Vector2Int[]> PrioritySixCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(9, 9), new Vector2Int( 10, 9), new Vector2Int( 11, 9), new Vector2Int( 12, 9)} },
        { 2, new Vector2Int[] {
            new Vector2Int( 10, 10), new Vector2Int( 11, 10), new Vector2Int( 12, 10)} },
        { 3, new Vector2Int[] {
            new Vector2Int( 11, 11), new Vector2Int( 12, 11)} },
        { 4, new Vector2Int[] { new Vector2Int( 12, 12) } }
    };

    public static readonly Dictionary<int, Vector2Int[]> PriorityCells = new Dictionary<int, Vector2Int[]>()
    {
        { 1, new Vector2Int[] {
            new Vector2Int(13, 0), new Vector2Int( 13, 1), new Vector2Int( 13, 2), new Vector2Int( 13, 3),
            new Vector2Int(9, 0), new Vector2Int( 10, 1), new Vector2Int( 11, 2), new Vector2Int( 12, 3),
            new Vector2Int(7, 0), new Vector2Int( 6, 1), new Vector2Int( 5, 2), new Vector2Int( 4, 3),
            new Vector2Int(3, 0), new Vector2Int( 3, 1), new Vector2Int( 3, 2), new Vector2Int( 3, 3),
            new Vector2Int(4, 9), new Vector2Int( 5, 9), new Vector2Int( 6, 9), new Vector2Int( 7, 9),
            new Vector2Int(9, 9), new Vector2Int( 10, 9), new Vector2Int( 11, 9), new Vector2Int( 12, 9)
        } },
        { 2, new Vector2Int[] {
            new Vector2Int(14, 0), new Vector2Int(14, 1), new Vector2Int( 14, 2), 
            new Vector2Int( 10, 0), new Vector2Int( 11, 1), new Vector2Int( 12, 2), 
            new Vector2Int( 6, 0), new Vector2Int( 5, 1), new Vector2Int( 4, 2), 
            new Vector2Int( 2, 0), new Vector2Int( 2, 1), new Vector2Int( 2, 2), 
            new Vector2Int( 4, 10), new Vector2Int( 5, 10), new Vector2Int( 6, 10), 
            new Vector2Int( 10, 10), new Vector2Int( 11, 10), new Vector2Int( 12, 10)
        } },
        { 3, new Vector2Int[] { 
            new Vector2Int( 15, 0), new Vector2Int(15, 1), 
            new Vector2Int( 11, 0), new Vector2Int( 12, 1),
            new Vector2Int( 5, 0), new Vector2Int( 4, 1),
            new Vector2Int( 1, 0), new Vector2Int( 1, 1),
            new Vector2Int( 4, 11), new Vector2Int( 5, 11),
            new Vector2Int( 11, 11), new Vector2Int( 12, 11) 
        } },
        { 4, new Vector2Int[] { new Vector2Int( 16, 0), new Vector2Int( 12, 0), new Vector2Int( 4, 0), new Vector2Int( 0, 0),
            new Vector2Int( 4, 12),new Vector2Int( 12, 12) } }
    };

    public float HexXOffset
    {
        get => hexXOffset;
        set => hexXOffset = value;
    }

    public float HexYOffset
    {
        get => hexYOffset;
        set => hexYOffset = value;
    }

    public List<HexTile> Tiles { get; set; } = new List<HexTile>();
    
    public List<Chip> Chips { get; set; } = new List<Chip>();

    public void GenerateBoard()
    {
        int totalRows = RowSizes.Length;
        int halfRows = totalRows / 2;

        for (int row = 0; row < totalRows; row++)
        {
            int rowSize = RowSizes[row];
            float yPos = (halfRows - row) * hexYOffset;
            float startX = -(rowSize - 1) * hexXOffset / 2;

            for (int col = 0; col < rowSize; col++)
            {
                float xPos = startX + col * hexXOffset;
                Vector2 position = new Vector2(xPos, yPos + yOffset);
                GameObject hex = Instantiate(hexPrefab, position, Quaternion.identity, transform);
                
                HexTile tile = hex.GetComponent<HexTile>();
                tile.SetData(position, row, col, checkersManager);
                Tiles.Add(tile);
            }
        }
        checkersManager.ThemeChinese.SetTheme(GameHelper.Theme);
    }

    public void SetPriority(int idPlayer)
    {
        var priorityCells = GetPriorityCells(idPlayer);
        foreach (var hexTile in Tiles)
        {
            hexTile.Priority = GetPriority(hexTile, priorityCells);
        }
    
        SetNegativePriority(idPlayer);
    }

    public void SetNegativePriority(int idPlayer)
    {
        var negativePriorityCells = GetPriorityCells((idPlayer + 3) % 6); // Смещение на 3 для получения противоположного набора
        foreach (var hexTile in Tiles)
        {
            int priority = -GetPriority(hexTile, negativePriorityCells);
            if (priority != 0)
            {
                hexTile.Priority = priority;
            }
        }
    }

    private Dictionary<int, Vector2Int[]> GetPriorityCells(int idPlayer)
    {
        switch (idPlayer)
        {
            case 0: return PriorityFourCells;
            case 1: return PriorityFiveCells;
            case 2: return PrioritySixCells;
            case 3: return PriorityOneCells;
            case 4: return PriorityTwoCells;
            case 5: return PriorityThreeCells;
            default:  return PriorityCells;
        }
    }

    private int GetPriority(HexTile tile, Dictionary<int, Vector2Int[]> priorityCells)
    {
        foreach (var priority in priorityCells)
        {
            foreach (var priorityCell in priority.Value)
            {
                if (tile.Row == priorityCell.x && tile.Col == priorityCell.y)
                {
                    return priority.Key;
                }
            }
        }

        return 0;
    }

    public void StartFilling()
    {
        foreach (var tile in Tiles)
        {
            int rowSize = RowSizes[tile.Row];
            
            // Определяем, принадлежит ли клетка какому-то игроку
            int player = GetPlayerZone(tile.Row, tile.Col, rowSize);
            if (player != -1)
            {
                if (!checkersManager.Players[player].IsActive)
                {
                    continue;
                }
                Chip chip = CreateChip(tile, ChooseChipByColor(checkersManager.Players[player].Color), player);
                checkersManager.Players[player].Chips.Add(chip);
                chip.Player = checkersManager.Players[player];
                Chips.Add(chip);
//                tile.gameObject.GetComponent<SpriteRenderer>().color = _playerColors[player];
            }
        }
    }
    
    public void StartSave(List<SaveChip> saveChips)
    {
        foreach (var data in saveChips)
        {
            HexTile tile = Tiles.Find(t => t.Row == data.Row && t.Col == data.Col);
            Chip chip = CreateChip(tile, ChooseChipByColor( checkersManager.Players[data.IdPlayer].Color), data.IdPlayer);
            checkersManager.Players[data.IdPlayer].Chips.Add(chip);
            chip.Player = checkersManager.Players[data.IdPlayer];
            Chips.Add(chip);
        }
    }

    public void ClearChips()
    {
        foreach (var tile in Tiles)
        {
            tile.RemoveChip();
        }
        for (int i = Chips.Count-1; i >= 0; i--)
        {
            Destroy(Chips[i]);
        }
        Chips.Clear();
        foreach (var player in checkersManager.Players)
        {
            player.Chips.Clear();
        }
        checkersManager.SetSelection(null);
    }
    
    public Sprite ChooseChipByColor(Color playerColor)
    {
        if (checkersManager.СolorToChipMap.TryGetValue(playerColor, out var chip))
            return chip;

        Debug.LogError("Нет фигуры такого цвета");
        return null;
    }

    private Chip CreateChip(HexTile tile, Sprite spritePrefab, int player)
    {
        string name = "";
        switch (player)
        {
            case 0: name = "Blue"; break;
            case 1: name = "Red"; break;
            case 2: name = "Green"; break;
            case 3: name = "Yellow"; break;
            case 4: name = "Violet"; break;
            case 5: name = "Cyan"; break;
        }
        
        GameObject chipGameObject = Instantiate(prefabChip, tile.Position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = chipGameObject.GetComponent<SpriteRenderer>();
        chipGameObject.name = "Chip"+name;
        spriteRenderer.sprite = spritePrefab;
        spriteRenderer.sortingOrder = 2;
        Destroy(chipGameObject.GetComponent<HexTile>());
        Chip chip = chipGameObject.AddComponent<Chip>();
//        Chip chip = chipGameObject.GetComponent<Chip>();
        chip.SetData(tile, checkersManager);
        return chip;
    }

    int GetPlayerZone(int row, int col, int rowSize)
    {
        // Нижний игрок (последние 4 ряда)
        if (row >= RowSizes.Length - 4) return 0;
        // Верхний игрок (первые 4 ряда)
        if (row < 4) return 3;
        // Левый верхний игрок (левый сектор)
        if (row >= 4 && row < 8 && rowSize - 9 > col) return 2;
        // Левый нижний игрок (левый сектор)
        if (row >= 9 && row < 13 && rowSize - 9 > col) return 1;
        // Правый верхний игрок (правый сектор)
        if (row >= 4 && row < 8 && rowSize - col <= rowSize - 9) return 4;
        // Правый нижний игрок (правый сектор)
        if (row >= 9 && row < 13 && rowSize - col <= rowSize - 9) return 5;

        return -1; // Не является зоной игрока
    }
}
