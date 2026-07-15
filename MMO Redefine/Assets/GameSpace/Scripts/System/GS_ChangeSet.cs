namespace GmSystem
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GS_ChangeSet
    {
        static public void ChangeActive(GameObject obj,bool on)
        {
            if(obj.activeSelf != on)obj.SetActive(on);
        }
        static public void ChangeText(TextMeshProUGUI tx,string str)
        {
            if(tx.text !=str)tx.text = str;
        }
        static public void ChangeColor(TextMeshProUGUI tx, Color col)
        {
            if (tx.color != col) tx.color= col;
        }
        static public void ChangeText(TMP_InputField txin, string str,bool forecCheck)
        {
            if (forecCheck && txin.isFocused) return;
            if (txin.text != str) txin.text = str;
        }
        static public void ChangeValue(TMP_Dropdown dr,int i)
        {
            if(dr.value != i)dr.value = i;
        }

        static public void ChangeTexture(RawImage im, Texture tx,bool nullClear)
        {
            if (im.texture != tx) im.texture = tx;
            if (nullClear)
            {
                if(tx != null && im.color != Color.white)im.color = Color.white;
                if(tx == null && im.color != Color.clear) im.color = Color.clear;
            }
        }
        static public void ChangeColor(RawImage im, Color col)
        {
            if (im.color != col) im.color = col;
        }
        static public void ChangeSprite(Image im, Sprite sp)
        {
            if (im.sprite != sp) im.sprite = sp;
        }
        static public void ChangeColor(Image im, Color col)
        {
            if (im.color != col) im.color = col;
        }
        static public void ChangeFill(Image im,float v)
        {
            if(im.fillAmount != v)im.fillAmount = v;
        }
        static public void ChangeOn(Toggle to, bool on)
        {
            if (to.isOn != on) to.isOn = on;
        }
        static public void ChangeValue(Slider bar,float v)
        {
            if (bar.value != v) bar.value = v;
        }
        static public void ChangeMax(Slider bar, float v)
        {
            if (bar.maxValue != v) bar.maxValue = v;
        }
        static public void ChangeMin(Slider bar, float v)
        {
            if (bar.minValue != v) bar.minValue = v;
        }
        static public void ChangeValue(Scrollbar bar, float v)
        {
            if (bar.value != v) bar.value = v;
        }
        static public void ChangeSize(Scrollbar bar, float v)
        {
            if (bar.size != v) bar.size = v;
        }
        static public void ChangeAnchorMin(RectTransform rt,Vector2 anc)
        {
            if (rt.anchorMin != anc) rt.anchorMin = anc;
        }
        static public void ChangeAnchorMax(RectTransform rt, Vector2 anc)
        {
            if (rt.anchorMax != anc) rt.anchorMax = anc;
        }


    }
}

