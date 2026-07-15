
namespace Datas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

#endif
    [CreateAssetMenu(menuName = "DataCre/EnemyBattle")]
    public partial class Data_EnemyBattle : ScriptableObject
    {
        public enum Enum_SortOption
        {
            タイプ,
            AID,
            開始時間
        }
        [System.Serializable]
        public class Class_EnemyBattleData_Brach
        {
            [HideInInspector] public string DispStr;
            public int AID;
            public string BName;
            public float EndTime;
            public void DispSet()
            {
                DispStr = "[" + AID + "]" + BName + "|" + EndTime + "秒";
            }
        }
        public List<Class_EnemyBattleData_Brach> Branchs;

        public Enum_SortOption SortOption;
        public bool SortUpper = true;

        public float EndTime;
        [SerializeReference]
        public List<Class_Data_EBEvent_Base> Acts;
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Selection.objects.Contains(this)) return;
            EndTime = 0;
            for(int i = 0; i < Branchs.Count; i++)
            {
                EndTime = Mathf.Max(EndTime, Branchs[i].EndTime);
                Branchs[i].DispSet();
            }
            for (int i = 0; i < Acts.Count; i++)
            {
                Acts[i].DispSet(i);
            }

        }
        [CustomEditor(typeof(Data_EnemyBattle))]
        [CanEditMultipleObjects]
        class Suzuten_Data_EnemyBattle_Editor : Editor
        {
            SerializedProperty eventsProp;
            ReorderableList reorderableList;

            Type[] eventTypes;
            string[] eventTypeNames;

            private int selectedIndex = -1;
            private Vector2 detailScroll;

            private void OnEnable()
            {
                if (serializedObject.isEditingMultipleObjects) return;

                eventsProp = serializedObject.FindProperty("Acts");

                var baseType = typeof(Class_Data_EBEvent_Base);
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
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Branchs"));
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

                detailScroll = EditorGUILayout.BeginScrollView(detailScroll,GUILayout.Height(200));
                reorderableList.DoLayoutList();
                EditorGUILayout.EndScrollView();
                var layersProp = serializedObject.FindProperty("Acts");
                selectedIndex = EditorGUILayout.IntSlider(selectedIndex, -1, layersProp.arraySize - 1);
                if (selectedIndex >= 0 && selectedIndex < layersProp.arraySize)
                {
                    var layerProp = layersProp.GetArrayElementAtIndex(selectedIndex);
                    layerProp.isExpanded = true;
                    EditorGUILayout.PropertyField(layerProp,true);
                }
                EditorGUILayout.Space(400);
                serializedObject.ApplyModifiedProperties();

            }

            void AddNewEventInstance(Type type)
            {
                if (!typeof(Class_Data_EBEvent_Base).IsAssignableFrom(type))
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
                var list = new List<Class_Data_EBEvent_Base>();

                for (int i = 0; i < listProp.arraySize; i++)
                {
                    var element = listProp.GetArrayElementAtIndex(i);
                    if (element.managedReferenceValue is Class_Data_EBEvent_Base ev)
                    {
                        list.Add(ev);
                    }
                }

                switch (sortOption)
                {
                    case Enum_SortOption.タイプ:
                        list = sortUpper ?
                            list.OrderBy(e => e.TypeName).ToList() :
                            list.OrderByDescending(e => e.TypeName).ToList();
                        break;

                    case Enum_SortOption.AID:
                        list = sortUpper ?
                            list.OrderBy(e => e.ActID).ToList() :
                            list.OrderByDescending(e => e.ActID).ToList();
                        break;

                    case Enum_SortOption.開始時間:
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

            #region 行動Event用

            void SetupReorderableList()
            {
                reorderableList = new ReorderableList(serializedObject, eventsProp, true, true, true, true);

                reorderableList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "行動イベント");
                };



                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = eventsProp.GetArrayElementAtIndex(index);
                    float endTime = serializedObject.FindProperty("EndTime").floatValue;

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
                    var ev = element.managedReferenceValue as Class_Data_EBEvent_Base;
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
                        ShowContextMenu(index);
                    }
                    EditorGUI.LabelField(rect, element.FindPropertyRelative("DispName").stringValue);
                    //EditorGUI.PropertyField(rect, element, false);



                };

                reorderableList.elementHeightCallback = index =>
                {
                    return EditorGUIUtility.singleLineHeight;
                };
                reorderableList.onSelectCallback = list =>
                {
                    selectedIndex = list.index;
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

            void ShowContextMenu(int index)
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
                EditorGUIUtility.systemCopyBuffer = $"[EBEvent]\n{typeName}\n{json}";
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
                if (!buffer.StartsWith("[EBEvent]\n")) return;

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

