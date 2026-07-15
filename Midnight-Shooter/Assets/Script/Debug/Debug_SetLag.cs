#if UNITY_EDITOR
using System.Threading;
using UnityEngine;

/* 内容
 * ・UnityEditorのみ
 * ・重くなった時の再現
 * ・処理負荷を再現
 */

public class Debug_SetLag : MonoBehaviour
{
    void Update()
    {
        //負荷のかけすぎに注意!!
        var pl_Input = Player_Input.PI.pi;
        if (pl_Input.actions["d_Delay"].IsPressed()) { DebugDelay(1.0f); }
        if (pl_Input.actions["d_BusyWait"].IsPressed()) { DebugBusyWait(1.0f); }
    }

    /// <summary>
    /// 遅延発生 再現
    /// </summary>
    /// <param name="seconds"> 遅延発生の時間 </param>
    void DebugDelay(float seconds)
    {
        Debug.LogWarning("<color=#888888>[Debug]</color>" +
            "高負荷テスト開始\n※PCに負荷はかかりません");
        Thread.Sleep((int)(seconds * 1000));
    }

    /// <summary>
    /// 処理負荷発生 再現
    /// </summary>
    /// <param name="seconds"> 処理負荷発生の時間 </param>
    void DebugBusyWait(float seconds)
    {
        Debug.LogWarning("<color=#888888>[Debug]</color>" +
            "CPU負荷テスト開始\n※<color=#660B0B>高負荷注意!!</color>");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < seconds) { }
    }
}
#endif
