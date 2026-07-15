
namespace UIs
{
    using System.Collections.Generic;
    using UnityEngine;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    using static UIs.UI_System;
    public class UI_Image2Ds_Make : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] UI_2DSet_Base Set2DUI;
        [SerializeField] List<UI_Image2Ds_Single> SingleUIs;
        [SerializeField] Transform AddButton;
        public void LateUpdate()
        {
            for(int i = 0; i < Mathf.Max(GetSave_2DImages.Count, SingleUIs.Count); i++)
            {
                if (i >= SingleUIs.Count)
                {
                    SingleUIs.Add(Instantiate(SingleUIs[0], SingleUIs[0].transform.parent));
                    if (AddButton != null) AddButton.SetAsLastSibling();
                }
                var sui = SingleUIs[i];
                if (i >= GetSave_2DImages.Count)
                {
                    ChangeActive(sui.gameObject, false);
                    continue;
                }
                ChangeActive(sui.gameObject, true);
                var img2d = GetSave_2DImages[i];
                sui.ID = i;
                ChangeText(sui.NameTx,img2d.name);
                var sid =0;
                if (CharaUI != null)
                {
                    sid = CharaUI.LChara.Model2DID;
                }
                if (Set2DUI != null)
                {
                    sid = Set2DUI.selID;
                }
                ChangeColor(sui.SelUI,OnColors(i == sid));
                ChangeTexture(sui.Images, img2d.IconGet,true);
            }
        }
        public void Add()
        {
            GetSave_2DImages.Add(new Class_Save_2DImageBase {name = ("新規" + GetSave_2DImages.Count),datas = new List<Class_Save_2DImageData>(1)});
        }
        public void Select(int ID)
        {
            if(CharaUI != null)
            {
                CharaUI.LChara.Model2DID = ID;
            }
            if(Set2DUI != null)
            {
                Set2DUI.selID = ID;
            }
        }
        public void ChangeSet(int s, bool back)
        {
            var ci = Mathf.Clamp(s + (!back ? 1 : -1), 0, GetSave_2DImages.Count - 1);
            var bset = GetSave_2DImages[ci];
            GetSave_2DImages[ci] = GetSave_2DImages[s];
            GetSave_2DImages[s] = bset;
            Save();
        }
    }
}

