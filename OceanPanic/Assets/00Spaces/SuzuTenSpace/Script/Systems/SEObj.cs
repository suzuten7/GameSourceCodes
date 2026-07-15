using Photon.Pun;
using UnityEngine;

public class SEObj : MonoBehaviourPun
{
    [SerializeField] AudioSource AudioS;
    [SerializeField] GameInfos.MessageE SoundType;
    [SerializeField] int DeleteTime = 0;
    int Times = 0;
    bool Play = false;
    private void Update()
    {
        if (!Play)
        {
            Play = true;
            var CRoom = PhotonNetwork.CurrentRoom;
            var RoomHashs = CRoom.CustomProperties;
            int IOni_Visit = -1;
            int IOni_Pilot = -1;
            if (RoomHashs.TryGetValue("Oni_Visit", out var GOni_Visit)) IOni_Visit = (int)GOni_Visit;
            if (RoomHashs.TryGetValue("Oni_Pilot", out var GOni_Pilot)) IOni_Pilot = (int)GOni_Pilot;
            bool Ifs = true;
            if(SoundType == GameInfos.MessageE.逃走者)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Pilot) Ifs = false;
                if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Visit) Ifs = false;
            }
            if (SoundType == GameInfos.MessageE.鬼)
            {
                Ifs = false;
                if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Pilot) Ifs = true;
                if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Visit) Ifs = true;
            }
            if (Ifs)
            {
                AudioS.Play();
            }
        }
        if (photonView.IsMine)
        {
            Times++;
            if (Times >= DeleteTime) PhotonNetwork.Destroy(gameObject);
        }
    }
}
