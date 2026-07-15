using UnityEngine;
using static BattleManager;
using static Manifesto;
public class Player_CamRot : MonoBehaviour
{
    [SerializeField] State_Base Sta;
    [SerializeField] float PosLerpPer;
    [SerializeField] float LookLerpPer;

    void Update()
    {
        PCamLockdObj CLockObj = null;
        for(int i = 0; i < BTManager.PCamLockdList.Count; i++)
        {
            var CLock = BTManager.PCamLockdList[i];
            if (CLock != null && CLock.gameObject.activeInHierarchy) CLockObj = CLock;
        }
        bool SetPos = false;
        bool SetLook = false;
        if (CLockObj != null)
        {
            if (CLockObj.SetPos)
            {
                SetPos = true;
                transform.position = Vector3.Lerp(transform.position, CLockObj.transform.position, PosLerpPer * 0.01f);
            }
            if (CLockObj.SetLook != Enum_PCamLock_Look.無)
            {
                SetLook = true;
                var ObjPos = Vector3.zero;
                switch (CLockObj.SetLook)
                {
                    case Enum_PCamLock_Look.オブジェ位置:ObjPos = CLockObj.transform.position;break;
                    case Enum_PCamLock_Look.キャラ位置:ObjPos = Sta.PosGet();break;
                }
                var TVect = ObjPos - transform.position;
                var LookVect = Vector3.Slerp(transform.forward, TVect.normalized, LookLerpPer * 0.01f); 
                var LookRot = Quaternion.LookRotation(LookVect, Vector3.forward).eulerAngles;
                LookRot.z = 0;
                transform.eulerAngles = LookRot;
            }
        }
        if (!SetPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, PosLerpPer * 0.01f);
        }
        if (!SetLook)
        {
            var TVect = transform.parent.forward;
            var LookVect = Vector3.Slerp(transform.forward, TVect.normalized, LookLerpPer * 0.01f);
            var LookRot = Quaternion.LookRotation(LookVect, Vector3.forward).eulerAngles;
            LookRot.z = 0;
            transform.eulerAngles = LookRot;
        }
    }
}
