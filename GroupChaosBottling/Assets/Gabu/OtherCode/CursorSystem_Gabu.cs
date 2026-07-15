using UnityEngine;
using static BattleManager;
public class CursorSystem_Gabu : MonoBehaviour
{
    public bool setCursorVisible = false;
    public GameObject vMouse = null;
    private void Start()
    {
        SetEnable(setCursorVisible);
    }
    private void Update()
    {
        if (BTManager != null && BTManager.End)
        {
            SetEnable(true);
        }
    }
    private void OnEnable()
    {
        SetEnable(setCursorVisible);
    }
    private void OnDisable()
    {
        SetEnable(!setCursorVisible);
    }
    private void SetEnable(bool b)
    {
        //Cursor.visible = b;
        if (vMouse != null)
        {
            vMouse.SetActive(b);
        }
    }
}
