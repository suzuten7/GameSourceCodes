using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using MyNamespace;

public class PlayerInputEventSettings_Gabu : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    private Dictionary<string, System.Action<InputAction.CallbackContext>> actionHandlers;
    [SerializeField]
    private ComandSystemCallback_Gabu externalHandlers;

    private void Awake()
    {

        // 他のスクリプトのメソッドをディクショナリに登録
        actionHandlers = new Dictionary<string, System.Action<InputAction.CallbackContext>>()
        {
            { "A", externalHandlers.OnCommandA },
            { "B", externalHandlers.OnCommandB },
            { "C", externalHandlers.OnCommandC },
            { "D", externalHandlers.OnCommandD },
            { "E", externalHandlers.OnCommandE },
            { "F", externalHandlers.OnCommandF },
            { "G", externalHandlers.OnCommandG },
            { "H", externalHandlers.OnCommandH },
            { "I", externalHandlers.OnCommandI },
            { "J", externalHandlers.OnCommandJ },
            { "K", externalHandlers.OnCommandK },
            { "L", externalHandlers.OnCommandL },
            { "M", externalHandlers.OnCommandM },
            { "N", externalHandlers.OnCommandN },
            { "O", externalHandlers.OnCommandO },
            { "P", externalHandlers.OnCommandP },
            { "Q", externalHandlers.OnCommandQ },
            { "R", externalHandlers.OnCommandR },
            { "S", externalHandlers.OnCommandS },
            { "T", externalHandlers.OnCommandT },
            { "U", externalHandlers.OnCommandU },
            { "V", externalHandlers.OnCommandV },
            { "W", externalHandlers.OnCommandW },
            { "X", externalHandlers.OnCommandX },
            { "Y", externalHandlers.OnCommandY },
            { "Z", externalHandlers.OnCommandZ },
            { ",", externalHandlers.OnCommandComma }
            // 必要に応じて他のアクションとそのハンドラを追加
        };

        // すべてのアクションマップを取得
        foreach (var actionMap in playerInput.actions.actionMaps)
        {
            // すべてのアクションにイベントハンドラを設定
            foreach (var action in actionMap.actions)
            {
                if (actionHandlers.ContainsKey(action.name))
                {
                    action.performed += actionHandlers[action.name];
                }
                else
                {
                    Debug.LogWarning($"No handler found for action: {action.name}");
                }
            }
        }
    }

    //private void OnDestroy()
    //{
    //    // イベントハンドラの解除
    //    foreach (var actionMap in playerInput.actions.actionMaps)
    //    {
    //        foreach (var action in actionMap.actions)
    //        {
    //            if (actionHandlers.ContainsKey(action.name))
    //            {
    //                action.performed -= actionHandlers[action.name];
    //            }
    //        }
    //    }
    //}

}
