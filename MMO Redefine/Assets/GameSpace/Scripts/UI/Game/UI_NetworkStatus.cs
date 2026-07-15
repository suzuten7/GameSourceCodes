namespace UIs
{
    using Fusion;
    using static FNet.Fusion_Manager;
    using System.Linq;
    using TMPro;
    using UnityEngine;

    using static GmSystem.GS_ChangeSet;

    public class UI_NetworkStatus : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI NetStateText;
        string roomID_Stg;
        string playerNum_Stg;

        void LateUpdate()
        {
            var runner = InsRunner;
            // --- ルーム名・Ping表示 ---
            Net_StgSets(runner);

            // --- Text更新 ---
            if (string.IsNullOrEmpty(roomID_Stg))ChangeText(NetStateText, "");
            else ChangeText(NetStateText,$"{roomID_Stg}\n{playerNum_Stg}");
        }

        // Photon関連
        void Net_StgSets(NetworkRunner runner)
        {
            // --- ルーム名表示 ---
            if (runner != null && SelectMode != GameMode.Single && runner.SessionInfo != null)
            {
                roomID_Stg = $"ルームID : {runner.SessionInfo.Name}";
            }
            else
            {
                roomID_Stg = "";
            }

            // --- Ping平均化 + 色フェード ---
            if (runner != null && runner.IsRunning)
            {

                // プレイヤー人数
                playerNum_Stg = "ルーム人数 : " + runner.ActivePlayers.Count() + " / " + runner.SessionInfo.MaxPlayers;
            }
            else
            {
                playerNum_Stg = "N/A";
            }
        }
    }
}
