
namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Data_Equips;
    using static Data_Items;
    using static Data_Get;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CreateAssetMenu(menuName = "DataCre/Equipment")]
    public class Data_Equipment : Data_Item
    {
        public float UpgradeCost;
        public Class_UpgradeBoost[] UpgradeBoosts;
        public Class_EquipmentAdds[] EquipmentAdds;
        public Class_EquipmentTrigger[] TriggerAttacks;
#if UNITY_EDITOR
        [CustomEditor(typeof(Data_Equipment))]
        class EquipDataEditor : Editor
        {
            private Data_Equipment data;
            private Data_Equipment_DBL dbl;

            private void OnEnable()
            {
                data = (Data_Equipment)target;
                var db = AssetDatabase.LoadAssetAtPath<Data_Base>(Data_Base.DataPath);
                dbl = db.Equipments;
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
                            data.Type = (int)(Enum_EquipType)EditorGUILayout.EnumPopup("Type", (Enum_EquipType)data.Type);
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
                        var dbls = dbl.DBL.Find(x => x.Type == (Enum_EquipType)data.Type);
                        if (dbls == null)
                        {
                            dbls = new Data_Equipment_DBL.DBL_Equipment { Type = (Enum_EquipType)data.Type, Datas = new List<Data_Equipment>() };
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
                    EditorGUILayout.LabelField(gid + "," + index + "," + category + type, EditorStyles.boldLabel);
                    // 登録済みの場合
                    EditorGUILayout.HelpBox("登録済み:" + (Enum_EquipType)type + "の" + index + "番目に存在します。", MessageType.None);
                    if (type != data.Type) EditorGUILayout.HelpBox("種類が異なります", MessageType.Warning);

                    if (GUILayout.Button("×登録解除"))
                    {
                        var equipList = dbl.DBL.Find(x => x.Type == (Enum_EquipType)type);
                        equipList.Datas[index] = null;
                        dbl.SaveDatabase();
                    }
                }

            }
        }
#endif
    }
}
