using UnityEngine;
public class Debug_FPSDisp : MonoBehaviour
{
    [SerializeField] int DebugDispID;
    [SerializeField] int WindowID;
    [SerializeField]
    private float m_updateInterval = 0.5f;

    private float m_accum;
    private int m_frames;
    private float m_timeleft;
    private float m_fps;

    private void Update()
    {
        m_timeleft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;

        if (0 < m_timeleft) return;

        m_fps = m_accum / m_frames;
        m_timeleft = m_updateInterval;
        m_accum = 0;
        m_frames = 0;
    }
    Rect windowRect = new Rect(0, 0, 100, 30);
    void OnGUI()
    {
        if (!UI_DebugDispSet.DebugDisps[DebugDispID]) return;
        windowRect = GUILayout.Window(WindowID, windowRect, MovableWindow, "FPS");
    }
    private void MovableWindow(int windowId)
    {
        GUILayout.Label("FPS: " + m_fps.ToString("f2"));

        GUI.DragWindow();
    }
}
