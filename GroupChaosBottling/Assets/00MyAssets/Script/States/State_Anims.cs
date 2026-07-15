using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static DataBase;
using static Manifesto;
public class State_Anims : MonoBehaviour
{
    public State_Base Sta;
    [SerializeField] Animator Anim;
    [SerializeField] Transform RightHandTrans;
    [SerializeField] Transform LeftHandTrans;
    Dictionary<Enum_WeponSet, Transform> SetTrans = new Dictionary<Enum_WeponSet, Transform>();
    Dictionary<Enum_WeponSet, int> WeponSets = new Dictionary<Enum_WeponSet, int>();
    Dictionary<Enum_WeponSet, GameObject> WeponSObjs = new Dictionary<Enum_WeponSet, GameObject>();

    void Update()
    {
        if (Sta == null) return;
        Anim.SetInteger("MoveID", Sta.Anim_MoveID);
        Anim.SetInteger("AtkID", Sta.Anim_AtkID);
        Anim.SetFloat("AtkSpeed", Sta.Anim_AtkSpeed);
        Anim.SetInteger("OtherID", Sta.Anim_OtherID);
        if (Anim.avatar != null)
        {
            SetTrans.TryAdd(Enum_WeponSet.右手, Anim.GetBoneTransform(HumanBodyBones.RightHand));
            SetTrans.TryAdd(Enum_WeponSet.左手, Anim.GetBoneTransform(HumanBodyBones.LeftHand));
        }

        if (Sta.WepValues != null)
        {
            var WeponKeys = Sta.WepValues.WeponSets.Keys.ToArray();
            for (int i = 0; i < WeponKeys.Length; i++)
            {
                var WepKey = (Enum_WeponSet)WeponKeys[i];
                WeponSets.TryAdd(WepKey, -1);
                WeponSObjs.TryAdd(WepKey, null);
                if (WeponSets[WepKey] != Sta.WepValues.WeponSets[(int)WepKey])
                {
                    WeponSets[WepKey] = Sta.WepValues.WeponSets[(int)WepKey];
                    if (WeponSObjs[WepKey] != null) Destroy(WeponSObjs[WepKey]);
                    if (WeponSets[WepKey] >= 0)
                    {
                        var InsWepon = Instantiate(DB.Wepons[WeponSets[WepKey]]);
                        InsWepon.transform.localScale *= 1f + Sta.SizeAdd * 0.01f;
                        var Trans = SetTrans.TryGetValue(WepKey, out var oTrans) ? oTrans : null;
                        InsWepon.transform.parent = Trans != null ? Trans : transform;
                        WeponSObjs[WepKey] = InsWepon;
                    }
                }
                var SetWep = WeponSObjs[WepKey];
                if (WeponSObjs[WepKey] != null)
                {
                    SetWep.transform.localPosition = Vector3.zero;
                    SetWep.transform.localPosition += Sta.WepValues.WeponPoss[(int)WepKey];
                    SetWep.transform.localRotation = Quaternion.identity;
                    SetWep.transform.localEulerAngles += Sta.WepValues.WeponRots[(int)WepKey];
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (WeponSObjs.TryGetValue(Enum_WeponSet.右手,out var RWep) && RWep!=null && RightHandTrans != null) RWep.transform.position = RightHandTrans.position;
        if (WeponSObjs.TryGetValue(Enum_WeponSet.左手, out var LWep) && LWep != null&& LeftHandTrans != null) LWep.transform.position = LeftHandTrans.position;
    }
}
