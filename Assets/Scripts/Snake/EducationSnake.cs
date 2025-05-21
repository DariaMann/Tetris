using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EducationSnake : MonoBehaviour
{
    [SerializeField] private EducationUi educationUi;
    
    [SerializeField] private float speed = 300f; // пикселей в секунду
    [SerializeField] private GameObject educationPanel;
    [SerializeField] private EducationFinger finger;
    
    [SerializeField] private GameObject leftPos;
    [SerializeField] private GameObject rightPos;
    [SerializeField] private GameObject upPos;
    
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private Transform segmentParent;
    [SerializeField] private RectTransform head;
    [SerializeField] private List<GameObject> foods = new List<GameObject>();
    
    [SerializeField] private CanvasGroup playButton;
    [SerializeField] private GameObject backButton;
    
    private List<RectTransform> segments = new List<RectTransform>();
    
    private Vector2Int headDirection;
    
    private Coroutine _tutorialCoroutine;
    private bool _isFirstShow;
    private bool _buttonPlayShowed;

    private void OnDisable()
    {
        StopTutorial();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameHelper.IsEdication)
        {
            Restart();
            ForcePlayButtonVisible();
        }
    }
    
    public void ShowEducation(bool isFirstEducation)
    {
        _isFirstShow = isFirstEducation;
        GameHelper.IsEdication = true;
        StopTutorial();
        ShowView(isFirstEducation);
        ShowEducation();
    }

    public void ShowEducation()
    {
        educationPanel.SetActive(true);

        PlayTutorial();
    }
    
    public void ShowView(bool isFirstEducation)
    {
        if (isFirstEducation)
        {
            playButton.gameObject.SetActive(true);
            backButton.SetActive(false);
        }
        else
        {
            playButton.gameObject.SetActive(false);
            backButton.SetActive(true);
        }
    }
    
    public void HideEducation()
    {
        GameHelper.IsEdication = false;
        StopTutorial();
        
        educationPanel.SetActive(false);
        
        if (_isFirstShow)
        {
            educationUi.ShowEducation();
        }
    }
    
    public void StopTutorial()
    {
        finger.IsTutorialRunning = false;

        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        ClearSegments();
        
        Vector2 start = new Vector2(-100,-100);
        head.anchoredPosition = start;
        RotateHead(Vector2Int.right);

        foreach (var food in foods)
        {
            food.SetActive(true);
        }
        
        finger.Stop();
    }
    
    public void Restart()
    {
        StopTutorial();

        PlayTutorial();
    }

    private void PlayTutorial()
    {
        if (_tutorialCoroutine != null) return;
        
        finger.IsTutorialRunning = true;
        _tutorialCoroutine = StartCoroutine(PlayMoveStep());
    }

    private IEnumerator PlayMoveStep()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            
            segments.Add(head);
            
            Vector2 start = new Vector2(-100,-100);
            head.anchoredPosition = start;
            yield return StartCoroutine(MoveSnakeHead(Vector2Int.right));
            if (!finger.IsTutorialRunning) yield break;
            foods[0].SetActive(false);
            headDirection = Vector2Int.right;
            AddSegment();
            yield return StartCoroutine(MoveFinger(Vector2Int.up));
            if (!finger.IsTutorialRunning) yield break;
            RotateHead(Vector2Int.up);
            
            yield return StartCoroutine(MoveSnakeHead(Vector2Int.up));
            if (!finger.IsTutorialRunning) yield break;
            foods[1].SetActive(false);
            headDirection = Vector2Int.up;
            AddSegment();
            yield return StartCoroutine(MoveFinger(Vector2Int.left));
            if (!finger.IsTutorialRunning) yield break;
            RotateHead(Vector2Int.left);
            
            yield return StartCoroutine(MoveSnakeHead(Vector2Int.left));
            if (!finger.IsTutorialRunning) yield break;
            foods[2].SetActive(false);
            headDirection = Vector2Int.left;
            AddSegment();
            yield return StartCoroutine(MoveFinger(Vector2Int.down));
            if (!finger.IsTutorialRunning) yield break;
            RotateHead(Vector2Int.down);
            
            yield return StartCoroutine(MoveSnakeHead(Vector2Int.down));
            if (!finger.IsTutorialRunning) yield break;
            RotateHead(Vector2Int.right);

            foreach (var food in foods)
            {
                food.SetActive(true);
            }

            ClearSegments();

            if (_isFirstShow && !_buttonPlayShowed)
            {
                yield return StartCoroutine(ShowPlayButton());
                _buttonPlayShowed = true;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private void RotateHead(Vector2Int direction)
    {
        if (direction == Vector2Int.right)
        {
            head.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2Int.up)
        {
            head.rotation = Quaternion.Euler(0, 0, 90);
        } 
        else if (direction == Vector2Int.left)
        {
            head.rotation = Quaternion.Euler(0, 0, 180);
        } 
        else if (direction == Vector2Int.down)
        {
            head.rotation = Quaternion.Euler(0, 0, 270);
        }
    }

    private float HeadOffset(Vector2Int direction)
    {
        if (direction == Vector2Int.right)
        {
            return -50;
        }
        else if (direction == Vector2Int.up)
        {
            return -50;
        } 
        else if (direction == Vector2Int.left)
        {
            return 50;
        } 
        else if (direction == Vector2Int.down)
        {
            return 50;
        }

        return 50;
    }

    private void AddSegment()
    {
        Transform segment = Instantiate(segmentPrefab, segmentParent);
        head.SetAsLastSibling();
        RectTransform rectSegment = segment.GetComponent<RectTransform>();
        rectSegment.anchoredPosition = segments[segments.Count - 1].anchoredPosition;
        
        segments.Add(rectSegment);
    }
    
    private void ClearSegments()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
    }

    private IEnumerator MoveSnakeHead(Vector2Int direction)
    {
        Vector2 start = head.anchoredPosition;
        Vector2 last = start;
        Vector2 nextCell = start;
        float headOffset = HeadOffset(direction);
        bool isHorizontal = direction == Vector2Int.right || direction == Vector2Int.left;
        int countCell = 4;

        while (countCell > 0)
        {
            last = nextCell;
            
            if (isHorizontal)
            {
                nextCell = new Vector2(last.x - headOffset, last.y);
            }
            else
            {
                nextCell = new Vector2(last.x, last.y - headOffset);
            }

            List<Vector2> segmentTargets = new List<Vector2>();
            for (int i = 1; i < segments.Count; i++)
            {
                segmentTargets.Add(segments[i - 1].anchoredPosition);
            }

            // Двигаемся к новой клетке
            while ((head.anchoredPosition - nextCell).sqrMagnitude > 0.01f)
            {
                // Двигаем голову
                head.anchoredPosition = Vector2.MoveTowards(head.anchoredPosition, target: nextCell, speed * Time.deltaTime);

                // Двигаем каждый сегмент к сохранённой цели
                for (int i = 1; i < segments.Count; i++)
                {
                    segments[i].anchoredPosition = Vector2.MoveTowards(
                        segments[i].anchoredPosition,
                        segmentTargets[i-1],
                        speed * Time.deltaTime
                    );
                }

                yield return null;
                if (!finger.IsTutorialRunning) yield break;
            }
            countCell--;
        }
    }

    private IEnumerator MoveFinger(Vector2Int direction)
    {
        Vector3 start1 = Vector3.zero;
        Vector3 end1 = Vector3.zero;

        if (direction == Vector2Int.right)
        {
            // первый клик и перемещение пальца
            start1 = leftPos.transform.position;
            end1 = rightPos.transform.position;
        }
        else if (direction == Vector2Int.left)
        {
            // первый клик и перемещение пальца
            start1 = rightPos.transform.position;
            end1 = leftPos.transform.position;
        }
        else if (direction == Vector2Int.up)
        {
            // первый клик и перемещение пальца
            start1 = rightPos.transform.position;
            end1 = upPos.transform.position;
        }
        else if (direction == Vector2Int.down)
        {
            // первый клик и перемещение пальца
            start1 = upPos.transform.position;
            end1 = rightPos.transform.position;
        }

        yield return StartCoroutine(finger.PlayFingerClickMove(start1, end1, true));
    }

    public IEnumerator ShowPlayButton()
    {
        playButton.alpha = 0f;
        playButton.interactable = false;
        playButton.blocksRaycasts = false;

        // Анимация появления
        yield return playButton.DOFade(1f, 0.3f).WaitForCompletion();

        // Если тут ошибка — кнопка останется прозрачной, но видимой
        if (!finger.IsTutorialRunning)
        {
            ForcePlayButtonVisible();
            yield break;
        }

        playButton.interactable = true;
        playButton.blocksRaycasts = true;
    }
    
    private void ForcePlayButtonVisible()
    {
        // Сделать кнопку видимой, если анимация не успела завершиться
        playButton.alpha = 1f;
        playButton.interactable = true;
        playButton.blocksRaycasts = true;
        _buttonPlayShowed = true;
    }
}