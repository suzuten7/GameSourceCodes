namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Data_Get;
    using static Data_Items;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(menuName = "DataCre/Data_ConsumableList")]
    public class Data_Consumable_DBL : ScriptableObject
    {
        public List<DBL_Consumable> DBL;

        [System.Serializable]
        public class DBL_Consumable
        {
            public Enum_ConsumableType Type;
            public List<Data_Consumables> Datas;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(DBL_Consumable))]
            class DBL_Consumable_Drawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    Rect Rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(Rect, property.FindPropertyRelative("Type"), GUIContent.none);
                    Rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(Rect, property.FindPropertyRelative("Datas"), GUIContent.none);
                }
                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Datas"), true);
                }
            }

#endif
        }
#if UNITY_EDITOR
        public void SaveDatabase()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        static Dictionary<Data_Consumables, int> DataToGIDDic = new();
        static Dictionary<int, Data_Consumables> GIDToDataDic = new();
        public int DataGetGID(Data_Consumables ItemD)
        {
            if (DataToGIDDic.TryGetValue(ItemD, out var oID)) return oID;
            for (int i = 0; i < DBL.Count; i++)
            {
                var index = DBL[i].Datas.IndexOf(ItemD);
                if (index >= 0)
                {
                    var id = GIDMake(Enum_ItemID.Consumables, (int)DBL[i].Type, index);
                    DataToGIDDic.Add(ItemD, id);
                    GIDToDataDic.Add(id, ItemD);
                    return id;
                }
            }
            return -1;
        }
        public Data_Consumables GIDGetData(int GID)
        {
            if (GID < (int)Enum_ItemID.Consumables || GID >= (int)Enum_ItemID.Consumables + (int)Enum_ItemID.Category) return null;
            if (GIDToDataDic.TryGetValue(GID, out var oData)) return oData;
            var index = GIDBrake(GID, out var itemCategory, out var itemType);
            var dbl = DBL.Find(x => x.Type == (Enum_ConsumableType)itemType);
            var data = dbl != null ? dbl.Datas[index] : null;
            DataToGIDDic.Add(data, GID);
            GIDToDataDic.Add(GID, data);
            return data;
        }

    }
}
