namespace Datas
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CreateAssetMenu(menuName ="DataCre/Enemy")]
    public class Data_Enemy : ScriptableObject
    {
        public string Name;
        [TextArea]
        public string Info;
        public Texture Icon;
        public Enum_EnemyType Type;
        public GameObject EnemyObj;

        public enum Enum_EnemyType
        {
            Normal,
            Sei,
            FBoss,
            RBoss,
            Harvest,
            Other,
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(Data_Enemy))]
        class EnemyDataEditor : Editor
        {
            private Data_Enemy data;
            private Data_Enemy_DBL dbl;

            private void OnEnable()
            {
                data = (Data_Enemy)target;
                var db = AssetDatabase.LoadAssetAtPath<Data_Base>(Data_Base.DataPath);
                dbl = db.Enemys;
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
                int id = dbl.DataGetID(data);
                int index = id % Data_Enemy_DBL.TypeSize;
                int type = id / Data_Enemy_DBL.TypeSize;
                if (id == -1)
                {
                    // 未登録の場合
                    EditorGUILayout.HelpBox("このデータは未登録です。", MessageType.Info);
                    if (GUILayout.Button("+データベースに登録"))
                    {
                        var dbls = dbl.DBL.Find(x => x.Type == (Enum_EnemyType)data.Type);
                        if (dbls == null)
                        {
                            dbls = new Data_Enemy_DBL.DBL_Enemy { Type = (Enum_EnemyType)data.Type, Datas = new System.Collections.Generic.List<Data_Enemy>() };
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
                    // 登録済みの場合
                    EditorGUILayout.HelpBox("登録済み:" + (Enum_EnemyType)type + "の" + index + "番目に存在します。", MessageType.None);
                    if (type != (int)data.Type) EditorGUILayout.HelpBox("種類が異なります", MessageType.Warning);

                    if (GUILayout.Button("×登録解除"))
                    {
                        var dbls = dbl.DBL.Find(x => x.Type == (Enum_EnemyType)type);
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

