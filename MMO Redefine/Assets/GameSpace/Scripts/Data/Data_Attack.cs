namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Linq;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_EnumToJpString;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEditor.AddressableAssets;
    using System.IO;
#endif

    [CreateAssetMenu(menuName = "DataCre/Attack")]
    public partial class Data_Attack : Data_SlotGID
    {
        public enum Enum_SortOption
        {
            [InspectorName("タイプ")]Type,
            BID,
            [InspectorName("開始時間")] StartTime
        }
        public enum Enum_AttackType
        {
            通常,
            スキル,
            必殺,
        }
        public enum Enum_Cost
        {
            HP定数,
            HP上限割合,
            現在HP割合,
            MP定数,
            MP上限割合,
            現在MP割合,
            ST定数,
            ST上限割合,
            現在ST割合,
            EX定数,
        }
        [System.Serializable]
        public class Class_AttackData_Costs
        {
            public Enum_Cost Type;
            public float Val;
        }
        [System.Serializable]
        public class Class_AttackData_Brach
        {
            public int BID;
            public string BName;
            public float EndTime;
            public bool NoEnd;
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(Class_AttackData_Brach))]
            public class Class_AttackData_BrachDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    var re = new Rect(position.x, position.y, 0, EditorGUIUtility.singleLineHeight);
                    var bid = property.FindPropertyRelative("BID");
                    var bname = property.FindPropertyRelative("BName");
                    var endtime = property.FindPropertyRelative("EndTime");
                    var noend = property.FindPropertyRelative("NoEnd");

                    re.width = 40;
                    EditorGUI.LabelField(re, "BID");
                    re.x += 40;
                    re.width = 50;
                    bid.intValue = EditorGUI.IntField(re, bid.intValue);

                    re.x += 50;
                    re.width += 50;
                    EditorGUI.LabelField(re, "BName");

                    re.width = position.width - 140;
                    re.x += 50;
                    bname.stringValue = EditorGUI.TextField(re, bname.stringValue);

                    re.y += EditorGUIUtility.singleLineHeight;
                    re.x = position.x;
                    re.width = 60;
                    EditorGUI.LabelField(re, "EndTime");
                    re.x += 60;
                    re.width = 50;
                    endtime.floatValue = EditorGUI.FloatField(re, endtime.floatValue);

                    re.x += 50;
                    re.width += 50;
                    EditorGUI.LabelField(re, "NoEnd");

                    re.width = position.width - 160;
                    re.x += 50;
                    noend.boolValue = EditorGUI.Toggle(re, noend.boolValue);

                }
                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight * 2;
                }
            }
            #endif
        }
        public string AddressableAddress;
        public Enum_AttackType AtkType;
        public float CT;
        public Class_AttackData_Costs[] Costs;
        public List<Class_AttackData_Brach> Branchs;
        public Enum_SortOption SortOption;
        public bool SortUpper = true;

        public float EndTime;
        public bool ASpdUse;
        [SerializeReference]
        public List<Class_AEvent_Base> Events;

        public string CostStrs(bool mline)
        {
            var str = "";
            for (int i = 0; i < Costs.Length; i++)
            {
                var cost = Costs[i];
                var costStr = "<color=#888888>???";
                var per = false;
                switch (cost.Type)
                {
                    case Enum_Cost.HP定数:
                        costStr = "<color=#FF0000>HP";
                        break;
                    case Enum_Cost.HP上限割合:
                        costStr = "<color=#FF0000>HP";
                        per = true;
                        break;
                    case Enum_Cost.現在HP割合:
                        costStr = "<color=#FF0000>HP";
                        per = true;
                        break;
                    case Enum_Cost.MP定数:
                        costStr = "<color=#0000FF>MP";
                        break;
                    case Enum_Cost.MP上限割合:
                        costStr = "<color=#0000FF>MP";
                        per = true;
                        break;
                    case Enum_Cost.現在MP割合:
                        costStr = "<color=#0000FF>MP";
                        per = true;
                        break;
                    case Enum_Cost.ST定数:
                        costStr = "<color=#FFFF00>ST";
                        break;
                    case Enum_Cost.ST上限割合:
                        costStr = "<color=#FFFF00>ST";
                        per = true;
                        break;
                    case Enum_Cost.現在ST割合:
                        costStr = "<color=#FFFF00>ST";
                        per = true;
                        break;
                    case Enum_Cost.EX定数:
                        costStr = "<color=#FFFFFF>EX";
                        break;

                }
                if (str != "") str += mline ? "\n" : ",";
                str += costStr + ":" + cost.Val.ToString("F1") + (per ? "%" : "") + "</color>";
            }
            return str;
        }

        public string Infos()
        {
            var str = "";
            if(CT>0)str += "<color=#00FF00>CT:" + CT + "秒</color>";
            if(Costs!=null && Costs.Length > 0)str += "\n" + CostStrs(true);
            if (str != "") str += "\n";
            str += "[詳細]";
            if (Branchs == null || Branchs.Count <= 0)
            {
                str += "\n<size=60%>" + AEventInfo(-1) + "</size>";
            }
            else for(int i = 0; i < Branchs.Count; i++)
                {
                    if (Branchs[i].BName != "") str += "\n「" + Branchs[i].BName + "」";
                    else if (Branchs.Count > 1) str += "\n「分岐番号" + i + "」";
                    str += "(" + Branchs[i].EndTime + "秒)";
                        str += "\n<size=60%>" + AEventInfo(Branchs[i].BID) + "</size>";
                }
                return str;
        }

        string AEventInfo(int BID)
        {
            var str = "";
            for (int i = 0; i < Events.Count; i++)
            {
                var ev = Events[i];
                if (BID >= 0 && ev.IfBID != BID) continue;
                var addstr = "";
                if (str != "") addstr += "\n";

                addstr += "[" + ev.TypeName + "]";
                if(Branchs.Count>0) addstr += "(" + ev.TimesStr() + "秒)";
                bool check = true;
                switch (ev)
                {
                    default:check = false; break;
                    case Class_AEvent_ShotMain:addstr += ShotMain(ev);break;
                    case Class_AEvent_MyValState: addstr += MyValState(ev);break;
                    case Class_AEvent_MyBuf: addstr += MyBuf(ev);break;
                }
                if(check)str += addstr;
            }
            str += Branch();
            return str;
            string Branch()
            {
                var brcstr = "";
                for (int i = 0; i < Events.Count; i++)
                {
                    var ev = Events[i];
                    if (!(ev is Class_AEvent_BChange)) continue;
                    if (BID >= 0 && ev.IfBID != BID) continue;
                    if (brcstr != "") brcstr += ",";
                    var bch = (Class_AEvent_BChange)ev;
                    if (Branchs.Count > 0) brcstr += "(" + ev.TimesStr() + "秒)";
                    for (int k = 0; k < bch.Ifs.Length; k++) brcstr += "(" + EnumToJp(bch.Ifs[k]) + ")";
                    brcstr += "=>";
                    var nbr = Branchs.Find(x => x.BID == bch.NBID);
                    if (nbr == null || nbr.BName == "") brcstr += bch.NBID;
                    else brcstr += nbr.BName;

                }
                return brcstr != "" ? ("\n[分岐]" + brcstr) : "";
            }
            string ShotMain(Class_AEvent_Base ev)
            {
                var smain = (Class_AEvent_ShotMain)ev;
                var messtr = "";
                if (smain.Fire.Count > 1) messtr += "(弾数:" + smain.Fire.Count + ")";
                messtr += "(弾速:" + smain.Fire.Speed.x;
                if (smain.Fire.Speed.x != smain.Fire.Speed.y) messtr += "～" + smain.Fire.Speed.y;
                messtr += ")";
                for (int k = 0; k < smain.Hits.Length; k++)
                {
                    if (smain.Hits.Length > 1) messtr += "\n";
                    var shit = smain.Hits[k];
                    messtr += !shit.Option.Heal ? "(攻撃)" : "(回復)";
                    if (shit.Option.EHit) messtr += "(敵命中)";
                    if (shit.Option.FHit) messtr += "(味方命中)";
                    if (shit.Option.MHit) messtr += "(自己命中)";
                    messtr += "(属性:" + (!shit.Option.EleRides ? EnumToJp(shit.Option.Element) : "変化") + ")";
                    if (shit.Dam.Mult != 0) messtr += "\n" + shit.Dam.CalStrGet();
                    for (int m = 0; m < shit.BufAdds.Length; m++)
                    {
                        messtr += ShotBuf(shit.BufAdds[m]);
                    }
                }
                return messtr;
            }
            string ShotBuf(Class_AEvent_ShotBufAdd mbf)
            {
                var messtr = "";
                messtr += "\n(" + EnumToJp(mbf.ID) + ")" + (mbf.Op != Enum_BufOp.Non ? ("(" + EnumToJp(mbf.Op) + ")") : "") + "(" + EnumToJp(mbf.Set) + ")";
                if (mbf.Set != Enum_BufSet.Remove)
                {
                    if (mbf.Op != Enum_BufOp.Non) messtr += "\n";
                    messtr += "(時間:";
                    messtr += (mbf.Set != Enum_BufSet.AddUp || mbf.Times.y <= 0 ? "" : "+") + mbf.Times.x;
                    if (mbf.Set == Enum_BufSet.AddUp && mbf.Times.y > 0) messtr += "max" + mbf.Times.y;
                    messtr += "秒)";
                    if (mbf.CPow.Length > 0)
                    {
                        messtr += "(段階:";
                        messtr += (mbf.Set != Enum_BufSet.AddUp ? "" : "+") + CalSStr(mbf.CPow);
                        if (mbf.Set == Enum_BufSet.AddUp && mbf.MPow.Length > 0)
                        {
                            messtr += "max";
                            messtr += CalSStr(mbf.MPow);
                        }
                        messtr += ")";
                    }
                }
                return messtr;
            }
            string MyValState(Class_AEvent_Base ev)
            {
                var mvs = (Class_AEvent_MyValState)ev;
                var messtr = "";
                messtr += mvs.State + ":" + CalSStr(mvs.Val);
                return messtr;
            }
            string MyBuf(Class_AEvent_Base ev)
            {
                var mbf = (Class_AEvent_MyBuf)ev;
                var messtr = "";
                messtr += "(" + EnumToJp(mbf.ID) + ")" + (mbf.Op != Enum_BufOp.Non ? ("(" + EnumToJp(mbf.Op) + ")") : "") + "(" + EnumToJp(mbf.Set) + ")";
                if (mbf.Set != Enum_BufSet.Remove)
                {
                    if (mbf.Op != Enum_BufOp.Non) messtr += "\n";
                    messtr += "(時間:";
                    messtr += (mbf.Set != Enum_BufSet.AddUp || mbf.AddTimes.y <= 0 ? "" : "+") + mbf.AddTimes.x;
                    if (mbf.Set == Enum_BufSet.AddUp && mbf.AddTimes.y > 0) messtr += "max" + mbf.AddTimes.y;
                    messtr += "秒)";
                    if (mbf.CPow.Length > 0)
                    {
                        messtr += "(段階:";
                        messtr += (mbf.Set != Enum_BufSet.AddUp ? "" : "+") + CalSStr(mbf.CPow);
                        if (mbf.Set == Enum_BufSet.AddUp && mbf.MPow.Length > 0)
                        {
                            messtr += "max";
                            messtr += CalSStr(mbf.MPow);
                        }
                        messtr += ")";
                    }
                }
                return messtr;
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Selection.objects.Contains(this)) return;
            EndTime = 0;
            for (int i = 0; i < Branchs.Count; i++)
            {
                EndTime = Mathf.Max(EndTime, Branchs[i].EndTime);
            }

            for (int i=0;i<Events.Count;i++)
            {
                Events[i].DispSet(i);
            }
            if (AddressableAddress == "")
            {
                // AssetDatabase 経由で自分のパスを取得
                string path = AssetDatabase.GetAssetPath(this);
                if (string.IsNullOrEmpty(path)) return;

                // Addressables の設定を取得
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null) return;

                // グループを探す / なければ作る
                string groupName = "Attacks";
                var group = settings.FindGroup(groupName) ??
                    settings.CreateGroup(groupName, false, false, false, null,
                        typeof(UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema));

                // GUIDを取得
                string guid = AssetDatabase.AssetPathToGUID(path);

                // 種類/ファイル名 のアドレスを生成
                string[] parts = path.Split('/');
                string fileName = Path.GetFileNameWithoutExtension(path);
                string address = $"{fileName}";

                if (address == null) return;
                // 登録 or 更新
                var entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, group);
                    Debug.Log($"Addressable登録: {path} → {address}");
                }
                entry.address = address;
                AddressableAddress = address;
                // 保存
                AssetDatabase.SaveAssets();
            }

        }
        [CustomEditor(typeof(Data_Attack))]
        [CanEditMultipleObjects]
        class Suzuten_Data_Attack_Editor : Editor
        {
            SerializedProperty eventsProp;
            ReorderableList reorderableList;

            Type[] eventTypes;
            string[] eventTypeNames;

            int selectIndex = -1;
            Vector2 scrolle_Event;
            Vector2 scrolle_Info;
            private Data_Attack data;
            private Data_Base db;

            private void OnEnable()
            {
                eventsProp = serializedObject.FindProperty("Events");

                var baseType = typeof(Class_AEvent_Base);
                eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                eventTypeNames = eventTypes.Select(t =>
                {
                    var prop = t.GetProperty("EnumName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (prop != null)
                    {
                        return prop.GetValue(null).ToString();
                    }
                    return t.Name;
                }).ToArray();

                SetupReorderableList();

                data = (Data_Attack)target;
                db = AssetDatabase.LoadAssetAtPath<Data_Base>(Data_Base.DataPath);
            }

            public override void OnInspectorGUI()
            {

                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AddressableAddress"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Info"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AtkType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CT"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Costs"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Branchs"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ASpdUse"));
                EditorGUILayout.LabelField($"End: {serializedObject.FindProperty("EndTime").floatValue:0.##}");

                if (serializedObject.isEditingMultipleObjects)
                {
                    EditorGUILayout.HelpBox(
                        "Effect editing is disabled in multi-object mode.",
                        MessageType.Info
                    );

                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                // SortOption Enum - 幅広め
                SerializedProperty sortProp = serializedObject.FindProperty("SortOption");
                sortProp.enumValueIndex = (int)(Enum_SortOption)EditorGUILayout.EnumPopup(
                    GUIContent.none,
                    (Enum_SortOption)sortProp.enumValueIndex,
                    GUILayout.Width(150)
                );

                // SortUpper bool - コンパクトに表示
                SerializedProperty sortUpperProp = serializedObject.FindProperty("SortUpper");
                sortUpperProp.boolValue = EditorGUILayout.ToggleLeft("↑", sortUpperProp.boolValue, GUILayout.Width(40));

                // 並び替えボタン
                if (GUILayout.Button("並び替え"))
                {
                    SortList(eventsProp, (Enum_SortOption)sortProp.enumValueIndex, sortUpperProp.boolValue);
                }

                EditorGUILayout.EndHorizontal();
                var layersProp = serializedObject.FindProperty("Events");
                scrolle_Event = EditorGUILayout.BeginScrollView(scrolle_Event, GUILayout.Height(Mathf.Clamp(layersProp.arraySize * 30,50, 200)));
                reorderableList.DoLayoutList();
                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
                selectIndex = EditorGUILayout.IntSlider(selectIndex, -1, layersProp.arraySize - 1);
                if (selectIndex >= 0 && selectIndex < layersProp.arraySize)
                {
                    var layerProp = layersProp.GetArrayElementAtIndex(selectIndex);
                    layerProp.isExpanded = true;
                    EditorGUILayout.PropertyField(layerProp, true);
                }
                EditorGUILayout.Space();
                serializedObject.ApplyModifiedProperties();
                scrolle_Info = EditorGUILayout.BeginScrollView(scrolle_Info, GUILayout.Height(200));
                EditorGUILayout.TextArea(data.Infos());
                EditorGUILayout.EndScrollView();
            }

            void AddNewEventInstance(Type type)
            {
                if (!typeof(Class_AEvent_Base).IsAssignableFrom(type))
                {
                    Debug.LogError("Invalid event type");
                    return;
                }

                var instance = Activator.CreateInstance(type);


                int index = eventsProp.arraySize;
                eventsProp.InsertArrayElementAtIndex(index);
                eventsProp.GetArrayElementAtIndex(index).managedReferenceValue = instance;


                serializedObject.ApplyModifiedProperties();
            }
            void SortList(SerializedProperty listProp, Enum_SortOption sortOption, bool sortUpper)
            {
                var list = new List<Class_AEvent_Base>();

                for (int i = 0; i < listProp.arraySize; i++)
                {
                    var element = listProp.GetArrayElementAtIndex(i);
                    if (element.managedReferenceValue is Class_AEvent_Base ev)
                    {
                        list.Add(ev);
                    }
                }

                switch (sortOption)
                {
                    case Enum_SortOption.Type:
                        list = sortUpper ?
                            list.OrderBy(e => e.TypeName).ToList() :
                            list.OrderByDescending(e => e.TypeName).ToList();
                        break;

                    case Enum_SortOption.BID:
                        list = sortUpper ?
                            list.OrderBy(e => e.IfBID).ToList() :
                            list.OrderByDescending(e => e.IfBID).ToList();
                        break;

                    case Enum_SortOption.StartTime:
                        list = sortUpper ?
                            list.OrderBy(e => e.Times.x).ToList() :
                            list.OrderByDescending(e => e.Times.x).ToList();
                        break;
                }

                // 再代入
                listProp.ClearArray();
                listProp.arraySize = list.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    listProp.GetArrayElementAtIndex(i).managedReferenceValue = list[i];
                }

                listProp.serializedObject.ApplyModifiedProperties();
            }


            #region 攻撃Event用

            void SetupReorderableList()
            {
                reorderableList = new ReorderableList(serializedObject, eventsProp, true, true, true, true);

                reorderableList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "攻撃イベント");
                    if (Event.current.type == EventType.ContextClick &&
                    rect.Contains(Event.current.mousePosition))
                    {
                        ShowContextMenuAll();
                        Event.current.Use();
                    }
                };


                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = eventsProp.GetArrayElementAtIndex(index);
                    var endTime = serializedObject.FindProperty("EndTime").floatValue;
                    rect.xMin += 10;
                    Rect backRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    backRect.xMin -= 1;
                    backRect.xMax += 1;
                    backRect.yMin -= 1;
                    backRect.yMax += 1;
                    var backColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.0f, 0.0f, 0.0f);
                    EditorGUI.DrawRect(backRect, backColor);

                    Rect timeRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    var timeColor = EditorGUIUtility.isProSkin ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.75f, 0.75f, 0.75f);
                    EditorGUI.DrawRect(timeRect, timeColor);

                    //時間範囲表示
                    var ev = element.managedReferenceValue as Class_AEvent_Base;
                    if (ev != null)
                    {
                        float minWidth = 50f;
                        float colLight = 0.65f;
                        float colDark = 0.65f;

                        float startRatio = Mathf.InverseLerp(0, endTime, ev.Times.x);
                        float endRatio = Mathf.InverseLerp(0, endTime, ev.Times.y);

                        float x = timeRect.x + timeRect.width * startRatio;
                        float width = timeRect.width * (endRatio - startRatio);

                        Rect barRect1 = new Rect(x, timeRect.y, width, timeRect.height);
                        Rect barRect2 = new Rect(x, timeRect.y, Mathf.Max(minWidth, width), timeRect.height);
                        if (barRect2.x > timeRect.x + timeRect.width / 3f * 2) barRect2.x -= barRect2.width / 3 * 2;
                        var barColor2 = EditorGUIUtility.isProSkin ? new Color(colDark, 0.0f, 0.0f) : new Color(1.0f, colLight, colLight);
                        EditorGUI.DrawRect(barRect2, barColor2); // 最小表示バー
                        if (ev.Times.z <= 0) barRect1.width = Mathf.Max(2, barRect1.width);
                        var barColor1 = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.0f, colDark) : new Color(colLight, colLight, 1.0f);
                        EditorGUI.DrawRect(barRect1, barColor1); // 青色のバー
                        if (ev.Times.z > 0)
                        {
                            for (int k = Mathf.RoundToInt(ev.Times.x * 60); k <= Mathf.RoundToInt(ev.Times.y * 60); k += Mathf.Max(1, Mathf.RoundToInt(ev.Times.z * 60)))
                            {
                                float cRatio = Mathf.InverseLerp(0, endTime, k / 60f);
                                Rect barRect3 = new Rect(timeRect.x + cRatio * timeRect.width, timeRect.y, 2, timeRect.height);
                                var barColor3 = EditorGUIUtility.isProSkin ? new Color(0.0f, colDark, 0.0f) : new Color(colLight, 1.0f, colLight);
                                EditorGUI.DrawRect(barRect3, barColor3); // 追加バー
                            }
                        }
                    }
                    //時間軸表示
                    int tcut = Mathf.RoundToInt(endTime * 10);
                    for (int i = 0; i < tcut; i++)
                    {
                        Rect lineRect = new Rect(timeRect.x + i / (float)tcut * timeRect.width, timeRect.y, 1, timeRect.height);
                        if (i % 5 == 0) lineRect.width = 3;
                        var lineColor = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.0f, 0.0f, 0.5f) : new Color(1.0f, 1.0f, 1.0f, 0.5f);
                        EditorGUI.DrawRect(lineRect, lineColor); // 縦線
                    }
                    // イベント取得
                    float indent = 10f; // 折りたたみアイコンの幅
                    Rect clickableArea = new Rect(rect.x + indent, rect.y, rect.width - indent, EditorGUIUtility.singleLineHeight);

                    if (clickableArea.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
                    {
                        Event.current.Use();
                        ShowContextMenuElement(index);
                    }
                    EditorGUI.LabelField(rect, element.FindPropertyRelative("DispName").stringValue);


                    //EditorGUI.PropertyField(rect, element, true);


                };
                reorderableList.onSelectCallback = list =>
                {
                    selectIndex = list.index;
                };
                reorderableList.elementHeightCallback = index =>
                {
                    return EditorGUIUtility.singleLineHeight;

                    //var element = eventsProp.GetArrayElementAtIndex(index);
                    //if (element.isExpanded)
                    //{
                    //    return EditorGUI.GetPropertyHeight(element, true);
                    //}
                    //else
                    //{
                    //    return EditorGUIUtility.singleLineHeight;
                    //}
                };


                reorderableList.onAddDropdownCallback = (buttonRect, list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < eventTypes.Length; i++)
                    {
                        int capture = i;
                        menu.AddItem(new GUIContent(eventTypeNames[i]), false, () =>
                        {
                            AddNewEventInstance(eventTypes[capture]);
                        });
                    }

                    menu.ShowAsContext();
                };

                reorderableList.onRemoveCallback = list =>
                {
                    eventsProp.DeleteArrayElementAtIndex(list.index);
                    serializedObject.ApplyModifiedProperties();
                };



            }
            void ShowContextMenuAll()
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy All"), false, CopyAll);
                menu.AddItem(new GUIContent("Paste All"), false, PasteAll);

                menu.ShowAsContext();
            }
            [System.Serializable]
            class EventContainer
            {
                [SerializeReference]
                public List<Class_AEvent_Base> Events;
            }
            void CopyAll()
            {
                serializedObject.Update();

                var container = new EventContainer();
                container.Events = ((Data_Attack)serializedObject.targetObject).Events;

                string json = EditorJsonUtility.ToJson(container, true);

                EditorGUIUtility.systemCopyBuffer = json;
            }
            void PasteAll()
            {
                serializedObject.Update();

                string json = EditorGUIUtility.systemCopyBuffer;

                if (string.IsNullOrEmpty(json))
                    return;

                var container = new EventContainer();

                EditorJsonUtility.FromJsonOverwrite(json, container);

                Undo.RecordObject(serializedObject.targetObject, "Paste Events");

                ((Data_Attack)serializedObject.targetObject).Events = container.Events;

                serializedObject.ApplyModifiedProperties();
            }

            void ShowContextMenuElement(int index)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("内部コピー"), false, () => CopyEvent(index));
                menu.AddItem(new GUIContent("内部ペースト"), false, () => PasteEvent(index));

                menu.AddItem(new GUIContent("クリップコピー"), false, () => CopyEvent2(index));
                menu.AddItem(new GUIContent("クリップペースト"), false, () => PasteEvent2(index));
                menu.AddItem(new GUIContent("削除"), false, () => DeleteEvent(index));
                menu.ShowAsContext();
            }

            // コピー用変数
            static string copiedJson = null;
            static System.Type copiedType = null;

            void CopyEvent(int index)
            {
                SerializedProperty element = eventsProp.GetArrayElementAtIndex(index);
                var original = element.managedReferenceValue;
                if (original != null)
                {
                    copiedJson = EditorJsonUtility.ToJson(original);
                    copiedType = original.GetType();
                }
            }
            void CopyEvent2(int index)
            {
                var element = eventsProp.GetArrayElementAtIndex(index);
                var obj = element.managedReferenceValue;

                if (obj == null) return;

                string json = EditorJsonUtility.ToJson(obj);
                string typeName = obj.GetType().AssemblyQualifiedName;

                // 複数行で区切って保存（\n を使う）
                EditorGUIUtility.systemCopyBuffer = $"[SuzutenEvent]\n{typeName}\n{json}";
            }

            void PasteEvent(int index)
            {
                if (copiedJson == null || copiedType == null) return;

                var element = eventsProp.GetArrayElementAtIndex(index);

                var newInstance = System.Activator.CreateInstance(copiedType);
                EditorJsonUtility.FromJsonOverwrite(copiedJson, newInstance);

                element.managedReferenceValue = newInstance;
                serializedObject.ApplyModifiedProperties();
            }
            void PasteEvent2(int index)
            {
                string buffer = EditorGUIUtility.systemCopyBuffer;
                if (!buffer.StartsWith("[SuzutenEvent]\n")) return;

                string[] parts = buffer.Split(new[] { '\n' }, 3);
                if (parts.Length < 3) return;

                string typeName = parts[1];
                string json = parts[2];

                var type = System.Type.GetType(typeName);
                if (type == null) return;

                var newInstance = System.Activator.CreateInstance(type);
                EditorJsonUtility.FromJsonOverwrite(json, newInstance);

                var element = eventsProp.GetArrayElementAtIndex(index);
                element.managedReferenceValue = newInstance;
                serializedObject.ApplyModifiedProperties();
            }

            void DeleteEvent(int index)
            {
                eventsProp.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            }
            #endregion


        }
#endif
    }



}
