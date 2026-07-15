using UnityEngine;
using static Suzuten_PlayerSets;
public class DisplayTest : MonoBehaviour
{
    [SerializeField] bool MultiDisplayMode=true;
    [SerializeField, Range(1, 8)]
    private int m_useDisplayCount = 2;

    private void Awake()
    {
        if (!MultiDisplayMode)
        {
            enabled= false;
            return;
        }
        int count = Mathf.Min(Display.displays.Length, m_useDisplayCount);

        for (int i = 0; i < count; ++i)
        {
            Display.displays[i].Activate();
        }
    }
    private void LateUpdate()
    {
        if (MultiDisplayMode)
        {
            for (int i = 0; i < PSs.Length; i++) PSs[i].Cam.targetDisplay = i;
        }
    }


}