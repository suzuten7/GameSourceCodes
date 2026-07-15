namespace Datas
{
    using UnityEngine;
    using System.Collections.Generic;
    using static Data_Enemy;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CreateAssetMenu(menuName = "DataCre/Enemy_DBL")]
    public class Data_Enemy_DBL : ScriptableObject
    {
        public const int TypeSize = 1000000;
        public List<DBL_Enemy> DBL;
        [System.Serializable]
        public class DBL_Enemy
        {
            public Enum_EnemyType Type;
            public List<Data_Enemy> Datas;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(DBL_Enemy))]
            class DBL_Enemy_Drawer : PropertyDrawer
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
        static Dictionary<Data_Enemy, int> DataToIDDic = new();
        static Dictionary<int, Data_Enemy> IDToDataDic = new();
        public int DataGetID(Data_Enemy EnemyD)
        {
            if (DataToIDDic.TryGetValue(EnemyD, out var oid)) return oid;
            for (int i = 0; i < DBL.Count; i++)
            {
                var index = DBL[i].Datas.IndexOf(EnemyD);
                if (index >= 0)
                {
                    var ID = (int)DBL[i].Type * TypeSize + index;
                    DataToIDDic.Add(EnemyD, ID);
                    IDToDataDic.Add(ID, EnemyD);
                    return ID;
                }
            }
            return -1;
        }
        public Data_Enemy IDGetData(int ID)
        {
            if (IDToDataDic.TryGetValue(ID, out var oData)) return oData;
            var index = ID % TypeSize;
            var type = ID / TypeSize;
            var dbl = DBL.Find(x => x.Type == (Enum_EnemyType)type);
            var data = dbl != null ? dbl.Datas[index] : null;
            IDToDataDic.Add(ID, data);
            DataToIDDic.Add(data, ID);
            return data;
        }
        public void DicRem(Data_Enemy EnemyD)
        {
            if (!DataToIDDic.TryGetValue(EnemyD, out var id)) return;
            DataToIDDic.Remove(EnemyD);
            IDToDataDic.Remove(id);
        }
    }
}

