namespace Player
{

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using Obj;
    public class Player_Waterd : MonoBehaviour
    {
        public Volume WaterVol;
        public float ChangeSpeed = 5f;
        public List<Obj_Water> Waters;
        private void FixedUpdate()
        {
            var check = false;
            for (int i = Waters.Count - 1; i >= 0; i--)
            {
                if (Waters[i] == null)
                {
                    Waters.RemoveAt(i);
                    continue;
                }
                check = true;
            }
            var wei = WaterVol.weight;
            if (check) wei += ChangeSpeed * 0.01f;
            else wei -= ChangeSpeed * 0.01f;
            WaterVol.weight = Mathf.Clamp01(wei);
        }
        private void OnTriggerEnter(Collider other)
        {
            var wat = other.GetComponent<Obj_Water>();
            if (wat != null) Waters.Add(wat);
        }
        private void OnTriggerExit(Collider other)
        {
            var wat = other.GetComponent<Obj_Water>();
            if (wat != null) Waters.Remove(wat);
        }
    }
}
