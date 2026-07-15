namespace Toole
{


    using UnityEngine;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
    using System.IO;
    using System.Linq;
#endif
    [CreateAssetMenu(menuName = "Toole/ImageCompose")]
    public class ImageComposeSO : ScriptableObject
    {
        public Vector2Int outputSize = new Vector2Int(256,256);
        public Color backColor;
        public List<Layer> layers = new List<Layer>();
        public Color previewCheck1 = new Color(0.7f, 0.7f, 0.7f);
        public Color previewCheck2 = new Color(0.3f, 0.3f, 0.3f);
        [System.Serializable]
        public class Layer
        {
            public bool noOutput;
            public bool locks;
            public bool hidePreview;

            public int order;
            public Texture2D texture;
            public Color color;

            public Vector2 anchor;
            public Vector2 pivot;
            public Vector2 position;

            public float rotation;
            public Vector2 size;
            public Vector2 scale;
        }

#if UNITY_EDITOR
        // ===============================
        // Editor 専用コード
        // ===============================
        static Texture2D GetReadable(Texture2D tex)
        {
            if (tex == null) return null;
            if (tex.isReadable) return tex;

            string path = AssetDatabase.GetAssetPath(tex);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return tex;

            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return tex;
        }
        public void GeneratePNG_EditorOnly()
        {
            ImageComposeSOEditor.Generate(this);
        }

        [CustomEditor(typeof(ImageComposeSO))]
        class ImageComposeSOEditor : Editor
        {
            ImageComposeSO data;
            int selectedLayerIndex = -1;
            ReorderableList layerList;
            int toolmode = 0;
            bool layerDrag = false;
            Vector2 mouseStartPos;
            Vector2 startscale;
            enum enum_toolmode
            {
                Select,
                Setting,
                Render,
                Move,
                Rotate,
                Scale,
            }
            void OnEnable()
            {
                data = (ImageComposeSO)target;
                ListSet();
            }
            void ListSet()
            {
                var layersProp = serializedObject.FindProperty("layers");

                layerList = new ReorderableList
                (
                    serializedObject,
                    layersProp,
                    draggable: true,
                    displayHeader: true,
                    displayAddButton: true,
                    displayRemoveButton: true
                );

                layerList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Layers");
                };

                layerList.elementHeightCallback = index =>
                {
                    var e = layerList.serializedProperty.GetArrayElementAtIndex(index);
                    float line = EditorGUIUtility.singleLineHeight + 2;

                    if (!e.isExpanded) return line + 4;
                    return 16 * line + 6;
                };


                layerList.drawElementCallback = (rect, index, active, focused) =>
                {


                    var e = layerList.serializedProperty.GetArrayElementAtIndex(index);

                    EditorGUI.BeginChangeCheck();
                    float line = EditorGUIUtility.singleLineHeight;
                    rect.xMin += 10;
                    rect.y += 2;
                    rect.height = line;
                    if (index == selectedLayerIndex) EditorGUI.DrawRect(rect, new Color(0, 0.25f, 0.5f, 0.4f));

                    var tex = e.FindPropertyRelative("texture").objectReferenceValue;
                    var setstr = "";
                    if (e.FindPropertyRelative("noOutput").boolValue)
                    {
                        if (setstr != "") setstr += "|";
                        setstr += "[NoOutput]";
                    }
                    if (e.FindPropertyRelative("locks").boolValue)
                    {
                        if (setstr != "") setstr += "|";
                        setstr += "[Locks]";
                    }
                    if (e.FindPropertyRelative("hidePreview").boolValue)
                    {
                        if (setstr != "") setstr += "|";
                        setstr += "[HidePreview]";
                    }
                    setstr += (tex != null ? tex.name : "Null");
                    EditorGUI.LabelField(rect, setstr, new GUIStyle { alignment = TextAnchor.MiddleRight });
                    if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
                    {
                        Event.current.Use();
                        ShowContextMenu(index);
                    }
                    EditorGUI.LabelField(rect, $"Layer {index}");

                    //bool expanded = EditorGUI.Foldout(rect, e.isExpanded, $"Layer {index}", true);
                    //if (EditorGUI.EndChangeCheck())
                    //{
                    //    // ★ 開閉した = この要素を選択
                    //    layerList.index = index;
                    //    selectedLayerIndex = index;
                    //    e.isExpanded = expanded;
                    //    GUI.FocusControl(null);
                    //}
                    //else
                    //{
                    //    e.isExpanded = expanded;
                    //}
                    //if (!e.isExpanded) return;

                    //EditorGUI.indentLevel++;
                    //rect.y += line + 5;
                    //EditorGUI.LabelField(rect, "Setting", EditorStyles.boldLabel);
                    //rect.y += line;
                    //Draw(ref rect, e, "noOutput");
                    //Draw(ref rect, e, "locks");
                    //Draw(ref rect, e, "hidePreview");
                    //EditorGUI.LabelField(rect, "Render", EditorStyles.boldLabel);
                    //rect.y += line;
                    //Draw(ref rect, e, "order");
                    //Draw(ref rect, e, "texture");
                    //Draw(ref rect, e, "color");
                    //EditorGUI.LabelField(rect, "Transform", EditorStyles.boldLabel);
                    //rect.y += line;
                    //Draw(ref rect, e, "anchor");
                    //Draw(ref rect, e, "pivot");
                    //Draw(ref rect, e, "position");
                    //Draw(ref rect, e, "rotation");
                    //Draw(ref rect, e, "size");
                    //Draw(ref rect, e, "scale");

                    //EditorGUI.indentLevel--;
                };


                layerList.onSelectCallback = list =>
                {
                    selectedLayerIndex = list.index;
                    Repaint(); // Preview と同期したいなら必須
                };
                layerList.onReorderCallback = list =>
                {
                    selectedLayerIndex = list.index;
                    Repaint();
                };
                layerList.onAddCallback = (list) =>
                {
                    layersProp.arraySize++;
                    int newIndex = layersProp.arraySize - 1;
                    var newLayer = layersProp.GetArrayElementAtIndex(newIndex);

                    // 初期値セット
                    newLayer.FindPropertyRelative("order").intValue = 0;
                    newLayer.FindPropertyRelative("texture").objectReferenceValue = null;
                    newLayer.FindPropertyRelative("color").colorValue = Color.white;
                    newLayer.FindPropertyRelative("anchor").vector2Value = new Vector2(0.5f, 0.5f);
                    newLayer.FindPropertyRelative("pivot").vector2Value = new Vector2(0.5f, 0.5f);
                    newLayer.FindPropertyRelative("position").vector2Value = Vector2.zero;
                    newLayer.FindPropertyRelative("rotation").floatValue = 0f;
                    newLayer.FindPropertyRelative("size").vector2Value = new Vector2(64, 64);
                    newLayer.FindPropertyRelative("scale").vector2Value = Vector2.one;

                    // 選択状態に
                    list.index = newIndex;

                    serializedObject.ApplyModifiedProperties();
                };
            }
            void ShowContextMenu(int index)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("内部コピー"), false, () => CopyLayer(index));
                menu.AddItem(new GUIContent("内部ペースト"), false, () => PasteLayer(index));

                menu.AddItem(new GUIContent("クリップコピー"), false, () => CopyLayer2(index));
                menu.AddItem(new GUIContent("クリップペースト"), false, () => PasteLayer2(index));
                menu.AddItem(new GUIContent("削除"), false, () => DeleteEvent(index));
                menu.ShowAsContext();
            }

            // コピー用変数
            static string copiedJson = null;

            void CopyLayer(int index)
            {
                copiedJson = EditorJsonUtility.ToJson(data.layers[index]);
            }
            void CopyLayer2(int index)
            {
                string json = EditorJsonUtility.ToJson(data.layers[index]);
                // 複数行で区切って保存（\n を使う）
                EditorGUIUtility.systemCopyBuffer = $"[ImageCompLayer]\n{json}";
            }

            void PasteLayer(int index)
            {
                if (copiedJson == null) return;
                EditorJsonUtility.FromJsonOverwrite(copiedJson, data.layers[index]);
                serializedObject.ApplyModifiedProperties();
            }
            void PasteLayer2(int index)
            {
                string buffer = EditorGUIUtility.systemCopyBuffer;
                if (!buffer.StartsWith("[ImageCompLayer]\n")) return;

                string[] parts = buffer.Split(new[] { '\n' });
                if (parts.Length < 2) return;

                string json = parts[1];

                EditorJsonUtility.FromJsonOverwrite(copiedJson, data.layers[index]);
                serializedObject.ApplyModifiedProperties();
            }

            void DeleteEvent(int index)
            {
                serializedObject.FindProperty("layers").DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            }
            void Draw(ref Rect r, SerializedProperty parent, string name)
            {
                EditorGUI.PropertyField(r, parent.FindPropertyRelative(name));
                r.y += EditorGUIUtility.singleLineHeight + 2;
            }
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("outputSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("backColor"));
                layerList.DoLayoutList();
                GUILayout.Space(8);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("previewCheck1"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("previewCheck2"));
                toolmode = GUILayout.Toolbar(toolmode, new string[] {"Select","Setting","Image", "Move", "Rotate", "Scale" },GUILayout.Height(24));
                var layersProp = serializedObject.FindProperty("layers");
                selectedLayerIndex = EditorGUILayout.IntSlider(selectedLayerIndex, -1, layersProp.arraySize-1);
                if (selectedLayerIndex >= 0 && selectedLayerIndex < layersProp.arraySize)
                {
                    var layerProp = layersProp.GetArrayElementAtIndex(selectedLayerIndex);
                    switch ((enum_toolmode)toolmode)
                    {
                        case enum_toolmode.Setting:
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("noOutput"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("locks"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("hidePreview"));
                            break;
                        case enum_toolmode.Render:
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("order"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("texture"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("color"));
                            break;
                        case enum_toolmode.Move:
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("anchor"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("pivot"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("position"));
                            break;
                        case enum_toolmode.Rotate:
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("rotation"));
                            break;
                        case enum_toolmode.Scale:
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("size"));
                            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("scale"));
                            break;
                    }
                }


                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
                DrawPreviewLayers();

                if (GUILayout.Button("Generate PNG (Overwrite)"))
                {
                    Generate(data);
                }
            }
            void DrawPreviewLayers()
            {
                var layersProp = serializedObject.FindProperty("layers");

                List<int> drawOrder = new List<int>();
                for (int i = 0; i < layersProp.arraySize; i++)drawOrder.Add(i);
                drawOrder.Sort((a, b) =>
                {
                    var la = layersProp.GetArrayElementAtIndex(a)
                        .FindPropertyRelative("order").intValue;
                    var lb = layersProp.GetArrayElementAtIndex(b)
                        .FindPropertyRelative("order").intValue;
                    return la.CompareTo(lb);
                });

                GUILayout.Label("Preview", EditorStyles.boldLabel);

                // プレビュー領域
                float maxWidth = EditorGUIUtility.currentViewWidth - 40;

                float ratio = (float)data.outputSize.y / data.outputSize.x;
                Rect previewRect = GUILayoutUtility.GetRect(maxWidth, maxWidth * ratio, GUILayout.ExpandWidth(false));

                Vector2 outputSize = data.outputSize;
                Vector2 previewSize = previewRect.size;
                // 出力 → プレビュー倍率
                Vector2 previewScale = new Vector2(previewSize.x / outputSize.x,previewSize.y / outputSize.y);
                // 背景チェック描画
                DrawChecker(previewRect);

                Event e = Event.current;
                // カーソル変更（UX向上）
                switch ((enum_toolmode)toolmode)
                {
                    case enum_toolmode.Select:
                        EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.Link);
                        break;
                    case enum_toolmode.Move:
                        EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.MoveArrow);
                        break;
                    case enum_toolmode.Rotate:
                        EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.RotateArrow);
                        break;
                    case enum_toolmode.Scale:
                        EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.ScaleArrow);
                        break;
                }
                Vector2 selanchor = Vector2.zero;
                Vector2 selpivot = Vector2.zero;
                Rect? seloutRect = null;
                var seloutRot = 0f;
                foreach (int i in drawOrder)
                {
                    var layerProp = layersProp.GetArrayElementAtIndex(i);
                    Texture2D tex = (Texture2D)layerProp.FindPropertyRelative("texture").objectReferenceValue;
                    if (tex == null) continue;
                    if (layerProp.FindPropertyRelative("hidePreview").boolValue) continue;
                    Vector2 anchor = layerProp.FindPropertyRelative("anchor").vector2Value;
                    anchor.y = 1f - anchor.y;
                    Vector2 pivot = layerProp.FindPropertyRelative("pivot").vector2Value;
                    pivot.y = 1f - pivot.y;
                    Vector2 pos = layerProp.FindPropertyRelative("position").vector2Value;
                    pos.y *= -1;
                    Vector2 size = layerProp.FindPropertyRelative("size").vector2Value;
                    Vector2 scale = layerProp.FindPropertyRelative("scale").vector2Value;
                    float rotation = layerProp.FindPropertyRelative("rotation").floatValue;
                    rotation *= -1;
                    Color color = layerProp.FindPropertyRelative("color").colorValue;

                    Vector2 finalSize = (size == Vector2.zero ? new Vector2(tex.width, tex.height) : size);
                    finalSize = Vector2.Scale(finalSize, scale);
                    Vector2 pivotOffset = Vector2.Scale(finalSize, pivot);

                    Vector2 anchorPos = Vector2.Scale(anchor, outputSize);
                    Vector2 drawPos = anchorPos + pos;          // ← 出力座標

                    Vector2 drawPosPreview = Vector2.Scale(drawPos, previewScale);
                    Vector2 finalSizePreview = Vector2.Scale(finalSize, previewScale);
                    Vector2 pivotOffsetPreview = Vector2.Scale(pivotOffset, previewScale);

                    Vector2 previewOffset = previewRect.position;

                    Rect rect = new Rect(
                        previewOffset + drawPosPreview - pivotOffsetPreview,
                        finalSizePreview
                    );

                    Vector2 pivotWorld = previewOffset + drawPosPreview;

                    Color prev = GUI.color;
                    GUI.color = color;

                    // 回転の基準を rect のピボット位置に修正
                    Vector2 pivotWorldPos = previewRect.position + drawPos;
                    GUIUtility.RotateAroundPivot(rotation, pivotWorld);
                    GUI.DrawTexture(rect, tex);
                    GUI.matrix = Matrix4x4.identity;
                    GUI.color = prev;

                    if ((enum_toolmode)toolmode == enum_toolmode.Select)
                    {
                        if(layerProp.FindPropertyRelative("locks").boolValue) DrawRotatedRectOutline(rect, rotation, new Color(0.5f, 0.5f, 0.5f, 0.3f));
                        else DrawRotatedRectOutline(rect, rotation, new Color(1,0,1,0.5f));
                        if (HitTestRotatedRect(rect, e.mousePosition, rotation) && e.type == EventType.MouseDown)
                        {
                            selectedLayerIndex = i;
                            DrawRotatedRectOutline(rect, rotation, Color.yellow);
                        }
                    }

                    // 選択レイヤーの強調
                    if (i == selectedLayerIndex)
                    {
                        selanchor = previewRect.min + previewRect.size * anchor;
                        selpivot = pivotWorld;
                        seloutRect = rect;
                        seloutRot = rotation;
                        //操作
                        if (previewRect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
                        {
                            layerDrag = true;
                            mouseStartPos = e.mousePosition;
                            startscale = layerProp.FindPropertyRelative("scale").vector2Value;
                        }
                        if(e.type == EventType.MouseUp) layerDrag = false;
                        if (layerDrag && e.type == EventType.MouseDrag)
                        {

                            var deltaOutput = new Vector2(e.delta.x / previewScale.x, -e.delta.y / previewScale.y);

                            switch ((enum_toolmode)toolmode)
                            {
                                case enum_toolmode.Move:
                                    layerProp.FindPropertyRelative("position").vector2Value += deltaOutput;
                                    break;
                                case enum_toolmode.Rotate:
                                    var pb = mouseStartPos - rect.center;
                                    var pd = mouseStartPos + e.delta - rect.center;
                                    var r1 = Mathf.Atan2(pb.y, pb.x) * Mathf.Rad2Deg;
                                    var r2 = Mathf.Atan2(pd.y, pd.x) * Mathf.Rad2Deg;
                                    mouseStartPos = e.mousePosition;
                                    var rot = layerProp.FindPropertyRelative("rotation").floatValue;
                                    rot = Mathf.Repeat(rot + Mathf.DeltaAngle(r2,r1) * 1.5f, 360f);
                                    layerProp.FindPropertyRelative("rotation").floatValue = rot;
                                    break;
                                case enum_toolmode.Scale:
                                    var ms = V2Rotate(mouseStartPos - rect.center,-rotation);
                                    var md = V2Rotate(mouseStartPos + e.delta - rect.center,-rotation);
                                    var m = (md - ms);
                                    m.x = Mathf.Abs(m.x);
                                    m.y = Mathf.Abs(m.y);
                                    if (Mathf.Abs(md.x) < Mathf.Abs(ms.x)) m.x *= -1;
                                    if (Mathf.Abs(md.y) < Mathf.Abs(ms.y)) m.y *= -1;
                                    if (startscale.x < 0) m.x *= -1;
                                    if (startscale.y < 0) m.y *= -1;
                                    var s = layerProp.FindPropertyRelative("scale").vector2Value;
                                    s += m * 0.01f;
                                    layerProp.FindPropertyRelative("scale").vector2Value = s;
                                    break;
                            }
                            serializedObject.ApplyModifiedProperties();
                            e.Use();


                        }
                    }
                }
                if (seloutRect != null)
                {
                    Handles.BeginGUI();
                    Color prev = Handles.color;
                    Handles.color = Color.red;
                    Handles.DrawWireDisc(selanchor, Vector3.forward, 5);
                    Handles.color = prev;
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(selpivot, Vector3.forward, 5);
                    Handles.color = prev;
                    Handles.EndGUI();
                    DrawRotatedRectOutline(seloutRect.Value, seloutRot, Color.yellow);
                }
                var outRectSize = 4f;
                EditorGUI.DrawRect(new Rect(previewRect.x - outRectSize, previewRect.y - outRectSize, previewRect.width + outRectSize * 2, outRectSize), Color.black);
                EditorGUI.DrawRect(new Rect(previewRect.x - outRectSize, previewRect.y + previewRect.height, previewRect.width + outRectSize * 2, outRectSize), Color.black);
                EditorGUI.DrawRect(new Rect(previewRect.x - outRectSize, previewRect.y - outRectSize, outRectSize, previewRect.height + outRectSize * 2), Color.black);
                EditorGUI.DrawRect(new Rect(previewRect.x + previewRect.width, previewRect.y - outRectSize, outRectSize, previewRect.height + outRectSize * 2), Color.black);
                GUILayout.Space(outRectSize+20);
            }
            Vector2 V2Rotate(Vector2 v, float angleDeg)
            {
                float rad = angleDeg * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                return new Vector2(
                    cos * v.x - sin * v.y,
                    sin * v.x + cos * v.y
                );
            }
            void DrawChecker(Rect r, float size = 16f)
            {
                Color c1 = data.previewCheck1;
                Color c2 = data.previewCheck2;

                int cols = Mathf.CeilToInt(r.width / size);
                int rows = Mathf.CeilToInt(r.height / size);

                for (int y = 0; y < rows; y++)
                    for (int x = 0; x < cols; x++)
                    {
                        bool even = ((x + y) & 1) == 0;

                        float px = r.x + x * size;
                        float py = r.y + y * size;

                        float w = Mathf.Min(size, r.xMax - px);
                        float h = Mathf.Min(size, r.yMax - py);

                        EditorGUI.DrawRect(new Rect(px, py, w, h), even ? c1 : c2);
                    }
            }
            bool HitTestRotatedRect(Rect rect,Vector2 mousePos,float rotationDeg)
            {
                if (rect.width < 0)
                {
                    rect.x += rect.width;
                    rect.width = -rect.width;
                }
                if (rect.height < 0)
                {
                    rect.y += rect.height;
                    rect.height = -rect.height;
                }
                Vector2 pivot = rect.center;

                Vector2 invMouse = InverseRotate(
                    mousePos,
                    pivot,
                    rotationDeg
                );
                return rect.Contains(invMouse);
            }
            Vector2 InverseRotate(Vector2 p, Vector2 pivot, float angleDeg)
            {
                float rad = -angleDeg * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                Vector2 d = p - pivot;

                return new Vector2(
                    cos * d.x - sin * d.y,
                    sin * d.x + cos * d.y
                ) + pivot;
            }
            Vector2 RotatePoint(Vector2 p, Vector2 pivot, float angleDeg)
            {
                float rad = angleDeg * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                Vector2 d = p - pivot;

                return new Vector2(
                    cos * d.x - sin * d.y,
                    sin * d.x + cos * d.y
                ) + pivot;
            }
            void DrawRotatedRectOutline(Rect rect, float rotationDeg, Color color)
            {
                Vector2 pivot = rect.center;

                Vector2 p0 = new Vector2(rect.xMin, rect.yMin);
                Vector2 p1 = new Vector2(rect.xMax, rect.yMin);
                Vector2 p2 = new Vector2(rect.xMax, rect.yMax);
                Vector2 p3 = new Vector2(rect.xMin, rect.yMax);

                p0 = RotatePoint(p0, pivot, rotationDeg);
                p1 = RotatePoint(p1, pivot, rotationDeg);
                p2 = RotatePoint(p2, pivot, rotationDeg);
                p3 = RotatePoint(p3, pivot, rotationDeg);

                Handles.BeginGUI();
                Color prev = Handles.color;
                Handles.color = color;

                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p1, p2);
                Handles.DrawLine(p2, p3);
                Handles.DrawLine(p3, p0);

                Handles.color = prev;
                Handles.EndGUI();
            }
            // ===============================
            // 実処理（Editor専用）
            // ===============================
            public static void Generate(ImageComposeSO data)
            {
                var size = data.outputSize;

                Texture2D dst = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);

                Color[] bg = new Color[size.x * size.y];
                for (int i = 0; i < bg.Length; i++)
                    bg[i] = data.backColor;
                dst.SetPixels(bg);

                foreach (var l in data.layers.OrderBy(x => x.order))
                {
                    DrawLayer(dst, l);
                }

                dst.Apply();

                string soPath = AssetDatabase.GetAssetPath(data);
                string dir = Path.GetDirectoryName(soPath);

                File.WriteAllBytes(Path.Combine(dir, data.name + ".png"), dst.EncodeToPNG());
                AssetDatabase.Refresh();
            }
            static void DrawLayer(Texture2D dst, ImageComposeSO.Layer l)
            {
                if (l.texture == null) return;
                if (l.scale.x == 0 || l.scale.y == 0) return;
                if (l.noOutput) return;

                Texture2D srcTex = GetReadable(l.texture);
                Color[] src = srcTex.GetPixels();

                int outW = dst.width;
                int outH = dst.height;

                // ---- サイズ決定 ----
                float baseW = (l.size.x <= 0f) ? srcTex.width : l.size.x;
                float baseH = (l.size.y <= 0f) ? srcTex.height : l.size.y;

                float scaleX = l.scale.x;
                float scaleY = l.scale.y;

                float absScaleX = Mathf.Abs(scaleX);
                float absScaleY = Mathf.Abs(scaleY);

                float finalW = baseW * absScaleX;
                float finalH = baseH * absScaleY;

                float pivotLocalX = (l.pivot.x - 0.5f) * finalW;
                float pivotLocalY = (l.pivot.y - 0.5f) * finalH;

                // ---- Pivot（生成画像内）----
                float anchorX = l.anchor.x * outW;
                float anchorY = l.anchor.y * outH;

                float pivotWorldX = anchorX + l.position.x;
                float pivotWorldY = anchorY + l.position.y;

                float rad = l.rotation * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                for (int y = 0; y < dst.height; y++)
                    for (int x = 0; x < dst.width; x++)
                    {
                        // 出力 → Pivot基準
                        float dx = x - pivotWorldX;
                        float dy = y - pivotWorldY;

                        // 逆回転（Pivot基準）
                        float rx = cos * dx + sin * dy;
                        float ry = -sin * dx + cos * dy;

                        // Pivot → 画像中心基準へ
                        rx += pivotLocalX;
                        ry += pivotLocalY;

                        // サイズ外
                        if (Mathf.Abs(rx) > finalW * 0.5f || Mathf.Abs(ry) > finalH * 0.5f) continue;

                        // UV化
                        float u = (rx + finalW * 0.5f) / finalW;
                        float v = (ry + finalH * 0.5f) / finalH;

                        if (scaleX < 0f) u = 1f - u;
                        if (scaleY < 0f) v = 1f - v;

                        int sx = Mathf.FloorToInt(u * srcTex.width);
                        int sy = Mathf.FloorToInt(v * srcTex.height);

                        if ((uint)sx >= srcTex.width || (uint)sy >= srcTex.height)
                            continue;

                        Color fg = src[sy * srcTex.width + sx] * l.color;
                        if (fg.a <= 0) continue;

                        Color bg = dst.GetPixel(x, y);
                        dst.SetPixel(x, y, fg.a * fg + (1 - fg.a) * bg);
                    }
            }

        }
#endif
    }
}
