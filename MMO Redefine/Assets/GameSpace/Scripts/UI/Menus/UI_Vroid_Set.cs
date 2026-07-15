namespace UIs
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using VRoidSDK.Examples.Core.Localize;
    using VRoidSDK.Examples.CharacterModelExample;
    using static GmSystem.GS_VroidDictionary;
    public class UI_Vroid_Set : CharacterModelExampleEventHandler
    {
        static UI_Vroid_Set Set;
        public UI_CharaBase CharaUI;
        [SerializeField] Routes Route;
        private void Start()
        {
            Set = this;
        }
        public override void OnModelLoaded(string modelId, GameObject go)
        {
            CharaUI.LChara.ModelVrm = modelId;
            VroidAdds(modelId, go);
        }

        public override void OnLangChanged(Translator.Locales locale)
        {
        }
        static public void VroidSelects(UI_CharaBase CharaUI)
        {
            Set.CharaUI = CharaUI;
            Set.Route.ShowCharacterModels();
        }
    }
}

