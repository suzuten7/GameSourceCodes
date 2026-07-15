using Photon.Pun;
using UnityEngine;
using static DataBase;
using static PlayerValue;
using System.Collections.Generic;
using System.Linq;
using static BattleManager;
using UnityEngine.Rendering.Universal;

public class Player_CharaSet : MonoBehaviourPun,IPunObservable
{
    [SerializeField] State_Base Sta;
    [SerializeField] GameObject StateUIObj;
    public int CharaID;
    int BCharaID=-1;
    [SerializeField] GameObject CharaObj;

    void Update()
    {
        if (photonView.IsMine) CharaID = Sta.PLValues.Sets.CharaID;
        if (BCharaID != CharaID)
        {
            BCharaID = CharaID;
            if(CharaObj!=null)Destroy(CharaObj);
            var InsChara = Instantiate(DB.Charas[CharaID].ModelObj, transform.position, transform.rotation);
            InsChara.transform.parent = transform;
            InsChara.Sta = Sta;
            CharaObj = InsChara.gameObject;
        }
        StateUIObj.SetActive(!Sta.photonView.IsMine || Sta.PLValues == null || Sta.PLValues.SubID > 0);
        bool CharaDisp = !Sta.photonView.IsMine || Sta.PLValues == null || Sta.PLValues.SubID > 0;
        if (PSaves.CamDistance > 1.2f) CharaDisp = true;
        else
        {
            PCamLockdObj CLockObj = null;
            for (int i = 0; i < BTManager.PCamLockdList.Count; i++)
            {
                var CLock = BTManager.PCamLockdList[i];
                if (CLock != null && CLock.gameObject.activeInHierarchy) CLockObj = CLock;
            }
            if (CLockObj != null && CLockObj.SetPos) CharaDisp = true;
        }
        if (CharaObj!=null) CharaObj.SetActive(CharaDisp);
    }
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CharaID);
        }
        else
        {
            CharaID = (int)stream.ReceiveNext();
        }
    }
}
