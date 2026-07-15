using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static BackSceneStack;
using static UnityEngine.UI.Button;

public class InputIntefaceSystem_Gabu : MonoBehaviour
{
    #region 変数

    private InputAction[] _action;
    [SerializeField] InputActionAsset playerInput;
    string[] actionNames = new string[0];
    [SerializeField, Header("Menu, Submit, Cancel")] string actionName;
    [SerializeField, Header("開くシーンのインターフェイス")] InstructionsSpaceAnimation targetScene = null;
    [SerializeField, Header("最下層（最初）のシーンか")] bool isLast = false;


    // Event delegates triggered on click.
    [FormerlySerializedAs("onStarted")]
    [SerializeField]
    private ButtonClickedEvent m_OnStardedClick = new ButtonClickedEvent();
    // Event delegates triggered on click.
    [FormerlySerializedAs("onStarted")]
    [SerializeField]
    private ButtonClickedEvent m_OnPreformedClick = new ButtonClickedEvent();
    // Event delegates triggered on click.
    [FormerlySerializedAs("onStarted")]
    [SerializeField]
    private ButtonClickedEvent m_OnCanceledClick = new ButtonClickedEvent();


    #endregion
    #region 関数

    private void OnGetThisAction(InputAction.CallbackContext context)
    {
        if (isLast && backSceneStack.SceneStack.Count > 0)
        {
            return;
        }
        if (context.started)
        {
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            if (m_OnStardedClick is not null)
            {
                m_OnStardedClick.Invoke();
            }
        }
        if (context.performed)
        {
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            if (m_OnPreformedClick is not null)
            {
                m_OnPreformedClick.Invoke();
            }
        }
        if (context.canceled)
        {
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            if (m_OnCanceledClick is not null)
            {
                m_OnCanceledClick.Invoke();
            }
        }
    }


    public void PushScene()
    {
        if (targetScene == null)
        {
            Debug.LogWarning("開くシーンのインターフェイスが設定されていません");
            return;
        }
        if (backSceneStack.SceneStack.Contains(targetScene))
        {
            Debug.LogWarning("このシーンはすでにスタックに存在します");
            return;
        }
        targetScene.Play();
        backSceneStack.Push(targetScene);
    }

    #endregion

    private void Awake()
    {
        actionNames = actionName.Split(',');
        try
        {
            foreach(var name in actionNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    Debug.LogWarning("アクション名が空白です");
                    continue;
                }
                var action = playerInput.FindAction(name.Trim());
                if (action != null)
                {
                    _action = new InputAction[] { action };
                    break; // 最初に見つかったアクションを使用
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("PlayerInputが設定されていません");
        }
        if (_action == null)
        {
            Debug.LogError($"存在しないアクションを指定しています:{actionName} of {playerInput} in {gameObject}");
        }
    }

    private void OnEnable()
    {
        try
        {
            foreach (var _action in _action)
            {
                if (_action == null)
                {
                    Debug.LogWarning("アクションがnullです");
                    continue;
                }
                _action.started += OnGetThisAction;
                _action.performed += OnGetThisAction;
                _action.canceled += OnGetThisAction;
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("アクションが見つかりませんでした");
        }
    }

    private void OnDisable()
    {
        try
        {
            foreach (var _action in _action)
            {
                if (_action == null)
                {
                    Debug.LogWarning("アクションがnullです");
                    continue;
                }
                _action.started -= OnGetThisAction;
                _action.performed -= OnGetThisAction;
                _action.canceled -= OnGetThisAction;
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("アクションが見つかりませんでした");
        }
    }
    public void CancelEvent()
    {
        m_OnCanceledClick.Invoke();
    }
}
