using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScorePlusAnimation: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private Tween _tween;
    private Color _colorLight;
    private Color _colorDark;
    
    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _colorDark = ColorUtility.TryParseHtmlString("#2C2926", out Color color7) ? color7 : Color.black;
        scoreText.color = GameHelper.GetRealTheme() == Themes.Light ? _colorDark : _colorLight;
        // Safety удаление через 3 секунды
        Destroy(gameObject, 3f);
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Destroy(gameObject);
        }
    }

    public void Play(int score)
    {
        scoreText.text = $"+{score}";
        var rect = GetComponent<RectTransform>();

        rect.localScale = Vector3.zero;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 0f);

        Sequence seq = DOTween.Sequence();
        _tween = seq; // сохраняем ссылку

        seq.Append(scoreText.DOFade(1f, 0.2f))
            .Join(rect.DOScale(1.2f, 0.2f))
            .Append(rect.DOAnchorPosY(rect.anchoredPosition.y + 100f, 0.8f).SetEase(Ease.OutCubic))
            .Join(scoreText.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
            .AppendCallback(() => Destroy(gameObject));
    }
    
    private void OnDisable()
    {
        // Защита: убираем висящие анимации
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }

        // Дополнительно — уничтожаем объект вручную
        Destroy(gameObject);
    }

}