using UnityEngine;

public class Ult_SpriteMaterialSet : MonoBehaviour
{
    [SerializeField] Ult_Base ultb;
    [SerializeField] SpriteRenderer[] srs;
    [SerializeField] Material myTeam;
    [SerializeField] Material otherTeam;
    void Update()
    {
        var view = ultb.pm == null;
        if (!view && Obj_LocalObjects.MyPlayer == null) view = true;
        if (!view && ultb.pm.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID) view = true;
        for (int i = 0; i < srs.Length; i++)
        {
            if (srs[i] != null) srs[i].material = view ? myTeam : otherTeam;
        }
    }
}
