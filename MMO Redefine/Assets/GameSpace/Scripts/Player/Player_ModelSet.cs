
namespace Player
{
    using State;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using UniVRM10;
    using static Datas.Data_Get;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_VroidDictionary;


    public class Player_ModelSet : MonoBehaviour
    {
        [SerializeField] Player_State PInfo;
        [SerializeField] Transform ModelTrans;
        [SerializeField] GameObject GameModel;
        [SerializeField] Vrm10Instance ModelVroid;
        [SerializeField] State_2DModel Model2D;
        [SerializeField] GameObject[] NearHides;
        byte _modelMode = 0;
        int _setModelID = -1;

        string _setVroidID = "";
        bool _vrmloads = false;
        bool _bnhide = false;
        public GameObject ModelGet
        {
            get
            {
                switch (_modelMode)
                {
                    default:return GameModel;
                    case 1:return ModelVroid != null ? ModelVroid.gameObject : GameModel;
                    case 2:return Model2D.gameObject;
                }
            }
        }
        void LateUpdate()
        {
            var nhide = CanControl(PInfo.Object) && PInfo.BotID < 0 && GetSave_Option.Cam_Pos.z <= 50;
            if (_bnhide != nhide)
            {
                _bnhide = nhide;
                foreach (var hides in NearHides)
                {
                    ChangeActive(hides, !nhide);
                }
            }     
            _modelMode = 0;
            switch(PInfo.PlayerValues.ModelMode)
            {
                case 1:
                    _modelMode = 1;
                    break;
                case 2:
                    _modelMode = 2;
                    break;
            }
            switch(_modelMode)
            {
                case 1:
                    if (_setVroidID != PInfo.PlayerValues.ModelExID && !_vrmloads)
                    {
                        _setVroidID = PInfo.PlayerValues.ModelExID;
                        _vrmloads = true;
                        VroidIDLoad(PInfo.PlayerValues.ModelExID, (objVrm) =>
                        {
                            if (ModelVroid != null) Destroy(ModelVroid.gameObject);
                            ModelVroid = objVrm.GetComponent<Vrm10Instance>();
                            ModelVroid.transform.parent = ModelTrans;
                            ModelVroid.transform.localPosition = Vector3.zero;
                            ModelVroid.transform.localRotation = Quaternion.identity;
                            var HumanCont = ModelVroid.AddComponent<State_HumanAnim>();
                            HumanCont.Sta = PInfo;
                            HumanCont.Anim = ModelVroid.GetComponent<Animator>();
                        }, (b) => { _vrmloads = false; });
                    }
                    if (ModelVroid == null) _modelMode = 0;
                    break;
                case 2:
                    Model2D.Imgs.Clear();
                    var model2d = PInfo.PlayerValues.Model2DSet;
                    for (int i = 0; i < model2d.datas.Count; i++)
                    {
                        var imgd = model2d.datas[i];
                        var id = -1;
                        for (int k = 0; k < Model2D.Imgs.Count; k++)
                        {
                            var ianim = Model2D.Imgs[k];
                            if (ianim.type != imgd.type) continue;
                            if (ianim.back != imgd.back) continue;
                            id = k;
                            break;
                        }
                        if (id < 0)
                        {
                            Model2D.Imgs.Add
                                (
                                new State_2DModel.Class_Img
                                {
                                    type = imgd.type,
                                    back = imgd.back,
                                    paturnSpeed = imgd.type < model2d.speeds.Count ? model2d.speeds[imgd.type] : 1,
                                    imgs = new(),
                                }
                                );
                            id = Model2D.Imgs.Count - 1;
                        }
                        Model2D.Imgs[id].imgs.Add(imgd.TextureGet);
                    }
                    if (Model2D.Imgs.Count <= 0) _modelMode = 0;
                    break;
            }

            if (_modelMode == 0 && _setModelID != PInfo.PlayerValues.ModelID)
            {
                _setModelID = PInfo.PlayerValues.ModelID;
                if (GameModel != null) Destroy(GameModel);
                GameModel = Instantiate(DB.Models[_setModelID].Model, transform.position, transform.rotation);
                bool noDown = false;
                if (GameModel.TryGetComponent<State_HumanAnim>(out var hanim))hanim.Sta = PInfo;
                if (GameModel.TryGetComponent<State_OtherAnim>(out var oanim))
                {
                    oanim.Sta = PInfo;
                    noDown = true;
                }
                if (GameModel.TryGetComponent<State_2DModel>(out var model2d))
                {
                    model2d.Sta = PInfo;
                    noDown = true;
                }
                if (noDown) GameModel.transform.parent = transform;
                else GameModel.transform.parent = ModelTrans;

            }

            if (GameModel != null)ChangeActive(GameModel,_modelMode == 0);
            if (ModelVroid != null)
            {
                ChangeActive(ModelVroid.gameObject, _modelMode == 1);
                ModelVroid.transform.localScale = Vector3.one * PInfo.PlayerValues.ModelExScale.x * 0.01f;
            }
            ChangeActive(Model2D.gameObject, _modelMode == 2);
            Model2D.transform.localScale = new Vector3(PInfo.PlayerValues.ModelExScale.x * 0.01f, PInfo.PlayerValues.ModelExScale.y * 0.01f, 1);
        }
        public Texture PlayerIconGet(out string Name)
        {
            var tx = DB.Models[PInfo.PlayerValues.ModelID].Icon;
            Name = DB.Models[PInfo.PlayerValues.ModelID].Name;

            switch (_modelMode)
            {
                case 1:
                    tx = ModelVroid.Vrm.Meta.Thumbnail;
                    Name = ModelVroid.Vrm.Meta.Name;
                    break;
                case 2:
                    tx = PInfo.PlayerValues.Model2DSet.IconGet;
                    Name = PInfo.PlayerValues.Model2DSet.name;
                    break;
            }
            switch (_modelMode)
            {
                default:
                    Name += "\n{Game}";
                    break;
                case 1:
                    Name += "\n{Vroid}";
                    break;
                case 2:
                    Name += "\n{2DImg}";
                    break;
            }
            return tx;
        }
    }
}
