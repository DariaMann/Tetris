using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BounceEffect : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float offsetY = 30f;        // Насколько вверх подпрыгивает
    [SerializeField] private float startOffsetY = -10f;  // Насколько ниже начало прыжка (временное)
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float pause = 0.2f;

    [Header("Scale Settings")]
    [SerializeField] private float stretchY = 1.1f;
    [SerializeField] private float squashY = 0.9f;
    [SerializeField] private float stretchX = 0.95f;
    [SerializeField] private float squashX = 1.05f;
    
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 10f;
    
    [Header("Explode Settings")]
    [SerializeField] private float explosionScale = 1.5f;
    [SerializeField] private float explosionDuration = 0.2f;
    [SerializeField] private Ease explosionEase = Ease.OutBack;
    [SerializeField] private float rotationDegrees = 360f;
    [SerializeField] private bool destroyAfterExplosion = true;

    private RectTransform _rt;
    private Image _image;
    private Sequence _bounceSequence;
    private Vector2 _startPos;
    private Vector3 _startScale;
    
    private Tween _currentShake;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _startPos = _rt.anchoredPosition;
        _startScale = _rt.localScale;
    }
    
    public void Explode()
    {
        Sequence sequence = DOTween.Sequence();

        // Масштабирование
        sequence.Append(_rt.DOScale(explosionScale, explosionDuration).SetEase(explosionEase));

        // Прозрачность
        sequence.Join(_image.DOFade(0f, explosionDuration));

        // Вращение
        sequence.Join(_rt.DORotate(new Vector3(0, 0, rotationDegrees), explosionDuration, RotateMode.FastBeyond360));

        // Уничтожение объекта после завершения анимации
        if (destroyAfterExplosion)
        {
            sequence.OnComplete(() => Destroy(gameObject));
        }
    }
    
    public void ShakeOnce()
    {
        // Останавливаем предыдущую анимацию
        _currentShake?.Kill();

        // Сбрасываем позицию перед шейком
        _rt.anchoredPosition = _startPos;

        // Запускаем новый шейк и сохраняем ссылку
        _currentShake = _rt.DOShakeAnchorPos(shakeDuration, shakeStrength, vibrato: 10, randomness: 90, snapping: false, fadeOut: true)
            .OnComplete(() =>
            {
                _rt.anchoredPosition = _startPos; // Гарантируем возврат
                _currentShake = null;
            });
    }

    public void StartBounce()
    {
        if (_bounceSequence != null && _bounceSequence.IsPlaying()) return;

        _bounceSequence = DOTween.Sequence();

        // Смещаем вниз перед началом прыжков
        _rt.anchoredPosition = _startPos + new Vector2(0, startOffsetY);

        // Прыжок вверх + вытягивание
        _bounceSequence.Append(_rt.DOAnchorPosY(_startPos.y + offsetY, duration).SetEase(Ease.OutSine));
        _bounceSequence.Join(_rt.DOScale(new Vector3(_startScale.x * stretchX, _startScale.y * stretchY, 1f), duration).SetEase(Ease.OutSine));

        // Возврат вниз + сплющивание
        _bounceSequence.Append(_rt.DOAnchorPosY(_startPos.y + startOffsetY, duration).SetEase(Ease.InSine));
        _bounceSequence.Join(_rt.DOScale(new Vector3(_startScale.x * squashX, _startScale.y * squashY, 1f), duration).SetEase(Ease.InSine));

        // Возврат к нормальному масштабу
        _bounceSequence.Append(_rt.DOScale(_startScale, 0.15f).SetEase(Ease.OutQuad));

        _bounceSequence.AppendInterval(pause);
        _bounceSequence.SetLoops(-1);
    }

    public void StopBounce()
    {
        if (_bounceSequence != null)
        {
            _bounceSequence.Kill();
            _bounceSequence = null;

            // Вернём на стартовую позицию и масштаб
            _rt.anchoredPosition = _startPos;
            _rt.localScale = _startScale;
        }
    }
}