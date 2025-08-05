using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    private Snake _snake;
    private FoodController _foodController;
    
    public Vector2Int Position { get; set; }

    private void Awake()
    {
        _snake = FindObjectOfType<Snake>();
        _foodController = FindObjectOfType<FoodController>();
    }
    
    public void RandomizePosition()
    {
        Bounds bounds = _foodController.GridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the food from spawning on the snake
        while (_snake.OccupiesIncludingNext(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y) {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        Position = new Vector2Int(x, y);
        transform.position = new Vector2(x, y);
    }
    
    public void LoadPosition(Vector2 position)
    {
        transform.position = position;
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        Position = new Vector2Int(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameHelper.SnakeSettings.ManyFood)
        {
            _foodController.CreateNewFoods();
            if (_foodController.Foods.Count > 1)
            {
                _foodController.Foods.Remove(this);
                Destroy(gameObject);
                return;
            }
        }

        RandomizePosition();
    }

}