namespace Datas
{
    using UnityEditor;
    using UnityEngine;
    using static Datas.Data_Enemy;

    [CreateAssetMenu(menuName ="DataCre/Acive")]
    public class Data_Acive : ScriptableObject
    {
        public string AciveKey;
        public string Name;
        [TextArea]
        public string Info;
        public Texture Icon;
        public Enum_AciveType Type;
        public Enum_ProgressType ProgressType;
        public string ProgressMax;

        public enum Enum_ProgressType
        {
            Int,
            Float,
        }

        public enum Enum_AciveType
        {
            Main,
            Target,
            Value,
            Other,
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(Data_Acive))]
        class AciveDataEditor : Editor
        {
            private Data_Acive data;
            private Data_Acive_DBL dbl;

            private void OnEnable()
            {
                data = (Data_Acive)target;
                var db = AssetDatabase.LoadAssetAtPath<Data_Base>(Data_Base.DataPath);
                dbl = db.Acives;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                // プロパティ順に描画
                var prop = serializedObject.GetIterator();
                bool enterChildren = true;
                while (prop.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    EditorGUILayout.PropertyField(prop, true);
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("データベース設定", EditorStyles.boldLabel);

                if (dbl == null)
                {
                    EditorGUILayout.HelpBox("データベースが見つかりません。\n" + Data_Base.DataPath, MessageType.Warning);
                    return;
                }

                // データベース内での位置を確認
                var keyid = dbl.DataGetKeyID(data);
                if (keyid.Item2 == -1)
                {
                    // 未登録の場合
                    EditorGUILayout.HelpBox("このデータは未登録です。", MessageType.Info);
                    if (GUILayout.Button("+データベースに登録"))
                    {
                        var dbls = dbl.DBL.Find(x => x.Type == (Enum_AciveType)data.Type);
                        if (dbls == null)
                        {
                            dbls = new Data_Acive_DBL.DBL_Acive { Type = (Enum_AciveType)data.Type, Datas = new System.Collections.Generic.List<Data_Acive>() };
                            dbl.DBL.Add(dbls);
                        }
                        var nullindex = dbls.Datas.IndexOf(null);
                        if (nullindex >= 0) dbls.Datas[nullindex] = data;
                        else dbls.Datas.Add(data);
                        dbl.SaveDatabase();

                    }
                }
                else
                {
                    //登録済みの場合
                    var type = keyid.Item2 / Data_Acive_DBL.TypeSize;
                    var index = keyid.Item2 % Data_Acive_DBL.TypeSize;
                    EditorGUILayout.HelpBox("登録済み:" + (Enum_AciveType)type + "の" + index + "番目に存在します。", MessageType.None);
                    if (type != (int)data.Type) EditorGUILayout.HelpBox("種類が異なります", MessageType.Warning);

                    if (GUILayout.Button("×登録解除"))
                    {
                        var dbls = dbl.DBL.Find(x => x.Type == (Enum_AciveType)type);
                        dbl.DicRem(data);
                        dbls.Datas[index] = null;
                        dbl.SaveDatabase();
                    }
                }
            }
        }
#endif
    }
}

