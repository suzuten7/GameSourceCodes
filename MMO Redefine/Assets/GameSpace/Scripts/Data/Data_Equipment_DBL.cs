namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Data_Get;
    using static Data_Items;
    using static Data_Equips;
#if UNITY_EDITOR
    using UnityEditor;
    using Unity.Android.Gradle.Manifest;
#endif

    [CreateAssetMenu(menuName = "DataCre/Data_EquipList")]
    public class Data_Equipment_DBL : ScriptableObject
    {
        public List<DBL_Equipment> DBL;
        [System.Serializable]
        public class DBL_Equipment
        {
            public Enum_EquipType Type;
            public List<Data_Equipment> Datas;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(DBL_Equipment))]
            class DBL_Equipment_Drawer : PropertyDrawer
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
        static Dictionary<Data_Equipment, int> DataToGIDDic = new();
        static Dictionary<int, Data_Equipment> GIDToDataDic = new();
        public int DataGetGID(Data_Equipment EquipD)
        {
            if (DataToGIDDic.TryGetValue(EquipD, out var oid)) return oid;
            for (int i = 0; i < DBL.Count; i++)
            {
                var index = DBL[i].Datas.IndexOf(EquipD);
                if (index >= 0)
                {
                    var ID = GIDMake(Enum_ItemID.Armor, (int)DBL[i].Type, index);
                    DataToGIDDic.Add(EquipD, ID);
                    GIDToDataDic.Add(ID, EquipD);
                    return ID;
                }
            }
            return -1;
        }
        public Data_Equipment GIDGetData(int GID)
        {
            if (GID < (int)Enum_ItemID.Armor || GID >= (int)Enum_ItemID.Akuse + (int)Enum_ItemID.Category) return null;
            if(GIDToDataDic.TryGetValue(GID,out var oData))return oData;
            var index = GIDBrake(GID, out var itemCategory, out var itemType);
            var dbl = DBL.Find(x => x.Type == (Enum_EquipType)itemType);
            var data = dbl != null ? dbl.Datas[index] : null;
            GIDToDataDic.Add(GID, data);
            DataToGIDDic.Add(data, GID);
            return data;
        }
    }
}

