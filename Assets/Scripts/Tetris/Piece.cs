using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float fastDropStepDelay = 0.05f;   // Новый delay для ускоренного падения
    [SerializeField] private float stepDelay = 1f;
    [SerializeField] private float moveDelay = 0.1f;
    
    [SerializeField] private float baseMoveDelay = 0.1f;   // Базовая задержка между движениями
    [SerializeField] private float minMoveDelay = 0.02f;   // Минимально возможная задержка
    
    [SerializeField] private float lockDelay = 0.5f;
    
    [SerializeField] private float maxDelay = 1.3f; // самая медленная скорость
    [SerializeField] private float minDelay = 0.2f; // самая быстрая скорость
    [SerializeField] private float scoreFactor = 0.03f; // насколько быстро убывает stepDelay с ростом счёта
    
    [SerializeField] private float rotateDelay = 0.05f;
    
    private float _nextRotateTime;
//    
//    [SerializeField] private float longSwipeThreshold = 2f; // пикселей — можно подбирать
//    
//    [SerializeField] private float minStepDelay = 0.1f;      // Минимальная задержка между падениями
//    [SerializeField] private float delayDecreaseStep = 0.05f; // На сколько уменьшать
//    [SerializeField] private float delayDecreaseInterval = 10f; // Интервал (в секундах), через который ускоряется падение

    private float _nextSpeedUpTime;

    private bool _isFastSwipingDown = false;
    
    private float _stepTime;
    private float _moveTime;
    private float _lockTime;

    private Vector2 _touchStartPos;
    private Vector2 _lastTouchPos;
    private bool _isDragging = false;
    private float _moveThreshold = 50f; // Минимальное расстояние для шага движения
    
    private bool _doHardDrop = false;
    
    public Board Board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    public int RotationIndex { get; private set; }
    
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        Data = data;
        Board = board;
        Position = position;

        RotationIndex = 0;
        SetAccelerationSpeed(GameHelper.TetrisSettings.Speed);  
        _stepTime = Time.time + stepDelay;
        _moveTime = Time.time + moveDelay;
        _lockTime = 0f;

        if (Cells == null) {
            Cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < Cells.Length; i++) {
            Cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        if (Board.GameOverPanel.IsGameOver)
        {
            return;
        }
        
        Board.Clear(this);
        
        if (!Input.GetMouseButton(0) && Input.touchCount == 0)
        { 
            SetAccelerationSpeed(GameHelper.TetrisSettings.Speed); // сброс к обычной скорости
        }

        // We use a timer to allow the player to make adjustments to the piece
        // before it locks in place
        _lockTime += Time.deltaTime;

        // Handle rotation (по нажатию клавиши)
        if (Time.time >= _nextRotateTime)
        {
            if (Input.GetKeyDown(KeyCode.Q)) {
                Rotate(-1);
                _nextRotateTime = Time.time + rotateDelay;
            } else if (Input.GetKeyDown(KeyCode.E)) {
                Rotate(1);
                _nextRotateTime = Time.time + rotateDelay;
            }
        }

        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }

        if (_doHardDrop)
        {
            _doHardDrop = false;
            HardDrop();
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > _moveTime) {
            HandleMoveInputs();
            HandleTouchInput();
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > _stepTime) {
            Step();
        }

        if (GameHelper.TetrisSettings.Acceleration)
        {
            SetSaveSpeed(GameHelper.TetrisSettings);
        }

        Board.Set(this);
    }

    private void HandleMoveInputs()
    {
        // Soft drop movement
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down)) {
                // Update the step time to prevent double movement
                
//                SetAccelerationSpeed(fastDropStepDelay);
//                stepDelay = fastDropStepDelay;
                _stepTime = Time.time + stepDelay; // обновляем таймер немедленно
            }
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.A)) {
            Move(Vector2Int.left);
        } else if (Input.GetKey(KeyCode.D)) {
            Move(Vector2Int.right);
        }
    }

    private void HandleTouchInput()
    {
        Vector2 currentPos = Vector2.zero;

        // === 🖐 ОБРАБОТКА ТАЧА ===
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            currentPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPos = currentPos;
                    _lastTouchPos = currentPos;
                    _isDragging = true;
                    _isFastSwipingDown = false;
                    break;

                case TouchPhase.Moved:
                    ProcessMove(currentPos);
                    break;

                case TouchPhase.Ended:
                    _isDragging = false;
                    _isFastSwipingDown = false;
                    TryRotate(currentPos);
                    break;
            }
        }
#if UNITY_EDITOR
        // === 🖱 ОБРАБОТКА МЫШИ (ПК) ===
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPos = Input.mousePosition;
            _lastTouchPos = Input.mousePosition;
            _isDragging = true;
            _isFastSwipingDown = false;
        }
        else if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
            ProcessMove(currentPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _isFastSwipingDown = false;
            TryRotate(Input.mousePosition);
        }
#endif
    }

// === 📌 ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ===

// Обработка движения
    private void ProcessMove(Vector2 currentPos)
    {
        if (!_isDragging) return;

        Vector2 delta = currentPos - _lastTouchPos;

        if (Mathf.Abs(delta.x) > _moveThreshold)
        {
            float swipeLength = Mathf.Abs(currentPos.x - _touchStartPos.x);
    
            // Чем длиннее свайп, тем меньше задержка (быстрее движение)
            float t = Mathf.InverseLerp(_moveThreshold, _moveThreshold * 5f, swipeLength);
            moveDelay = Mathf.Lerp(baseMoveDelay, minMoveDelay, t);
            
            Move(delta.x > 0 ? Vector2Int.right : Vector2Int.left);
            _lastTouchPos = currentPos;
        }

        if (Mathf.Abs(delta.y) > _moveThreshold)
        {
            if (delta.y < 0 && Move(Vector2Int.down))
            {
//                stepDelay = fastDropStepDelay;
                _stepTime = Time.time + stepDelay; // обновляем таймер немедленно
            }

            _lastTouchPos = currentPos;
        }
        // Проверяем на длинный свайп вниз
//        float totalSwipeDown = _touchStartPos.y - currentPos.y;
//        
//        Debug.Log($"TotalSwipeDown: {totalSwipeDown}, Threshold: {_moveThreshold * longSwipeThreshold}");
//
//        if (!_isFastSwipingDown && totalSwipeDown > _moveThreshold * longSwipeThreshold)
//        {
//            _isFastSwipingDown = true;
//        }
//
//        // Если уже в режиме быстрого свайпа — ускоряем фигуру
//        if (_isFastSwipingDown && Move(Vector2Int.down))
//        {
//            stepDelay = fastDropStepDelay;
//            _stepTime = Time.time + stepDelay;
//        }
//
//        _lastTouchPos = currentPos;
    }

// Обработка клика (поворота)
    private void TryRotate(Vector2 endPos)
    {
        float touchDistance = Vector2.Distance(_touchStartPos, endPos);
        Debug.Log($"Touch Distance: {touchDistance}, Threshold: {_moveThreshold}");

        if (touchDistance < _moveThreshold && Time.time >= _nextRotateTime)
        {
            Debug.Log("Rotate triggered!");
            Rotate(1);
            _nextRotateTime = Time.time + rotateDelay;
        }
    }

    private void Step()
    {
        _stepTime = Time.time + stepDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (_lockTime >= lockDelay) {
            Lock();
        }
    }

    public void OnTetraminoDownClick()
    {
        _doHardDrop = true;
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece();
        Board.NextRandomTetromino();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = Board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            Position = newPosition;
            _moveTime = Time.time + moveDelay;
            _lockTime = 0f; // reset
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = RotationIndex;

        // Rotate all of the cells using a rotation matrix
        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = global::Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];

            int x, y;

            switch (Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < Data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = Data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, Data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }
    
    private void Acceleration()
    {
        if (!GameHelper.TetrisSettings.Acceleration)
        {
            return;
        }
        GameHelper.TetrisSettings.Speed = Mathf.Max(minDelay, maxDelay - Board.SaveScores.CurrentScore * scoreFactor);
        JsonHelper.SaveTetrisSettings(GameHelper.TetrisSettings);
        SetAccelerationSpeed(GameHelper.TetrisSettings.Speed);
    }
    
    private void SetSaveSpeed(TetrisSettings tetrisSettings)
    {
        if (tetrisSettings.Acceleration)
        {
//            SetAccelerationSpeed(snakeSettings.AccelerationSpeed);
            Acceleration();
        }
        else
        {
            SetAccelerationSpeed(GameHelper.TetrisSettings.Speed);
        }
    }

    private void SetAccelerationSpeed(float accelerationSpeed)
    {
        stepDelay = accelerationSpeed;

        // Дополнительно масштабируем другие задержки
        float speedFactor = Mathf.InverseLerp(maxDelay, minDelay, accelerationSpeed);
    
        rotateDelay = Mathf.Lerp(0.05f, 0.01f, speedFactor); // подбери границы по ощущениям
        moveDelay = Mathf.Lerp(baseMoveDelay, minMoveDelay, speedFactor);
    }

}