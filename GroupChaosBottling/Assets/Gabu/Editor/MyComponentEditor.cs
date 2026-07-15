using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputIntefaceSystem_Gabu))]
public class MyComponentEditor : Editor
{
    private SerializedProperty targetSceneProp;
    private SerializedProperty actionNameProp;
    private SerializedProperty isLastProp;
    private SerializedProperty playerInputProp;
    private SerializedProperty onStartedClickProp;
    private SerializedProperty onPreformedClickProp;
    private SerializedProperty onCanceledClickProp;

    // 定数でマジックナンバーとマジックストリングを管理
    private const float DROP_AREA_HEIGHT = 50f;
    private const string DROP_AREA_TEXT = "Drag & Drop GameObject Here to Auto-Assign";
    private const string TARGET_SCENE_PROPERTY_NAME = "targetScene";

    private void OnEnable()
    {
        targetSceneProp = serializedObject.FindProperty(TARGET_SCENE_PROPERTY_NAME);
        actionNameProp = serializedObject.FindProperty("actionName");
        isLastProp = serializedObject.FindProperty("isLast");
        playerInputProp = serializedObject.FindProperty("playerInput");
        onStartedClickProp = serializedObject.FindProperty("m_OnStardedClick");
        onPreformedClickProp = serializedObject.FindProperty("m_OnPreformedClick");
        onCanceledClickProp = serializedObject.FindProperty("m_OnCanceledClick");
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

        // イベント設定セクション
        DrawEventSettings();

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
        EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(actionNameProp);
        EditorGUILayout.PropertyField(isLastProp);
        EditorGUILayout.PropertyField(playerInputProp);
    }

    /// <summary>
    /// イベント設定セクションを描画
    /// </summary>
    private void DrawEventSettings()
    {
        EditorGUILayout.LabelField("イベント設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onStartedClickProp, new GUIContent("Started Event"));
        EditorGUILayout.PropertyField(onPreformedClickProp, new GUIContent("Performed Event"));
        EditorGUILayout.PropertyField(onCanceledClickProp, new GUIContent("Canceled Event"));
    }
}