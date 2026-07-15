namespace Datas
{

    using System.Collections.Generic;
    using UnityEngine;
    using static Data_Items;
    using static Data_Get;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(menuName = "DataCre/Data_WeponList")]
    public class Data_Wepon_DBL : ScriptableObject
    {
        public List<DBL_Wepon> DBL;
        [System.Serializable]
        public class DBL_Wepon
        {
            public Enum_WeponType Type;
            public List<Data_Wepon> Datas;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(DBL_Wepon))]
            public class DBL_Wepon_Drawer : PropertyDrawer
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
        static Dictionary<Data_Wepon, int> DataToGIDDic = new();
        static Dictionary<int, Data_Wepon> GIDToDataDic = new();
        public int DataGetGID(Data_Wepon WeponD)
        {
            if (DataToGIDDic.TryGetValue(WeponD, out var oID)) return oID;
            for (int i = 0; i < DBL.Count; i++)
            {
                var index = DBL[i].Datas.IndexOf(WeponD);
                if (index >= 0)
                {
                    var id = GIDMake(Enum_ItemID.Wepon, (int)DBL[i].Type, index);
                    DataToGIDDic.Add(WeponD, id);
                    GIDToDataDic.Add(id, WeponD);
                    return id;
                }
            }
            return -1;
        }
        public Data_Wepon GIDGetData(int GID)
        {
            if (GID < (int)Enum_ItemID.Wepon || GID >= (int)Enum_ItemID.Wepon + (int)Enum_ItemID.Category) return null;
            if(GIDToDataDic.TryGetValue(GID,out var oData)) return oData;

            var index = GIDBrake(GID, out var itemCategory, out var itemType);
            var dbls = DBL.Find(x => x.Type == (Enum_WeponType)itemType);
            var data = dbls != null ? dbls.Datas[index] : null;
            GIDToDataDic.Add(GID, data);
            DataToGIDDic.Add(data, GID);
            return data;
        }

    }
}
