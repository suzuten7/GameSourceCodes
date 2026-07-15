using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_HeikinSets : MonoBehaviour
{
    [SerializeField] Suzuten_CharaData CD;
    [SerializeField] Suzuten_DataBase DB;
    [SerializeField] bool On;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (On && CD != null && DB != null)
        {
            On = false;
            float[] Values = new float[19];
            int Counts = 0;
            for (int i = 0; i < DB.Charas.Length; i++)
            {
                var Chara = DB.Charas[i];
                if (Chara != CD && !Chara.UseBandChara)
                {
                    Counts++;
                    Values[0] += Chara.MHP;
                    Values[1] += Chara.HPRegene;
                    Values[2] += Chara.StanRegist;
                    Values[3] += Chara.STRegene;
                    Values[4] += Chara.MSP;
                    Values[5] += Chara.SPRegene;
                    Values[6] += Chara.StartSPPer;
                    Values[7] += Chara.DamageSPPer;
                    Values[8] += Chara.Atk;
                    Values[9] += Chara.MMP;
                    Values[10] += Chara.MPRegene;
                    Values[11] += Chara.PhisPow;
                    Values[12] += Chara.PhisRange;
                    Values[13] += Chara.PhisStanTime;
                    Values[14] += Chara.GroundSpeed;
                    Values[15] += Chara.AirSpeed;
                    Values[16] += Chara.BoostSpeed;
                    Values[17] += Chara.JumpPower;
                    Values[18] += Chara.DownPower;
                }
            }
            for(int i = 0; i < Values.Length; i++)Values[i] = Mathf.Floor(Values[i] / Counts);
            CD.MHP = (int)Values[0];
            CD.HPRegene = (int)Values[1];
            CD.StanRegist = (int)Values[2];
            CD.STRegene = (int)Values[3];
            CD.MSP = (int)Values[4];
            CD.SPRegene = (int)Values[5];
            CD.StartSPPer = (int)Values[6];
            CD.DamageSPPer = (int)Values[7];
            CD.Atk = (int)Values[8];
            CD.MMP = (int)Values[9];
            CD.MPRegene = (int)Values[10];
            CD.PhisPow = (int)Values[11];
            CD.PhisRange = (int)Values[12];
            CD.PhisStanTime = (int)Values[13];
            CD.GroundSpeed = (int)Values[14];
            CD.AirSpeed = (int)Values[15];
            CD.BoostSpeed = (int)Values[16];
            CD.JumpPower = (int)Values[17];
            CD.DownPower = (int)Values[18];

        }
    }
#endif
}
