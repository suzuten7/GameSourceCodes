using UnityEngine;
using Fusion;
public class Net_RigSync : NetworkBehaviour
{
    [SerializeField] Rigidbody2D Rig;
    [SerializeField] float SyncTime;
    [SerializeField] bool warp;
    public bool noStop;
    float _times = 0;
    [Networked] Vector3 _net_Pos { get; set; }
    [Networked] Vector3 _net_vect { get; set; }
    [Networked] Quaternion _net_rot { get; set; }
    [Networked] float _net_LastTime { get; set; }
    bool _spawne = false;
    bool _check = false;

    bool stops = false;
    Vector3 svect;
    Vector3 spos;
    public void Update()
    {
        if (!_spawne) return;
        if (!_check)
        {
            _check = true;
            if (Rig == null) Rig = GetComponent<Rigidbody2D>();
        }
        if (Net_Connect.CanControl(Object))
        {
            //Rig.bodyType = RigidbodyType2D.Dynamic;
            _times -= Time.unscaledDeltaTime;
            if (_times <= 0)
            {
                _times = Mathf.Max(0.1f, SyncTime);
                // ネットワーク変数に現在の物理状態を反映

                RPC_ServSet(Rig.transform.position, Rig.linearVelocity, Rig.transform.rotation, Net_Connect.TimeGet);
            }
        }
        else if (!warp)
        {
            var timed = (Net_Connect.TimeGet - _net_LastTime);
            float lerpPer = 1f / Mathf.Max(0.1f, SyncTime) / 60f;
            var dampPow = Rig.linearDamping;
            var gravVect = Physics2D.gravity * Rig.gravityScale;
            var gravPos = Vector3.zero;
            if (dampPow > 0f)
            {
                var invDamp = 1f / dampPow;
                gravPos = (gravVect * invDamp * timed
                    - gravVect * invDamp * invDamp * (1f - Mathf.Exp(-dampPow * timed)))
                    * 0.01f;
            }
            else gravPos = gravVect * timed * timed * 0.5f * 0.01f;
            var dampIntegral = timed;
            if (dampPow > 0f) dampIntegral = (1f / dampPow) * (Mathf.Exp(-dampPow * 0) - Mathf.Exp(-dampPow * timed));

            // ネット数値 から予測位置を計算
            Vector3 targetPos = _net_Pos + _net_vect * dampIntegral + gravPos;

            // 他人のクライアント → 予測位置に従って補間表示
            //Rig.bodyType = RigidbodyType2D.Kinematic;
            Rig.transform.position = Vector3.Lerp(Rig.transform.position, targetPos, 0.5f);
            Rig.transform.rotation = Quaternion.Slerp(Rig.transform.rotation.normalized, _net_rot.normalized, lerpPer);
            Rig.linearVelocity = _net_vect;
        }
        else
        {
            Rig.transform.position = _net_Pos;
            Rig.transform.rotation = _net_rot;
            Rig.linearVelocity = _net_vect;
        }

        if (Obj_LocalObjects.TimeStopd && !noStop)
        {
            if (!stops)
            {
                stops = true;
                svect = Rig.linearVelocity;
                spos = Rig.transform.position;
            }
            Rig.linearVelocity = Vector3.zero;
            Rig.transform.position = spos;
        }
        else
        {
            if (stops)
            {
                stops = false;
                Rig.linearVelocity = svect;
                if(Net_Connect.CanControl(Object)) RPC_ServSet(Rig.transform.position, Rig.linearVelocity, Rig.transform.rotation, Net_Connect.TimeGet);
            }
        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ServSet(Vector3 pos, Vector3 vect, Quaternion rot, float netTime)
    {
        _net_Pos = pos;
        _net_vect = vect;
        _net_rot = rot;
        _net_LastTime = netTime;
    }


    static public void NStartSet(NetworkObject baseObj, Vector3 vect)
    {
        NStartSet(baseObj, baseObj.transform.position, baseObj.transform.eulerAngles, vect);
    }
    static public void NStartSet(NetworkObject baseObj,Vector3 pos,Vector3 rot, Vector3 vect)
    {
        var rigSync = baseObj.GetComponent<Net_RigSync>();
        if (rigSync == null) rigSync = baseObj.GetComponentInChildren<Net_RigSync>();
        if (rigSync == null)
        {
            return;
        }
        rigSync._net_Pos = pos;
        rigSync._net_vect = vect;
        rigSync._net_rot = Quaternion.Euler(rot);
        rigSync._net_LastTime = Net_Connect.InsRunner.Tick * Net_Connect.InsRunner.DeltaTime;
        if (Net_Connect.CanControl(rigSync.Object)) rigSync.Rig.linearVelocity = vect;
    }
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            Rig.transform.position = _net_Pos;
            Rig.transform.rotation = _net_rot;
        }
        _spawne = true;
    }
}
