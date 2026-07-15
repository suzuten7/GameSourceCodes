using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class Title_LobbySinUIs : MonoBehaviour
{
    public TitleUIs Titles;
    public TextMeshProUGUI RoomIDTx;
    public TextMeshProUGUI MessageTx;
    public TextMeshProUGUI VersionTx;
    public TextMeshProUGUI PlayersTx;
    [System.NonSerialized] public RoomInfo Room;

    public void Disp(RoomInfo Rooms)
    {
        Room = Rooms;
        RoomIDTx.text = Rooms.Name;
        if (Rooms.CustomProperties.TryGetValue("Message", out var Message) && Message.ToString() != "") MessageTx.text = Message.ToString();
        else MessageTx.text = "のーメッセージ";
        if (Rooms.CustomProperties.TryGetValue("GameStarts", out var GameStarts) && (bool)GameStarts) MessageTx.text += "<color=#FFFF00>(ゲーム中)</color>";
        else MessageTx.text += "<color=#00FF00>(待機中)</color>";
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
    }
    public void Joins()
    {
        Titles.LobbyRoomJoin(Room);
    }
}
