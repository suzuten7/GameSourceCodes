using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Title_Sin_RoomPlayer : MonoBehaviour
{
    public Player PPlayer;
    public TextMeshProUGUI PIDTx;
    public TextMeshProUGUI StateTx;
    public TextMeshProUGUI PlayerNametx;
    public GameObject MasterOnlys;

    public void MasterChanges()
    {
        PhotonNetwork.SetMasterClient(PPlayer);
    }
    public void Kicks()
    {
        PhotonNetwork.CloseConnection(PPlayer);
    }
}
