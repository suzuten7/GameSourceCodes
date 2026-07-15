namespace Datas
{
    using UnityEngine;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static FNet.Fusion_NetValue;
    using System.Collections.Generic;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(menuName ="DataCre/DropTable")]
    public class Data_DropTable : ScriptableObject
    {
        public Class_DropTable[] Drops;
        [System.Serializable]
        public class Class_DropTable
        {
            public string TableName;
            public float DropChancePer;
            [Tooltip("Drops内抽選数,0以下で全出現")]public int DropsSelects;
            public Class_DropOption[] Drops;
        }
        [System.Serializable]
        public class Class_DropOption
        {
            public Data_Item DropItems;
            public Vector2Int DropCounts;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_DropOption))]
            public class Class_DropOptionDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    // ラベル表示（PrefixLabelで左端のラベル領域を確保）
                    position = EditorGUI.PrefixLabel(position, label);

                    Rect ditemRect = new (position.x, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
                    Rect dcountRect = new (position.x + position.width / 2, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);

                    var ditemProp = property.FindPropertyRelative("DropItems");
                    var dcountProp = property.FindPropertyRelative("DropCounts");

                    EditorGUI.PropertyField(ditemRect, ditemProp, GUIContent.none);
                    EditorGUI.PropertyField(dcountRect, dcountProp, GUIContent.none);
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight * 1;
                }
            }
#endif
        }

        public List<(int, int)> DropGet(int Team)
        {
            var DropList = new List<(int, int)>();

            foreach (var DTb in Drops)
            {
                var f_dco = DTb.DropChancePer / 100f * DificChange(Team, Enum_DifcVal.EXP);
                var dcount = Mathf.FloorToInt(f_dco);
                if (Random.value < f_dco % 1f) dcount++;
                for (int i = 0; i < dcount; i++)
                {
                    if (DTb.DropsSelects <= 0 || DTb.DropsSelects >= DTb.Drops.Length)
                    {
                        for (int k = 0; k < DTb.Drops.Length; k++)
                        {
                            DropList.Add(Dr(DTb.Drops[k]));
                        }
                    }
                    else
                    {
                        var table = DTb.Drops.ToList();
                        for(int k = 0; k < DTb.DropsSelects; k++)
                        {
                            var index = Random.Range(0, table.Count);
                            DropList.Add(Dr(table[index]));
                            table.RemoveAt(index);
                        }
                    }

                }

            }
            return DropList;
        }
        (int,int) Dr(Class_DropOption DropOp)
        {
            var itemGID = ItemDataFindGID(DropOp.DropItems);
            switch (ItemGIDCategoryGet(itemGID))
            {
                default:
                    return (itemGID, Random.Range(DropOp.DropCounts.x, DropOp.DropCounts.y));
                case Enum_ItemID.Wepon:
                case Enum_ItemID.Armor:
                case Enum_ItemID.Akuse:
                    return (itemGID, Random.Range(DropOp.DropCounts.x, DropOp.DropCounts.y));
            }
        }
    }
}

