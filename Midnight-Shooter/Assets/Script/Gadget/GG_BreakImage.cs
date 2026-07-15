using UnityEngine;

public class GG_BreakImage : MonoBehaviour
{
    [SerializeField] GG_Base ggb;
    [SerializeField] SpriteRenderer sr;
    void Update()
    {
        float now = ggb.now_HP;
        float max = Data_Base.DB.gadgets[ggb.gid].max_HP;
        sr.material.SetFloat("_Breaks", now / max);
    }
}
