using UnityEngine;
using System.Linq;
using static BattleManager;
public class Debug_BMListGets : MonoBehaviour
{
    [SerializeField] int DebugDispID;
    [SerializeField] int WindowID;
    [SerializeField] float UpdateCT;
    [SerializeField] Type ObjType;

    enum Type
    {
        Enemy,
        Shot,
    }
    float times;
    int Count;
    private void Update()
    {
        times -= Time.unscaledDeltaTime;
        if(times <= 0)
        {
            times = UpdateCT;
            if (BTManager == null)
            {
                Count = 0;
            }
            else
            {
                switch (ObjType)
                {
                    case Type.Enemy:
                        Count = BTManager.StateList.FindAll(x => x.Team != 0).Count;
                        break;
                    case Type.Shot:
                        Count = FindObjectsByType<Shot_Obj>(FindObjectsSortMode.None).Length;
                        break;
                }
            }

        }
    }

    Rect windowRect = new Rect(0, 0, 150, 30);
    void OnGUI()
    {
        if (!UI_DebugDispSet.DebugDisps[DebugDispID]) return;
        windowRect = GUILayout.Window(WindowID, windowRect, MovableWindow, "Count:" + ObjType);
    }
    private void MovableWindow(int windowId)
    {
        GUILayout.Label("Count:" + Count);

        GUI.DragWindow();
    }
}
