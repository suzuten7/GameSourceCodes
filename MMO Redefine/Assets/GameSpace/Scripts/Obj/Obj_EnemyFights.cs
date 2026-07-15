
namespace Obj
{
    using FNet;
    using Fusion;
    using Player;
    using State;
    using System.Collections.Generic;
    using UnityEngine;
    using static FNet.Fusion_Manager;

    public class Obj_EnemyFights : Obj_ActionObject
    {
        [SerializeField] List<State_StateBase> enemys;
        [SerializeField] int enemyLvBase;
        [SerializeField] int enemyLvWave;
        [SerializeField] int enemyCount;
        [SerializeField] float enemySpawneCT;
        [SerializeField] Vector2 enemySpawneRange;
        [SerializeField] float areaSize = 30;
        List<State_StateBase> summons = new();
        int time = 0;
        bool actives = false;
        int wave;
        public override void PlayAction(Player_State PSta)
        {
            RPC_Actived();
        }
        [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
        void RPC_Actived()
        {
            actives = true;
            time = 0;
            wave = 0;
        }
        void FixedUpdate()
        {
            if (!CanControl(Object)) return;
            NoActiond = actives;
            if (!actives) return;
            var check = false;
            foreach(var hit in Physics.OverlapSphere(transform.position, areaSize))
            {
                if (!hit.TryGetComponent<State_StateHit>(out var shit)) continue;
                if (shit.State as Player_State)
                {
                    if (shit.State.HP <= 0) continue;
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                for(int i = 0; i < summons.Count; i++)
                {
                    if (summons[i] == null) continue;
                    //Destroy(summons[i].gameObject);
                    Runner.Despawn(summons[i].Object);
                }
                actives = false;
                summons.RemoveAll(x => x == null);
                return;
            }
            time--;
            var sch = true;
            for (int i = 0; i < summons.Count; i++)
            {
                if (summons[i] == null) continue;
                if (summons[i].HP <= 0) continue;
                sch = false;
            }
            if (sch) time = 0;
            if (time > 0) return;
            time = Mathf.RoundToInt(enemySpawneCT * 60);
            for (int i = 0; i < enemyCount; i++)
            {
                var add = Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(enemySpawneRange.x, enemySpawneRange.y);
                var pos = transform.position + add;
                Runner.Spawn
                (
                    enemys[Random.Range(0, enemys.Count)],
                    pos,
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                    PlayerRef.None,
                    onBeforeSpawned: (runner, obj) =>
                    {
                        Fusion_RigSync.NStartSet(obj,Vector3.zero);
                        var Sta = obj.GetComponent<State_StateBase>();
                        var Lv = Sta.CommonValues.LV;
                        Lv = enemyLvBase + wave * enemyLvWave;
                        Lv += NetValue.LvAdd;
                        if (Lv != Sta.CommonValues.LV) Sta.LvSets(Lv);
                        summons.Add(Sta);
                    }
                );
            }
            wave++;
            summons.RemoveAll(x => x == null);
        }
    }
}
