using DG.Tweening;
using UnityEngine;

public class SkilUIAnimation_Gabu : UIAnimationSystem_Gabu
{
    [SerializeField]
    private Color _gradationColor = Color.white;

    protected override void NormalAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }
        _image.DOColor(_imageColor, duration: 0.4f);
        _transform.DOScale(_unitScale, duration: 0.4f);
    }
    protected override void HighlightedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.0f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
        else
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.2f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
    }
    protected override void PressedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0f, v: 0.1f), duration: 0f);

            _transform.DOScale(_unitScale * 1.05f, duration: 0f);
        }
        else
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.1f, v: 0.1f), duration: 0f);

            _transform.DOScale(_unitScale * 1.05f, duration: 0f);
        }
    }
    protected override void SelectedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
        else
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.2f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
    }
    protected override void DisabledAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(ChengeHSV(_imageColor, s: 0, v: 0.4f), duration: 0.05f);

            _transform.DOScale(_unitScale, duration: 0.05f);
        }
        else
        {
            _image.DOColor(ChengeHSV(_imageColor, s: 0.4f, v: 0.4f), duration: 0.05f);

            _transform.DOScale(_unitScale, duration: 0.05f);
        }
    }
}
