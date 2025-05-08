using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    [SerializeField] private float stepDelay = 1f;
    [SerializeField] private float maxDelay = 1.3f; // самая медленная скорость
    [SerializeField] private float minDelay = 0.2f; // самая быстрая скорость
    [SerializeField] private float scoreFactor = 0.02f; // насколько быстро убывает stepDelay с ростом счёта
    
    private float _minDistanceSwipe = 100f;
    private float _maxTimeSwipe = 0.2f;
    private float _lockDelay = 0.5f;
    
    private float _touchStartTime;
    private float _nextRotateTime;
    private float _nextSpeedUpTime;
    
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
        _moveTime = Time.time;
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
        if (Board.GameOverPanel.IsGameOver || Board.IsPaused)
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
                _nextRotateTime = Time.time;
            } else if (Input.GetKeyDown(KeyCode.E)) {
                Rotate(1);
                _nextRotateTime = Time.time;
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
                    _touchStartTime = Time.time;
                    _touchStartPos = currentPos;
                    _lastTouchPos = currentPos;
                    _isDragging = true;
                    break;

                case TouchPhase.Moved:
                    ProcessMove(currentPos);
                    break;

                case TouchPhase.Ended:
                    _isDragging = false;
                    TryRotate(currentPos);
                    break;
            }
        }
#if UNITY_EDITOR
        // === 🖱 ОБРАБОТКА МЫШИ (ПК) ===
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartTime = Time.time;
            _touchStartPos = Input.mousePosition;
            _lastTouchPos = Input.mousePosition;
            _isDragging = true;
        }
        else if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
            ProcessMove(currentPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
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
            Move(delta.x > 0 ? Vector2Int.right : Vector2Int.left);
            _lastTouchPos = currentPos;
        }

        if (Mathf.Abs(delta.y) > _moveThreshold)
        {
            if (delta.y < 0 && Move(Vector2Int.down))
            {
                _stepTime = Time.time + stepDelay; // обновляем таймер немедленно
            }

            _lastTouchPos = currentPos;
        }
    }
    
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

#if UNITY_EDITOR
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
#else
    if (Input.touchCount == 0)
        return false;

    PointerEventData pointerData = new PointerEventData(EventSystem.current)
    {
        position = Input.GetTouch(0).position
    };
#endif

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }


// Обработка клика (поворота)
    private void TryRotate(Vector2 endPos)
    {
        if (IsPointerOverUI())
            return;

        float touchDistance = Vector2.Distance(_touchStartPos, endPos);
        float verticalDistance = _touchStartPos.y - endPos.y;
        float swipeDuration = Time.time - _touchStartTime;
        
        // Проверка на резкий свайп вниз
        if (verticalDistance > _minDistanceSwipe &&
            swipeDuration < _maxTimeSwipe &&
            Mathf.Abs(_touchStartPos.x - endPos.x) < verticalDistance * 0.5f) // важное ограничение!
        {
            Debug.Log("Hard drop via fast vertical swipe");
            _doHardDrop = true;
            return;
        }

        // Иначе обычный поворот
        if (touchDistance < _moveThreshold && Time.time >= _nextRotateTime)
        {
            Debug.Log("Rotate triggered!");
            Rotate(1);
            _nextRotateTime = Time.time;
        }
    }

    private void Step()
    {
        _stepTime = Time.time + stepDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (_lockTime >= _lockDelay) {
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
        AudioManager.Instance.PlayClickChipSound();
        
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
            _moveTime = Time.time;
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
    }
}