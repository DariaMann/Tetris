using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    [SerializeField] private CheckersManager checkersManager;
    
    [SerializeField] private GameObject hexPrefab; // Префаб гексагональной клетки
    [SerializeField] private float hexXOffset = 0.5f; // Горизонтальный шаг между клетками

    [SerializeField] private float hexYOffset = 0.4325f; // Вертикальный шаг между рядами

    [SerializeField] private GameObject prefabChip;
    [SerializeField] private Sprite redChip;
    [SerializeField] private Sprite blackChip;
    [SerializeField] private Sprite greenChip;
    [SerializeField] private Sprite yellowChip;
    [SerializeField] private Sprite violetChip;
    [SerializeField] private Sprite cyanChip;

    public int[] RowSizes { get; set; } = { 1, 2, 3, 4, 13, 12, 11, 10, 9, 10, 11, 12, 13, 4, 3, 2, 1 };

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
                Vector2 position = new Vector2(xPos, yPos);
                GameObject hex = Instantiate(hexPrefab, position, Quaternion.identity, transform);
                
                HexTile tile = hex.GetComponent<HexTile>();
                tile.SetData(position, row, col, checkersManager);
                Tiles.Add(tile);
            }
        }
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
                Chip chip = CreateChip(tile, ChoseChipByType(player));
                checkersManager.Players[player].Chips.Add(chip);
                chip.Player = checkersManager.Players[player];
                Chips.Add(chip);
//                tile.gameObject.GetComponent<SpriteRenderer>().color = _playerColors[player];
            }
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
    
    public Sprite ChoseChipByType(int color)
    {
        switch (color)
        {
            case 0: return blackChip;
            case 1: return redChip;
            case 2: return greenChip;
            case 3: return yellowChip;
            case 4: return violetChip;
            case 5: return cyanChip;
        }

        Debug.LogError("Нет фигуры такого цвета");
        return null;
    }

    private Chip CreateChip(HexTile tile, Sprite spritePrefab)
    {
        GameObject chipGameObject = Instantiate(prefabChip, tile.Position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = chipGameObject.GetComponent<SpriteRenderer>();
        chipGameObject.name = "Chip";
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
