
using UnityEditor;
using UnityEngine;
using static Datas.Data_Attack;
using static GmSystem.GS_GlobalState;

namespace Datas
{
    public partial class Data_EnemyBattle
    {
        public enum Enum_EBattleIfs
        {
            [InspectorName("ターゲット距離_x")]TargetDis_x,
            [InspectorName("ターゲット水平距離_x")] TargetVirtDis_x,
            [InspectorName("ターゲット垂直距離_x")] TargetHoriDis_x,
            [InspectorName("HP割合_x")] HPPer_x = 100,
            [InspectorName("HP数値_x")] HPC_x,
            [InspectorName("MP割合_x")] MPPer_x,
            [InspectorName("MP数値_x")] MPC_x,
            [InspectorName("ST割合_x")] STPer_x,
            [InspectorName("ST数値_x")] STC_x,
            [InspectorName("EX数値_x")] EX_x,
        }
        public enum Enum_EBattle_Rot
        {
            [InspectorName("ワールド")] World = -1,
            [InspectorName("自身向き")] MyLook,
            [InspectorName("ターゲット方向")] TargetLook,
            [InspectorName("最終攻撃者方向")] LastAttackLook,
            [InspectorName("最終命中者方向")] LastHitLook,
        }
        public enum Enum_EBEventType
        {
            ベース = 0,
            分岐 = 1,
            移動 = 2,
            回転 = 4,
            攻撃 = 3,
            自己数値変化 = 5,
            自己バフ = 6,
        }
        [System.Serializable]
        public class Class_Data_EBEvent_If
        {
            public Enum_EBattleIfs Ifs;
            public bool Target;
            public float Val;
            public bool Down;
        }
        [System.Serializable]
        public class Class_Data_EBEvent_BNexts
        {
            [Tooltip("重み")]
            public float P;
            public int ID;
            public float Time;
        }
        [System.Serializable]
        abstract public class Class_Data_EBEvent_Base
        {
            [HideInInspector] public string DispName;
            public string Memo;
            public int ActID;
            public int[] ActIDs;
            public Vector3 Times;
            public Class_Data_EBEvent_If[] Ifs;
            public static Enum_EBEventType EnumName => Enum_EBEventType.ベース;
            public virtual Enum_EBEventType TypeName => Enum_EBEventType.ベース;
            public virtual void DispSet(int index)
            {
                DispName = DStrBase(index);
            }
            protected string DStrBase(int index)
            {
                var dstr = "[" + index + "]" + TypeName + "|";
                if (Memo != "") dstr += "メモ:" + Memo + "|";
                dstr += "ActID:" + ActID;
                if (ActIDs != null) for (int i = 0; i < ActIDs.Length; i++) dstr += "/" + ActIDs[i];
                dstr += ",時間:(" + Times.x;
                if (Times.y > Times.x) dstr += "～" + Times.y;
                if (Times.z > 0) dstr += "v" + Times.z;
                dstr += ")秒|";
                if (Ifs.Length > 0) dstr += "条件数" + Ifs.Length + "|";
                return dstr;
            }
        }

        [System.Serializable]
        public class Class_Data_EBEvent_Branch : Class_Data_EBEvent_Base
        {
            public Class_Data_EBEvent_BNexts[] Nexts;
            new public static Enum_EBEventType EnumName => Enum_EBEventType.分岐;
            public override Enum_EBEventType TypeName => Enum_EBEventType.分岐;
        }
        [System.Serializable]
        public class Class_Data_EBEvent_Move : Class_Data_EBEvent_Base
        {
            public Enum_EBattle_Rot BaseRot;
            public Vector3 AddVect;
            public bool VeUsed;
            public float SpeedPer;
            public float LookPer;
            public bool SetVect;
            new public static Enum_EBEventType EnumName => Enum_EBEventType.移動;
            public override Enum_EBEventType TypeName => Enum_EBEventType.移動;
        }
        [System.Serializable]
        public class Class_Data_EBEvent_Rot : Class_Data_EBEvent_Base
        {
            public Enum_EBattle_Rot BaseRot;
            public Vector3 AddRot;
            public float LookPer;
            new public static Enum_EBEventType EnumName => Enum_EBEventType.回転;
            public override Enum_EBEventType TypeName => Enum_EBEventType.回転;
        }
        [System.Serializable]
        public class Class_Data_EBEvent_Attack : Class_Data_EBEvent_Base
        {
            public Data_Attack AttackD;
            public int ID;
            public float ASpdAdd;
            public bool Stay;
            public bool NoUse;
            public Vector3 PosOffSet;
            public Vector3 RotOffSet;
            new public static Enum_EBEventType EnumName => Enum_EBEventType.攻撃;
            public override Enum_EBEventType TypeName => Enum_EBEventType.攻撃;
        }
        public class Class_Data_EBEvent_MyValState : Class_Data_EBEvent_Base
        {
            public Enum_ValState State;
            public Class_ValCalc[] Val;
            new public static Enum_EBEventType EnumName => Enum_EBEventType.自己数値変化;
            public override Enum_EBEventType TypeName => Enum_EBEventType.自己数値変化;
            public override void DispSet(int index)
            {
                var dstr = DStrBase(index);
                dstr += "State:" + State;
                dstr += "+" + CalSStr(Val);
                DispName = dstr;
            }
        }
        public class Class_Data_EBEvent_MyBuf : Class_Data_EBEvent_Base
        {
            public Enum_Buf ID;
            public Enum_BufOp Op;
            public short Index;
            public Enum_BufSet Set;
            public Vector2 AddTimes;
            public Class_ValCalc[] CPow;
            public Class_ValCalc[] MPow;

            new public static Enum_EBEventType EnumName => Enum_EBEventType.自己バフ;
            public override Enum_EBEventType TypeName => Enum_EBEventType.自己バフ;
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
            [CustomPropertyDrawer(typeof(Class_Data_EBEvent_MyBuf))]
            public class Class_Data_EBEvent_MyBufDrawer : PropertyDrawer
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
    }

}
