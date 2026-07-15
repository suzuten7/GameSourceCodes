using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* 内容
 * ・FPSとPingの表示
 */

public class UI_FPSPing : MonoBehaviour
{
    [SerializeField, Tooltip("更新頻度(s)")] float update_Time = 0.5f;
    [SerializeField] TextMeshProUGUI fps_Text;
    [SerializeField] TextMeshProUGUI ping_Text;

    float time_Counter;
    int frame_Counter;
    float fps;

    int pingSampleCount = 30;
    Queue<float> pingSamples = new();


    void Update()
    {
        //FPS計算
        time_Counter += Time.deltaTime;
        frame_Counter++;

        if (time_Counter >= update_Time)
        {
            fps = frame_Counter / time_Counter;

            fps_Text.text = $"FPS : {Library_UI.FormatNum((int)Mathf.Ceil(fps))}";

            time_Counter = 0f;
            frame_Counter = 0;
        }
        Net_StgSets(Net_Connect.InsRunner);
    }
    // Photon関連
    void Net_StgSets(NetworkRunner runner)
    {
        // --- Ping平均化 + 色フェード ---
        if (runner != null && runner.GameMode != GameMode.Single && runner.IsRunning)
        {
            PlayerRef localPlayer = runner.LocalPlayer;
            double rttSeconds = runner.GetPlayerRtt(localPlayer);
            float rttMs = (float)(rttSeconds * 1000.0f);

            pingSamples.Enqueue(rttMs);
            while (pingSamples.Count > pingSampleCount) pingSamples.Dequeue();

            float avgPing = 0f;
            foreach (var p in pingSamples) avgPing += p;
            avgPing /= pingSamples.Count;

            ping_Text.text = $"Ping : {avgPing:F0}ms";
        }
        else
        {
            ping_Text.text = "Ping: --";
        }
    }
}
