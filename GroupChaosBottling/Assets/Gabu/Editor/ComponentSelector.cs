using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BackScenePusher))]
public class ComponentSelector : Editor
{

    private SerializedProperty targetSceneProp;

    // 定数でマジックナンバーとマジックストリングを管理
    private const float DROP_AREA_HEIGHT = 50f;
    private const string DROP_AREA_TEXT = "Drag & Drop GameObject Here to Auto-Assign";
    private const string TARGET_SCENE_PROPERTY_NAME = "targetScene";

    private void OnEnable()
    {
        targetSceneProp = serializedObject.FindProperty(TARGET_SCENE_PROPERTY_NAME);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 基本設定セクション
        DrawBasicSettings();

        EditorGUILayout.Space(10);

        // ドラッグ&ドロップエリア（targetSceneのみ）
        DrawDropArea();
        EditorGUILayout.PropertyField(targetSceneProp);

        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// ドラッグ&ドロップエリアを描画し、ドロップイベントを処理する
    /// </summary>
    private void DrawDropArea()
    {
        Rect dropArea = CreateDropAreaRect();
        GUI.Box(dropArea, DROP_AREA_TEXT);

        HandleDragAndDropEvents(dropArea);
    }

    /// <summary>
    /// ドロップエリアのRectを作成
    /// </summary>
    private Rect CreateDropAreaRect()
    {
        return GUILayoutUtility.GetRect(0, DROP_AREA_HEIGHT, GUILayout.ExpandWidth(true));
    }

    /// <summary>
    /// ドラッグ&ドロップイベントを処理
    /// </summary>
    private void HandleDragAndDropEvents(Rect dropArea)
    {
        Event currentEvent = Event.current;

        if (!IsDragEvent(currentEvent) || !dropArea.Contains(currentEvent.mousePosition))
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (currentEvent.type == EventType.DragPerform)
        {
            ProcessDragPerform();
        }

        currentEvent.Use();
    }

    /// <summary>
    /// イベントがドラッグ関連かどうかを判定
    /// </summary>
    private bool IsDragEvent(Event evt)
    {
        return evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform;
    }

    /// <summary>
    /// ドラッグ完了時の処理
    /// </summary>
    private void ProcessDragPerform()
    {
        DragAndDrop.AcceptDrag();

        foreach (Object draggedObject in DragAndDrop.objectReferences)
        {
            if (TryAssignTargetFromGameObject(draggedObject))
                break; // 最初に見つかったコンポーネントで終了
        }
    }

    /// <summary>
    /// GameObjectからInstructionsSpaceAnimationコンポーネントを検索してアサイン
    /// </summary>
    /// <param name="draggedObject">ドラッグされたオブジェクト</param>
    /// <returns>アサインに成功した場合true</returns>
    private bool TryAssignTargetFromGameObject(Object draggedObject)
    {
        if (!(draggedObject is GameObject gameObject))
            return false;

        var foundComponents = gameObject.GetComponentsInChildren<InstructionsSpaceAnimation>(true);

        if (foundComponents.Length == 0)
            return false;

        targetSceneProp.objectReferenceValue = foundComponents[0];
        return true;
    }

    /// <summary>
    /// 基本設定セクションを描画
    /// </summary>
    private void DrawBasicSettings()
    {
        EditorGUILayout.LabelField("スタックシーン設定", EditorStyles.boldLabel);
    }
}
