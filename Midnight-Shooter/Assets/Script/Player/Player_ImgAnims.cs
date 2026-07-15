using System.Collections.Generic;
using UnityEngine;

public class Player_ImgAnims : MonoBehaviour
{
    public SpriteRenderer sr;
    public List<imgAnim> imgAnims;
    public int moveAnimID;
    public int atkAnimID;
    public bool death;
    [SerializeField] int pi;
    [SerializeField] Texture tx;
    public enum AnimType
    {
        Stay,
        MoveAny,
        Walk,
        NMove,
        Run,

        AtkAny,
        GunAtk,
        MelleAtk,
        GadgetAtk,
        UltAtk,

        Death,
    }
    [System.Serializable]
    public class imgAnim
    {
        public AnimType animType;
        public float paturnSpeed;
        public List<Texture> imgs;
    }
    void Update()
    {
        imgAnim cAnim = null;
        for (int i = 0; i < imgAnims.Count; i++)
        {
            var anim = imgAnims[i];
            var actives = false;
            if (anim.animType == AnimType.Stay && moveAnimID <= 0) actives = true;
            if (anim.animType == AnimType.MoveAny && moveAnimID > 0) actives = true;
            if (anim.animType == AnimType.AtkAny && atkAnimID > 0) actives = true;
            if (anim.animType == AnimType.Death && death) actives = true;
            if (anim.animType > AnimType.MoveAny && anim.animType < AnimType.AtkAny)
            {
                if ((int)anim.animType - (int)AnimType.MoveAny == moveAnimID) actives = true;
            }
            if (anim.animType > AnimType.AtkAny && anim.animType < AnimType.Death)
            {
                if ((int)anim.animType - (int)AnimType.AtkAny == atkAnimID) actives = true;
            }
            if (actives) cAnim = anim;
        }
        if (cAnim == null && imgAnims.Count > 0) cAnim = imgAnims[0];
        if (cAnim == null) return;
        var pid = (int)Mathf.Repeat(Time.time * cAnim.paturnSpeed, cAnim.imgs.Count);
        var img = cAnim.imgs[pid];
        pi = pid;
        tx = img;
        sr.material.SetTexture("_Texture",img);
    }
}
