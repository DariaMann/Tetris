using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;
    
//    private Vector2 touchStartPos;
//    private float swipeThreshold = 50f; // Минимальная дистанция свайпа в пикселях

    private Vector2 touchStartPos;
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private float moveThreshold = 50f; // Минимальное расстояние для шага движения

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null) {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++) {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        // We use a timer to allow the player to make adjustments to the piece
        // before it locks in place
        lockTime += Time.deltaTime;

        // Handle rotation
        if (Input.GetKeyDown(KeyCode.Q)) {
            Rotate(-1);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            Rotate(1);
        }

        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > moveTime) {
            HandleMoveInputs();
            HandleTouchInput();
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime) {
            Step();
        }

        board.Set(this);
    }

    private void HandleMoveInputs()
    {
        // Soft drop movement
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down)) {
                // Update the step time to prevent double movement
                stepTime = Time.time + stepDelay;
            }
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.A)) {
            Move(Vector2Int.left);
        } else if (Input.GetKey(KeyCode.D)) {
            Move(Vector2Int.right);
        }
    }
    
    void HandleTouchInput()
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
                touchStartPos = currentPos;
                lastTouchPos = currentPos;
                isDragging = true;
                break;

            case TouchPhase.Moved:
                ProcessMove(currentPos);
                break;

            case TouchPhase.Ended:
                isDragging = false;
                TryRotate(currentPos);
                break;
        }
    }
    
    // === 🖱 ОБРАБОТКА МЫШИ (ПК) ===
//    if (Input.GetMouseButtonDown(0)) 
//    {
//        touchStartPos = Input.mousePosition;
//        lastTouchPos = Input.mousePosition;
//        isDragging = true;
//    }
//    else if (Input.GetMouseButton(0))
//    {
//        currentPos = Input.mousePosition;
//        ProcessMove(currentPos);
//    }
//    else if (Input.GetMouseButtonUp(0))
//    {
//        isDragging = false;
//        TryRotate(Input.mousePosition);
//    }
}

// === 📌 ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ===

// Обработка движения
void ProcessMove(Vector2 currentPos)
{
    if (!isDragging) return;

    Vector2 delta = currentPos - lastTouchPos;

    if (Mathf.Abs(delta.x) > moveThreshold)
    {
        Move(delta.x > 0 ? Vector2Int.right : Vector2Int.left);
        lastTouchPos = currentPos;
    }

    if (Mathf.Abs(delta.y) > moveThreshold)
    {
        if (delta.y < 0 && Move(Vector2Int.down)) 
        {
            stepTime = Time.time + stepDelay;
        }
        lastTouchPos = currentPos;
    }
}

// Обработка клика (поворота)
void TryRotate(Vector2 endPos)
{
    float touchDistance = Vector2.Distance(touchStartPos, endPos);
    Debug.Log($"Touch Distance: {touchDistance}, Threshold: {moveThreshold}");

    if (touchDistance < moveThreshold)
    {
        Debug.Log("Rotate triggered!");
        Rotate(1);
    }
}

    
    
//    void HandleTouchInput()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);
//
//            switch (touch.phase)
//            {
//                case TouchPhase.Began:
//                    touchStartPos = touch.position;
//                    lastTouchPos = touch.position;
//                    isDragging = true;
//                    break;
//
//                case TouchPhase.Moved:
//                    if (isDragging)
//                    {
//                        Vector2 delta = touch.position - lastTouchPos;
//
//                        if (Mathf.Abs(delta.x) > moveThreshold)
//                        {
//                            if (delta.x > 0)
//                                Move(Vector2Int.right);
//                            else
//                                Move(Vector2Int.left);
//
//                            lastTouchPos = touch.position;
//                        }
//
//                        if (Mathf.Abs(delta.y) > moveThreshold)
//                        {
//                            if (delta.y < 0) // Если палец двигается вниз
//                                if (Move(Vector2Int.down)) {
//                                    // Update the step time to prevent double movement
//                                    stepTime = Time.time + stepDelay;
//                                }
//
//                            lastTouchPos = touch.position;
//                        }
//                    }
//                    break;
//
//                case TouchPhase.Ended:
//                    isDragging = false;
//
//                    // Если отпустили палец без значительного движения — поворачиваем фигуру
//                    float touchDistance = Vector2.Distance(touchStartPos, touch.position);
//                    Debug.Log($"Touch Distance: {touchDistance}, Threshold: {moveThreshold}");
//
//                    if (touchDistance < moveThreshold)
//                    {
//                        Debug.Log("Rotate triggered!");
//                        Rotate(1);
//                    }
//                    break;
//            }
//        }
//        
//        if (Input.GetMouseButtonDown(0)) // Нажатие мыши
//        {
//            touchStartPos = Input.mousePosition;
//            lastTouchPos = Input.mousePosition;
//            isDragging = true;
//        }
//        else if (Input.GetMouseButton(0)) // Движение мыши с зажатой кнопкой
//        {
//            if (isDragging)
//            {
//                Vector2 delta = (Vector2)Input.mousePosition - lastTouchPos;
//
//                if (Mathf.Abs(delta.x) > moveThreshold)
//                {
//                    if (delta.x > 0)
//                        Move(Vector2Int.right);
//                    else
//                        Move(Vector2Int.left);
//
//                    lastTouchPos = Input.mousePosition;
//                }
//
//                if (Mathf.Abs(delta.y) > moveThreshold)
//                {
//                    if (delta.y < 0) // Двигаем фигуру вниз
//                        if (Move(Vector2Int.down)) {
//                            // Update the step time to prevent double movement
//                            stepTime = Time.time + stepDelay;
//                        }
//
//                    lastTouchPos = Input.mousePosition;
//                }
//            }
//        }
//        else if (Input.GetMouseButtonUp(0)) // Отпускание кнопки мыши
//        {
//            isDragging = false;
//
//            float clickDistance = Vector2.Distance(touchStartPos, Input.mousePosition);
//            Debug.Log($"Click Distance: {clickDistance}, Threshold: {moveThreshold}");
//
//            if (clickDistance < moveThreshold)
//            {
//                Debug.Log("Rotate triggered!");
//                Rotate(1);
//            }
//        }
//
//    }


    
//    void HandleTouchInput()
//    {
//        if (Input.touchCount > 0)
//        {
//            // Управление для сенсорных экранов
//            Touch touch = Input.GetTouch(0);
//            switch (touch.phase)
//            {
//                case TouchPhase.Began:
//                    touchStartPos = touch.position;
//                    break;
//
//                case TouchPhase.Ended:
//                    ProcessSwipeOrTap(touch.position);
//                    break;
//            }
//        }
//        else if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши нажата
//        {
//            touchStartPos = Input.mousePosition;
//        }
//        else if (Input.GetMouseButtonUp(0)) // Отпущена
//        {
//            ProcessSwipeOrTap(Input.mousePosition);
//        }
//    }
//
//// Метод, который проверяет свайп или клик
//    void ProcessSwipeOrTap(Vector2 touchEndPos)
//    {
//        Vector2 swipeDelta = touchEndPos - touchStartPos;
//
//        if (swipeDelta.magnitude < swipeThreshold)
//        {
//            // Одиночный тап или клик - поворачиваем фигуру
//            Rotate(1);
//        }
//        else if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
//        {
//            // Горизонтальный свайп
//            if (swipeDelta.x > 0)
//                Move(Vector2Int.right); // Вправо
//            else
//                Move(Vector2Int.left); // Влево
//        }
//        else if (swipeDelta.y < 0)
//        {
//            // Свайп вниз
//            if (Move(Vector2Int.down)) {
//                // Update the step time to prevent double movement
//                stepTime = Time.time + stepDelay;
//            }
//        }
//    }


    private void Step()
    {
        stepTime = Time.time + stepDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (lockTime >= lockDelay) {
            Lock();
        }
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
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
        board.NextRandomTetromino();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
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

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

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

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }

}