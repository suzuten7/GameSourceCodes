using UnityEngine;
using static BattleManager;
public class State_Hit : MonoBehaviour
{
    public State_Base Sta;
    [Tooltip("ダメージ増加%x=近距離,y=遠距離")]public Vector2 DamAdds;
    [Tooltip("ブレイク増加%x=近距離,y=遠距離")] public Vector2 BreakAdds;

    private void Start()
    {
        BTManager.HitList.Add(this);
    }
    public Vector3 PosGet()
    {
        return transform.position;
    }
}
