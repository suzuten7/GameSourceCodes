
namespace FNet
{
    using Fusion;
    using Fusion.Sockets;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using Player;
    using static Fusion_Manager;
    using static GmSystem.GS_SaveValues;
    public class Fusion_PlayerSet : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] GameObject serverCamPrefab;
        GameObject _servObj;


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("プレイヤー配置");
            if (!runner.IsClient)
            {
                Debug.Log($"Spawn player for {player}");
                //var pos = new Vector3(UnityEngine.Random.Range(-10000f, 10000f), -10000, UnityEngine.Random.Range(-10000f, 10000f));
                var pos = Vector3.zero;
                // プレイヤーを生成
                var rot = Quaternion.identity;
                var pobj = runner.Spawn(
                    FMananger.playerPrefab,
                    pos,
                    rot,
                    player, // 所有者
                    onBeforeSpawned: (runner, obj) =>
                    {
                        Fusion_RigSync.NStartSet(obj, pos,Vector3.zero, rot);
                        Player_State.NStartSet(obj, -1,-1);
                    });
                runner.SetPlayerObject(player, pobj);
            }
            AciveSet("Start", 1);
        }

        // 他のコールバック（空実装でOK）
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner)
        {

        }
        public void OnDisconnectedFromServer(Fusion.NetworkRunner runner, Fusion.Sockets.NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        //public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (SelectMode == GameMode.Server)
            {
                if (_servObj != null) return;
                Debug.Log("サーバーカメラ設定");
                _servObj = Instantiate(serverCamPrefab, Vector3.zero, Quaternion.identity);
            }
        }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(Fusion.NetworkRunner runner, Fusion.NetworkObject obj, Fusion.PlayerRef player) { }
        public void OnObjectEnterAOI(Fusion.NetworkRunner runner, Fusion.NetworkObject obj, Fusion.PlayerRef player) { }
        public void OnPlayerLeft(Fusion.NetworkRunner runner, Fusion.PlayerRef player) { }
        public void OnReliableDataProgress(Fusion.NetworkRunner runner, Fusion.PlayerRef player, Fusion.Sockets.ReliableKey key, float progress) { }
        public void OnReliableDataReceived(Fusion.NetworkRunner runner, Fusion.PlayerRef player, Fusion.Sockets.ReliableKey key, System.ArraySegment<byte> data) { }

    }
}
