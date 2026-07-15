using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_KeySet : MonoBehaviour
{
    [SerializeField, Tooltip("設定のタイトルテキスト")] TextMeshProUGUI title_Text;
    [SerializeField] InputActionReference actionRef;
    [SerializeField] int actionID;
    [SerializeField] TextMeshProUGUI overKeyTx;
    [SerializeField] TextMeshProUGUI baseKeyTx;
    [SerializeField] UI_OptionManager opm;
    string title_Str;
    string bkey = "";
    string path
    {
        get
        {
            return actionRef.name + "_" + actionID;
        }
    }
    public bool change
    {
        get
        {
            var efkey = actionRef.action.bindings[actionID].effectivePath;
            return efkey != bkey;
        }
    }
    private void Start()
    {
        title_Str = title_Text.text;
    }
    void Update()
    {
        overKeyTx.text = actionRef.action.bindings[actionID].effectivePath.Split("/")[1];
        baseKeyTx.text = actionRef.action.bindings[actionID].path.Split("/")[1];

        title_Text.text = UI_OptionManager.ChangeStr(title_Text.text, change);
    }
    public void StartRebind()
    {
        Debug.Log(path + "_Rebind Start");
        actionRef.action.Disable();
        actionRef.action.PerformInteractiveRebinding()
            .WithControlsExcluding(path)
            .OnComplete(op =>
            {
                op.Dispose();
                actionRef.action.Enable();
                Debug.Log(path + "_Rebind Complete");
                opm.CheckChanged();
            })
            .Start();
    }
    public void Save()
    {
        string overrideKey = actionRef.action.bindings[actionID].overridePath;
        Library_SaveFiles.SaveFile("Option", "KeyRide_" + path, overrideKey);
        bkey = actionRef.action.bindings[actionID].effectivePath;
    }
    public void Load()
    {
        var key = Library_SaveFiles.LoadFileStr("Option", "KeyRide_" + path,"");
        if(key != "")actionRef.action.ApplyBindingOverride(actionID, key);
        bkey = actionRef.action.bindings[actionID].effectivePath;
    }
    public void Chancel()
    {
        actionRef.action.ApplyBindingOverride(actionID, bkey);
    }
    public void Resets()
    {
        actionRef.action.RemoveBindingOverride(actionID);
        opm.CheckChanged();
    }
}
