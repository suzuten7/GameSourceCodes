using UnityEngine;

public class Quality_TerrainSet : MonoBehaviour
{
    [SerializeField] Terrain Terraind;
    [SerializeField] Quality_TerrainData TerrainQD;

    int BackQLV = -1;
    void OnValidate()
    {
        SettingChange();
    }
    void Update()
    {
        if(BackQLV != QualitySettings.GetQualityLevel())
        {
            BackQLV = QualitySettings.GetQualityLevel();
            SettingChange();
        }
    }

    void SettingChange()
    {
        var QLV = QualitySettings.GetQualityLevel();
        if (Terraind == null) return;
        if (TerrainQD == null) return;
        if (QLV > TerrainQD.TerrainSettings.Length) return;
        var TQSet = TerrainQD.TerrainSettings[QLV];
        Terraind.detailObjectDensity = TQSet.Grass_Density*0.01f;
        Terraind.detailObjectDistance = TQSet.Grass_Distance;
    }
}
