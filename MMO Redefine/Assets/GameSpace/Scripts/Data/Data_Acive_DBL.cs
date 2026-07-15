namespace Datas
{
    using UnityEngine;
    using System.Collections.Generic;
    using static Datas.Data_Acive;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CreateAssetMenu(menuName ="DataCre/Acive_DBL")]
    public class Data_Acive_DBL : ScriptableObject
    {
        public const int TypeSize = 1000000;
        public List<DBL_Acive> DBL;

        [System.Serializable]
        public class DBL_Acive
        {
            public Enum_AciveType Type;
            public List<Data_Acive> Datas;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(DBL_Acive))]
            class DBL_Acive_Drawer : PropertyDrawer
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
        static Dictionary<string, (Data_Acive,int)> KeyToDataIDDic = new();
        static Dictionary<Data_Acive, (string,int)> DataToKeyIDDic = new();
        public (string,int) DataGetKeyID(Data_Acive AciveD)
        {
            if (DataToKeyIDDic.TryGetValue(AciveD, out var oKeyID)) return oKeyID;
            var ID = -1;
            for (int i = 0; i < DBL.Count; i++)
            {
                var Index = DBL[i].Datas.FindIndex(x => x == AciveD);
                if (Index >= 0)
                {
                    ID = (int)DBL[i].Type * TypeSize + Index;
                    break;
                }
            }
            if(ID >= 0)
            {
                DataToKeyIDDic.Add(AciveD, (AciveD.AciveKey, ID));
                KeyToDataIDDic.Add(AciveD.AciveKey, (AciveD, ID));
            }
            return (AciveD.AciveKey, ID);
        }
        public (Data_Acive,int) KeyGetDataID(string Key)
        {
            if (KeyToDataIDDic.TryGetValue(Key, out var oDataID)) return oDataID;
            var ID = -1;
            Data_Acive AciveD = null;
            for (int i = 0; i < DBL.Count; i++)
            {
                var Index = DBL[i].Datas.FindIndex(x => x.AciveKey == Key);
                if(Index >= 0)
                {
                    AciveD = DBL[i].Datas[Index];
                    ID = (int)DBL[i].Type * TypeSize + Index;
                    break;
                }
            }
            if(ID >= 0)
            {
                DataToKeyIDDic.Add(AciveD, (AciveD.AciveKey, ID));
                KeyToDataIDDic.Add(AciveD.AciveKey, (AciveD, ID));
            }
            return (AciveD, ID);
        }
        public void DicRem(Data_Acive AciveD)
        {
            if (!DataToKeyIDDic.TryGetValue(AciveD, out var keyid))return;
            DataToKeyIDDic.Remove(AciveD);
            KeyToDataIDDic.Remove(keyid.Item1);
        }
    }
}
