using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    [SerializeField] private Segment segmentHead;
    [SerializeField] private FoodController foodController;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private Vector2Int direction = Vector2Int.right;
    [SerializeField] private Vector2Int boardSize = new Vector2Int(15,17);
    [SerializeField] private float speed = 20f;
    [SerializeField] private int initialSize = 4;
    private int minSpeed = 5;
    private int maxSpeed = 12;
    private bool _canCheckDead;
    private bool _isStartMove;
    private bool _isDead;

    private readonly List<Segment> segments = new List<Segment>();

    public List<Segment> Segments => segments;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isDragging = false;
    private readonly Queue<Vector2Int> directionQueue = new Queue<Vector2Int>();

    public Vector2Int Direction
    {
        get => direction;
        set => direction = value;
    }

    private void Update()
    {
        if (GameManagerSnake.Instance.GameOverPanel.IsGameOver || GameHelper.IsPause || GameHelper.IsEdication || !_isStartMove)
        {
            return;
        }

        HandleInput();
        SetSaveSpeed(GameHelper.SnakeSettings);

        ApplyNextDirection();
        MoveHead();
    }
    
    void MoveHead()
    {
        // Если достигнута цель — назначить новую
        float dis = Vector2.Distance(segmentHead.transform.position, segmentHead.NextCell);
        
        if (dis < 0.5f)
        {
            CheckOnDead();
        }

        if (dis < 0.01f)
        {
            foreach (var segment in segments)
            {
                segment.SetCurrentPosition();
            }
            PrepareNextMove();
        }

        // Двигаемся к цели
        for (int i = 0; i < segments.Count; i++)
        {
            if (Mathf.Abs(segments[i].NextCell.x - segments[i].CurrentCell.x) > boardSize.x / 2 ||
                Mathf.Abs(segments[i].NextCell.y - segments[i].CurrentCell.y) > boardSize.y / 2)
            {
                segments[i].SetCurrentPosition(); // моментально переместить
                
                for (int j = i+1; j < segments.Count; j++)
                {
                    segments[j].SetCurrentPosition(); 
                }
            }
            else
            {
                Vector2 nextPos = new Vector2(segments[i].NextCell.x, segments[i].NextCell.y);
                segments[i].transform.position = Vector3.MoveTowards(segments[i].transform.position, nextPos, speed * Time.deltaTime);
            }
        }
    }

    private void PrepareNextMove(bool isRotate = false)
    {
        float disCur = Vector2.Distance(segmentHead.transform.position, segmentHead.CurrentCell);
        float disNext = Vector2.Distance(segmentHead.transform.position, segmentHead.NextCell);
        Vector2Int currCell = segmentHead.CurrentCell;
        if (disNext < disCur)
        {
            currCell = segmentHead.NextCell;
            
            foreach (var segment in segments)
            {
                segment.SetCurrentPosition();
            }
        }
        
        Vector2Int headTargetPos = currCell + direction;
        Vector2Int? pos = Traverse(headTargetPos.x, headTargetPos.y);
        if (!pos.HasValue) return;
        Vector2Int newHeadPos = pos.Value;
        
        if (isRotate)
        {
            Debug.Log("START POS x= " +currCell.x+", y= "+currCell.y);
            Debug.Log("NEXT POS x= " +newHeadPos.x+", y= "+newHeadPos.y);
        }

        if (newHeadPos == segments[1].CurrentCell)
        {
            Debug.Log("Едет в первый хвост");
            currCell = segmentHead.NextCell;
            
            foreach (var segment in segments)
            {
                segment.SetCurrentPosition();
            }
            headTargetPos = currCell + direction;
            pos = Traverse(headTargetPos.x, headTargetPos.y);
            if (!pos.HasValue) return;
            newHeadPos = pos.Value;
        }

        segmentHead.NextCell = newHeadPos;

        for (int i = 1; i < segments.Count; i++)
        {
            segments[i].NextCell = segments[i - 1].CurrentCell;
        }

        RotateHead();
    }
    
    public void CheckOnDead()
    {
        if (!_canCheckDead)
        {
            return; 
        }
        for (int i = 1; i < segments.Count; i++)
        {
            if (segments[i].CurrentCell == segmentHead.NextCell)
            {
                Dead();
            }
        }
    }

    private void Dead()
    {
        if (_isDead)
        {
            return;
        }
        _isDead = true;
        
        Debug.Log("DEAD");
        GameHelper.VibrationStart();
        GameManagerSnake.Instance.GameOver();
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
                case TouchPhase.Moved:
                    Vector2 delta = touch.deltaPosition;
                    // Фильтрация мелких движений
                    if (delta.magnitude > 15f)
                    {
                        TrySwipeFromDelta(delta.normalized);
                    }
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
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;

            if (delta.magnitude > 15f)
            {
                TrySwipeFromDelta(delta.normalized);
                touchStartPos = Input.mousePosition; // обновим старт для следующего движения
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
    
    private void TrySwipeFromDelta(Vector2 normalizedDelta)
    {
        if (Mathf.Abs(normalizedDelta.x) > Mathf.Abs(normalizedDelta.y))
        {
            if (normalizedDelta.x > 0 && direction.y != 0f)
            {
                TryChangeDirection(Vector2Int.right);
            }
            else if (normalizedDelta.x < 0 && direction.y != 0f)
            {
                TryChangeDirection(Vector2Int.left);
            }
        }
        else
        {
            if (normalizedDelta.y > 0 && direction.x != 0f)
            {
                TryChangeDirection(Vector2Int.up);
            }
            else if (normalizedDelta.y < 0 && direction.x != 0f)
            {
                TryChangeDirection(Vector2Int.down);
            }
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

    private Vector2Int? Traverse(int x, int y)
    {
        bool isTraverse = false;
        
        // Оборачиваем координаты по ширине и высоте
        int halfWidth = boardSize.x / 2;
        int halfHeight = boardSize.y / 2;

        if (x > halfWidth)
        {
            x = -halfWidth;
            isTraverse = true;
        }
        else if (x < -halfWidth)
        {
            x = halfWidth;
            isTraverse = true;
        }

        if (y > halfHeight)
        {
            y = -halfHeight;
            isTraverse = true;
        }
        else if (y < -halfHeight)
        {
            y = halfHeight;
            isTraverse = true;
        }

        if (isTraverse)
        {
            if (GameHelper.SnakeSettings.MoveThroughWalls) {
                return new Vector2Int(x, y);
            } else {
                GameHelper.VibrationStart();
                GameManagerSnake.Instance.GameOver();
                return null;
            }
        }
        return new Vector2Int(x, y);
    }

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
        Segment seg = segment.GetComponent<Segment>();
        
        seg.SetFirstCurrentPosition(segments[segments.Count - 1].CurrentCell);
        seg.NextCell = segments[segments.Count - 1].CurrentCell;
        
        segments.Add(seg);
        if (untagged)
        {
            segment.gameObject.tag = "Untagged";
        }
        if (addScore)
        {
            GameManagerSnake.Instance.SaveScores.ChangeScore(1);
            Acceleration();
        }

        return segment;
    }
    
    public void ResetState()
    {
        GameManagerSnake.Instance.SaveScores.ChangeScore(0);
        _isDead = false;
        direction = Vector2Int.right;
        RotateHead();

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segmentHead.SetFirstCurrentPosition(new Vector2Int(0,0));
        segments.Clear();
        segments.Add(segmentHead);
        
        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow(false, true);
        }

        _canCheckDead = true;
        _isStartMove = true;
        
        PrepareNextMove();
        
        foodController.Reset();
        foodController.CreateNewFoods();
    }
    
    public void LoadSave(SaveDataSnake data)
    {
        GameManagerSnake.Instance.SaveScores.ChangeScore(data.Score);
        GameManagerSnake.Instance.SaveScores.IsWin = data.IsWin;
        
        direction = new Vector2Int(data.DirectionX, data.DirectionY);
        RotateHead();
        
        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
//        segmentHead.SetFirstCurrentPosition(new Vector2Int(data.HeadX, data.HeadY));
        segments.Clear();
        segments.Add(segmentHead);

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

        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].SetFirstCurrentPosition(new Vector2Int(data.SaveSegments[i].X, data.SaveSegments[i].Y));
        }

        foreach (var segment in segmentsSnake)
        {
            if (segment == null)
            {
                continue;
            }
            segment.gameObject.tag = "Obstacle";
        }

        _canCheckDead = true;
        _isStartMove = true;
        
        PrepareNextMove();
        
//        StartCoroutine(DelayedAddTrigger(segmentsSnake));

        foodController.LoadedFood(data.SaveFoods);
    }

    public bool Occupies(int x, int y)
    {
        foreach (Segment segment in segments)
        {
            if (segment.CurrentCell.x == x &&
                segment.CurrentCell.y == y) {
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
            Dead();
        }
    }

    private void TryChangeDirection(Vector2Int newDirection)
    {
        // Если это первое направление в кадре — проверяем противоположность с текущим
        Vector2Int lastDirection = directionQueue.Count > 0 ? directionQueue.Peek() : direction;

        if (newDirection + lastDirection != Vector2Int.zero)
        {
            float disPos = Vector2.Distance(segmentHead.transform.position, segmentHead.NextCell);
            Debug.Log("DISTANCE: " + disPos);
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
            PrepareNextMove(true);
        }
    }

    private void Acceleration()
    {
        if (!GameHelper.SnakeSettings.Acceleration)
        {
            return;
        }

        float max = Mathf.Max(minSpeed, minSpeed + (segments.Count - initialSize) * 0.03f);
        if (max > maxSpeed)
        {
            max = maxSpeed;
        }

        GameHelper.SnakeSettings.Speed = max;
        MyJsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
        SetAccelerationSpeed(GameHelper.SnakeSettings.Speed);
    }
    
    private void SetSaveSpeed(SnakeSettings snakeSettings)
    {
        if (snakeSettings.Acceleration)
        {
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