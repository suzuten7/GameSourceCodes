
namespace FNet
{
    using Fusion;
    using State;
    using UnityEngine;
    using static Fusion_Manager;
    public class Fusion_RigSync : NetworkBehaviour
    {
        [SerializeField] Rigidbody Rig;
        [SerializeField] float SyncTime;
        float _times = 0;
        [Networked] Vector3 _net_Pos { get; set; }
        [Networked] Vector3 _net_vect { get; set; }
        [Networked] Quaternion _net_rot { get; set; }
        [Networked] int _net_LastTime { get; set; }
        bool _spawne = false;
        State_RigAdds _rigAdd;
        bool _check = false;
        public void Update()
        {
            if (!_spawne) return;
            if (!_check)
            {
                _check = true;
                if (Rig == null) Rig = GetComponent<Rigidbody>();
                if (Rig != null && _rigAdd == null) _rigAdd = Rig.GetComponent<State_RigAdds>();
            }
            if (CanControl(Object))
            {
                Rig.isKinematic = false;
                _times -= Time.unscaledDeltaTime;
                if (_times <= 0)
                {
                    _times = Mathf.Max(0.1f, SyncTime);
                    // ネットワーク変数に現在の物理状態を反映

                    RPC_ServSet(Rig.position, Rig.linearVelocity,Rig.rotation, FixServerTime());
                }
            }
            else
            {
                var timed = (FixServerTime() - _net_LastTime) / 60f;
                float lerpPer = 1f / Mathf.Max(0.1f, SyncTime) / 60f;
                var dampPow = Rig.linearDamping;
                var gravVect = Vector3.zero;
                if (_rigAdd == null && Rig.useGravity) gravVect = Physics.gravity;
                if (_rigAdd != null) gravVect = Physics.gravity * _rigAdd.GravPer*0.01f;
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
                Rig.isKinematic = true;
                Rig.transform.position = Vector3.Lerp(Rig.position, targetPos,0.5f);
                Rig.transform.rotation = Quaternion.Slerp(Rig.rotation.normalized,_net_rot.normalized, lerpPer);
            }
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ServSet(Vector3 pos, Vector3 vect, Quaternion rot, int netTime)
        {
            _net_Pos = pos;
            _net_vect = vect;
            _net_rot = rot;
            _net_LastTime = netTime;
        }

        static public void NStartSet(NetworkObject baseObj,Vector3 vect)
        {
            NStartSet(baseObj, baseObj.transform.position, vect, baseObj.transform.rotation);
        }
        static public void NStartSet(NetworkObject baseObj, Vector3 pos,Vector3 vect, Quaternion rot)
        {
            var rigSync= baseObj.GetComponent<Fusion_RigSync>();
            if (rigSync == null) rigSync = baseObj.GetComponentInChildren<Fusion_RigSync>();
            if (rigSync == null)
            {
                Fusion_TransSync.NStartSetTrans(baseObj, pos, rot);
                return;
            }
            rigSync._net_Pos = pos;
            rigSync._net_vect = vect;
            rigSync._net_rot = rot;
            rigSync._net_LastTime = FixServerTime();
            if (CanControl(rigSync.Object)) rigSync.Rig.linearVelocity = vect;
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
}
