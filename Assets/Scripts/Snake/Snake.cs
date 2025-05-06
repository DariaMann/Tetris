using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private FoodController foodController;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private Vector2Int direction = Vector2Int.right;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private int initialSize = 4;
    private int minSpeed = 5;

    private readonly List<Transform> segments = new List<Transform>();
    private float nextUpdate;
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isDragging = false;
    private bool hasMovedThisFrame = false;
    private readonly Queue<Vector2Int> directionQueue = new Queue<Vector2Int>();

    public bool IsPaused { get; private set; } = false;

    private void Start()
    {
        LoadLastPlay();
    }
    
    void OnApplicationQuit()
    {
        SaveLastPlay();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLastPlay();
        }
    }
    
    private void OnDestroy()
    {
        SaveLastPlay();
    }
    
    public void PausedGame(bool isPause)
    {
        IsPaused = isPause;
    }
    
    private void LoadLastPlay()
    {
        SaveDataSnake saveData = GameHelper.SaveSnake.SaveDataSnake;
        if (saveData == null)
        {
            ResetState();
            return;
        }

        LoadSave(saveData);
    }
    
    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveSnake.SaveDataSnake = null;
            JsonHelper.SaveSnake(GameHelper.SaveSnake);
            return;
        }

        SaveDataSnake data = new SaveDataSnake(saveScores.IsWin, transform.position, foodController.Foods, direction, saveScores.CurrentScore);
        GameHelper.SaveSnake.SaveDataSnake = data;
        JsonHelper.SaveSnake(GameHelper.SaveSnake);
    }

    public void GameOver()
    {
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }
    
    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        
        ResetState();
    }

    private void Update()
    {
        if (gameOver.IsGameOver || IsPaused)
        {
            return;
        }

        HandleInput();
        
        // Обновление позиции змейки
        if (Time.time >= nextUpdate)
        {
            ApplyNextDirection();
            Move();
            hasMovedThisFrame = true;

            SetSaveSpeed(GameHelper.SnakeSettings);
            nextUpdate = Time.time + (1f / (speed * speedMultiplier));
        }
    }
    
    private void HandleInput()
    {
        // Мгновенное реагирование на клавиши
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                TryChangeDirection(Vector2Int.up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                TryChangeDirection(Vector2Int.down);
        }
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                TryChangeDirection(Vector2Int.right);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                TryChangeDirection(Vector2Int.left);
        }

        // Свайпы на телефоне
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    touchEndPos = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        // Движение мышью
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            touchEndPos = Input.mousePosition;
            isDragging = false;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = touchEndPos - touchStartPos;

        if (swipeDelta.magnitude < 50) return; // Игнорируем маленькие движения

        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            if (swipeDelta.x > 0 && direction.y != 0f) // Двигаемся вправо, если змейка идет вверх или вниз
            {
                TryChangeDirection(Vector2Int.right);
            }
            else if (swipeDelta.x < 0 && direction.y != 0f) // Двигаемся влево, если змейка идет вверх или вниз
            {
                TryChangeDirection(Vector2Int.left);
            }
        }
        else
        {
            if (swipeDelta.y > 0 && direction.x != 0f) // Двигаемся вверх, если змейка идет влево или вправо
            {
                TryChangeDirection(Vector2Int.up);
            }
            else if (swipeDelta.y < 0 && direction.x != 0f) // Двигаемся вниз, если змейка идет влево или вправо
            {
                TryChangeDirection(Vector2Int.down);
            }
        }
    }
    
    private void Move()
    {
        // Перемещаем тело змейки
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        // Перемещаем голову
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);
        RotateHead();
    }
    
//    private void FixedUpdate()
//    {
//        hasMovedThisFrame = false;
//
//        if (gameOver.IsGameOver || IsPaused)
//            return;
//
//        if (Time.time < nextUpdate)
//            return;
//
//        // Применить следующее направление
//        ApplyNextDirection();
//
//        Move();
//        hasMovedThisFrame = true;
//
//        SetSaveSpeed(GameHelper.SnakeSettings);
//        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
//    }

//    private void FixedUpdate()
//    {
//        if (gameOver.IsGameOver || IsPaused)
//        {
//            return;
//        }
//        // Wait until the next update before proceeding
//        if (Time.time < nextUpdate) {
//            return;
//        }
//
//        // Set each segment's position to be the same as the one it follows. We
//        // must do this in reverse order so the position is set to the previous
//        // position, otherwise they will all be stacked on top of each other.
//        for (int i = segments.Count - 1; i > 0; i--) {
//            segments[i].position = segments[i - 1].position;
//        }
//
//        // Move the snake in the direction it is facing
//        // Round the values to ensure it aligns to the grid
//        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
//        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
//        transform.position = new Vector2(x, y);
//        RotateHead();
//        // Set the next update time based on the speed
//        SetSaveSpeed(GameHelper.SnakeSettings);
//        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
//    }

    private void RotateHead()
    {
        if (direction == Vector2Int.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2Int.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        } 
        else if (direction == Vector2Int.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        } 
        else if (direction == Vector2Int.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }

    public Transform Grow(bool addScore = true, bool untagged = false)
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
        if (untagged)
        {
            segment.gameObject.tag = "Untagged";
        }
        if (addScore)
        {
            saveScores.ChangeScore(1);
            Acceleration();
        }

        return segment;
    }
    
    public IEnumerator DelayedAddTrigger(List<Transform> segmentsSnake)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var segment in segmentsSnake)
        {
            segment.gameObject.tag = "Obstacle";
        }
    }

    public void ResetState()
    {
        saveScores.ChangeScore(0);
        
        direction = Vector2Int.right;
        transform.position = Vector3.zero;
        RotateHead();

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow(false, true);
        }
        
        foodController.Reset();
        foodController.CreateNewFoods();
    }
    
    public void LoadSave(SaveDataSnake data)
    {
        saveScores.ChangeScore(data.Score);
        saveScores.IsWin = data.IsWin;
        
        direction = new Vector2Int(data.DirectionX, data.DirectionY);
        transform.position = new Vector2(data.HeadX, data.HeadY);
        RotateHead();
        
        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        List<Transform> segmentsSnake = new List<Transform>();
        
        int countSegments = initialSize + data.Score;
        // -1 since the head is already in the list
        for (int i = 0; i < countSegments - 1; i++) {
            if (i < initialSize - 1)
            {
                Grow(false, true);
            }
            else
            {
                Transform seg = Grow(false, true);
                segmentsSnake.Add(seg);
            }
        }

        StartCoroutine(DelayedAddTrigger(segmentsSnake));

        foodController.LoadedFood(data.SaveFoods);
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y) {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            AudioManager.Instance.PlaySuccessLineSound();
            Grow();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
//            ResetState();
            GameHelper.VibrationStart();
            GameOver();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (GameHelper.SnakeSettings.MoveThroughWalls) {
                Traverse(other.transform);
            } else {
//                ResetState();
                GameHelper.VibrationStart();
                GameOver();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        } else if (direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }
    
    private void TryChangeDirection(Vector2Int newDirection)
    {
        // Если это первое направление в кадре — проверяем противоположность с текущим
        Vector2Int lastDirection = directionQueue.Count > 0 ? directionQueue.Peek() : direction;

        if (newDirection + lastDirection != Vector2Int.zero)
        {
            // Добавляем только если это не последнее в очереди
            if (directionQueue.Count == 0 || directionQueue.Peek() != newDirection)
            {
                directionQueue.Enqueue(newDirection);
            }
        }
    }
    
    private void ApplyNextDirection()
    {
        if (directionQueue.Count > 0)
        {
            direction = directionQueue.Dequeue();
            RotateHead();
        }
    }

    private void Acceleration()
    {
        if (!GameHelper.SnakeSettings.Acceleration)
        {
            return;
        }
        GameHelper.SnakeSettings.Speed = Mathf.Max(minSpeed, minSpeed + (segments.Count - initialSize) * 0.1f);
        JsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
        SetAccelerationSpeed(GameHelper.SnakeSettings.Speed);
    }
    
    private void SetSaveSpeed(SnakeSettings snakeSettings)
    {
        if (snakeSettings.Acceleration)
        {
//            SetAccelerationSpeed(snakeSettings.AccelerationSpeed);
            Acceleration();
        }
        else
        {
            SetAccelerationSpeed(GameHelper.SnakeSettings.Speed);
        }
    }

    private void SetAccelerationSpeed(float accelerationSpeed)
    {
        speed = accelerationSpeed;
    }

}