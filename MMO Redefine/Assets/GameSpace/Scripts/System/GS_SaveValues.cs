namespace GmSystem
{
    using FNet;
    using Obj;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static FNet.Fusion_Chat;
    using static FNet.Fusion_Reliable;
    using static GmSystem.GS_VroidDictionary;
    public class GS_SaveValues
    {
        #region Class
        [System.Serializable]
        public class Class_Local_CharaSet
        {
            public string Name;
            public byte ModelMode;
            public int ModelID;
            public string ModelVrm;
            public int ScaleVrm;
            public int Model2DID;
            public Vector2Int Scale2D;
            public int SetLV;
            public Class_State_EquipmentValues[] SetWepons;
            public int[] WeponSkin;
            public Class_State_EquipmentValues[] SetArmors;
            public Class_State_EquipmentValues[] SetAkuses;
            public byte[] JobIDs;
            public List<Class_Save_JTreeBase> JobTrees = new ();
            public List<int> ShortCutSets = StartShortCuts;
            public List<int> BotSets = StartShortCuts;
            public Class_Save_BotOption BotOption;
            public Class_Local_CharaSet(int c)
            {
                CharaSetUp(this, c);
            }
            public Class_Local_CharaSet()
            {
                CharaSetUp(this, 0);
            }
            public int LV
            {
                get
                {
                    return SetLV > 0 ? Mathf.Min(SetLV, LPlayerVal.LV) : LPlayerVal.LV;
                }
            }
            string bdata = "";
            Texture2D img = null;
            public Texture PlayerIconGet(out string Name)
            {
                var tx = DB.Models[ModelID].Icon;
                Name = DB.Models[ModelID].Name;

                switch (ModelMode)
                {
                    case 1:
                        var vrm = VroidGet(ModelVrm);
                        if (vrm != null &&vrm.Value.Item2!= null) tx = vrm.Value.Item2;
                        if (vrm != null) Name = vrm.Value.Item1;
                        break;
                    case 2:
                        img = GetSave_2DImages[Model2DID].IconGet;
                        if (img != null) tx = img;
                        break;
                }
                switch (ModelMode)
                {
                    default:
                        Name += "\n{Game}";
                        break;
                    case 1:
                        Name += "\n{Vroid}";
                        break;
                    case 2:
                        Name += "\n{2DImg}";
                        break;
                }

                return tx;
            }
        }
        [System.Serializable]
        public class Class_Local_PlayerValues
        {
            public long PlayTimes;
            public int UseChara;
            public int LV;
            public float EXP;
            public float Gold;
            public Vector3 RespawnePos;

            public Dictionary<int, int> ItemsDic = new ();
            public Dictionary<int, int> ConsumablesDic = new ();
            public List<Class_State_EquipmentValues> Wepons = new();
            public List<Class_State_EquipmentValues> Armors = new ();
            public List<Class_State_EquipmentValues> Akuses = new ();

            public void ItemAdd(int GID, string itemDataStr)
            {
                var idata = new Class_State_EquipmentValues();
                var itemCount = 1;
                if (itemDataStr != "")
                {
                    try
                    {
                        idata = JsonUtility.FromJson<Class_State_EquipmentValues>(itemDataStr);
                    }
                    catch
                    {
                        if (int.TryParse(itemDataStr, out var oItemCount)) itemCount = oItemCount;
                    }
                }
                switch (ItemGIDCategoryGet(GID))
                {
                    case Enum_ItemID.Material:
                    case Enum_ItemID.Consumables:
                        ItemAdd(GID, itemCount);
                        break;
                    case Enum_ItemID.Wepon:
                        idata.GID = GID;
                        Wepons.Add(idata);
                        var WepD = ItemGIDDataGet(GID);
                        Fusion_Chat.LocalMessage(Enum_MesID.System, "武器獲得", WepD.Name + "LV:" + idata.LV);
                        break;
                    case Enum_ItemID.Armor:
                        idata.GID = GID;
                        Armors.Add(idata);
                        var ArmD = ItemGIDDataGet(GID);
                        Fusion_Chat.LocalMessage(Enum_MesID.System, "防具獲得", ArmD.Name + "LV:" + idata.LV);
                        break;
                    case Enum_ItemID.Akuse:
                        idata.GID = GID;
                        Akuses.Add(idata);
                        var AkuD = ItemGIDDataGet(GID);
                        Fusion_Chat.LocalMessage(Enum_MesID.System, "アクセ獲得", AkuD.Name + "LV:" + idata.LV);
                        break;
                }
            }
            public void ItemAdd(int GID, int itemCount)
            {
                switch (ItemGIDCategoryGet(GID))
                {
                    case Enum_ItemID.Material:
                        if (ItemsDic.ContainsKey(GID)) ItemsDic[GID] += itemCount;
                        else ItemsDic.Add(GID, itemCount);
                        var ItemD = ItemGIDDataGet(GID);
                        Fusion_Chat.LocalMessage(Enum_MesID.System, "素材獲得", ItemD.Name + "×" + itemCount);
                        break;
                    case Enum_ItemID.Consumables:
                        if (ConsumablesDic.ContainsKey(GID)) ConsumablesDic[GID] += itemCount;
                        else ConsumablesDic.Add(GID, itemCount);
                        var ConsumD = ItemGIDDataGet(GID);
                        Fusion_Chat.LocalMessage(Enum_MesID.System, "消耗品獲得", ConsumD.Name + "×" + itemCount);
                        break;
                }
            }
            public void ItemRem(int GID, int IndCount)
            {
                switch (ItemGIDCategoryGet(GID))
                {
                    case Enum_ItemID.Material:
                        if (ItemsDic.ContainsKey(GID))
                        {
                            ItemsDic[GID] -= IndCount;
                            if (ItemsDic[GID] <= 0) ItemsDic.Remove(GID);
                        }
                        break;
                    case Enum_ItemID.Consumables:
                        if (ConsumablesDic.ContainsKey(GID))
                        {
                            ConsumablesDic[GID] -= IndCount;
                            if (ConsumablesDic[GID] <= 0) ConsumablesDic.Remove(GID);
                        }
                        break;
                    case Enum_ItemID.Wepon:
                        if (IndCount >= 0 && Wepons.Count > IndCount && Wepons[IndCount].GID == GID) Wepons.RemoveAt(IndCount);
                        if (IndCount < 0)
                        {
                            var find = Wepons.FindIndex(x => x.GID == GID);
                            Wepons.RemoveAt(find);
                        }
                        break;
                    case Enum_ItemID.Armor:
                        if (IndCount >= 0 && Armors.Count > IndCount && Armors[IndCount].GID == GID) Armors.RemoveAt(IndCount);
                        if (IndCount < 0)
                        {
                            var find = Armors.FindIndex(x => x.GID == GID);
                            Armors.RemoveAt(find);
                        }
                        break;
                    case Enum_ItemID.Akuse:
                        if (IndCount >= 0 && Akuses.Count > IndCount && Akuses[IndCount].GID == GID) Akuses.RemoveAt(IndCount);
                        if (IndCount < 0)
                        {
                            var find = Akuses.FindIndex(x => x.GID == GID);
                            Akuses.RemoveAt(find);
                        }
                        break;
                }
            }
            public int ItemCount(int GID, int Index = -1)
            {
                var count = 0;
                switch (ItemGIDCategoryGet(GID))
                {
                    case Enum_ItemID.Material: ItemsDic.TryGetValue(GID, out count); break;
                    case Enum_ItemID.Consumables: ConsumablesDic.TryGetValue(GID, out count); break;
                    case Enum_ItemID.Wepon:
                        if (Index >= 0 && Wepons.Count > Index && Wepons[Index].GID == GID) count = 1;
                        if (Index < 0 && Wepons.Find(x => x.GID == GID) != null) count = 1;
                        break;
                    case Enum_ItemID.Armor:
                        if (Index >= 0 && Armors.Count > Index && Armors[Index].GID == GID) count = 1;
                        if (Index < 0 && Armors.Find(x => x.GID == GID) != null) count = 1;
                        break;
                    case Enum_ItemID.Akuse:
                        if (Index >= 0 && Akuses.Count > Index && Akuses[Index].GID == GID) count = 1;
                        if (Index < 0 && Akuses.Find(x => x.GID == GID) != null) count = 1;
                        break;
                }
                return count;
            }
        }
        [System.Serializable]
        public class Class_Save_2DImageBase
        {
            public string name;
            public List<Class_Save_2DImageData> datas;
            public List<float> speeds = new();
            public Class_Save_2DImageBase()
            {
                name = "無名";
                datas = new List<Class_Save_2DImageData>(1);
                speeds = new List<float>();
            }
            public Texture2D IconGet
            {
                get
                {
                    if (datas.Count > 0)
                    {
                        return datas[0].TextureGet;
                    }
                    else return null;
                }
            }
        }
        [System.Serializable]
        public class Class_Save_2DImageData
        {
            public int type;
            public bool back;
            public bool dot;
            public string data = "";
            string datab = "";
            Texture2D texture = null;
            public Texture2D TextureGet
            {
                get
                {
                    if (datab == data) return texture;
                    datab = data;
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(data);
                        texture = new Texture2D(2, 2);
                        texture.LoadImage(bytes);
                        texture.filterMode = dot ? FilterMode.Point : FilterMode.Bilinear;
                        return texture;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
        [System.Serializable]
        public class Class_Save_JTreeBase
        {
            public List<Class_JobTrees> Groups;
            public Class_Save_JTreeBase()
            {
                Groups = new List<Class_JobTrees>();
            }
            static public List<Class_Save_JTreeBase> Clones(List<Class_Save_JTreeBase> bases)
            {
                var copys = new List<Class_Save_JTreeBase>();
                for (int i = 0; i < bases.Count; i++)
                {
                    copys.Add(new Class_Save_JTreeBase());
                    for (int k = 0; k < bases[i].Groups.Count; k++)
                    {
                        copys[i].Groups.Add(new Class_JobTrees());
                        for (int m = 0; m < bases[i].Groups[k].LVs.Count; m++)
                        {
                            copys[i].Groups[k].LVs.Add(bases[i].Groups[k].LVs[m]);
                        }
                    }
                }
                return copys;
            }
        }
        [System.Serializable]
        public class Class_Save_BotOption
        {
            public Vector2Int MoveRange;

            public int AtkModeCenterSec;
            public int RandModeMoveSec;
            public int RandModeTargetSec;

            public int AtkFBTrySec;
            public int AtkFBChance;
            public int ShortCutTrySec;
            public int ShortCutChance;
            public int ShortCutUse;
            public Class_Save_BotOption()
            {
                MoveRange.x = 50;
                MoveRange.y = 100;

                AtkModeCenterSec = 20;
                RandModeMoveSec = 15;
                RandModeTargetSec = 10;

                AtkFBTrySec = 70;
                AtkFBChance = 500;
                ShortCutTrySec = 30;
                ShortCutChance = 1000;
                ShortCutUse = 1;
            }
            static public Class_Save_BotOption Copy(Class_Save_BotOption cp)
            {
                var copys = new Class_Save_BotOption
                {
                    MoveRange = cp.MoveRange,

                    AtkModeCenterSec = cp.AtkModeCenterSec,
                    RandModeMoveSec = cp.RandModeMoveSec,
                    RandModeTargetSec = cp.RandModeTargetSec,

                    AtkFBTrySec = cp.AtkFBTrySec,
                    AtkFBChance = cp.AtkFBChance,
                    ShortCutTrySec = cp.ShortCutTrySec,
                    ShortCutChance = cp.ShortCutChance,
                    ShortCutUse = cp.ShortCutUse,
                };
                return copys;
            }
        }
        [System.Serializable]
        public class Class_Save_Chara : Class_Local_CharaSet
        {
            public Class_Save_Chara()
            {
                Name = "無名キャラ";
                ModelID = 0;
                ModelVrm = "";
                ScaleVrm = 100;
                Model2DID = 0;
                Scale2D = new Vector2Int(100, 100);
                SetWepons = new Class_State_EquipmentValues[4]
                {
                    new (){ GID = GIDMake(Enum_ItemID.Wepon,(int)Enum_WeponType.LongSword,0), LV = 1 },
                    new (){ GID = GIDMake(Enum_ItemID.Wepon,(int)Enum_WeponType.Shild,0), LV = 1 },
                    new () { GID = GIDMake(Enum_ItemID.Wepon,(int)Enum_WeponType.HandGun,0), LV = 1 },
                    new () { GID = GIDMake(Enum_ItemID.Wepon,(int)Enum_WeponType.HandGun,0), LV = 1 }
                };
                SetArmors = new Class_State_EquipmentValues[4]
                {
                    new (){ GID = -1, LV = 1 },
                    new () { GID = -1, LV = 1 },
                    new () { GID = -1, LV = 1 },
                    new() { GID = -1, LV = 1 }
                };
                SetAkuses = new Class_State_EquipmentValues[4]
                {
                    new () { GID = -1, LV = 1 },
                    new () { GID = -1, LV = 1 },
                    new (){ GID = -1, LV = 1 },
                    new () { GID = -1, LV = 1 }
                };
                JobIDs = new byte[]
                {
                    0,
                    1,
                };
                JobTrees = new List<Class_Save_JTreeBase>();
                ShortCutSets = StartShortCuts;
                BotSets = StartBotSets;
                BotOption = new Class_Save_BotOption();
            }
        }
        [System.Serializable]
        public class Class_Save_State : Class_Local_PlayerValues
        {
            public List<Vector2Int> ItemsList;
            public List<Vector2Int> ConsumablesList;
            public Class_Save_State()
            {
                RespawnePos = new Vector3(0f, 15f, 0f);
                LV = 1;
                EXP = 0;
                ItemsList = new List<Vector2Int>
                {
                    new (GIDMake(Enum_ItemID.Material, (int)Enum_ItemType.Material, 0), 5),
                    new (GIDMake(Enum_ItemID.Material, (int)Enum_ItemType.Material, 1), 3),
                    new (GIDMake(Enum_ItemID.Material, (int)Enum_ItemType.Material, 2), 1),
                };
                ConsumablesList = new List<Vector2Int>
                {
                    new (GIDMake(Enum_ItemID.Consumables, (int)Enum_ConsumableType.Consumables, 0), 10),
                    new (GIDMake(Enum_ItemID.Consumables, (int)Enum_ConsumableType.Consumables, 1), 5),
                    new (GIDMake(Enum_ItemID.Consumables, (int)Enum_ConsumableType.Consumables, 2), 4)
                };
                Wepons = new ();
                Armors = new ();
                Akuses = new ();

            }
        }
        [System.Serializable]
        public class Class_Save_Option
        {
            public int MaxFPS;
            public int QualityLV;
            public bool LongCamUse;
            /// <summary>
            /// 0=ウィンドウ,1=プレイヤー,2=ショートカット,3=ログ,4=モバイル移動,5=モバイル入力
            /// </summary>
            public List<int> UISizes;
            /// <summary>
            /// 0=全体1=味方ダメ,2=敵ダメ,3=味方回復,4=敵回復,5=味方他,6=敵他,7=敵全般,8=その他,9=自プレ,10=他プレ,11=自ボット,12=他ボット,13=自召喚,14=他召喚
            /// </summary>
            public List<int> DSizes;

            public int Cam_MvSpeed;
            public int Cam_TgSpeed;
            public Vector3Int Cam_Pos;
            public Class_Save_Option()
            {
                MaxFPS = 60;
                QualityLV = 3;
                LongCamUse = false;
                UISizes = new List<int>(6);
                for (int i = 0; i < UISizes.Count; i++)
                {
                    UISizes[i] = 100;
                }
                Cam_MvSpeed = 100;
                Cam_TgSpeed = 100;
                Cam_Pos = new Vector3Int(-30, 0, 500);

                DSizes = new List<int>(15);
                for(int i = 0; i < DSizes.Count; i++)
                {
                    DSizes[i] = 100;
                }

            }
        }
        [System.Serializable]
        public class Class_JobTrees
        {
            public List<byte> LVs;
            public Class_JobTrees()
            {
                LVs = new List<byte>();
            }
        }
        [System.Serializable]
        public class Class_Save_AciveBase
        {
            public List<Class_Save_AciveSin> Acives;
            public Class_Save_AciveBase()
            {
                Acives = new List<Class_Save_AciveSin>();
            }
        }
        [System.Serializable]
        public class Class_Save_AciveSin
        {
            public string ID;
            public byte Get;
            public string Progress;
        }
        [System.Serializable]
        public class Class_Save_EnemyBase
        {
            public int Kills;
            public List<Class_Save_EnemySin> Enemys;
            public Class_Save_EnemyBase()
            {
                Enemys = new();
            }
        }
        [System.Serializable]
        public class Class_Save_EnemySin
        {
            public int ID;
            public int Kills;
        }
        #endregion
        #region Settings
        static string Paths
        {
            get
            {
                var path = Application.persistentDataPath + "/" + SaveNameGet;
                return path;
            }
        }
        static public string SaveNameGet
        {
            get
            {
                var name = Application.isEditor ? "Save_Edit" : "Save_Bulid";
#if UNITY_EDITOR
                string projectPath = Directory.GetParent(Application.dataPath).FullName;
                string projectName = Path.GetFileName(projectPath);
                name += "_" + projectName;
#endif
                return name;
            }
        }
        static public List<int> StartShortCuts
        {
            get
            {
                return new List<int>
                    {
                    GIDMake(Enum_ItemID.Consumables, (int)Enum_ConsumableType.Consumables, 0),
                    GIDMake(Enum_ItemID.Consumables, (int)Enum_ConsumableType.Consumables, 1),
                    GIDMake(Enum_ItemID.Consumables, (int)Enum_ConsumableType.Consumables, 2),
                    GIDMake(Enum_ItemID.Skill, 0, 0),
                    GIDMake(Enum_ItemID.Skill, 0, 1),
                    GIDMake(Enum_ItemID.Skill, 0, 2),
                    GIDMake(Enum_ItemID.Skill, 0, 3),
                    GIDMake(Enum_ItemID.Skill, 0, 4),
                    GIDMake(Enum_ItemID.Skill, 0, 5),
                    GIDMake(Enum_ItemID.Skill, 0, 6),
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                     };
            }
        }
        static public List<int> StartBotSets
        {
            get
            {
                return new List<int>
                    {
                    GIDMake(Enum_ItemID.Skill, 0, 0),
                    GIDMake(Enum_ItemID.Skill, 0, 1),
                    GIDMake(Enum_ItemID.Skill, 0, 2),
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                    -1,
                     };
            }
        }
        static public float NextEXPGet(int LV)
        {
            return (400 + (LV * 100)) * Mathf.Pow(1.05f, LV / 10);
        }
        #endregion
        #region Statics
        static public string PlayerName;
        static public Class_Local_PlayerValues LPlayerVal = new ();
        static public List<Class_Local_CharaSet> LPlayerCharas = new ();
        static Class_Save_State Save_State;
        static public Class_Save_State GetSave_State
        {
            get
            {
                if (Save_State == null) Load("State");
                if (Save_State.UseChara >= GetSave_Charas.Count)
                {
                    Debug.Log("キャラ" + Save_State.UseChara + "が超えています");
                    Save_State.UseChara = 0;
                }

                return Save_State;
            }
        }
        static List<Class_Save_Chara> Save_Charas;
        static public List<Class_Save_Chara> GetSave_Charas
        {
            get
            {
                if (Save_Charas == null) Load("Charas");
                return Save_Charas;
            }
        }
        static Class_Save_Option Save_Option;
        static public Class_Save_Option GetSave_Option
        {
            get
            {
                if (Save_Option == null) Load("Option");
                return Save_Option;
            }
        }
        static List<Class_Save_2DImageBase> Save_2DImages;
        static public List<Class_Save_2DImageBase> GetSave_2DImages
        {
            get
            {
                if (Save_2DImages == null) Load("2DImages");
                return Save_2DImages;
            }
        }

        static Class_Save_AciveBase Save_Acive;
        static public Class_Save_AciveBase GetSave_Acive
        {
            get
            {
                if (Save_Acive == null) Load("Acive");
                return Save_Acive;
            }
        }

        static Class_Save_EnemyBase Save_Enemy;
        static public Class_Save_EnemyBase GetSave_Enemy
        {
            get
            {
                if (Save_Enemy == null) Load("Enemy");
                return Save_Enemy;
            }
        }
        #endregion
        #region Meshod
        static public void SaveSetLocal()
        {
            LPlayerVal.PlayTimes = Save_State.PlayTimes;
            LPlayerVal.UseChara = Save_State.UseChara;
            LPlayerVal.LV = Save_State.LV;
            LPlayerVal.EXP = Save_State.EXP;
            LPlayerVal.Gold = Save_State.Gold;
            LPlayerVal.RespawnePos = Save_State.RespawnePos;
            LPlayerVal.ItemsDic.Clear();
            for (int i = 0; i < Save_State.ItemsList.Count; i++) LPlayerVal.ItemsDic.TryAdd(Save_State.ItemsList[i].x, Save_State.ItemsList[i].y);
            LPlayerVal.ConsumablesDic.Clear();
            for (int i = 0; i < Save_State.ConsumablesList.Count; i++) LPlayerVal.ConsumablesDic.TryAdd(Save_State.ConsumablesList[i].x, Save_State.ConsumablesList[i].y);

            LPlayerVal.Wepons.Clear();
            for (int i = 0; i < Save_State.Wepons.Count; i++)
            {
                LPlayerVal.Wepons.Add(Save_State.Wepons[i]);
            }
            LPlayerVal.Armors.Clear();
            for (int i = 0; i < Save_State.Armors.Count; i++)
            {
                LPlayerVal.Armors.Add(Save_State.Armors[i]);
            }
            LPlayerVal.Akuses.Clear();
            for (int i = 0; i < Save_State.Akuses.Count; i++)
            {
                LPlayerVal.Akuses.Add(Save_State.Akuses[i]);
            }
            for (int c = 0; c < GetSave_Charas.Count; c++)
            {
                if (c >= LPlayerCharas.Count) LPlayerCharas.Add(new Class_Local_CharaSet());
                CharaSetUp(LPlayerCharas[c], c);
            }
        }
        static public void CharaSetUp(Class_Local_CharaSet LChara, int c)
        {
            LChara.Name = "無名キャラ" + (c + 1);
            var SetCount = 4;
            LChara.SetWepons = new Class_State_EquipmentValues[]
            {
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 }
            };
            LChara.WeponSkin = new int[] { -1, -1, -1, -1};
            LChara.SetArmors = new Class_State_EquipmentValues[]
            {
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 }
            };
            LChara.SetAkuses = new Class_State_EquipmentValues[]
            {
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 },
                   new (){GID = -1,LV = 0 }
            };
            LChara.JobIDs = new byte[2] { 0, 1 };
            LChara.JobTrees = new List<Class_Save_JTreeBase>();
            LChara.ShortCutSets = StartShortCuts;
            LChara.BotSets = StartBotSets;
            LChara.BotOption = new Class_Save_BotOption();
            var GS_Chara = c < GetSave_Charas.Count ? GetSave_Charas[c] : null;
            if (GS_Chara != null)
            {
                LChara.Name = GS_Chara.Name;
                LChara.ModelMode = GS_Chara.ModelMode;
                LChara.ModelID = GS_Chara.ModelID;
                LChara.ModelVrm = GS_Chara.ModelVrm;
                LChara.ScaleVrm = GS_Chara.ScaleVrm;
                LChara.Model2DID = GS_Chara.Model2DID;
                LChara.Scale2D = GS_Chara.Scale2D;
                LChara.SetLV = GS_Chara.SetLV;
                for (int i = 0; i < Mathf.Min(LChara.SetWepons.Length, SetCount); i++) LChara.SetWepons[i] = GS_Chara.SetWepons[i];
                for (int i = 0; i < Mathf.Min(LChara.WeponSkin.Length, SetCount); i++) LChara.WeponSkin[i] = GS_Chara.WeponSkin[i];
                for (int i = 0; i < Mathf.Min(LChara.SetArmors.Length, SetCount); i++) LChara.SetArmors[i] = GS_Chara.SetArmors[i];
                for (int i = 0; i < Mathf.Min(LChara.SetAkuses.Length, SetCount); i++) LChara.SetAkuses[i] = GS_Chara.SetAkuses[i];
                for (int i = 0; i < Mathf.Min(LChara.JobIDs.Length, 2); i++) LChara.JobIDs[i] = GS_Chara.JobIDs[i];
                LChara.JobTrees = Class_Save_JTreeBase.Clones(GS_Chara.JobTrees);
                for (int i = 0; i < Mathf.Min(GS_Chara.ShortCutSets.Count, 20); i++) LChara.ShortCutSets[i] = GS_Chara.ShortCutSets[i];
                for (int i = 0; i < Mathf.Min(GS_Chara.BotSets.Count, 20); i++) LChara.BotSets[i] = GS_Chara.BotSets[i];
                LChara.BotOption = Class_Save_BotOption.Copy(GS_Chara.BotOption);
            }
            else
            {
                LChara.ModelMode = 0;
                LChara.ModelID = 0;
                LChara.ModelVrm = "";
                LChara.ScaleVrm = 100;
                LChara.Model2DID = 0;
                LChara.Scale2D = new Vector2Int(100,100);
            }
            for (int i = 0; i < DB.JobDatas.Length; i++)
                {
                    if (LChara.JobTrees.Count <= i) LChara.JobTrees.Add(new Class_Save_JTreeBase());
                    var LJTrees = LChara.JobTrees[i];
                    for (int k = 0; k < DB.JobDatas[i].JTGroupSet.Count; k++)
                    {
                        if (LJTrees.Groups.Count <= k) LJTrees.Groups.Add(new Class_JobTrees());
                        var LJTGroups = LJTrees.Groups[k];
                        for (int m = LJTGroups.LVs.Count; m < DB.JobDatas[i].JTGroupSet[k].JTreeGroup.JobTrees.Count; m++) LJTGroups.LVs.Add(0);
                    }
                }
        }
        static void LocalSetSave()
        {
            var GS_State = GetSave_State;

            GS_State.PlayTimes = LPlayerVal.PlayTimes;
            GS_State.UseChara = LPlayerVal.UseChara;
            GS_State.LV = LPlayerVal.LV;
            GS_State.EXP = LPlayerVal.EXP;
            GS_State.Gold = LPlayerVal.Gold;
            GS_State.RespawnePos = LPlayerVal.RespawnePos;

            GS_State.ItemsList.Clear();
            var ItemKeys = LPlayerVal.ItemsDic.Keys.ToArray();
            for (int i = 0; i < ItemKeys.Length; i++) GS_State.ItemsList.Add(new Vector2Int(ItemKeys[i], LPlayerVal.ItemsDic[ItemKeys[i]]));
            GS_State.ConsumablesList.Clear();
            var ConsumablesKeys = LPlayerVal.ConsumablesDic.Keys.ToArray();
            for (int i = 0; i < ConsumablesKeys.Length; i++) GS_State.ConsumablesList.Add(new Vector2Int(ConsumablesKeys[i], LPlayerVal.ConsumablesDic[ConsumablesKeys[i]]));
            GS_State.Wepons = LPlayerVal.Wepons.ToList();
            GS_State.Armors = LPlayerVal.Armors.ToList();
            GS_State.Akuses = LPlayerVal.Akuses.ToList();


            for (int c = 0; c < LPlayerCharas.Count; c++)
            {
                var LChara = LPlayerCharas[c];
                if (c >= GetSave_Charas.Count) GetSave_Charas.Add(new Class_Save_Chara());
                var GS_Chara = GetSave_Charas[c];
                GS_Chara.Name = LChara.Name;
                GS_Chara.ModelMode = LChara.ModelMode;
                GS_Chara.ModelID = LChara.ModelID;
                GS_Chara.ModelVrm = LChara.ModelVrm;
                GS_Chara.ScaleVrm = LChara.ScaleVrm;
                GS_Chara.Model2DID = LChara.Model2DID;
                GS_Chara.Scale2D = LChara.Scale2D;
                GS_Chara.SetLV = LChara.SetLV;
                for (int i = 0; i < LChara.SetWepons.Length; i++) GS_Chara.SetWepons[i] = LChara.SetWepons[i];
                for (int i = 0; i < LChara.WeponSkin.Length; i++) GS_Chara.WeponSkin[i] = LChara.WeponSkin[i];
                for (int i = 0; i < LChara.SetArmors.Length; i++) GS_Chara.SetArmors[i] = LChara.SetArmors[i];
                for (int i = 0; i < LChara.SetAkuses.Length; i++) GS_Chara.SetAkuses[i] = LChara.SetAkuses[i];
                for (int i = 0; i < LChara.JobIDs.Length; i++)
                {
                    GS_Chara.JobIDs[i] = LChara.JobIDs[i];
                }
                GS_Chara.JobTrees = Class_Save_JTreeBase.Clones(LChara.JobTrees);
                for (int i = 0; i < LChara.ShortCutSets.Count; i++) GS_Chara.ShortCutSets[i] = LChara.ShortCutSets[i];
                for (int i = 0; i < LChara.BotSets.Count; i++) GS_Chara.BotSets[i] = LChara.BotSets[i];
                GS_Chara.BotOption = Class_Save_BotOption.Copy(LChara.BotOption);

            }

        }
        static public void SaveClear()
        {
            Save_State = new Class_Save_State();
            Save_Charas.Clear();
            Save_Charas.Add(new Class_Save_Chara());
        }
        static void SaveFile(string folder, string fileName,string dataStr)
        {
            var fpath = Paths;
            if(folder != "") fpath += "/" + folder;
            try
            {
                if (!Directory.Exists(fpath))
                {
                    Directory.CreateDirectory(fpath);
                }
                var writer = new StreamWriter(fpath + "/" + fileName);
                writer.Write(dataStr);
                writer.Flush();
                writer.Close();
            }
            catch
            {
                Debug.Log("保存失敗" + fpath + "/" + fileName);
            }
        }
        static void SaveTextureAsPNG(string folder, string fileName, Texture2D texture)
        {
            var fpath = Paths;
            if (folder != "") fpath += "/" + folder;
            try
            {
                if (!Directory.Exists(fpath))
                {
                    Directory.CreateDirectory(fpath);
                }
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(fpath + "/" + fileName + ".png", bytes);
            }
            catch
            {
                Debug.Log("保存失敗" + fpath + "/" + fileName + ".png");
            }
        }
        static string LoadFile(string folder, string fileName)
        {
            try
            {
                var fpath = Paths;
                if (folder != "") fpath += "/" + folder;
                var reader = new StreamReader(fpath + "/" + fileName);
                string str = reader.ReadToEnd();
                reader.Close();
                return str;
            }
            catch
            {
                return "";
            }
        }
        static public void Save()
        {
            LocalSetSave();
            PlayerPrefs.SetString("Name_" + SaveNameGet, PlayerName);
            PlayerPrefs.SetString("LCS_LTime_" + SaveNameGet, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            SaveFile("","State.sav", JsonUtility.ToJson(Save_State));
            SaveFile("","Option.sav", JsonUtility.ToJson(Save_Option));
            if (Directory.Exists(Paths + "/Charas"))
            {
                if (Directory.Exists(Paths + "/CharasOld")) Directory.Delete(Paths + "/CharasOld",true);
                Directory.Move(Paths + "/Charas", Paths + "/CharasOld");
            }
            for (int i = 0; i < Save_Charas.Count; i++)
            {
                var fileName = "Charas_" + i.ToString("D3") + ".sav";
                SaveFile("Charas", fileName, JsonUtility.ToJson(Save_Charas[i]));
            }
            if (Directory.Exists(Paths + "/2DImages"))
            {
                if (Directory.Exists(Paths + "/2DImagesOld")) Directory.Delete(Paths + "/2DImagesOld", true);
                Directory.Move(Paths + "/2DImages", Paths + "/2DImagesOld");
            }
            for (int i = 0; i < Save_2DImages.Count; i++)
            {
                var fileName = "2DImages_" + i.ToString("D3") + ".sav";
                SaveFile("2DImages", fileName, JsonUtility.ToJson(Save_2DImages[i]));

                //SaveTextureAsPNG("2DImagePng","Img_" + i, UIs.UI_2DSet_Base.PaletteStrToTexture(Save_2DImages[i].Sets[0].Data));
                
            }
            SaveFile("", "Acive.sav", JsonUtility.ToJson(Save_Acive));
            SaveFile("", "Enemy.sav", JsonUtility.ToJson(Save_Enemy));

        }
        static public void Load(string Type)
        {
            string loadJson;
            switch (Type)
            {
                case "State":
                    loadJson = LoadFile("","State.sav");
                    if (loadJson != "")
                    {
                        Save_State = JsonUtility.FromJson<Class_Save_State>(loadJson);
                    }
                    else
                    {
                        Save_State = new Class_Save_State();
                    }
                    break;
                case "Option":
                    loadJson = LoadFile("", "Option.sav");
                    if (loadJson != "")
                    {
                        Save_Option = JsonUtility.FromJson<Class_Save_Option>(loadJson);
                    }
                    else
                    {
                        Save_Option = new Class_Save_Option();
                    }
                    for (int i = Save_Option.UISizes.Count; i < 6; i++) Save_Option.UISizes.Add(100);
                    for (int i = Save_Option.DSizes.Count; i < 15; i++) Save_Option.DSizes.Add(100);
                    break;
                case "Charas":
                    Save_Charas = new List<Class_Save_Chara>();
                    var charaPaths = new string[] {"Charas","CharasOld" };
                    foreach(var cpath in charaPaths)
                    {
                        if (Directory.Exists(Paths + "/" + cpath))
                        {
                            string[] files = Directory.GetFiles(Paths + "/" + cpath, "*.sav");
                            for (int i = 0; i < files.Length; i++)
                            {
                                string fileName = Path.GetFileName(files[i]);
                                loadJson = LoadFile(cpath, fileName);
                                if (loadJson != "")
                                {
                                    var LoadChara = JsonUtility.FromJson<Class_Save_Chara>(loadJson);
                                    Save_Charas.Add(LoadChara);
                                }
                            }
                            if(Save_Charas.Count>0)break;
                        }
                    }

                    if (Save_Charas == null || Save_Charas.Count <= 0)
                    {
                        Save_Charas = new ()
                        {
                            new ()
                        };
                    }
                    break;
                case "2DImages":
                    Save_2DImages = new List<Class_Save_2DImageBase>();
                    var Path2D = new string[] { "2DImages", "2DImagesOld" };
                    foreach (var cpath in Path2D)
                    {
                        if (Directory.Exists(Paths + "/" + cpath))
                        {
                            string[] files = Directory.GetFiles(Paths + "/" + cpath, "*.sav");
                            for (int i = 0; i < files.Length; i++)
                            {
                                string fileName = Path.GetFileName(files[i]);
                                loadJson = LoadFile(cpath, fileName);
                                if (loadJson != "")
                                {
                                    var LoadChara = JsonUtility.FromJson<Class_Save_2DImageBase>(loadJson);
                                    Save_2DImages.Add(LoadChara);
                                }
                            }
                            if (Save_2DImages.Count > 0) break;
                        }
                    }

                    if (Save_2DImages == null || Save_2DImages.Count <= 0)Save_2DImages = new(){new()};
                    break;
                case "Acive":
                    loadJson = LoadFile("", "Acive.sav");
                    if (loadJson != "")Save_Acive = JsonUtility.FromJson<Class_Save_AciveBase>(loadJson);
                    else Save_Acive = new ();
                    break;
                case "Enemy":
                    loadJson = LoadFile("", "Enemy.sav");
                    if (loadJson != "") Save_Enemy = JsonUtility.FromJson<Class_Save_EnemyBase>(loadJson);
                    else Save_Enemy = new ();
                        break;
            }

        }

        static public void DataSet(Class_Save_State Set)
        {
            Save_State = Set;
        }
        static public void DataSet(List<Class_Save_Chara> Set)
        {
            Save_Charas = Set;
        }
        static public void DataSet(List<Class_Save_2DImageBase> Set)
        {
            Save_2DImages = Set;
        }
        static public Class_Save_AciveSin AciveGet(string ID,bool nullAdd)
        {
            var aci = GetSave_Acive.Acives.Find(x => x.ID == ID);
            if (aci == null && nullAdd)
            {
                aci = new() { ID = ID };
                GetSave_Acive.Acives.Add(aci);
            }
            return aci;
        }
        static public void AciveSet(string Key,byte mode)
        {
            var aci = AciveGet(Key,mode == 1);
            if (aci == null) return;
            if (mode == 2)
            {
                if (aci == null) return;
                if (aci.Get == 1 || aci.Get == 2)
                {
                    aci.Get = (byte)(aci.Get == 1 ? 2 : 1);
                    aci.Progress = "";
                }
            }
            else
            {
                if (aci == null)
                {
                    aci = new() { ID = Key };
                    GetSave_Acive.Acives.Add(aci);
                }
                var data = DB.Acives.KeyGetDataID(Key).Item1;
                if (aci.Get != 1 && mode == 1)
                Fusion_Reliable.ChatMessage(Enum_MesID.System, "システム", PlayerName + "が実績[" + Key + "]" + data.Name+"を解除しました");
                aci.Get = mode;
            }
        }
        static public void AciveProgressSetM(string Key,System.Object sval)
        {
            var aci = AciveGet(Key,true);
            if (aci.Get == 1) return;
            var val = AciveProgressValGet(Key, sval);
            var data = DB.Acives.KeyGetDataID(Key).Item1;
            switch (data.ProgressType)
            {
                case Datas.Data_Acive.Enum_ProgressType.Int:
                    AciveProgressCheck(Key, Mathf.Max((int)val, (int)sval));
                    break;
                case Datas.Data_Acive.Enum_ProgressType.Float:
                    AciveProgressCheck(Key,Mathf.Max((float)val, (float)sval));
                    break;
            }
        }
        static public void AciveProgressAdd(string Key, System.Object sval)
        {
            var aci = AciveGet(Key,true);
            if (aci.Get == 1) return;
            var val = AciveProgressValGet(Key, sval);
            var data = DB.Acives.KeyGetDataID(Key).Item1;
            switch (data.ProgressType)
            {
                case Datas.Data_Acive.Enum_ProgressType.Int:
                    AciveProgressCheck(Key, (int)val + (int)sval);
                    break;
                case Datas.Data_Acive.Enum_ProgressType.Float:
                    AciveProgressCheck(Key, (float)val + (float)sval);
                    break;
            }

        }
        static System.Object AciveProgressValGet(string Key, System.Object aval)
        {
            var aci = AciveGet(Key,false);
            if (aci == null) return 0;
            var data = DB.Acives.KeyGetDataID(Key).Item1;
            switch (data.ProgressType)
            {
                case Datas.Data_Acive.Enum_ProgressType.Int:
                    return int.TryParse(aci.Progress, out var oicv) ? oicv : 0;
                case Datas.Data_Acive.Enum_ProgressType.Float:
                    return float.TryParse(aci.Progress, out var ofcv) ? ofcv : 0;
            }
            return 0;
        }
        static void AciveProgressCheck(string Key, System.Object aval)
        {
            var aci = AciveGet(Key,false);
            if (aci == null) return;
            var data = DB.Acives.KeyGetDataID(Key).Item1;
            if (data.ProgressMax == "") return;
            switch (data.ProgressType)
            {
                case Datas.Data_Acive.Enum_ProgressType.Int:
                    aci.Progress = aval.ToString();
                    var imv = int.TryParse(data.ProgressMax, out var oimv) ? oimv : 0;
                    if ((int)aval >= imv) AciveSet(Key, 1);
                    break;
                case Datas.Data_Acive.Enum_ProgressType.Float:
                    aci.Progress = aval.ToString();
                    var fmv = float.TryParse(data.ProgressMax, out var ofmv) ? ofmv : 0;
                    if ((float)aval >= fmv) AciveSet(Key, 1);
                    break;
            }
        }

        static public void EnemyAdd(int ID,bool Kills)
        {
            var e = GetSave_Enemy.Enemys.Find(x => x.ID == ID);
            if (e == null)
            {
                e = new Class_Save_EnemySin {ID = ID,Kills = 0 };
                GetSave_Enemy.Enemys.Add(e);
            }
            if (Kills)
            {
                GetSave_Enemy.Kills++;
                e.Kills++;
            }
        }
        #endregion
    }
}

