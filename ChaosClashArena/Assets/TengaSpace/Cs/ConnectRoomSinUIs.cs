using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class ConnectRoomSinUIs : MonoBehaviour
{
    [SerializeField] Suzuten_DataBase DB;
    public Suzuten_PhotonUIs Connects;
    public TextMeshProUGUI RoomIDTx;
    public TextMeshProUGUI MessageTx;
    public TextMeshProUGUI CreaterTx;
    public TextMeshProUGUI PlayersTx;
    public TextMeshProUGUI GameVerTx;
    public TextMeshProUGUI CharaTx;
    [System.NonSerialized]public RoomInfo Room;

    public void Disp(RoomInfo Rooms)
    {
        Room = Rooms;
        RoomIDTx.text = Rooms.Name;
        if (Rooms.CustomProperties.TryGetValue("Message", out var Message)&&Message.ToString() != "")MessageTx.text = Message.ToString();
        else MessageTx.text = "のーメッセージ";
        if (Rooms.CustomProperties.TryGetValue("CreaterName", out var Creater)) CreaterTx.text = Creater.ToString();
        else CreaterTx.text = "???";
        PlayersTx.text = Rooms.PlayerCount + "/" + Rooms.MaxPlayers;
        if (Rooms.CustomProperties.TryGetValue("GameVer", out var GameVer)) GameVerTx.text = GameVer.ToString();
        else GameVerTx.text = "404NotNetVer";
        if (Rooms.CustomProperties.TryGetValue("CharaID", out var CharaID))
        {
            if ((int)CharaID >= 0)
            {
                CharaTx.text = "キャラ:" + DB.Charas[(int)CharaID].CharaName;
            }
            else
            {
                switch ((int)CharaID)
                {
                    default:
                        CharaTx.text = "キャラ:ランダム";
                        break;
                    case -2:
                        CharaTx.text = "キャラ:ランダム(除外込み)";
                        break;
                    case -1:
                        CharaTx.text = "キャラ:ランダム(禁止込み)";
                        break;
                }

            }
        }
        else CharaTx.text = "キャラ:???";
    }
    public void Joins()
    {
        Connects.GameStart_Room(Room);
    }
}
