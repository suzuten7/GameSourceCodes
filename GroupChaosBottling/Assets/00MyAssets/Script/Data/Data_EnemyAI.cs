using UnityEngine;
using static Manifesto;
[CreateAssetMenu(menuName ="DataCre/EnemyAI")]
public class Data_EnemyAI : ScriptableObject
{
    public Class_Enemy_AtkAI[] AtkAIs;
    [Tooltip("行動周期f")]public int TimerLim;
    [Tooltip("非ターゲット時\n行動周期リセット")] public bool NoTargetResetTime;
    private void OnValidate()
    {
        for (int i = 0; i < AtkAIs.Length; i++)
        {
            var AI = AtkAIs[i];
            AI.EditDisp = "[" + i + "]";
            AI.EditDisp += "Time(" + AI.TimeIf.x + "～" + AI.TimeIf.y + ")";
            AI.EditDisp += ",OIfCount:" + AI.OtherIfs.Length;
            AI.EditDisp += ",AtkSlot:" + AI.AtkSlot;
            AI.EditDisp += ",AtkName:" + (AI.AtkD != null ? AI.AtkD.Name : "無");
        }
    }
}
