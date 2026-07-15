using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Net_Lobbys : MonoBehaviour, INetworkRunnerCallbacks
{
    public List<Button> Buttons;
    NetworkRunner crunner;
    private void Update()
    {
        if(crunner == null && Net_Connect.InsRunner != null)
        {
            crunner = Net_Connect.InsRunner;
            Net_Connect.InsRunner.AddCallbacks(this);
        }
    }
    public void OnSessionListUpdated(NetworkRunner runner,List<SessionInfo> sessionList)
    {
        Debug.Log("ロビー更新");
        for(int i = 0; i < Mathf.Max(Buttons.Count, sessionList.Count); i++)
        {
            if (i >= sessionList.Count)
            {
                Buttons[i].gameObject.SetActive(false);
                continue;
            }
            if(i >= Buttons.Count)
            {
                Buttons.Add(Instantiate(Buttons[0], Buttons[0].transform.parent));
            }
            Buttons[i].gameObject.SetActive(true);
            var name = sessionList[i].Name;
            Buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = name + "(" + sessionList[i].PlayerCount + "/" + sessionList[i].MaxPlayers + ")";
            Buttons[i].onClick.RemoveAllListeners();
            Buttons[i].onClick.AddListener(() => Join(name));
        }
        foreach (var session in sessionList)
        {
            Debug.Log(
                $"Room:{session.Name} " +
                $"Players:{session.PlayerCount}/{session.MaxPlayers}");
        }
    }
    async public void Join(string sname)
    {

        var result = await crunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sname,
            SceneManager = Net_Connect.NetCon.sceneManager,
        });

        Debug.Log(result.Ok? "参加成功" : result.ShutdownReason.ToString());
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
    // 他のコールバック（空実装でOK）
    public void OnInput(NetworkRunner runner, NetworkInput input){}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner){}
    public void OnDisconnectedFromServer(Fusion.NetworkRunner runner, Fusion.Sockets.NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    //public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner){}
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(Fusion.NetworkRunner runner, Fusion.NetworkObject obj, Fusion.PlayerRef player) { }
    public void OnObjectEnterAOI(Fusion.NetworkRunner runner, Fusion.NetworkObject obj, Fusion.PlayerRef player) { }
    public void OnPlayerLeft(Fusion.NetworkRunner runner, Fusion.PlayerRef player) { }
    public void OnReliableDataProgress(Fusion.NetworkRunner runner, Fusion.PlayerRef player, Fusion.Sockets.ReliableKey key, float progress) { }
    public void OnReliableDataReceived(Fusion.NetworkRunner runner, Fusion.PlayerRef player, Fusion.Sockets.ReliableKey key, System.ArraySegment<byte> data) { }

}
