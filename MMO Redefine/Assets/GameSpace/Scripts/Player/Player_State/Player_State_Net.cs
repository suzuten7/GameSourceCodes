namespace Player
{
    using FNet;
    using Fusion;
    using Obj;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_FireBaseSet;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;

    public partial class Player_State
    {
        Class_State_PlayerValues _bnt_PlayerValues;
        #region Net用
        [Networked] byte _net_ModelMode { get; set; }
        [Networked] int _net_ModelID { get; set; }
        [Networked] NetworkString<_64> _net_ModelExID { get; set; }
        [Networked] Vector2Int _net_ModelExScale { get; set; }
        int _Img2DHash;
        [Networked] byte _net_JobID1 { get; set; }
        [Networked, Capacity(200)] NetworkLinkedList<byte> _net_JTreeLVs1 => default;
        [Networked] byte _net_JobID2 { get; set; }
        [Networked, Capacity(200)] NetworkLinkedList<byte> _net_JTreeLVs2 => default;
        [Networked] bool _net_WepBack { get; set; }
        int _EquipHash;
        [Networked] int _net_EquipHash { get; set; }
        [Networked] int _net_WepSkin1 { get; set; }
        [Networked] int _net_WepSkin2 { get; set; }
        [Networked] int _net_WepSkin3 { get; set; }
        [Networked] int _net_WepSkin4 { get; set; }
        [Networked] int _net_BotID { get; set; }
        [Networked] int _net_CharaID { get; set; }

        #endregion
        protected override bool NetServs()
        {
            if (!base.NetServs()) return false;
            if (_bnt_PlayerValues == null) _bnt_PlayerValues = new Class_State_PlayerValues();
            for (int i = 0; i < PlayerValues.Jobs.Length; i++)
            {
                if (_bnt_PlayerValues.Jobs[i] == null) _bnt_PlayerValues.Jobs[i] = new Class_State_Player_Job();
            }
            if (_bnt_PlayerValues.ModelMode != PlayerValues.ModelMode ||
               _bnt_PlayerValues.ModelID != PlayerValues.ModelID ||
               _bnt_PlayerValues.ModelExID != PlayerValues.ModelExID||
               _bnt_PlayerValues.ModelExScale != PlayerValues.ModelExScale
               )
            {
                _bnt_PlayerValues.ModelMode = PlayerValues.ModelMode;
                _bnt_PlayerValues.ModelID = PlayerValues.ModelID;
                _bnt_PlayerValues.ModelExID = PlayerValues.ModelExID;
                _bnt_PlayerValues.ModelExScale = PlayerValues.ModelExScale;
                RPC_ServModel(PlayerValues.ModelMode, PlayerValues.ModelID, PlayerValues.ModelExID, PlayerValues.ModelExScale);
            }


            if (PlayerValues.ModelMode == 2) _ = FBSet_Img2D_Save();

            var BJob1 = _bnt_PlayerValues.Jobs[0];
            var BJob2 = _bnt_PlayerValues.Jobs[1];
            var CJob1 = PlayerValues.Jobs[0];
            var CJob2 = PlayerValues.Jobs[1];
            if (
               BJob1.ID != CJob1.ID ||
               BJob2.ID != CJob2.ID ||
               _bnt_PlayerValues.WepBack != PlayerValues.WepBack)
            {
                BJob1.ID = CJob1.ID;
                BJob2.ID = CJob2.ID;
                _bnt_PlayerValues.WepBack = PlayerValues.WepBack;
                RPC_ServPSet(CJob1.ID, CJob2.ID, PlayerValues.WepBack);
            }
            var skinCheck = false;
            for (int i = 0; i < 4; i++)
            {
                if (_bnt_PlayerValues.WeponSkin[i] != PlayerValues.WeponSkin[i])
                {
                    _bnt_PlayerValues.WeponSkin[i] = PlayerValues.WeponSkin[i];
                    skinCheck = true;

                }
            }
            if (skinCheck) RPC_ServSkin(
                PlayerValues.WeponSkin[0],
                PlayerValues.WeponSkin[1],
                PlayerValues.WeponSkin[2],
                PlayerValues.WeponSkin[3]);

            _ = FBSet_Equip_Save();

            for (int m = 0; m < 2; m++)
            {
                var jtreechange = false;
                var CJob = PlayerValues.Jobs[m];
                var BJob = _bnt_PlayerValues.Jobs[m];
                if (BJob.Trees.Count != CJob.Trees.Count) jtreechange = true;
                if (!jtreechange)
                    for (int i = 0; i < CJob.Trees.Count; i++)
                    {
                        if (BJob.Trees[i].LVs.Count != CJob.Trees[i].LVs.Count)
                        {
                            jtreechange = true;
                            break;
                        }
                        for (int k = 0; k < CJob.Trees[i].LVs.Count; k++)
                        {
                            if (BJob.Trees[i].LVs[k] != CJob.Trees[i].LVs[k])
                            {
                                jtreechange = true;
                                break;
                            }
                        }
                        if (jtreechange) break;

                    }
                if (jtreechange)
                {
                    BJob.Trees.Clear();
                    List<byte> JTBytes = new List<byte>();
                    for (int i = 0; i < CJob.Trees.Count; i++)
                    {
                        BJob.Trees.Add(new Class_JobTrees());
                        for (int k = 0; k < CJob.Trees[i].LVs.Count; k++)
                        {
                            BJob.Trees[i].LVs.Add(CJob.Trees[i].LVs[k]);
                            JTBytes.Add(CJob.Trees[i].LVs[k]);
                        }
                    }
                    RPC_ServSetJTree((byte)m, JTBytes.ToArray());
                }
            }
            return true;
        }
        string DocStr
        {
            get
            {
                return InsRunner.SessionInfo.Name + "_" + Object.Id.Raw.ToString();
            }
        }
        protected override void NetsLocalSet()
        {

            base.NetsLocalSet();
            PlayerValues.ModelMode = _net_ModelMode;
            PlayerValues.ModelID = _net_ModelID;
            PlayerValues.ModelExID = _net_ModelExID.Value;
            PlayerValues.ModelExScale = _net_ModelExScale;

            if (PlayerValues.ModelMode == 2) _ = FBSet_Img2D_Load();

            PlayerValues.WeponSkin[0] = _net_WepSkin1;
            PlayerValues.WeponSkin[1] = _net_WepSkin2;
            PlayerValues.WeponSkin[2] = _net_WepSkin3;
            PlayerValues.WeponSkin[3] = _net_WepSkin4;

            for (int m = 0; m < PlayerValues.Jobs.Length; m++)
            {
                var JID = _net_JobID1;
                var JTV = _net_JTreeLVs1;
                switch (m)
                {
                    case 1:
                        JID = _net_JobID2;
                        JTV = _net_JTreeLVs2;
                        break;
                }
                var PVJ = PlayerValues.Jobs[m];
                PVJ.ID = JID;
                PVJ.Trees.Clear();
                var JDGroups = DB.JobDatas[JID].JTGroupSet;
                int group = 0;
                int count = 0;
                for (int i = 0; i < JTV.Count; i++)
                {
                    if (count >= JDGroups[group].JTreeGroup.JobTrees.Count)
                    {
                        group++;
                        count = 0;
                    }
                    if (PVJ.Trees.Count <= group) PVJ.Trees.Add(new Class_JobTrees());
                    if (PVJ.Trees[group].LVs.Count <= count) PVJ.Trees[group].LVs.Add(0);
                    PVJ.Trees[group].LVs[count] = JTV[i];
                    count++;
                }
            }

            _ = FBSet_Equip_Load();

            PlayerValues.WepBack = _net_WepBack;
        }
        async Task FBSet_Img2D_Save()
        {
            var hash = HashGet(PlayerValues.Model2DSet);
            if (_Img2DHash == hash)
            {
                Debug.Log("2D同データ");
                return;
            }
            _Img2DHash = hash;

            Debug.Log("2D送信開始");
            try
            {
                await FireBaseSave("Img2Ds", DocStr + "_Check", "End",false);
                var DataValues = new Dictionary<string, object>()
                {
                    {"Name", PlayerValues.Model2DSet.name},
                    {"Imgs_Count", PlayerValues.Model2DSet.datas.Count},
                    {"Speeds",PlayerValues.Model2DSet.speeds}
                };
                for (int i = 0; i < PlayerValues.Model2DSet.datas.Count; i++)
                {
                    DataValues.Add("Imgs_" + i.ToString("D3"), JsonUtility.ToJson(PlayerValues.Model2DSet.datas[i]));
                }
                await FireBaseSave("Img2Ds", DocStr + "_Data", DataValues);
                var CheckValues = new Dictionary<string, object>()
                {
                    {"End",true },
                    {"Time",FixServerTime()}
                };
                await FireBaseSave("Img2Ds", DocStr + "_Check", CheckValues);
                PlayerValues.ModelExID = hash.ToString();
                Debug.Log("2D送信完了");
            }
            catch
            {
                Debug.Log("2D送信失敗");
            }
        }
        async Task FBSet_Img2D_Load()
        {
            var hash = int.TryParse(PlayerValues.ModelExID, out var v) ? v : 0;
            if (_Img2DHash == hash)
            {
                Debug.Log("2D最新");
                return;
            }
            _Img2DHash = hash;

            var loadCheckSnap = await FireBaseLoadSnap("Img2Ds", DocStr + "_Check");
            if (loadCheckSnap == null)
            {
                Debug.Log("2Dデータがありません");
                return;
            }
            var ends = FireBaseFiledGet(loadCheckSnap, "End",false);
            if (!ends)
            {
                Debug.Log("2Dデータエラー");
                return;
            }

            Debug.Log("2D受信開始");
            try
            {
                var loadDataSnap = await FireBaseLoadSnap("Img2Ds", DocStr + "_Data");
                var name = FireBaseFiledGet(loadDataSnap, "Name","");
                var imgCount = FireBaseFiledGet(loadDataSnap, "Imgs_Count",0);
                var imgs = new List<Class_Save_2DImageData>();
                for(int i = 0; i < imgCount; i++)
                {
                    var img = FireBaseFiledGet(loadDataSnap, "Imgs_" + i.ToString("D3"),"");
                    var modelImg = JsonUtility.FromJson<Class_Save_2DImageData>(img);
                    imgs.Add(modelImg);
                }
                PlayerValues.Model2DSet.datas = imgs;
                var spd = FireBaseFiledGet(loadDataSnap, "Speeds", new int[0]);
                for (int i = 0; i < 4; i++)
                {
                    PlayerValues.Model2DSet.speeds[i] = spd.Length > i ? spd[i] : 100;
                }
                Debug.Log("2D受信完了");
            }
            catch
            {
                Debug.Log("2D受信失敗");
            }
        }
        async Task FBSet_Equip_Save()
        {
            bool equipChange = false;
            for (int i = 0; i < PlayerValues.SetWepons.Length; i++)
            {
                var h1 = HashGet(PlayerValues.SetWepons[i]);
                var h2 = HashGet(_bnt_PlayerValues.SetWepons[i]);
                if (h1 != h2)
                {
                    equipChange = true;
                    break;
                }
            }
            if (!equipChange)
                for (int i = 0; i < PlayerValues.SetArmors.Length; i++)
                {
                    var h1 = HashGet(PlayerValues.SetArmors[i]);
                    var h2 = HashGet(_bnt_PlayerValues.SetArmors[i]);
                    if (h1 != h2)
                    {
                        equipChange = true;
                        break;
                    }
                }
            if (!equipChange)
                for (int i = 0; i < PlayerValues.SetAkuses.Length; i++)
                {
                    var h1 = HashGet(PlayerValues.SetAkuses[i]);
                    var h2 = HashGet(_bnt_PlayerValues.SetAkuses[i]);
                    if (h1 != h2)
                    {
                        equipChange = true;
                        break;
                    }
                }
            if (!equipChange)
            {
                Debug.Log("装備同データ");
                return;
            }
            _bnt_PlayerValues.SetWepons = PlayerValues.SetWepons.ToArray();
            _bnt_PlayerValues.SetArmors = PlayerValues.SetArmors.ToArray();
            _bnt_PlayerValues.SetAkuses = PlayerValues.SetAkuses.ToArray();
            Debug.Log("装備送信開始");
            try
            {
                var Values = new Dictionary<string, object>();
                var Hashs = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    Values.Add("Slot_" + (i).ToString("D2"), JsonUtility.ToJson(PlayerValues.SetWepons[i]));
                    Hashs.Add(HashGet(PlayerValues.SetWepons[i]));
                    Values.Add("Slot_" + (i+4).ToString("D2"), JsonUtility.ToJson(PlayerValues.SetArmors[i]));
                    Hashs.Add(HashGet(PlayerValues.SetArmors[i]));
                    Values.Add("Slot_" + (i+8).ToString("D2"), JsonUtility.ToJson(PlayerValues.SetAkuses[i]));
                    Hashs.Add(HashGet(PlayerValues.SetAkuses[i]));
                }
                await FireBaseSave("Equip", DocStr + "_Data", Values);
                var EHash = HashGet(Hashs);
                _EquipHash = EHash;
                RPC_ServEquip(EHash);
                Debug.Log("装備送信完了" + EHash);
            }
            catch
            {
                Debug.Log("装備送信失敗");
            }
        }
        async Task FBSet_Equip_Load()
        {
            if (_EquipHash == _net_EquipHash)
            {
                Debug.Log("装備最新");
                return;
            }
            _EquipHash = _net_EquipHash;
            Debug.Log("装備受信開始");
            try
            {
                var snap = await FireBaseLoadSnap("Equip", DocStr + "_Data");
                for(int i = 0; i < 12; i++)
                {
                    var data = FireBaseFiledGet(snap, "Slot_" + i.ToString("D2"), "");
                    if (data != "")
                    {
                        var equip = JsonUtility.FromJson<Class_State_EquipmentValues>(data);
                        if (i >= 0 && i < 4) PlayerValues.SetWepons[i] = equip;
                        if (i >= 4 && i < 8) PlayerValues.SetArmors[i - 4] = equip;
                        if (i >= 8 && i < 12) PlayerValues.SetAkuses[i - 8] = equip;
                    }
                }
                Debug.Log("装備受信完了");
            }
            catch
            {
                Debug.Log("装備受信失敗");
            }
        }
        async Task FBSet_EquipItemDrop(int GID, string data)
        {

            var hash = HashGet(data);
            await FireBaseSave("Drop", DocStr + "_" + hash.ToString(), "data", data);
            RPC_ItemDrop(GID, DocStr + "_" + hash.ToString());
            Debug.Log("装備アイテムドロップ"+ GID+ ";"+ hash + ";" +data);
        }
        async Task FBSet_EquipItemGet(int GID, string key)
        {
            
            var dstr = "";
            if (key != "")
            {
                if (int.TryParse(key,out var olv))
                {
                    var equval = new Class_State_EquipmentValues
                    {
                        GID = GID,
                        LV = olv,
                    };
                    dstr = JsonUtility.ToJson(equval);
                }
                else
                {
                    var Snap = await FireBaseLoadSnap("Drop", key);
                    dstr = FireBaseFiledGet(Snap, "data", "");
                }
            }
            LPlayerVal.ItemAdd(GID, dstr);
            Debug.Log("装備アイテム取得" + GID + ";" + key + ";" + dstr);
        }

        #region RPC_値同期
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServModel(byte syModelMode,int syModelID, NetworkString<_64> syModelExID,Vector2Int syModelExScale)
        {
            _net_ModelMode = syModelMode;
            _net_ModelID = syModelID;
            _net_ModelExID = syModelExID;
            _net_ModelExScale = syModelExScale;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServPSet(byte syJob1, byte syJob2, bool syWepb)
        {
            _net_JobID1 = syJob1;
            _net_JobID2 = syJob2;
            _net_WepBack = syWepb;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServSkin(int sk1, int sk2, int sk3, int sk4)
        {
            _net_WepSkin1 = sk1;
            _net_WepSkin2 = sk2;
            _net_WepSkin3 = sk3;
            _net_WepSkin4 = sk4;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServEquip(int hash)
        {
            _net_EquipHash = hash;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServSetJTree(byte jid, byte[] treelvs)
        {
            NetworkLinkedList<byte>? jtrees = null;
            switch (jid)
            {
                case 0: jtrees = _net_JTreeLVs1; break;
                case 1: jtrees = _net_JTreeLVs2; break;
            }
            jtrees.Value.Clear();
            for (int i = 0; i < treelvs.Length; i++) jtrees.Value.Add(treelvs[i]);
        }
        #endregion
        #region RPC_処理
        [Rpc(RpcSources.All, RpcTargets.InputAuthority)]
        async public void RPC_ItemAdd(int GID, NetworkString<_64> itemDataStr)
        {
            Debug.Log("アイテム取得"+GID + ":"+ itemDataStr.Value);
            switch (ItemGIDCategoryGet(GID))
            {
                default:
                    Debug.Log("アイテム");
                    LPlayerVal.ItemAdd(GID, itemDataStr.Value);
                    break;
                case Enum_ItemID.Wepon:
                case Enum_ItemID.Armor:
                case Enum_ItemID.Akuse:
                    Debug.Log("装備");
                    await FBSet_EquipItemGet(GID, itemDataStr.Value);
                    break;
            }
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ItemDrop(int itemGID, NetworkString<_64> itemDataStr)
        {
            Runner.Spawn(DB.ItemObj, PosGet, SettingValues.Rig.rotation, PlayerRef.None,
            onBeforeSpawned: (runner, obj) =>
            {
                var itemVect = new Vector3(Random.value - 0.5f, Random.value / 2f, Random.value - 0.5f).normalized
                                * Random.Range(40f, 200f) * 0.01f;
                Fusion_RigSync.NStartSet(obj, PosGet, itemVect, SettingValues.Rig.rotation);
                var iobj = obj.GetComponent<Obj_ItemObj>();
                iobj.ItemGID = itemGID;
                iobj.ItemDataStr = itemDataStr;
            });
        }

        void OffItemDrop(int itemGID, Class_State_EquipmentValues EqupVal)
        {
            Runner.Spawn(DB.ItemObj, PosGet, SettingValues.Rig.rotation, PlayerRef.None,
            onBeforeSpawned: (runner, obj) =>
            {
                var itemVect = new Vector3(Random.value - 0.5f, Random.value / 2f, Random.value - 0.5f).normalized
                                * Random.Range(40f, 200f) * 0.01f;
                Fusion_RigSync.NStartSet(obj, PosGet, itemVect, SettingValues.Rig.rotation);
                var iobj = obj.GetComponent<Obj_ItemObj>();
                iobj.ItemGID = itemGID;
                iobj.ItemDataStr = "";
                iobj.EquipVal = EqupVal;
            });
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_BotSpawne(int CharaID, int BotID)
        {
            var noBot = true;
            foreach (var pl in PStaList)
            {
                if (pl == null) continue;
                if (pl.Object.InputAuthority != Object.InputAuthority) continue;
                if (BotID > 0)
                {
                    if (pl.BotID != BotID) continue;
                    if (pl.CharaID == CharaID) noBot = false;
                    Runner.Despawn(pl.Object);
                }
                else if (pl.BotID == 0)
                {
                    if (pl.CharaID != CharaID) continue;
                    noBot = false;
                    Runner.Despawn(pl.Object);
                }
            }
            if (!noBot) return;
            var pos = PosGet;
            // プレイヤーを生成
            var rot = Quaternion.Euler(RotGet);
            var pobj = InsRunner.Spawn(
                FMananger.playerPrefab,
                Vector3.zero,
                Quaternion.identity,
                Object.InputAuthority, // 所有者
                onBeforeSpawned: (InsRunner, obj) =>
                {
                    Fusion_RigSync.NStartSet(obj, pos, Vector3.zero, rot);
                    Player_State.NStartSet(obj, CharaID, BotID);
                });
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_BotClear()
        {
            foreach (var pl in PStaList)
            {
                if (pl == null) continue;
                if (pl.Object.InputAuthority != Object.InputAuthority) continue;
                if (pl.BotID >= 0)Runner.Despawn(pl.Object);
            }
        }

        [Rpc(RpcSources.All,RpcTargets.InputAuthority)]
        public void RPC_EnemyAdd(int ID,bool kill)
        {
            EnemyAdd(ID,kill);
        }
        #endregion
    }
}
