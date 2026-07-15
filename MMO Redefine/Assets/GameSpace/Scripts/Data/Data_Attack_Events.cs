using System.Collections.Generic;
using UnityEngine;
using static GmSystem.GS_GlobalState;
using static GmSystem.GS_EnumToJpString;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using State;
namespace Datas
{
    public partial class Data_Attack
    {
        [System.Serializable]
        public class Class_AEvent_CalValue
        {
            [TextArea]
            public string CalStr;
            public Class_ValCalc[] DamCals;
            public Class_ValCalc[] DefCals;
            public float DefPer;
            public float Mult;
            public string CalStrGet()
            {
                var ostr = "";
                var rng = 0;
                if (DamCals != null && DamCals.Length > 0)
                {
                    var damcal = CalSStr(DamCals, false);
                    ostr = "攻撃値:" + damcal;
                    rng += damcal.Split("+").Length;
                }
                if (DefCals != null && DefCals.Length > 0)
                {
                    var defcal = CalSStr(DefCals, true);
                    ostr += "\n防御値:" + defcal;
                    if (DefPer > 0 && DefPer != 100) ostr += "×" + DefPer + "%";
                    rng += defcal.Split("+").Length;
                }
                if (ostr != "") ostr += "\n";
                ostr += "倍率×" + Mult + "%";

                if (rng < 4)
                {
                    ostr = ostr.Replace("\n+", "+");
                    ostr = ostr.Replace("\n", ",");
                }
                return ostr;
            }
            public float CalValGet(State_StateBase usta, State_StateBase hsta, Vector2 MultPer, Dictionary<Enum_ValueBase, float> otherValue = null)
            {
                var damc = Calc(DamCals, usta, hsta, otherValue) * (1f + MultPer.x * 0.01f);
                damc = Mathf.Max(0, damc);
                var defc = Calc(DefCals, usta, hsta, otherValue) * (1f + MultPer.y * 0.01f);
                var val = damc >= defc ? (damc - (defc / 2f)) : (Mathf.Pow(damc, 2f) / (defc * 2));

                val *= Mult * 0.01f;
                return val;
            }
        }
        [System.Serializable]
        public class Class_ValCalc
        {
            public bool HitSta;
            public Enum_ValueBase Base;
            public Enum_StateAddsOption Op;
            public float Mult;
            #if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Datas.Data_Attack.Class_ValCalc))]
            public class Class_ValCalcDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // ラベル表示（PrefixLabelで左端のラベル領域を確保）
                    position = EditorGUI.PrefixLabel(position, label);

                    float spacing = 6f;
                    float boolWidth = 50f;
                    float baseWidth = 100f;

                    // HitSta の Rect
                    Rect hitStaRect = new Rect(position.x, position.y, boolWidth, EditorGUIUtility.singleLineHeight);
                    float bX = hitStaRect.xMax + spacing;
                    float bWidth = position.xMax - bX;
                    // Base の Rect
                    Rect baseRect = new Rect(bX, position.y, bWidth, EditorGUIUtility.singleLineHeight);
                    position.y += EditorGUIUtility.singleLineHeight;
                    // Base の Rect
                    Rect opRect = new Rect(position.x, position.y, baseWidth, EditorGUIUtility.singleLineHeight);
                    // Mult は右端まで伸ばすので、残り幅を全て使用
                    float multX = opRect.xMax + spacing;
                    float multWidth = position.xMax - multX;
                    Rect multRect = new Rect(multX, position.y, multWidth, EditorGUIUtility.singleLineHeight);

                    var hitStaProp = property.FindPropertyRelative("HitSta");
                    var baseProp = property.FindPropertyRelative("Base");
                    var opProp = property.FindPropertyRelative("Op");
                    var multProp = property.FindPropertyRelative("Mult");

                    hitStaProp.boolValue = EditorGUI.ToggleLeft(hitStaRect, "受ステ", hitStaProp.boolValue);
                    EditorGUI.PropertyField(baseRect, baseProp, GUIContent.none);
                    EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
                    EditorGUI.PropertyField(multRect, multProp, GUIContent.none);
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight*2;
                }
            }
            #endif
        }
        static public float Calc(Class_ValCalc[] calcs, State_StateBase usta, State_StateBase tsta, Dictionary<Enum_ValueBase, float> otherValue = null)
        {
            float dam = 0;
            foreach (var ca in calcs)
            {
                var val = ca.Mult * 0.01f;
                var sta = !ca.HitSta ? usta : tsta;
                switch (ca.Base)
                {
                    case Enum_ValueBase.Const: val *= 100; break;
                    case Enum_ValueBase.MaxHP: val *= sta.ValGet(Enum_StateAddsType.MaxHP, ca.Op); break;
                    case Enum_ValueBase.HPRegene: val *= sta.ValGet(Enum_StateAddsType.HPRegene, ca.Op); break;
                    case Enum_ValueBase.CHP: val *= sta.HP; break;

                    case Enum_ValueBase.MaxMP: val *= sta.ValGet(Enum_StateAddsType.MaxMP, ca.Op); break;
                    case Enum_ValueBase.MPRegene: val *= sta.ValGet(Enum_StateAddsType.MPRegene, ca.Op); break;
                    case Enum_ValueBase.CMP: val *= sta.MP; break;

                    case Enum_ValueBase.MaxST: val *= sta.ValGet(Enum_StateAddsType.MaxST, ca.Op); break;
                    case Enum_ValueBase.STRegene: val *= sta.ValGet(Enum_StateAddsType.STRegene, ca.Op); break;
                    case Enum_ValueBase.CST: val *= sta.ST; break;

                    case Enum_ValueBase.CEX: val *= sta.EX; break;

                    case Enum_ValueBase.AtkRatio:
                        val = sta.ValGet(Enum_StateAddsType.PAtk, ca.Op) * Mathf.Clamp01(1f - ca.Mult * 0.01f) +
                            sta.ValGet(Enum_StateAddsType.MAtk, ca.Op) * Mathf.Clamp01(ca.Mult * 0.01f);
                        break;
                    case Enum_ValueBase.DefRatio:
                        val = sta.ValGet(Enum_StateAddsType.PDef, ca.Op) * Mathf.Clamp01(1f - ca.Mult * 0.01f) +
                            sta.ValGet(Enum_StateAddsType.MDef, ca.Op) * Mathf.Clamp01(ca.Mult * 0.01f);
                        break;
                    case Enum_ValueBase.PAtk: val *= sta.ValGet(Enum_StateAddsType.PAtk, ca.Op); break;
                    case Enum_ValueBase.MAtk: val *= sta.ValGet(Enum_StateAddsType.MAtk, ca.Op); break;
                    case Enum_ValueBase.PDef: val *= sta.ValGet(Enum_StateAddsType.PDef, ca.Op); break;
                    case Enum_ValueBase.MDef: val *= sta.ValGet(Enum_StateAddsType.MDef, ca.Op); break;

                    case Enum_ValueBase.CritPer: val *= sta.ValGet(Enum_StateAddsType.CritPer, ca.Op); break;
                    case Enum_ValueBase.CritMult: val *= sta.ValGet(Enum_StateAddsType.CritMult, ca.Op); break;
                    case Enum_ValueBase.AtkSpeed: val *= sta.ValGet(Enum_StateAddsType.AtkSpeed, ca.Op); break;
                    case Enum_ValueBase.MoveSpeed: val *= sta.ValGet(Enum_StateAddsType.MoveSpeed, ca.Op); break;

                    case Enum_ValueBase.AddHealMult: val *= sta.ValGet(Enum_StateAddsType.AddHealMult, ca.Op); break;
                    case Enum_ValueBase.TakeHealMult: val *= sta.ValGet(Enum_StateAddsType.TakeHealMult, ca.Op); break;

                    case Enum_ValueBase.AllEleRegist: val *= sta.ValGet(Enum_StateAddsType.AllEleRegist, ca.Op); break;
                    case Enum_ValueBase.NonEleRegist: val *= sta.ValGet(Enum_StateAddsType.NonEleRegist, ca.Op); break;
                    case Enum_ValueBase.FireEleRegist: val *= sta.ValGet(Enum_StateAddsType.FireEleRegist, ca.Op); break;
                    case Enum_ValueBase.WaterEleRegist: val *= sta.ValGet(Enum_StateAddsType.WaterEleRegist, ca.Op); break;
                    case Enum_ValueBase.WindEleRegist: val *= sta.ValGet(Enum_StateAddsType.WindEleRegist, ca.Op); break;
                    case Enum_ValueBase.EarthEleRegist: val *= sta.ValGet(Enum_StateAddsType.EarthEleRegist, ca.Op); break;
                    case Enum_ValueBase.LightEleRegist: val *= sta.ValGet(Enum_StateAddsType.LightEleRegist, ca.Op); break;
                    case Enum_ValueBase.DarkEleRegist: val *= sta.ValGet(Enum_StateAddsType.DarkEleRegist, ca.Op); break;

                    default:
                        if (otherValue == null) val = 0f;
                        else val *= otherValue.TryGetValue(ca.Base, out var oOVal) ? oOVal : 0f;
                        break;
                }
                dam += val;
            }
            return dam;
        }
        static public string CalSStr(Class_ValCalc[] cals, bool hits = false)
        {
            if (cals.Length <= 0) return "0";
            var calstr = "";
            for (int i = 0; i < cals.Length; i++)
            {
                var ca = cals[i];
                if (calstr != "")
                {
                    if (i % 2 == 0) calstr += "\n";
                    calstr += "+";
                }

                var tstaStr = "";
                if (ca.HitSta != hits) tstaStr = !ca.HitSta ? "与." : "受.";
                switch (ca.Base)
                {
                    case Enum_ValueBase.Const: calstr += ca.Mult; break;
                    case Enum_ValueBase.AtkRatio:
                        if (ca.Mult <= 0) calstr += tstaStr + "物攻100%";
                        else if (ca.Mult >= 100) calstr += tstaStr + "魔攻100%";
                        else calstr += tstaStr + "物:魔攻" + (100f - ca.Mult) + ":" + ca.Mult + "%";
                        break;
                    case Enum_ValueBase.DefRatio:
                        if (ca.Mult <= 0) calstr += tstaStr + "物防100%";
                        else if (ca.Mult >= 100) calstr += tstaStr + "魔防100%";
                        else calstr += tstaStr + "物:魔防" + (100f - ca.Mult) + ":" + ca.Mult + "%";
                        break;
                    default:
                        var baseStr = EnumToJp(ca.Base);
                        baseStr = baseStr.Replace("基礎", "基");
                        baseStr = baseStr.Replace("物理", "物");
                        baseStr = baseStr.Replace("魔法", "魔");
                        baseStr = baseStr.Replace("攻撃力", "攻");
                        baseStr = baseStr.Replace("防御力", "防");
                        baseStr = baseStr.Replace("回復", "回");
                        baseStr = baseStr.Replace("速度", "速");
                        baseStr = baseStr.Replace("属性耐性", "属耐");
                        calstr += tstaStr + baseStr + ca.Mult + "%";
                        break;
                }
            }
            return calstr;
        }

        public enum Enum_BranchIfs
        {
            [InspectorName("単入力")]SingleInput,
            [InspectorName("長入力")] LongInput,
            [InspectorName("離入力")] OutInput,
            [InspectorName("未入力")] NonInput,
            [InspectorName("与ダメージ時")] AddDamage = 100,
            [InspectorName("受ダメージ時")] TakeDamage,
            [InspectorName("与回復時")] AddHeal,
            [InspectorName("受回復時")] TakeHeal,
        }
        public enum Enum_RangeType
        {
            [InspectorName("近距離")] Short,
            [InspectorName("中距離")] Midle,
            [InspectorName("遠距離")] Long,
            [InspectorName("その他射程")] Other,
        }
        public enum Enum_AtkType
        {
            [InspectorName("通常")] Normal,
            [InspectorName("重撃")] Hev,
            [InspectorName("スキル")] Skill,
            [InspectorName("必殺")] EX,
            [InspectorName("その他攻撃")] Other,
        }
        public enum Enum_AEventType
        {
            ベース,
            分岐 = 1,
            弾発射 = 2,
            自己移動 = 3,
            自己回転 = 6,
            自己数値変化 = 4,
            自己バフ = 5,
            召喚 = 7,
            アニメーション = 100,
            SE,
            EX = 200,
        }
        public enum Enum_AEventEXs
        {
            [InspectorName("ボット指示_戦闘")] Bot_Battle,
            [InspectorName("ボット指示_追従")] Bot_Follow,
            [InspectorName("ボット指示_散乱")] Bot_Scat,
            [InspectorName("ボット指示_主移動")] Bot_Tp,
            [InspectorName("ショートワープ")] ShortWarp,
            [InspectorName("ランダムヒール")] RandomHeal,
            [InspectorName("ランダムバフ")] RandomBuf,
            [InspectorName("イベントメイク")] EventMake,
            [InspectorName("ハーフハーフ")] HalfHalf,

        }
        public enum Enum_PosBase
        {
            [InspectorName("自身位置")] MyPosition,
            [InspectorName("ターゲット位置")] TargetPositon,
        }
        public enum Enum_RotBase
        {
            [InspectorName("固定")] Fixed = -1,
            [InspectorName("自身向き")] MyLook,
            [InspectorName("自身_ターゲット方向")] MyToTarget,
            [InspectorName("弾_ターゲット方向")] ShotToTarget,
            [InspectorName("カメラ方向")] CameraLook,
        }
        public enum Enum_TransChangeM
        {
            [InspectorName("ズレ")] OffSet,
            [InspectorName("ブレ")] Random,
            [InspectorName("変化")] Change,
        }
        public enum Enum_TransChangeA
        {
            [InspectorName("発数拡散")] CountSpred,
            [InspectorName("発数Sin")] CountSin,
            [InspectorName("発数Cos")] CountCos,
            [InspectorName("時間倍数")] TimeMult=10,
            [InspectorName("時間Sin")] TimeSin,
            [InspectorName("時間Cos")] TimeCos,
        }
        static public Vector3 Atk_TransChange(Class_AEvent_ShotChange sc, int countc,int countm, int time)
        {
            var vect = Vector3.zero;
            var val = 0f;
            var dv = sc.AddVal.y != 0 ? sc.AddVal.y : 1;
            switch (sc.AChange)
            {
                case Enum_TransChangeA.CountSpred:
                    val = countc - (countm - 1) / 2f; break;
                case Enum_TransChangeA.CountSin:
                    val = Mathf.Sin((countc + sc.AddVal.x) / dv * Mathf.PI * 2); break;
                case Enum_TransChangeA.CountCos:
                    val = Mathf.Cos((countc + sc.AddVal.x) / dv * Mathf.PI * 2); break;
                case Enum_TransChangeA.TimeMult:
                    val = (time / 60f + sc.AddVal.x) / dv; break;
                case Enum_TransChangeA.TimeSin:
                    val = Mathf.Sin((time / 60f + sc.AddVal.x) / dv * Mathf.PI * 2); break;
                case Enum_TransChangeA.TimeCos:
                    val = Mathf.Cos((time / 60f + sc.AddVal.x) / dv * Mathf.PI * 2); break;
            }
            switch (sc.TChange)
            {
                case Enum_TransChangeM.OffSet:
                    vect = sc.VectVal;
                    break;
                case Enum_TransChangeM.Random:
                    vect.x = UnityEngine.Random.Range(-sc.VectVal.x, sc.VectVal.x);
                    vect.y = UnityEngine.Random.Range(-sc.VectVal.y, sc.VectVal.y);
                    vect.z = UnityEngine.Random.Range(-sc.VectVal.z, sc.VectVal.z);
                    break;
                case Enum_TransChangeM.Change:
                    vect = sc.VectVal * val;
                    break;
            }
            return vect;
        }
        [System.Serializable]
        abstract public class Class_AEvent_Base
        {
            [HideInInspector] public string DispName;
            public string Memo;
            public int IfBID;
            public int[] IfBIDs;
            public Vector3 Times;
            public static Enum_AEventType EnumName => Enum_AEventType.ベース;
            public virtual Enum_AEventType TypeName => Enum_AEventType.ベース;
            public virtual void DispSet(int index)
            {
                DispName = DStrBase(index);
            }
            protected string DStrBase(int index)
            {
                var dstr = "[" + index + "]" + TypeName + "|";
                if (Memo != "") dstr += "メモ:" + Memo + "|";
                dstr += "BID:" + IfBID;
                if(IfBIDs!=null)for (int i = 0; i < IfBIDs.Length; i++) dstr += "/" + IfBIDs[i];
                dstr += ",時間:(" + TimesStr() + ")秒|";
                return dstr;
            }
            public string TimesStr()
            {
                var str = Times.x.ToString();
                if (Times.y > Times.x) str += "～" + Times.y;
                if (Times.z > 0) str += "間隔" + Times.z;
                return str;
            }
        }
        public class Class_AEvent_BChange : Class_AEvent_Base
        {
            public int NBID;
            public float NTime;
            public Enum_BranchIfs[] Ifs;
            public Class_AttackData_Costs[] Costs;
            public float IfStayTime;

            new public static Enum_AEventType EnumName => Enum_AEventType.分岐;
            public override Enum_AEventType TypeName => Enum_AEventType.分岐;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "NBID:" + NBID;
                dstr += ",NTime:" + NTime + "秒";
                dstr += ",If:×" + Ifs.Length;
                dstr += ",IfStay:" + IfStayTime + "秒";
                DispName = dstr;
            }
        }
        #region AEvent_Shot
        [System.Serializable]
        public class Class_AEvent_ShotMain : Class_AEvent_Base
        {
            public GameObject Shot;
            public Class_AEvent_ShotFire Fire;
            public Class_AEvent_ShotHit[] Hits;
            public float HitCT;
            public float HitEXCharge;
            [Tooltip("挑発ヘイト増加量(x=ダメージ変化補正%,y=挑発値)")] public Vector2 HateAdd;
            new public static Enum_AEventType EnumName => Enum_AEventType.弾発射;
            public override Enum_AEventType TypeName => Enum_AEventType.弾発射;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += (Shot != null ? Shot.name : "無");
                dstr += ",Count:" + Fire.Count;
                dstr += ",Speed:" + Fire.Speed.x;
                if (Fire.Speed.y != Fire.Speed.x) dstr += "～" + Fire.Speed.y;
                dstr += ",TBase:" + Fire.PosBase + "," + Fire.RotBase;
                dstr += ",Hit:×" + Hits.Length;
                dstr += ",HCT:" + HitCT + "秒";
                dstr += ",HEXCh:" + HitEXCharge + "%";
                DispName = dstr;
                for (int i = 0; i < Hits.Length; i++) Hits[i].DispStrSet(i);
            }
        }
        [System.Serializable]
        public class Class_AEvent_ShotFire
        {
            public int Count;
            public Vector2 Speed;
            public Enum_PosBase PosBase;
            public Class_AEvent_ShotChange[] PosChange;
            public Enum_RotBase RotBase;
            public Class_AEvent_ShotChange[] RotChange;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_AEvent_ShotFire))]
            public class Class_AEvent_ShotFireDrawer : PropertyDrawer
            {
                bool dispImage = false;
                float times = 0;
                float timeWait = 100;
                float timeLenght = 0;
                float grav = 0;
                float damp = 0;

                Vector3 enemyPos = new Vector3(0f, 0f, 5f);
                int seed = 0;
                int shotLenght = 1;
                const float ImageSize = 200f;
                const float CharaSize = 6f;
                const float ShotSize = 2f;
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // 折りたたみの描画
                    property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

                    if (!property.isExpanded)
                    {
                        // 折りたたみ中は一行だけの高さを返す
                        return;
                    }

                    EditorGUI.indentLevel++;
                    // Rect
                    Rect baseRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
                    var propCount = property.FindPropertyRelative("Count");
                    EditorGUI.PropertyField(baseRect, propCount);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    var propSpeed = property.FindPropertyRelative("Speed");
                    EditorGUI.PropertyField(baseRect, propSpeed);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    var propPosBase = property.FindPropertyRelative("PosBase");
                    EditorGUI.PropertyField(baseRect, propPosBase);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    var propPosChange = property.FindPropertyRelative("PosChange");
                    EditorGUI.PropertyField(baseRect, propPosChange);
                    baseRect.y += EditorGUI.GetPropertyHeight(propPosChange, true);
                    var propRotBase = property.FindPropertyRelative("RotBase");
                    EditorGUI.PropertyField(baseRect, propRotBase);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    var propRotChange = property.FindPropertyRelative("RotChange");
                    EditorGUI.PropertyField(baseRect, propRotChange);
                    baseRect.y += EditorGUI.GetPropertyHeight(propRotChange, true);
                    dispImage = EditorGUI.Toggle(baseRect, "弾道シミュレーション", dispImage);
                    if (!dispImage)
                    {
                        EditorGUI.indentLevel--;
                        return;
                    }
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    times = EditorGUI.Slider(baseRect, "経過時間", times, 0, 10f);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    Rect timeWaitRect = new Rect(baseRect.x, baseRect.y, baseRect.width / 2f, EditorGUIUtility.singleLineHeight);
                    Rect timeLenghtRect = new Rect(baseRect.x + baseRect.width / 2f, baseRect.y, baseRect.width / 2f, EditorGUIUtility.singleLineHeight);
                    timeWait = EditorGUI.FloatField(timeWaitRect, "発射間隔", timeWait);
                    timeLenght = EditorGUI.FloatField(timeLenghtRect, "持続時間", timeLenght);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    Rect gravRect = new Rect(baseRect.x, baseRect.y, baseRect.width / 2f, EditorGUIUtility.singleLineHeight);
                    Rect dragRect = new Rect(baseRect.x + baseRect.width / 2f, baseRect.y, baseRect.width / 2f, EditorGUIUtility.singleLineHeight);
                    grav = EditorGUI.FloatField(gravRect, "重力%", grav);
                    damp = EditorGUI.FloatField(dragRect, "速度減衰", damp);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    shotLenght = EditorGUI.IntSlider(baseRect, "弾長", shotLenght, 1, 50);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    enemyPos = EditorGUI.Vector3Field(baseRect, "敵位置", enemyPos);
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    Rect seedButtonRect = new Rect(baseRect.x, baseRect.y, 200, EditorGUIUtility.singleLineHeight);
                    Rect SeedValRect = new Rect(baseRect.x + 200, baseRect.y, baseRect.xMax - 200, EditorGUIUtility.singleLineHeight);
                    seed = EditorGUI.IntField(SeedValRect, seed);
                    if (GUI.Button(seedButtonRect, "Seed変更"))
                    {
                        seed = new System.Random().Next();
                    }
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    for (int k = 0; k < 2; k++)
                    {
                        UnityEngine.Random.InitState(seed);
                        Rect labelRect = new Rect(baseRect.x, baseRect.y, ImageSize, EditorGUIUtility.singleLineHeight);
                        Rect imageRect = new Rect(baseRect.x, baseRect.y + EditorGUIUtility.singleLineHeight, ImageSize, ImageSize);
                        if (k == 1)
                        {
                            imageRect.x = baseRect.xMax - ImageSize;
                            labelRect.x = baseRect.xMax - ImageSize;
                        }
                        EditorGUI.LabelField(labelRect, k == 0 ? "真上(Z,X)" : "真横(Z,Y)");
                        EditorGUI.DrawRect(imageRect, new Color(0.1f, 0.1f, 0.1f, 0.3f));
                        var GSize = ImageSize / 20f;
                        for (int i = 0; i < 20; i++)
                        {
                            Rect xlineRect = new Rect(imageRect.x, imageRect.y + (GSize * i), imageRect.width, 1);
                            EditorGUI.DrawRect(xlineRect, Color.black);
                            Rect ylineRect = new Rect(imageRect.x + (GSize * i), imageRect.y, 1, imageRect.height);
                            EditorGUI.DrawRect(ylineRect, Color.black);
                        }
                        Rect xyZeroLineRect = new Rect(imageRect.x + imageRect.width / 2f, imageRect.y, 2, imageRect.height);
                        EditorGUI.DrawRect(xyZeroLineRect, k == 0 ? new Color(0.5f, 0.0f, 0.0f) : new Color(0.0f, 0.5f, 0.0f));
                        Rect zZeroLineRect = new Rect(imageRect.x, imageRect.y + imageRect.height / 2f, imageRect.width, 2);
                        EditorGUI.DrawRect(zZeroLineRect, new Color(0.0f, 0.0f, 0.5f));

                        var hcSize = CharaSize / 2f;
                        Rect charaRect = new Rect(imageRect.x + imageRect.width / 2f - hcSize, imageRect.y + imageRect.height / 2f - hcSize, CharaSize, CharaSize);
                        EditorGUI.DrawRect(charaRect, Color.yellow);
                        charaRect.x += enemyPos.z * GSize;
                        charaRect.y += (k == 0 ? enemyPos.x : enemyPos.y) * -GSize;

                        EditorGUI.DrawRect(charaRect, Color.red);
                        var count = Mathf.Min(propCount.intValue, 100);
                        var tmc = Mathf.RoundToInt(Mathf.Min(times, timeLenght) * 60f) / Mathf.Max(1, Mathf.RoundToInt(timeWait * 60)) + 1;
                        for (int t = 0; t < tmc; t++)
                        {
                            var timed = times - (t * Mathf.Max(1f / 60f, timeWait));
                            for (int i = 0; i < count; i++)
                            {
                                var pos = Vector3.zero;
                                switch ((Enum_PosBase)propPosBase.enumValueIndex)
                                {
                                    case Enum_PosBase.TargetPositon:
                                        pos = enemyPos;
                                        break;
                                }
                                for (int j = 0; j < propPosChange.arraySize; j++)
                                {
                                    var propPCh = propPosChange.GetArrayElementAtIndex(j);
                                    var vval = propPCh.FindPropertyRelative("VectVal").vector3Value;
                                    switch ((Enum_TransChangeM)propPCh.FindPropertyRelative("TChange").enumValueIndex)
                                    {
                                        case Enum_TransChangeM.OffSet: pos += vval; break;
                                        case Enum_TransChangeM.Random:
                                            pos.x += UnityEngine.Random.Range(-vval.x, vval.x);
                                            pos.y += UnityEngine.Random.Range(-vval.y, vval.y);
                                            pos.z += UnityEngine.Random.Range(-vval.z, vval.z);
                                            break;
                                        case Enum_TransChangeM.Change:
                                            pos += vval * (i - ((count - 1f) / 2f));
                                            break;
                                    }
                                }
                                var rot = Vector3.zero;
                                switch ((Enum_RotBase)(propRotBase.intValue))
                                {
                                    case Enum_RotBase.MyLook:
                                        if (enemyPos != Vector3.zero)
                                            rot = Quaternion.LookRotation(enemyPos).eulerAngles;
                                        break;
                                    case Enum_RotBase.ShotToTarget:
                                        if (enemyPos - pos != Vector3.zero)
                                            rot = Quaternion.LookRotation(enemyPos - pos).eulerAngles;
                                        break;
                                }
                                for (int j = 0; j < propRotChange.arraySize; j++)
                                {
                                    var propRCh = propRotChange.GetArrayElementAtIndex(j);
                                    var vval = propRCh.FindPropertyRelative("VectVal").vector3Value;
                                    switch ((Enum_TransChangeM)propRCh.FindPropertyRelative("TChange").enumValueIndex)
                                    {
                                        case Enum_TransChangeM.OffSet: rot += vval; break;
                                        case Enum_TransChangeM.Random:
                                            rot.x += UnityEngine.Random.Range(-vval.x, vval.x);
                                            rot.y += UnityEngine.Random.Range(-vval.y, vval.y);
                                            rot.z += UnityEngine.Random.Range(-vval.z, vval.z);
                                            break;
                                        case Enum_TransChangeM.Change:
                                            rot += vval * (i - ((count - 1f) / 2f));
                                            break;
                                    }
                                }

                                if (damp > 0f)
                                {
                                    var invDamp = 1f / damp;
                                    pos += (Physics.gravity * grav * invDamp * timed
                                        - Physics.gravity * grav * invDamp * invDamp * (1f - Mathf.Exp(-damp * timed)))
                                        * 0.01f;
                                }
                                else pos += Physics.gravity * grav * timed * timed * 0.5f * 0.01f;
                                var dampIntegral = timed;
                                if (damp > 0f) dampIntegral = (1f / damp) * (Mathf.Exp(-damp * 0) - Mathf.Exp(-damp * timed));
                                var speed = UnityEngine.Random.Range(propSpeed.vector2Value.x, propSpeed.vector2Value.y) / 10f;
                                for (int j = 0; j < shotLenght; j++)
                                {
                                    Rect shotRect = new Rect(imageRect.x + imageRect.width / 2f - (ShotSize / 2f), imageRect.y + imageRect.height / 2f - (ShotSize / 2f), ShotSize, ShotSize);
                                    shotRect.x += pos.z * GSize;
                                    shotRect.y += (k == 0 ? pos.x : pos.y) * -GSize;
                                    var dx = (speed * dampIntegral + j * 20) * Mathf.Cos(rot.y * Mathf.Deg2Rad);
                                    var dy = (speed * dampIntegral + j * 20) * Mathf.Sin((k == 0 ? -rot.y : rot.x) * Mathf.Deg2Rad);
                                    shotRect.x += dx / GSize;
                                    shotRect.y += dy / GSize;
                                    EditorGUI.DrawRect(shotRect, Color.magenta);
                                }
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    if (!property.isExpanded)
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }

                    float height = EditorGUIUtility.singleLineHeight * 6;
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("PosChange"), true);
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("RotChange"), true);

                    if (!dispImage) return height;
                    height += EditorGUIUtility.singleLineHeight * 6;
                    height += ImageSize;
                    return height;
                }

            }
#endif
        }

        [System.Serializable]
        public class Class_AEvent_ShotChange
        {
            public Enum_TransChangeM TChange;
            public Vector3 VectVal;
            [Tooltip("変化設定")] public Enum_TransChangeA AChange;
            [Tooltip("変化補正((変値+x)/y)")]public Vector2 AddVal;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_AEvent_ShotChange))]
            public class Class_AEvent_ShotChangeDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // ラベル表示（PrefixLabelで左端のラベル領域を確保）
                    position = EditorGUI.PrefixLabel(position, label);


                    // Rect
                    Rect baseRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(baseRect, property.FindPropertyRelative("TChange"));
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(baseRect, property.FindPropertyRelative("VectVal"));
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(baseRect, property.FindPropertyRelative("AChange"));
                    baseRect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(baseRect, property.FindPropertyRelative("AddVal"));
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight * 4;
                }
            }
#endif
        }

        [System.Serializable]
        public class Class_AEvent_ShotHit
        {
            [HideInInspector] public string DispStr;
            public Class_AEvent_ShotOption Option;
            public Class_AEvent_CalValue Dam;
            public Class_AEvent_ShotBufAdd[] BufAdds;
            public void DispStrSet(int index)
            {
                var dstr = "[" + index + "]";
                var tstr = "";
                if (Option.EHit) tstr += "敵";
                if (Option.FHit)
                {
                    if (tstr != "") tstr += ",";
                    tstr += "味";
                }
                if (Option.MHit)
                {
                    if (tstr != "") tstr += ",";
                    tstr += "自";
                }
                if (tstr == "") tstr = "無";
                dstr += "(" + tstr + ")";
                dstr += "<" + (!Option.Heal ? "ダメ" : "回復");
                dstr += "," + EnumToJp(Option.AtkType) + "," + EnumToJp(Option.RangeType);
                dstr += "," + (Option.EleRides ? "属性変化" : (EnumToJp(Option.Element) + "属性"));
                dstr += ">";
                dstr += Dam.CalStrGet().Replace("\n", "");
                if (BufAdds != null && BufAdds.Length > 0) dstr += ",状態×" + BufAdds.Length;
                DispStr = dstr;
                Dam.CalStr = Dam.CalStrGet();
                for (int i = 0; i < BufAdds.Length; i++) BufAdds[i].DispStrSet(i);
            }
        }
        [System.Serializable]
        public class Class_AEvent_ShotOption
        {
            public bool EHit;
            public bool FHit;
            public bool MHit;
            public bool DeathHit;
            public bool Heal;
            public Enum_AtkType AtkType;
            public Enum_RangeType RangeType;
            public bool NoHitRegist;
            public bool NoDamAdd;
            public bool NoBaseRegist;
            public Enum_Element Element;
            public bool EleRides;
            public bool NoEleRegist;
            public Vector2 ChangeCritPer;
            public Vector2 ChangeCritMult;
            public Vector2 ChangeHitPer;
            public Vector2 ChangeDogePer;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_AEvent_ShotOption))]
            public class Class_AEvent_ShotOptionDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // 折りたたみの描画
                    property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

                    if (!property.isExpanded)
                    {
                        // 折りたたみ中は一行だけの高さを返す
                        return;
                    }
                    EditorGUI.indentLevel++;
                    position = EditorGUI.IndentedRect(position);

                    float lineHeight = EditorGUIUtility.singleLineHeight;
                    float spacing = 4f;
                    float fieldWidth = (position.width - spacing * 2) / 3;

                    Rect r = new Rect(position.x, position.y, fieldWidth, lineHeight);
                    r.y += EditorGUIUtility.singleLineHeight;
                    var heal = property.FindPropertyRelative("Heal");
                    heal.boolValue = EditorGUI.ToggleLeft(r, "回復", heal.boolValue);
                    r.x += fieldWidth + spacing;
                    var eHit = property.FindPropertyRelative("EHit");
                    eHit.boolValue = EditorGUI.ToggleLeft(r, "敵命中", eHit.boolValue);
                    r.x += fieldWidth + spacing;
                    var fHit = property.FindPropertyRelative("FHit");
                    fHit.boolValue = EditorGUI.ToggleLeft(r, "味方命中", fHit.boolValue);
                    r.x = position.x;
                    r.y += EditorGUIUtility.singleLineHeight;
                    var atkType = property.FindPropertyRelative("AtkType");
                    EditorGUI.PropertyField(r, atkType, GUIContent.none);
                    r.x += fieldWidth + spacing;
                    var mHit = property.FindPropertyRelative("MHit");
                    mHit.boolValue = EditorGUI.ToggleLeft(r, "自身命中", mHit.boolValue);
                    r.x += fieldWidth + spacing;
                    var deathHit = property.FindPropertyRelative("DeathHit");
                    deathHit.boolValue = EditorGUI.ToggleLeft(r, "戦闘不能命中", deathHit.boolValue);

                    r.y += EditorGUIUtility.singleLineHeight;
                    r.x = position.x;
                    var rangeType = property.FindPropertyRelative("RangeType");
                    EditorGUI.PropertyField(r, rangeType, GUIContent.none);
                    r.x += fieldWidth + spacing;
                    var noDamAdd = property.FindPropertyRelative("NoDamAdd");
                    noDamAdd.boolValue = EditorGUI.ToggleLeft(r, "ダメ増加無効", noDamAdd.boolValue);
                    r.x += fieldWidth + spacing;
                    var noBaseRegist = property.FindPropertyRelative("NoBaseRegist");
                    noBaseRegist.boolValue = EditorGUI.ToggleLeft(r, "通常軽減無効", noBaseRegist.boolValue);
                    r.y += EditorGUIUtility.singleLineHeight;
                    r.x = position.x;
                    var element = property.FindPropertyRelative("Element");
                    EditorGUI.PropertyField(r, element, GUIContent.none);
                    r.x += fieldWidth + spacing;
                    var eleRides = property.FindPropertyRelative("EleRides");
                    eleRides.boolValue = EditorGUI.ToggleLeft(r, "属性上書き", eleRides.boolValue);
                    r.x += fieldWidth + spacing;
                    var noEleRegist = property.FindPropertyRelative("NoEleRegist");
                    noEleRegist.boolValue = EditorGUI.ToggleLeft(r, "属性軽減無効", noEleRegist.boolValue);
                    r.y += EditorGUIUtility.singleLineHeight;
                    r.x = position.x;
                    r.width = position.width;
                    var noHitRegist = property.FindPropertyRelative("NoHitRegist");
                    noHitRegist.boolValue = EditorGUI.ToggleLeft(r, "部位軽減無効", noHitRegist.boolValue);
                    r.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(r, "各補正:x=加算,y=乗算+%");
                    r.y += EditorGUIUtility.singleLineHeight;
                    var changeCritPer = property.FindPropertyRelative("ChangeCritPer");
                    changeCritPer.vector2Value = EditorGUI.Vector2Field(r, "会心確率", changeCritPer.vector2Value);
                    r.y += EditorGUIUtility.singleLineHeight;
                    var changeCritMult = property.FindPropertyRelative("ChangeCritMult");
                    changeCritMult.vector2Value = EditorGUI.Vector2Field(r, "会心倍率", changeCritMult.vector2Value);
                    r.y += EditorGUIUtility.singleLineHeight;
                    var changeHitPer = property.FindPropertyRelative("ChangeHitPer");
                    changeHitPer.vector2Value = EditorGUI.Vector2Field(r, "命中確率", changeHitPer.vector2Value);
                    r.y += EditorGUIUtility.singleLineHeight;
                    var changeDogePer = property.FindPropertyRelative("ChangeDogePer");
                    changeDogePer.vector2Value = EditorGUI.Vector2Field(r, "回避確率", changeDogePer.vector2Value);
                    r.y += EditorGUIUtility.singleLineHeight;

                    EditorGUI.indentLevel--;
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    if (!property.isExpanded)
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }
                    return EditorGUIUtility.singleLineHeight * 10;
                }
            }
#endif
        }
        [System.Serializable]
        public class Class_AEvent_ShotBufAdd
        {
            [HideInInspector] public string DispStr;
            public Enum_Buf ID;
            public Enum_BufOp Op;
            public short Index;
            public Enum_BufSet Set;
            public Vector2 Times;
            public Class_ValCalc[] CPow;
            public Class_ValCalc[] MPow;
            public void DispStrSet(int lindex)
            {
                var dstr = "[" + lindex + "]";
                dstr += ID.ToString() + "_" + Op.ToString() + "(" + Index + ")" + Set.ToString();
                dstr += ",Time:" + Times;
                if (CPow != null && CPow.Length > 0) dstr += ",CPow:" + CalSStr(CPow);
                if (MPow != null && MPow.Length > 0) dstr += ",MPow:" + CalSStr(MPow);
                DispStr = dstr;
            }
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_AEvent_ShotBufAdd))]
            public class Class_AEvent_ShotBufAddDrawer : PropertyDrawer
            {
                float spacing = 5f;
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // 折りたたみの描画
                    property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

                    if (!property.isExpanded)
                    {
                        // 折りたたみ中は一行だけの高さを返す
                        return;
                    }

                    EditorGUI.indentLevel++;

                    float lineHeight = EditorGUIUtility.singleLineHeight;

                    float y = position.y + lineHeight;
                    // 1行目: ID, Index, Set 横並び
                    float idWidth = 130f;
                    float opWidth = 120f;
                    float indexLabelWidth = 50f; // 「Index」ラベル幅
                                                 // Indexフィールドは残り幅を全部使う
                    float setWidth = 100f;

                    Rect idRect = new Rect(position.x, y, idWidth, EditorGUIUtility.singleLineHeight);
                    Rect opRect = new Rect(idRect.xMax, y, opWidth, EditorGUIUtility.singleLineHeight);

                    Rect indexLabelRect = new Rect(opRect.xMax, y, indexLabelWidth, EditorGUIUtility.singleLineHeight);

                    // indexRectは「Index」ラベルの右端からsetRectの開始位置までの幅を使う
                    float indexX = indexLabelRect.xMax;
                    float indexWidth = position.xMax - indexX - setWidth;

                    Rect indexRect = new Rect(indexX, y, indexWidth, EditorGUIUtility.singleLineHeight);

                    Rect setRect = new Rect(indexRect.xMax, y, setWidth, EditorGUIUtility.singleLineHeight);

                    var idProp = property.FindPropertyRelative("ID");
                    var opProp = property.FindPropertyRelative("Op");
                    var indexProp = property.FindPropertyRelative("Index");
                    var setProp = property.FindPropertyRelative("Set");

                    EditorGUI.PropertyField(idRect, idProp, GUIContent.none);
                    EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
                    EditorGUI.LabelField(indexLabelRect, "Index");

                    EditorGUI.PropertyField(indexRect, indexProp, GUIContent.none);

                    EditorGUI.PropertyField(setRect, setProp, GUIContent.none);

                    // 2行目以降：Times
                    var timesProp = property.FindPropertyRelative("Times");
                    y += lineHeight + spacing;
                    Rect timesRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(timesRect, timesProp);

                    // 3行目以降：CPow 配列（折りたたみ可能）
                    var cPowProp = property.FindPropertyRelative("CPow");
                    y += lineHeight + spacing;
                    float cPowHeight = EditorGUI.GetPropertyHeight(cPowProp, true);
                    Rect cPowRect = new Rect(position.x, y, position.width, cPowHeight);
                    EditorGUI.PropertyField(cPowRect, cPowProp, true);

                    y += cPowHeight + spacing;

                    // 4行目以降：MPow 配列（折りたたみ可能）
                    var mPowProp = property.FindPropertyRelative("MPow");
                    float mPowHeight = EditorGUI.GetPropertyHeight(mPowProp, true);
                    Rect mPowRect = new Rect(position.x, y, position.width, mPowHeight);
                    EditorGUI.PropertyField(mPowRect, mPowProp, true);

                    EditorGUI.indentLevel--;
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    if (!property.isExpanded)
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }

                    float height = EditorGUIUtility.singleLineHeight; // Foldout 行

                    height += EditorGUIUtility.singleLineHeight + spacing; // ID,Index,Set 横並び行

                    height += EditorGUIUtility.singleLineHeight + spacing; // Times

                    var cPowProp = property.FindPropertyRelative("CPow");
                    height += EditorGUI.GetPropertyHeight(cPowProp, true) + spacing;

                    var mPowProp = property.FindPropertyRelative("MPow");
                    height += EditorGUI.GetPropertyHeight(mPowProp, true) + spacing;

                    return height;
                }
            }
#endif
        }

        #endregion
        #region AEvent_Other
        public class Class_AEvent_MyMove : Class_AEvent_Base
        {
            public Enum_RotBase RotBase;
            public Vector3 RotOffSet;
            public Vector3 Vect;
            public float RotPer;
            new public static Enum_AEventType EnumName => Enum_AEventType.自己移動;
            public override Enum_AEventType TypeName => Enum_AEventType.自己移動;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "Vect:" + Vect;
                DispName = dstr;
            }
        }
        public class Class_AEvent_MyLook : Class_AEvent_Base
        {
            public Enum_RotBase RotBase;
            public Vector3 RotOffSet;
            public float RotPer;
            new public static Enum_AEventType EnumName => Enum_AEventType.自己回転;
            public override Enum_AEventType TypeName => Enum_AEventType.自己回転;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "Rot:" + RotOffSet;
                DispName = dstr;
            }
        }
        public class Class_AEvent_MyValState : Class_AEvent_Base
        {
            public Enum_ValState State;
            public Class_ValCalc[] Val;
            new public static Enum_AEventType EnumName => Enum_AEventType.自己数値変化;
            public override Enum_AEventType TypeName => Enum_AEventType.自己数値変化;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "State:" + State;
                dstr += "+" + CalSStr(Val);
                DispName = dstr;
            }
        }
        public class Class_AEvent_MyBuf : Class_AEvent_Base
        {
            public Enum_Buf ID;
            public Enum_BufOp Op;
            public short Index;
            public Enum_BufSet Set;
            public Vector2 AddTimes;
            public Class_ValCalc[] CPow;
            public Class_ValCalc[] MPow;

            new public static Enum_AEventType EnumName => Enum_AEventType.自己バフ;
            public override Enum_AEventType TypeName => Enum_AEventType.自己バフ;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += ID.ToString() + "(" + Index + ")" + Set.ToString();
                dstr += ",Time:" + AddTimes.x;
                if (AddTimes.y > 0) dstr += "m" + AddTimes.y;
                if (CPow != null && CPow.Length > 0)
                {
                    dstr += ",Pow:" + CalSStr(CPow);
                    if (MPow != null && MPow.Length > 0) dstr += "m" + CalSStr(MPow);
                }
                DispName = dstr;
            }
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_AEvent_MyBuf))]
            public class Class_AEvent_MyBufDrawer : PropertyDrawer
            {
                float spacing = 5f;
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // 折りたたみの描画
                    property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

                    if (!property.isExpanded)
                    {
                        // 折りたたみ中は一行だけの高さを返す
                        return;
                    }

                    EditorGUI.indentLevel++;

                    float lineHeight = EditorGUIUtility.singleLineHeight;
                    float y = position.y + lineHeight;
                    // Memo
                    var MemoProp = property.FindPropertyRelative("Memo");
                    Rect MemoRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(MemoRect, MemoProp);
                    y += lineHeight + spacing;
                    var IfBProp = property.FindPropertyRelative("IfBID");
                    Rect IfBRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(IfBRect, IfBProp);
                    y += lineHeight + spacing;
                    var IfBsProp = property.FindPropertyRelative("IfBIDs");
                    Rect IfBsRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(IfBsRect, IfBsProp);
                    y += EditorGUI.GetPropertyHeight(IfBsProp, true) + spacing;
                    var TimesProp = property.FindPropertyRelative("Times");
                    Rect TimesRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(TimesRect, TimesProp);
                    // 1行目: ID, Index, Set 横並び
                    float idWidth = 130f;
                    float indexLabelWidth = 50f; // 「Index」ラベル幅
                                                 // Indexフィールドは残り幅を全部使う
                    float setWidth = 100f;

                    y += lineHeight + spacing;

                    Rect idRect = new Rect(position.x, y, idWidth, EditorGUIUtility.singleLineHeight);
                    Rect opRect = new Rect(idRect.xMax, y, idWidth, EditorGUIUtility.singleLineHeight);

                    Rect indexLabelRect = new Rect(opRect.xMax, y, indexLabelWidth, EditorGUIUtility.singleLineHeight);

                    // indexRectは「Index」ラベルの右端からsetRectの開始位置までの幅を使う
                    float indexX = indexLabelRect.xMax;
                    float indexWidth = position.xMax - indexX - setWidth;

                    Rect indexRect = new Rect(indexX, y, indexWidth, EditorGUIUtility.singleLineHeight);

                    Rect setRect = new Rect(indexRect.xMax, y, setWidth, EditorGUIUtility.singleLineHeight);

                    var idProp = property.FindPropertyRelative("ID");
                    var opProp = property.FindPropertyRelative("Op");
                    var indexProp = property.FindPropertyRelative("Index");
                    var setProp = property.FindPropertyRelative("Set");

                    EditorGUI.PropertyField(idRect, idProp, GUIContent.none);
                    EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
                    EditorGUI.LabelField(indexLabelRect, "Index");

                    EditorGUI.PropertyField(indexRect, indexProp, GUIContent.none);

                    EditorGUI.PropertyField(setRect, setProp, GUIContent.none);

                    // 2行目以降：Times
                    var atimesProp = property.FindPropertyRelative("AddTimes");
                    y += lineHeight + spacing;
                    Rect atimesRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(atimesRect, atimesProp);

                    // 3行目以降：CPow 配列（折りたたみ可能）
                    var cPowProp = property.FindPropertyRelative("CPow");
                    y += lineHeight + spacing;
                    float cPowHeight = EditorGUI.GetPropertyHeight(cPowProp, true);
                    Rect cPowRect = new Rect(position.x, y, position.width, cPowHeight);
                    EditorGUI.PropertyField(cPowRect, cPowProp, true);

                    y += cPowHeight + spacing;

                    // 4行目以降：MPow 配列（折りたたみ可能）
                    var mPowProp = property.FindPropertyRelative("MPow");
                    float mPowHeight = EditorGUI.GetPropertyHeight(mPowProp, true);
                    Rect mPowRect = new Rect(position.x, y, position.width, mPowHeight);
                    EditorGUI.PropertyField(mPowRect, mPowProp, true);

                    EditorGUI.indentLevel--;
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    if (!property.isExpanded)
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }

                    float height = EditorGUIUtility.singleLineHeight; // Foldout 行

                    height += (EditorGUIUtility.singleLineHeight + spacing) * 3;

                    var IfBsProp = property.FindPropertyRelative("IfBIDs");
                    height += EditorGUI.GetPropertyHeight(IfBsProp, true) + spacing;
                    height += EditorGUIUtility.singleLineHeight + spacing; // ID,Index,Set 横並び行


                    height += EditorGUIUtility.singleLineHeight + spacing; // Times

                    var cPowProp = property.FindPropertyRelative("CPow");
                    height += EditorGUI.GetPropertyHeight(cPowProp, true) + spacing;

                    var mPowProp = property.FindPropertyRelative("MPow");
                    height += EditorGUI.GetPropertyHeight(mPowProp, true) + spacing;

                    return height;
                }
            }
#endif
        }
        public class Class_AEvent_Summon : Class_AEvent_Base
        {
            public GameObject State;
            public Class_AEvent_ShotFire Fire;
            public bool NoTeam;
            public float HPPer;
            public float AtkPer;
            public float DefPer;
            public float TimeLimit;
            new public static Enum_AEventType EnumName => Enum_AEventType.召喚;
            public override Enum_AEventType TypeName => Enum_AEventType.召喚;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "HP割合:" + HPPer;
                dstr += "攻撃力割合:" + AtkPer;
                dstr += "防御力割合:" + DefPer;
                dstr += "時間制限:" + TimeLimit + "秒";
                DispName = dstr;
            }
        }
        public class Class_AEvent_Anim : Class_AEvent_Base
        {
            public int ComboID;
            public AnimationClip AnimClip;
            public float AnimSpeed;
            new public static Enum_AEventType EnumName => Enum_AEventType.アニメーション;
            public override Enum_AEventType TypeName => Enum_AEventType.アニメーション;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "AnimClip:" + (AnimClip != null ? AnimClip.name : "null");
                dstr += "ComboID:" + ComboID;
                dstr += ",AnimSpeed:" + AnimSpeed.ToString("F1") + "%";
                DispName = dstr;
            }
        }
        public class Class_AEvent_SE : Class_AEvent_Base
        {
            public AudioSource SEFile;
            public float VolumePer;
            public float PitchPer;
            new public static Enum_AEventType EnumName => Enum_AEventType.SE;
            public override Enum_AEventType TypeName => Enum_AEventType.SE;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "SEFile:" + (SEFile != null ? SEFile.name : "無");
                dstr += ",Volume:" + VolumePer + "%";
                dstr += ",Pitch:" + PitchPer + "%";
                DispName = dstr;
            }
        }
        public class Class_AEvent_EX : Class_AEvent_Base
        {
            public Enum_AEventEXs EXs;
            new public static Enum_AEventType EnumName => Enum_AEventType.EX;
            public override Enum_AEventType TypeName => Enum_AEventType.EX;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "EX:" + EXs;
                DispName = dstr;
            }
        }

        [System.Serializable]
        public class Class_BufAdd
        {
            public Enum_Buf ID;
            public short Index;
            public Enum_BufOp Op;
            public Enum_BufSet Set;
            public Vector2 Times;
            public Vector2Int Pows;
        }



        #endregion
    }
}

