
namespace FNet
{
    using Fusion.Sockets;
    using Fusion;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class Fusion_RunTemp : MonoBehaviour, INetworkRunnerCallbacks
    {
        bool outlog = false;
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] プレイヤー切断: {player}");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] プレイヤー参加: {player}");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] 入力送信（OnInput）が呼ばれました");
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] 入力がありません: {player}");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] シャットダウン: {shutdownReason}");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] サーバーに接続されました");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] サーバーから切断されました: {reason}");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] 接続要求を受け取りました");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] 接続失敗: {remoteAddress} - {reason}");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] ユーザーシミュレーションメッセージを受信しました");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] セッションリストが更新されました（{sessionList.Count}件）");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] 認証レスポンスを受け取りました");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] ホストマイグレーションが発生しました");
        }

        /*
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] 信頼性付きデータを受信（旧）: {player}, サイズ: {data.Count}バイト");
        }
        */

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] シーンのロードが完了しました");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            if (!outlog) return;
            Debug.Log("[Fusion] シーンのロードを開始しました");
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] オブジェクトがAOIから離れました: {obj.name}, プレイヤー: {player}");
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] オブジェクトがAOIに入りました: {obj.name}, プレイヤー: {player}");
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] 信頼性付きデータ転送進行中: {player}, キー: {key}, 進行度: {progress}");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            if (!outlog) return;
            Debug.Log($"[Fusion] 信頼性付きデータを受信（新）: {player}, キー: {key}, サイズ: {data.Count}バイト");
        }
    }
}
