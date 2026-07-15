using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialSettings))]
public class ImagePreviewerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // デフォルトのインスペクター描画
        DrawDefaultInspector();

        TutorialSettings settings = (TutorialSettings)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Icon Preview", EditorStyles.boldLabel);

        // アイコンの表示と差し替え
        Rect iconRect = PreviwImage(settings.icon, "Icon");
        ChangeImage(iconRect, ref settings.icon, 0);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Images Array Preview", EditorStyles.boldLabel);

        // 配列が null なら初期化
        if (settings.images == null)
            settings.images = new Texture[0];

        // 配列サイズ調整
        int newSize = EditorGUILayout.IntField("Image Count", settings.images.Length);
        if (newSize != settings.images.Length)
        {
            System.Array.Resize(ref settings.images, newSize);
            EditorUtility.SetDirty(settings);
        }

        // 各画像を表示・変更
        for (int i = 0; i < settings.images.Length; i++)
        {
            Texture tex = settings.images[i];
            Rect imageRect = PreviwImage(tex, $"Image {i}");
            ChangeImage(imageRect, ref settings.images[i], 1000 + i); // 固有の control ID
        }
    }

    private Rect PreviwImage(Texture image, string label)
    {
        if (image == null)
        {
            EditorGUILayout.LabelField(label, "(No Image)");
            return GUILayoutUtility.GetRect(0, 0);
        }

        GUILayout.Label(label, EditorStyles.boldLabel);

        float maxPreviewSize = 256;
        float aspect = (float)image.width / image.height;

        float width = Mathf.Min(image.width, maxPreviewSize);
        float height = width / aspect;

        Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));

        // ★ここを変更：確実に表示するため GUI.DrawTexture を使用
        GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);

        return rect;
    }

    private void ChangeImage(Rect rect, ref Texture texture, int controlId)
    {
        // 画像表示は PreviwImage() 側に任せる

        // 完全に透明なボタンを置いてクリックだけ取得（見た目なし）
        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
        {
            EditorGUIUtility.ShowObjectPicker<Texture2D>(texture, false, "", controlId);
        }

        if (Event.current.commandName == "ObjectSelectorUpdated" &&
            EditorGUIUtility.GetObjectPickerControlID() == controlId)
        {
            texture = (Texture2D)EditorGUIUtility.GetObjectPickerObject();
            EditorUtility.SetDirty(target);
            Repaint();
        }
    }

}

//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(TutorialSettings))]
//public class ImagePreviewerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        // デフォルトのインスペクター描画
//        DrawDefaultInspector();

//        // 対象のオブジェクト取得
//        TutorialSettings previewer = (TutorialSettings)target;
//        Rect iconRect = PreviwImage(previewer.icon, "icon");
//        Rect imageRect = PreviwImage(previewer.images, "image");

//        ChangeImage(iconRect, ref previewer.icon, 0);
//        ChangeImage(imageRect, ref previewer.images, 1);

//    }

//    private Rect PreviwImage(Texture image, string label)
//    {
//        // 画像が設定されている場合に表示
//        if (image == null)
//        {
//            return new Rect();
//        }
//        GUILayout.Label(label, EditorStyles.boldLabel);

//        // アスペクト比を維持して描画（最大256x256）
//        float maxPreviewSize = 256;
//        float aspect = (float)image.width / image.height;

//        float width = Mathf.Min(image.width, maxPreviewSize);
//        float height = width / aspect;

//        Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
//        EditorGUI.DrawTextureTransparent(rect, image);
//        return rect;
//    }

//    private bool OpenSerect(Rect rect, Texture texture, int controlId)
//    {
//        // クリックでObject Pickerを開く
//        if (GUI.Button(rect, texture))
//        {
//            EditorGUIUtility.ShowObjectPicker<Texture2D>(texture, false, "", controlId);
//            return true;
//        }
//        return false;
//    }

//    private void ChangeImage(Rect rect, ref Texture texture, int controlId)
//    {

//        // クリックでObject Pickerを開く
//        if (GUI.Button(rect, texture))
//        {
//            EditorGUIUtility.ShowObjectPicker<Texture2D>(texture, false, "", controlId);
//        }

//        // Object Pickerの選択が更新された時にのみ
//        if (Event.current.commandName == "ObjectSelectorUpdated" &&
//            EditorGUIUtility.GetObjectPickerControlID() == controlId)
//        {
//            texture = (Texture2D)EditorGUIUtility.GetObjectPickerObject();
//            EditorUtility.SetDirty(texture);
//            Repaint(); // エディタ更新
//        }
//    }
//}
