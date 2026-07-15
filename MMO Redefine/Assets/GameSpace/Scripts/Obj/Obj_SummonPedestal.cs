namespace Obj
{
    using FNet;
    using Fusion;
    using Player;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using State;
    using static GmSystem.GS_ChangeSet;
    public class Obj_SummonPedestal : Obj_ActionObject
    {
        [SerializeField] GameObject[] MobObjs;
        [SerializeField, Tooltip("敵Lv(0以下は基礎Lv)")] int Lv;
        [SerializeField] float RemDistance;
        [SerializeField] float SpawneCT;
        [SerializeField] Transform InfoUI;
        [SerializeField] TextMeshProUGUI InfoTx;
        [SerializeField]MeshRenderer[] MRends;
        [SerializeField] Material OnMat;
        [SerializeField] Material NoMat;


        [SerializeField] NetworkObject InsMob;
        [Networked] int _CT { get; set; }
        int CTd
        {
            get
            {
                if (Object == null) return 0;
                else return _CT;
            }
        }
        private void FixedUpdate()
        {
            if (!CanControl(Object)) return;
            var ndis = float.MaxValue;
            foreach (var pl in PStaList)
            {
                if (pl == null) continue;
                var dis = Vector3.Distance(transform.position, pl.SettingValues.Rig.position);
                ndis = Mathf.Min(ndis, dis);
            }
            if (InsMob != null)
            {
                if (ndis > RemDistance)
                {
                    Runner.Despawn(InsMob);
                    _CT = 0;
                }
                else
                {
                    _CT = FixServerTime() + Mathf.RoundToInt(SpawneCT * 60);
                }
            }
            NoActiond = InsMob != null || FixServerTime() < _CT;
        }
        private void LateUpdate()
        {
            if(Camera.main!=null) InfoUI.LookAt(Camera.main.transform.position);
            var nac = Object != null && Object.IsSpawnable && !NoActiond;
            foreach (var mr in MRends) mr.material = !nac ? OnMat : NoMat;
            
            if (!nac)ChangeText(InfoTx, "強敵召喚");
            else
            {
                var times = (CTd - FixServerTime()) / 60;
                ChangeText(InfoTx, "召喚待ち(" + (times / 60).ToString("D2") + ":" + (times % 60).ToString("D2") + ")");
            }
            
        }
        public override void PlayAction(Player_State PSta)
        {
            RPC_Summon();
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Summon()
        {
            if (InsMob != null) return;
            if (FixServerTime() < _CT) return;

            var pos = transform.position;
            var rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            InsMob = Runner.Spawn(
                MobObjs[Random.Range(0, MobObjs.Length)], pos, rot, PlayerRef.None,
                onBeforeSpawned: (runner, obj) =>
                {
                    Fusion_RigSync.NStartSet(obj, pos, Vector3.zero, rot);
                    var Sta = obj.GetComponent<State_StateBase>();
                    var slv = Sta.CommonValues.LV;
                    if (Lv > 0) slv = Lv;
                    slv += NetValue.LvAdd;
                    if (Sta.CommonValues.LV != slv) Sta.LvSets(slv);
                });


        }
    }
}
