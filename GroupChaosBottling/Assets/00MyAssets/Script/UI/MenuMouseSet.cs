using UnityEngine;
using static SoftwareCursorPositionAdjuster;
public class UI_MenuMouseSet : MonoBehaviour
{
    public void MenuSet(bool Set)
    {
        MouseSets.Menus = Set;
    }
}
