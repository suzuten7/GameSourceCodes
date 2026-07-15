using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackSceneStack : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction backAction = null;
    [Header("戻るアクションの名前、こっち設定はして")]
    public string backActionName = "Cancel";
    public static BackSceneStack backSceneStack { get; private set; }

    private Stack<IUIAnimation> sceneStack = new Stack<IUIAnimation>();
    public Stack<IUIAnimation> SceneStack { get => sceneStack;}


    private void Awake()
    {
        if (backAction == null)
        {
            backAction = inputActions.FindAction(backActionName);
            if (backAction == null)
            {
                Debug.LogError($"Back action '{backActionName}' not found in InputActionAsset.");
                return;
            }
        }
        backAction.canceled += ctx => Pop();
        if (backSceneStack != null)
        {
            Destroy(gameObject); return;
        }
        backSceneStack = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Push(IUIAnimation panel)
    {
        SceneStack.Push(panel);
    }

    public void Pop()
    {
        if (SceneStack.Count == 0)
        {
            Debug.LogWarning("スタックは空です");
            return;
        }

        var top = SceneStack.Pop();
        top.Reverse();
    }

    public void PopAll()
    {
        while (SceneStack.Count > 0)
        {
            SceneStack.Pop();
        }
    }
}
