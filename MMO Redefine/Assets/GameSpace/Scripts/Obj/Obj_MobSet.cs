
namespace Obj
{
    using Fusion;
    using UnityEngine;
    using FNet;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using State;

    public class Obj_MobSet : MonoBehaviour
    {
        [SerializeField]Enum_MobType MobType;
        [SerializeField, Tooltip("モブプレファブ")] GameObject[] MobObjs;
        [SerializeField, Tooltip("プレイヤー感知距離:x=出現,y=消滅")] Vector2 CheckRange;
        [SerializeField, Tooltip("出現CT(秒)")] float SpawneCT;
        [SerializeField, Tooltip("出現半径x～y")] Vector2 SpawneRange;
        [SerializeField, Tooltip("敵Lv(x～y)(0以下は基礎Lv)")] Vector2Int EnemyLv;
        [Header("変数")]
        [SerializeField]NetworkObject InsMob;
        [SerializeField]int CT = 0;
        float ndis = float.MaxValue;
        int CheckWait = 0;

        enum Enum_MobType
        {
            Zako,
            Sei,
            FBoss,
            RBoss,
            Harvest,
            Mix,
            Other,
        }
        void FixedUpdate()
        {
            var runner = InsRunner;
            if (runner == null || !runner.IsServer)
            {
                return;
            }
            CheckWait--;
            if (CheckWait <= 0)
            {
                CheckWait = 120;
                ndis = float.MaxValue;
                for(int i=0;i<PStaList.Count;i++)
                {
                    var pl = PStaList[i];
                    if (pl == null) continue;
                    var dis = Vector3.Distance(transform.position, pl.SettingValues.Rig.position);
                    ndis = Mathf.Min(ndis, dis);
                }
            }

            var serverTime = FixServerTime();
            if (InsMob != null)
            {
                if (ndis > CheckRange.y)
                {
                    runner.Despawn(InsMob);
                    CT = 0;
                }
                else
                {
                    CT = serverTime + Mathf.RoundToInt(SpawneCT * 60);
                }
                return;
            }
            else
            {
                if (serverTime < CT || ndis > CheckRange.x) return;
                var pos = transform.position;
                pos += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized * Random.Range(SpawneRange.x, SpawneRange.y);
                var rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                InsMob = runner.Spawn(
                    MobObjs[Random.Range(0,MobObjs.Length)], pos, rot, PlayerRef.None,
                    onBeforeSpawned: (runner, obj) =>
                    {
                        Fusion_RigSync.NStartSet(obj, pos, Vector3.zero, rot);
                        var Sta = obj.GetComponent<State_StateBase>();
                        var Lv = Sta.CommonValues.LV;
                        if (EnemyLv.x > 0 && EnemyLv.y > 0) Lv = Random.Range(EnemyLv.x, EnemyLv.y + 1);
                        Lv += NetValue.LvAdd;
                        if(Lv != Sta.CommonValues.LV)Sta.LvSets(Lv);
                    });

            }

        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, CheckRange.x);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CheckRange.y);
            var alpha = 0.1f;
            var Col = Color.blue;
            Col.a = alpha;
            Gizmos.color = Col;
            Gizmos.DrawSphere(transform.position, SpawneRange.y);
            Col = Color.yellow;
            Col.a = alpha;
            Gizmos.color = Col;
            Gizmos.DrawSphere(transform.position,SpawneRange.x);

        }
        private void OnDrawGizmos()
        {
            var col = Color.white;
            var size = 1.0f;
            switch (MobType)
            {
                case Enum_MobType.Zako: col = Color.blue;break;
                case Enum_MobType.Sei:col = Color.cyan;break;
                case Enum_MobType.FBoss:col = Color.red; size = 5f; break;
                case Enum_MobType.RBoss:col = Color.magenta; size = 5f; break;
                case Enum_MobType.Harvest:col = Color.green;break;
                case Enum_MobType.Mix:col = Color.yellow;break;
            }
            col.a = 0.7f;
            Gizmos.color = col;
            Gizmos.DrawCube(transform.position+Vector3.up * 15, new Vector3(size, 30, size));
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 20);
        }
    }
}
