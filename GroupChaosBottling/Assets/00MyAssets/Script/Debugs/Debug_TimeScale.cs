using UnityEngine;
public class Debug_TimeScale : MonoBehaviour
{
    [SerializeField] int DebugDispID;
    [SerializeField] int WindowID;

    Rect windowRect = new Rect(0, 0, 100, 30);
    void OnGUI()
    {
        if (!UI_DebugDispSet.DebugDisps[DebugDispID]) return;
        windowRect = GUILayout.Window(WindowID, windowRect, MovableWindow, "TimeScale");
    }
    private void MovableWindow(int windowId)
    {
        GUILayout.Label("Scale: " + Time.timeScale.ToString("f2"));
        Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 10);
        GUI.DragWindow();
    }
}
