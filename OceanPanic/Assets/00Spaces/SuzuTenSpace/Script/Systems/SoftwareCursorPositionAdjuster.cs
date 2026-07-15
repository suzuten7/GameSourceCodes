
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class SoftwareCursorPositionAdjuster : MonoBehaviour
{
    PlayerInput PI;
    [SerializeField] GameObject MouseObj;
    [SerializeField] VirtualMouseInput _virtualMouse;
    [SerializeField] InputSystemUIInputModule _inputSystemUIInputModule;
    [SerializeField] Canvas _canvas;
    [SerializeField] string[] NonControlSchemes;

    private float _lastScaleFactor = 1;

    // 現在のCanvasスケール
    private float CurrentScale =>
        _virtualMouse.cursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable
            ? 1
            : _canvas.scaleFactor;

    // Canvasのスケールを監視して、VirtualMouseの座標を補正する
    private void Start()
    {
        PI = FindFirstObjectByType<PlayerInput>();
    }
    private void Update()
    {
        if (!_virtualMouse.enabled) return;
        //画面内固定
        Vector2 Pos = _virtualMouse.virtualMouse.position.value;
        if (_canvas != null)
        {
            // Clamp to canvas.
            var pixelRect = _canvas.pixelRect;
            float Scale = _canvas.scaleFactor;
            Pos.x = Mathf.Clamp(Pos.x, pixelRect.xMin, pixelRect.xMin + pixelRect.width / Scale);
            Pos.y = Mathf.Clamp(Pos.y, pixelRect.yMin, pixelRect.yMin + pixelRect.height / Scale);
        }
        //VirtualMouse座標変化
        InputState.Change(_virtualMouse.virtualMouse.position, Pos);

        if (PI != null)
        {
            bool Non = false;
            for(int i=0;i< NonControlSchemes.Length; i++)if(PI.currentControlScheme == NonControlSchemes[i])Non = true;
            MouseObj.SetActive(!Non);
        }

        // Canvasのスケール取得
        var scale = CurrentScale;
        // スケールが変化した時のみ、以降の処理を実行
        if (Math.Abs(scale - _lastScaleFactor) == 0) return;
        // VirtualMouseInputのカーソルのスケールを変更するProcessorを適用
            _inputSystemUIInputModule.point.action.ApplyBindingOverride(new InputBinding
            { overrideProcessors = $"VirtualMouseScaler(scale={scale})" });
            _lastScaleFactor = scale;


    }
}
