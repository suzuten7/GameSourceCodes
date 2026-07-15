using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HpBarAnimation : ColorSystem
{
    public readonly float value = 0.65f;
    public readonly float _duration = 0.07f;
    public readonly Ease _easeType = Ease.InOutQuad;
    public readonly int _loopCount = 2;
    public readonly LoopType _loopType = LoopType.Yoyo;
    public readonly Color _normalColor = new Color(0.3f, 0.9f, 0);

    public void OnChengeValue(Image hpImage)
    {
        Sequence sequence = DOTween.Sequence();

        hpImage.color = Color.white;
        sequence.Append(hpImage.DOColor(Color.black, _duration).SetEase(_easeType).SetLoops(_loopCount, _loopType))
                .Append(hpImage.DOColor(_normalColor, _duration / 4).SetEase(Ease.Linear));
    }
}
