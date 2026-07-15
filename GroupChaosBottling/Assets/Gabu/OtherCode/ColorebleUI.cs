using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorebleUI : MonoBehaviour, IUIAnimation
{

    #region 変数

    [SerializeField]
    private bool playOnStart = true;

    private Tween _tween;

    [SerializeField]
    private Image image = null;

    [SerializeField] Color _startColor = Color.white;
    [SerializeField] Color _endColor = Color.black;
    [SerializeField] float _duration = 0.3f;
    [SerializeField] Ease _easeType = Ease.Linear;
    [SerializeField] int _loopCount = 0;
    [SerializeField] LoopType _loopType = LoopType.Restart;

    public bool IsPlaying => _tween != null && _tween.IsActive() && _tween.IsPlaying();
    public bool IsPaused => _tween != null && _tween.IsActive() && !_tween.IsPlaying();
    public bool IsReversed => _tween != null && _tween.IsActive() && isReversed;
    private bool isReversed = false;

    public event Action OnStart;
    public event Action OnComplete;
    public event Action OnUpdate;


    #endregion

    #region 関数

    public void Play()
    {
        if (_tween == null || isReversed)
        {
            _tween = CreateAnimation();
        }
        isReversed = false;

        _tween.Play();
        OnStart?.Invoke();
    }

    public void Pause()
    {
        _tween?.Pause();
    }

    public void Resume()
    {
        _tween?.Play();
    }

    public void Reverse()
    {
        if (_tween == null || !isReversed)
        {
            _tween = CreateReverseAnimation();
        }
        isReversed = true;

        _tween.Play();
        OnStart?.Invoke();
    }

    public void Stop()
    {
        _tween?.Kill();
        _tween = null;
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
        RestartTween();
    }

    public void SetEasing(Ease easeType)
    {
        _easeType = easeType;
        RestartTween();
    }

    public void SetLoop(int loopCount, LoopType loopType)
    {
        _loopCount = loopCount;
        _loopType = loopType;
        RestartTween();
    }

    private void RestartTween()
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        _tween = CreateAnimation();
    }

    private Tween CreateAnimation()
    {
        image.color = _startColor;

        return image.DOColor(_endColor, _duration)
            .SetEase(_easeType)
            .SetLoops(_loopCount, _loopType)
            .OnComplete(() => OnComplete?.Invoke())
            .OnUpdate(() => OnUpdate?.Invoke());
    }

    private Tween CreateReverseAnimation()
    {
        image.color = _endColor;

        return image.DOColor(_startColor, _duration)
            .SetEase(_easeType)
            .SetLoops(_loopCount, _loopType)
            .OnComplete(() => OnComplete?.Invoke())
            .OnUpdate(() => OnUpdate?.Invoke());
    }

    #endregion

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }
}
