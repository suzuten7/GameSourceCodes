namespace UIs
{
    using Fusion;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_ChangeSet;

    public class UI_FPSPing : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI FPSPing_Text;
        [SerializeField] Gradient FPS_Grad;
        [SerializeField] Gradient Ping_Grad;

        [SerializeField] int fpsSampleCount = 30;
        [SerializeField] int pingSampleCount = 30;

        string fps_Stg;
        string ping_Stg;

        Queue<float> fpsSamples = new ();
        Queue<float> pingSamples = new ();

        void LateUpdate()
        {
            var runner = InsRunner;

            // --- FPS 平均化 ---
            float currentFPS = 1.0f / Time.deltaTime;
            fpsSamples.Enqueue(currentFPS);
            while (fpsSamples.Count > fpsSampleCount) fpsSamples.Dequeue();

            float avgFPS = 0f;
            foreach (var f in fpsSamples) avgFPS += f;
            avgFPS /= fpsSamples.Count;

            // FPS色フェード
            Color fpsColor = GetFPSColor(avgFPS);
            string fpsColorHex = ColorUtility.ToHtmlStringRGB(fpsColor);
            fps_Stg = $"<color=#{fpsColorHex}>FPS : {avgFPS:F1}</color>";

            // --- ルーム名・Ping表示 ---
            Net_StgSets(runner);

            // --- Text更新 ---
            if (string.IsNullOrEmpty(runner.SessionInfo.Name))
                ChangeText(FPSPing_Text, $"オフライン\n{fps_Stg}");
            else
            {
                string modestr;
                switch (runner.Mode)
                {
                    default: modestr = runner.Mode.ToString(); break;
                    case SimulationModes.Server: modestr = "サーバー"; break;
                    case SimulationModes.Host: modestr = "ホスト"; break;
                    case SimulationModes.Client: modestr = "クライアント"; break;
                }
                ChangeText(FPSPing_Text,$"{runner.SessionInfo.Name}\nモード:{modestr}\n{ping_Stg}\n{fps_Stg}");
            }

        }

        // Photon関連
        void Net_StgSets(NetworkRunner runner)
        {
            // --- Ping平均化 + 色フェード ---
            if (runner != null && runner.IsRunning)
            {
                PlayerRef localPlayer = runner.LocalPlayer;
                double rttSeconds = runner.GetPlayerRtt(localPlayer);
                float rttMs = (float)(rttSeconds * 1000.0f);

                pingSamples.Enqueue(rttMs);
                while (pingSamples.Count > pingSampleCount) pingSamples.Dequeue();

                float avgPing = 0f;
                foreach (var p in pingSamples) avgPing += p;
                avgPing /= pingSamples.Count;

                // Ping色フェード
                Color pingColor = GetPingColor(avgPing);
                string pingColorHex = ColorUtility.ToHtmlStringRGB(pingColor);
                ping_Stg = $"<color=#{pingColorHex}>Ping : {avgPing:F0}ms</color>";
            }
            else
            {
                ping_Stg = "Ping: --";
            }
        }

        // FPS色フェード
        Color GetFPSColor(float fps)
        {
            float t = Mathf.Clamp01(fps / 60f);
            return FPS_Grad.Evaluate(t);
        }

        // Ping色フェード
        Color GetPingColor(float ping)
        {
            float t = 1f - Mathf.Clamp01(ping / 200f);
            return Ping_Grad.Evaluate(t);
        }
    }
}

