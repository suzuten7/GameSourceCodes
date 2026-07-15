namespace State
{
    using UnityEngine;
    public class State_Wepon2D : MonoBehaviour
    {
        public Texture Imgs;
        Texture bimg;
        [SerializeField] MeshRenderer[] WepMeshs;
        private void LateUpdate()
        {
            if (bimg == Imgs) return;
            bimg = Imgs;
            for (int i = 0; i < WepMeshs.Length; i++)
            {
                WepMeshs[i].material.SetTexture("_BaseMap", Imgs);
            }
        }
    }
}

