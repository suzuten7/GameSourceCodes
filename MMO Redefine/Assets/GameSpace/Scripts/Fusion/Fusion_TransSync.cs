namespace FNet
{
    using Fusion;
    using UnityEngine;
    using static Fusion_Manager;
    public class Fusion_TransSync : NetworkBehaviour
    {
        [SerializeField] Transform Trans;
        [SerializeField] float SyncTime;
        float _times = 0;
        [Networked] Vector3 _net_pos { get; set; }
        [Networked] Quaternion _net_rot { get; set; }

        bool _spawne = false;

        bool _check = false;
        public void Update()
        {
            if (!_spawne) return;
            if (!_check)
            {
                _check = true;
                if (Trans == null) Trans = transform;
            }
            if (CanControl(Object))
            {
                _times -= Time.unscaledDeltaTime;
                if (_times <= 0)
                {
                    _times = Mathf.Max(0.1f, SyncTime);
                    RPC_ServSet(Trans.position, Trans.rotation);
                }
            }
            else
            {
                Trans.position = _net_pos;
                Trans.rotation = _net_rot;
            }
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ServSet(Vector3 pos, Quaternion rot)
        {
            _net_pos = pos;
            _net_rot = rot;
        }


        static public void NStartSetTrans(NetworkObject baseObj, Vector3 pos, Quaternion rot)
        {
            var transSync = baseObj.GetComponentInChildren<Fusion_TransSync>();
            if (transSync == null)
            {
                return;
            }
            transSync._net_pos = pos;
            transSync._net_rot = rot;
        }
        public override void Spawned()
        {
            transform.position = _net_pos;
            transform.rotation = _net_rot;
            _spawne = true;
        }
    }
}
