using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class SinsUI_LobbyRoomUIs : MonoBehaviour
{
    [Tooltip("ルームIDテキスト"), SerializeField] TextMeshProUGUI RoomIDTx;
    [Tooltip("メッセージテキスト"), SerializeField] TextMeshProUGUI MessageTx;
    [Tooltip("ゲームバージョンテキスト"), SerializeField] TextMeshProUGUI VersionTx;
    [Tooltip("プレイヤー数テキスト"), SerializeField] TextMeshProUGUI PlayersTx;
    [Tooltip("プレイヤー数テキスト"), SerializeField] TextMeshProUGUI JoinTx;
    [System.NonSerialized] public RoomInfo Room;

    public void Disp(RoomInfo Rooms)
    {
        Room = Rooms;
        RoomIDTx.text = Rooms.Name;
        if (Rooms.CustomProperties.TryGetValue("Message", out var Message) && Message.ToString() != "") MessageTx.text = Message.ToString();
        else MessageTx.text = "のーメッセージ";
        if (Rooms.CustomProperties.TryGetValue("GameVer", out var GameVer))
        {
            VersionTx.text = "バージョン:" + GameVer.ToString();
            if (Application.version != GameVer.ToString()) VersionTx.color = Color.red;
            else VersionTx.color = Color.white;
        }
        else
        {
            VersionTx.text = "バージョン:???";
            VersionTx.color = Color.magenta;
        }
        PlayersTx.text = Rooms.PlayerCount + "/" + Rooms.MaxPlayers;
        JoinTx.text = Rooms.IsOpen ? "参加" : "ゲーム中";
    }
    public void Joins()
    {
        PhotonNetwork.JoinRoom(Room.Name);
    }
}
