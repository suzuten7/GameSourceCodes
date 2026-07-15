
namespace FNet
{
    using Fusion;
    using State;
    using UnityEngine;
    using static Datas.Data_Get;
    using static Fusion_Manager;
    using static UnityEngine.Rendering.DebugUI.Table;

    public class Fusion_NetValue : NetworkBehaviour
    {
        [Networked] public int LvAdd { get; set; }
        [Networked] public int Dific { get; set; }
        [Networked] public int PvPDamMult { get; set; }
        private void Start()
        {
            NetValue = this;
        }
        readonly static public string[] Strs_Dific = new string[] { "低", "中", "高", "超", "極" };
        public enum Enum_DifcVal
        {
            [InspectorName("無")]Non = -1,
            [InspectorName("HP")] HP,
            [InspectorName("攻撃")] Atk,
            [InspectorName("防御")] Def,
            [InspectorName("EXP")] EXP,
        }
        static public float DificChange(int team, Enum_DifcVal ValType)
        {
            if (team > -1000 && team < 0) return 1f;
            if (FNet.Fusion_Manager.NetValue == null) return 1f;
            float CValue = 0;
            switch (FNet.Fusion_Manager.NetValue.Dific)
            {
                case 0:
                    switch (ValType)
                    {
                        case Enum_DifcVal.HP: CValue = -30f; break;
                        case Enum_DifcVal.Atk: CValue = -50f; break;
                        case Enum_DifcVal.Def: CValue = -20f; break;
                        case Enum_DifcVal.EXP: CValue = -60f; break;
                    }
                    break;
                case 2:
                    switch (ValType)
                    {
                        case Enum_DifcVal.HP: CValue = 30f; break;
                        case Enum_DifcVal.Atk: CValue = 40f; break;
                        case Enum_DifcVal.Def: CValue = 20f; break;
                        case Enum_DifcVal.EXP: CValue = 20f; break;
                    }
                    break;
                case 3:
                    switch (ValType)
                    {
                        case Enum_DifcVal.HP: CValue = 75f; break;
                        case Enum_DifcVal.Atk: CValue = 100f; break;
                        case Enum_DifcVal.Def: CValue = 50f; break;
                        case Enum_DifcVal.EXP: CValue = 50f; break;
                    }
                    break;
                case 4:
                    switch (ValType)
                    {
                        case Enum_DifcVal.HP: CValue = 150f; break;
                        case Enum_DifcVal.Atk: CValue = 300f; break;
                        case Enum_DifcVal.Def: CValue = 100f; break;
                        case Enum_DifcVal.EXP: CValue = 100f; break;
                    }
                    break;
            }
            return 1f + CValue * 0.01f;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Rpc_Summon(int ID, int Lv, Vector3 pos,Vector3 rot)
        {
            var obj = Runner.Spawn(
                 DB.Enemys.IDGetData(ID).EnemyObj,
                 pos,
                 Quaternion.Euler(rot),
                 null,
                 onBeforeSpawned: (runner, obj) =>
                 {
                     Fusion_RigSync.NStartSet(obj, pos, Vector3.zero, Quaternion.Euler(rot));
                     var sta = obj.GetComponent<State_StateBase>();
                     if(Lv>0)sta.LvSets(Lv);
                 });
        }
    }
}

