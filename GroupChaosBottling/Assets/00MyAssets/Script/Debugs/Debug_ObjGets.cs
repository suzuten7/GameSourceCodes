using UnityEngine;

public class Debug_ObjGets : MonoBehaviour
{
    [SerializeField] int DebugDispID;
    [SerializeField] int WindowID;
    [SerializeField] float UpdateCT;
    [SerializeField] string ObjTag;
    float times;
    int Count;
    private void Update()
    {
        times -= Time.unscaledDeltaTime;
        if(times <= 0)
        {
            times = UpdateCT;
            Count = GameObject.FindGameObjectsWithTag(ObjTag).Length;
        }
    }

    Rect windowRect = new Rect(0, 0, 150, 30);
    void OnGUI()
    {
        if (!UI_DebugDispSet.DebugDisps[DebugDispID]) return;
        windowRect = GUILayout.Window(WindowID, windowRect, MovableWindow, "ObjGets\nTag[" + ObjTag + "]");
    }
    private void MovableWindow(int windowId)
    {
        GUILayout.Space(20);
        GUILayout.Label("\nCount:" + Count);

        GUI.DragWindow();
    }
}
