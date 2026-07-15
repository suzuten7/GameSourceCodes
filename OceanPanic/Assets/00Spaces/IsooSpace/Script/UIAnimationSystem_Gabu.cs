using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationSystem_Gabu : MonoBehaviour
{
    #region 変数
    protected bool _isButton = false;
    protected bool _isEexecute = true;
    protected int _i_currentAnimation = 0;
    protected int _i_lastAnimation = 0;

    [SerializeField]
    protected Animator _animator;

    [SerializeField]
    protected Transform _transform;
    [SerializeField]
    protected Vector3 _unitPosition;
    [SerializeField]
    protected Vector3 _unitRotation;
    [SerializeField]
    protected Vector3 _unitScale;

    [SerializeField]
    protected Image _image;

    [SerializeField, Header("基本色")]
    protected Color _imageColor = new Color(0.75f, 0.75f, 0.75f);

    [SerializeField]
    protected TextMeshProUGUI _tmp;

    [SerializeField, Header("テキストの色")]
    protected Color _tmpColor = new Color(0.9f, 0.9f, 0.9f);

    [SerializeField, Header("自動で色の彩度、明度を変更")]
    protected bool _isAutoColor = false;
    [SerializeField, Header("S(彩度)gaが変更されなくなる")]
    protected bool _isMonochrome = true;

    enum AnimatorState
    {
        Normal = 0, Highlighted, Pressed, Selected, Disabled
    }
    #endregion

    #region 関数
    /// <summary>
    /// Animatorの再生中のアニメーションを確認します。
    /// </summary>
    /// <returns></returns>
    protected int CheckAnimationState()
    {
        if (_isButton || _animator == null)
        {
            return (int)AnimatorState.Normal;
        }

        foreach (string state in Enum.GetNames(typeof(AnimatorState)))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return (int)Enum.Parse(typeof(AnimatorState), state);
            }
        }

        return (int)AnimatorState.Normal;
    }

    /// <summary>
    /// ColorのH,S,Vを変更します
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    protected Color ChengeHSV(Color currentColor, float h, float s = 0.6f, float v = 0.6f)
    {
        Color newColor;
        Color.RGBToHSV(currentColor, out newColor.r, out newColor.g, out newColor.b);
        newColor = new Color(h, s, v);
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }
    /// <summary>
    /// ColorのS,V(彩度,明度)だけを変えます
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    protected Color ChengeHSV(Color currentColor, float s = 0.6f, float v = 0.6f)
    {
        Color newColor;
        Color.RGBToHSV(currentColor, out newColor.r, out newColor.g, out newColor.b);
        newColor = new Color(newColor.r, s, v);
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }

    /// <summary>
    /// ColorのS(彩度)だけを変えます
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    protected Color ChengeHSV(Color currentColor, float v = 0.6f)
    {
        Color newColor;
        Color.RGBToHSV(currentColor, out newColor.r, out newColor.g, out newColor.b);
        newColor = new Color(newColor.r, newColor.g, v);
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }

    /// <summary>
    /// RGBをHSVに変換し引き算を行い再びRGBに戻す関数です。足し算もあります
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="sabtractColor"></param>
    /// <returns></returns>
    protected Color SubtractionHSV(Color currentColor, Color sabtractColor)
    {
        Color.RGBToHSV(currentColor, out currentColor.r, out currentColor.g, out currentColor.b);
        Color.RGBToHSV(sabtractColor, out sabtractColor.r, out sabtractColor.g, out sabtractColor.b);
        Color newColor = currentColor - sabtractColor;
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }

    /// <summary>
    /// RGBをHSVに変換し引き算を行い再びRGBに戻す関数です。足し算もあります
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    protected Color SubtractionHSV(Color currentColor, float h, float s, float v)
    {
        Color.RGBToHSV(currentColor, out currentColor.r, out currentColor.g, out currentColor.b);
        Color newColor = currentColor - new Color(h, s, v);
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }

    /// <summary>
    /// RGBをHSVに変換し足し算を行い再びRGBに戻す関数です。引き算もあります
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="addColor"></param>
    /// <returns></returns>
    protected Color AdditionHSV(Color currentColor, Color addColor)
    {
        Color.RGBToHSV(currentColor, out currentColor.r, out currentColor.g, out currentColor.b);
        Color.RGBToHSV(addColor, out addColor.r, out addColor.g, out addColor.b);
        Color newColor = currentColor + addColor;
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }

    /// <summary>
    /// RGBをHSVに変換し足し算を行い再びRGBに戻す関数です。引き算もあります
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    protected Color AdditionHSV(Color currentColor, float h, float s, float v)
    {
        Color.RGBToHSV(currentColor, out currentColor.r, out currentColor.g, out currentColor.b);
        Color newColor = currentColor + new Color(h, s, v);
        return Color.HSVToRGB(newColor.r, newColor.g, newColor.b);
    }



    protected virtual void NormalAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }
        _image.DOColor(_imageColor, duration: 0.4f);
        _tmp.DOColor(_tmpColor, duration: 0.4f);
        _transform.DOScale(_unitScale, duration: 0.4f);
    }
    protected virtual void HighlightedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.0f, v: 0.2f), duration: 0.2f);
            _tmp.DOColor(AdditionHSV(_tmpColor, h: 0f, s: 0.0f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
        else
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.2f, v: 0.2f), duration: 0.2f);
            _tmp.DOColor(AdditionHSV(_tmpColor, h: 0f, s: 0.2f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
    }
    protected virtual void PressedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0f, v: 0.1f), duration: 0f);
            _tmp.DOColor(AdditionHSV(_tmpColor, h: 0f, s: 0f, v: 0.1f), duration: 0f);

            _transform.DOScale(_unitScale * 1.05f, duration: 0f);
        }
        else
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.1f, v: 0.1f), duration: 0f);
            _tmp.DOColor(AdditionHSV(_tmpColor, h: 0f, s: 0.1f, v: 0.1f), duration: 0f);

            _transform.DOScale(_unitScale * 1.05f, duration: 0f);
        }
    }
    protected virtual void SelectedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0f, v: 0.2f), duration: 0.2f);
            _tmp.DOColor(AdditionHSV(_tmpColor, h: 0f, s: 0f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
        else
        {
            _image.DOColor(AdditionHSV(_imageColor, h: 0f, s: 0.2f, v: 0.2f), duration: 0.2f);
            _tmp.DOColor(AdditionHSV(_tmpColor, h: 0f, s: 0.2f, v: 0.2f), duration: 0.2f);

            _transform.DOScale(_unitScale * 1.1f, duration: 0.2f);
        }
    }
    protected virtual void DisabledAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        if (_isMonochrome)
        {
            _image.DOColor(ChengeHSV(_imageColor, s: 0, v: 0.4f), duration: 0.05f);
            _tmp.DOColor(ChengeHSV(_tmpColor, s: 0, v: 0.4f), duration: 0.05f);

            _transform.DOScale(_unitScale, duration: 0.05f);
        }
        else
        {
            _image.DOColor(ChengeHSV(_imageColor, s: 0.4f, v: 0.4f), duration: 0.05f);
            _tmp.DOColor(ChengeHSV(_tmpColor, s: 0.4f, v: 0.4f), duration: 0.05f);

            _transform.DOScale(_unitScale, duration: 0.05f);
        }
    }
    #endregion

    protected void Start()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_transform == null)
        {
            _transform = GetComponent<Transform>();
        }
        if (_unitPosition != _transform.position)
        {
            _unitPosition = _transform.position;
        }
        if (_unitRotation != _transform.eulerAngles)
        {
            _unitRotation = _transform.eulerAngles;
        }
        if (_unitScale != _transform.localScale)
        {
            _unitScale = _transform.localScale;
        }
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
        if (_tmp == null)
        {
            _tmp = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (_isAutoColor)
        {
            _imageColor = ChengeHSV(_imageColor, s: 0.65f, v: 0.7f);
            _tmpColor = ChengeHSV(_tmpColor, s: 0.65f, v: 0.7f);
        }
    }

    protected void Update()
    {
        if (_animator == null)
        {
            return;
        }

        _i_currentAnimation = CheckAnimationState();
        switch (_i_currentAnimation)
        {
            case (int)AnimatorState.Normal:
                NormalAnimation();
                break;
            case (int)AnimatorState.Highlighted:
                HighlightedAnimation();
                break;
            case (int)AnimatorState.Pressed:
                PressedAnimation();
                break;
            case (int)AnimatorState.Selected:
                SelectedAnimation();
                break;
            case (int)AnimatorState.Disabled:
                DisabledAnimation();
                break;
        }

        _i_lastAnimation = _i_currentAnimation;
    }
}
