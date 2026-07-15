namespace Datas
{
    using UnityEngine;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using static Data_Items;
    using static Data_Get;
    [CreateAssetMenu(menuName = "DataCre/Consumables")]
    public class Data_Consumables : Data_Item
    {
        public Data_Attack Attack;
#if UNITY_EDITOR
        [CustomEditor(typeof(Data_Consumables))]
        class ConsumDataEditor : Editor
        {
            private Data_Consumables data;
            private Data_Consumable_DBL dbl;

            private void OnEnable()
            {
                data = (Data_Consumables)target;
                var db = AssetDatabase.LoadAssetAtPath<Data_Base>(Data_Base.DataPath);
                dbl = db.Consumables;
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
                    // 特定プロパティだけ置き換え描画
                    switch (prop.name)
                    {
                        case "Type":
                            data.Type = (int)(Enum_ConsumableType)EditorGUILayout.EnumPopup("Type", (Enum_ConsumableType)data.Type);
                            break;
                        default:
                            EditorGUILayout.PropertyField(prop, true);
                            break;
                    }
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
                int gid = dbl.DataGetGID(data);
                int index = GIDBrake(gid, out var category, out var type);

                if (gid == -1)
                {
                    // 未登録の場合
                    EditorGUILayout.HelpBox("このデータは未登録です。", MessageType.Info);
                    if (GUILayout.Button("+データベースに登録"))
                    {
                        var dbls = dbl.DBL.Find(x => x.Type == (Enum_ConsumableType)data.Type);
                        if (dbls == null)
                        {
                            dbls = new Data_Consumable_DBL.DBL_Consumable { Type = (Enum_ConsumableType)data.Type, Datas = new List<Data_Consumables>() };
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
                    EditorGUILayout.HelpBox("登録済み:" + (Enum_ConsumableType)type + "の" + index + "番目に存在します。", MessageType.None);
                    if (type != data.Type) EditorGUILayout.HelpBox("種類が異なります", MessageType.Warning);

                    if (GUILayout.Button("×登録解除"))
                    {
                        var equipList = dbl.DBL.Find(x => x.Type == (Enum_ConsumableType)type);
                        equipList.Datas[index] = null;
                        dbl.SaveDatabase();
                    }
                }
            }
        }
#endif
    }
}
