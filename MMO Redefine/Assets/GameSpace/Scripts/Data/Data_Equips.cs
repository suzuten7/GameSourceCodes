namespace Datas
{

    using UnityEngine;
    using static GmSystem.GS_GlobalState;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using static Data_Get;
    using static GmSystem.GS_SaveValues;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    public class Data_Equips
    {
        [System.Serializable]
        public class Class_State_EquipmentValues
        {
            public int GID;
            public string GetDateStr;
            public int LV;
            public float Exp;
            public List<Class_EquipmentAddOp> AddOps;
            public DateTime GetDateTime
            {
                get
                { 
                return DateTime.TryParseExact(
                    GetDateStr,
                    "yyyy/MM/dd-HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt) ?
                    dt : DateTime.MinValue;
                }
            }
            public float NextExp
            {
                get
                {
                    var Cost = 0f;
                    var ItemD = ItemGIDDataGet(GID);
                    if (ItemD == null) return Cost;
                    switch (ItemD)
                    {
                        case Data_Wepon:
                            Cost = ((Data_Wepon)ItemD).UpgradeCost;
                            break;
                        case Data_Equipment:
                            Cost = ((Data_Equipment)ItemD).UpgradeCost;
                            break;
                    }
                    Cost *= LV;
                    return Cost;
                }
            }
            public Class_State_EquipmentValues()
            {
                GID = -1;
                GetDateStr = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");
                LV = 1;
                Exp = 0;
                AddOps = new List<Class_EquipmentAddOp>();
            }
        }
        [System.Serializable]
        public class Class_EquipmentAddOp
        {
            public Enum_StateAddsType State;
            public Enum_StateAddsOption Option;
            public int LV;
            public bool Lock;
        }
        [System.Serializable]
        public class Class_EquipmentIf
        {
            public Enum_EquipIf If;
            public Vector2 Val;
            #if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_EquipmentIf))]
            public class Class_EquipmentIfDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // ラベル表示（PrefixLabelで左端のラベル領域を確保）
                    position = EditorGUI.PrefixLabel(position, label);

                    Rect ifRect = new Rect(position.x, position.y, position.width / 2f, EditorGUIUtility.singleLineHeight);
                    var ifProp = property.FindPropertyRelative("If");
                    Rect valRect = new Rect(position.x + position.width / 2f, position.y, position.width / 2f, EditorGUIUtility.singleLineHeight);
                    var valProp = property.FindPropertyRelative("Val");
                    EditorGUI.PropertyField(ifRect, ifProp, GUIContent.none);
                    EditorGUI.PropertyField(valRect, valProp, GUIContent.none);
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight * 1;
                }
            }
            #endif
        }
        [System.Serializable]
        public class Class_EquipmentAdds
        {
            public Class_EquipmentIf[] EquipIfs;
            public Enum_StateAddsType State;
            public Enum_StateAddsOption Option;
            [Tooltip("x=基本,y=Lv増加量")]public Vector2 Values;

            #if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Datas.Data_Equips.Class_EquipmentAdds))]
            public class Class_EquipmentAddsDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // ラベル表示（PrefixLabelで左端のラベル領域を確保）
                    position = EditorGUI.PrefixLabel(position, label);

                    Rect ifsRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    var ifsProp = property.FindPropertyRelative("EquipIfs");
                    EditorGUI.PropertyField(ifsRect, ifsProp, GUIContent.none);
                    EditorGUI.LabelField(ifsRect, "条件");
                    position.y += EditorGUI.GetPropertyHeight(ifsProp, true);

                    Rect stateRect = new Rect(position.x, position.y, position.width / 2f, EditorGUIUtility.singleLineHeight);
                    var staProp = property.FindPropertyRelative("State");
                    EditorGUI.PropertyField(stateRect, staProp, GUIContent.none);
                    Rect opRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
                    var opProp = property.FindPropertyRelative("Option");
                    EditorGUI.PropertyField(opRect, opProp, GUIContent.none);

                    position.y += EditorGUIUtility.singleLineHeight;

                    Rect valRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    var valProp = property.FindPropertyRelative("Values");
                    EditorGUI.PropertyField(valRect, valProp, GUIContent.none);

                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    var height = EditorGUIUtility.singleLineHeight * 2;
                    var equipIfsProp = property.FindPropertyRelative("EquipIfs");
                    height += EditorGUI.GetPropertyHeight(equipIfsProp, true);
                    return height;
                }
            }
            #endif
        }
        [System.Serializable]
        public class Class_EquipmentTrigger
        {
            public Enum_EquipTrigger Trigger;
            public Class_EquipmentIf[] EquipIfs;
            public bool TriggerStaTransUse;
            public Data_Attack Attack;
        }

        public enum Enum_EquipIf
        {
            [InspectorName("HP割合_x以上")]HPPer_xUp,
            [InspectorName("HP割合_x以下")] HPPer_xDown,
            [InspectorName("MP割合_x以上")] MPPer_x_Up,
            [InspectorName("MP割合_x以下")] MPPer_x_Down,
            [InspectorName("ST割合_x以上")] STPer_xUp,
            [InspectorName("ST割合_x以下")] STPer_xDown,
            [InspectorName("EX_x以上")] EX_xUp,
            [InspectorName("EX_x以下")] EX_xDown,

            [InspectorName("両素手")] WHand = 100,
            [InspectorName("片素手")] SHand,
            [InspectorName("盾持ち")] Shild,
            [InspectorName("双武器")] WWepon,

        }
        public enum Enum_EquipTrigger
        {
            [InspectorName("無条件")] Non,

            [InspectorName("与ダメージ時")] AddDamage = 100,
            [InspectorName("受ダメージ時")] TakeDamage,
            [InspectorName("与回復時")] AddHeal,
            [InspectorName("受回復時")] TakeHeal,
            [InspectorName("死亡時")] Death,

            [InspectorName("通常使用")] NormalAtk = 200,
            [InspectorName("重撃使用")] HevAtk,
            [InspectorName("スキル使用")] Skill,
            [InspectorName("消耗品使用")] Consum,
        }

        [System.Serializable]
        public class Class_UpgradeBoost
        {
            public Data_Item ItemD;
            public float ExpAddPer;
        }

    }
}

