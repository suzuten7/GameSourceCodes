using UnityEngine;
using static PlayerValue;
public class UI_SizeChange : MonoBehaviour
{
    [SerializeField] GameObject BuUI;
    [SerializeField] RectTransform AtkUI;
    [SerializeField] RectTransform MoveStick;
    [SerializeField] RectTransform JumpDashUI;
    float AtkUI_Width = -1f;
    float JumpDashUI_Width = -1f;
    int BAtkUISize = -1;
    int BMoveStickSize = -1;
    int BJumpDashUISize = -1;
    private void Start()
    {
        AtkUI_Width = AtkUI.sizeDelta.x;
        JumpDashUI_Width = JumpDashUI.sizeDelta.x;
    }
    void Update()
    {
        if(BAtkUISize != PSaves.AtkUISize)
        {
            BAtkUISize = PSaves.AtkUISize;
            AtkUI.localScale = Vector3.one * PSaves.AtkUISize / 100f;
            var AtkSize = AtkUI.sizeDelta;
            AtkSize.x = AtkUI_Width * PSaves.AtkUISize / 100f;
            AtkUI.sizeDelta = AtkSize;
            UIChange();
        }
        if (BMoveStickSize != PSaves.MoveStickSize)
        {
            BMoveStickSize = PSaves.MoveStickSize;
            MoveStick.localScale = Vector3.one * PSaves.MoveStickSize / 100f;
            UIChange();
        }
        if (BJumpDashUISize != PSaves.JumpDashUISize)
        {
            BJumpDashUISize = PSaves.JumpDashUISize;
            JumpDashUI.localScale = Vector3.one * PSaves.JumpDashUISize / 100f;
            var JumpDashSize = JumpDashUI.sizeDelta;
            JumpDashSize.x = JumpDashUI_Width * PSaves.JumpDashUISize / 100f;
            JumpDashUI.sizeDelta = JumpDashSize;
            UIChange();
        }
    }
    private void UIChange()
    {
        BuUI.gameObject.SetActive(false);
        BuUI.gameObject.SetActive(true);
    }
}
