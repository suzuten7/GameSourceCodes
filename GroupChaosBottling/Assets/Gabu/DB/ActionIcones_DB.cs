using UnityEngine;

[CreateAssetMenu(fileName = "ActionIcones_DB", menuName = "Gabu/DB/ActionIcones_DB")]
public class ActionIcones_DB : ScriptableObject
{
    public int default_index = 0;
    public int play_index = 9;
    public int ui_index = 13;

    [Header("0:マウスカーソル、1:xbox、2:PlayStation、3:その他")]
    public ActionIcone[] Menu_icons = new ActionIcone[4];
    public ActionIcone[] Move_icons = new ActionIcone[4];
    public ActionIcone[] Dash_icons = new ActionIcone[4];
    public ActionIcone[] Jump_icons = new ActionIcone[4];
    public ActionIcone[] Target_icons = new ActionIcone[4];
    public ActionIcone[] Chenge_icons = new ActionIcone[4];
    public ActionIcone[] N_Atk_icons = new ActionIcone[4];
    public ActionIcone[] S1_Atk_icons = new ActionIcone[4];
    public ActionIcone[] S2_Atk_icons = new ActionIcone[4];
    public ActionIcone[] E_Atk_icons = new ActionIcone[4];
    public ActionIcone[] Look_icons = new ActionIcone[4];
    public ActionIcone[] UIMove_icons = new ActionIcone[4];
    public ActionIcone[] UICofirm_icons = new ActionIcone[4];
    public ActionIcone[] UIBack_icons = new ActionIcone[4];
    public ActionIcone[] VMouseMove_icons = new ActionIcone[4];
}

