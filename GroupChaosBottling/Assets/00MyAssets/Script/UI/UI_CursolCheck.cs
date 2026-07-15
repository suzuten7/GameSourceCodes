using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_CursolCheck : MonoBehaviour
{
    [SerializeField] Image TestImage;
    [SerializeField] bool UseTest;
    public bool Check;
    bool LChecks;
    private void Update()
    {
        Check = LChecks || !SoftwareCursorPositionAdjuster.Vis;
        var TCol = Check ? Color.white : Color.red;
        if (UseTest) TCol.a = 0.15f;
        else TCol.a = 0.0f;
        if (TestImage.color != TCol) TestImage.color = TCol;
    }
    public void Sets(bool B)
    {
        LChecks = B;
    }
}
