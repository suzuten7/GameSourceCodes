using UnityEngine;
[CreateAssetMenu(menuName ="QualityData/Terrain")]
public class Quality_TerrainData : ScriptableObject
{
    [EnumIndex(typeof(Quality_Name.Enum_QName))]
    public Class_TerrainSetting[] TerrainSettings;
    [System.Serializable]
    public class Class_TerrainSetting
    {
        [Tooltip("草密度%")]public float Grass_Density;
        [Tooltip("草描画距離")] public float Grass_Distance;
    }
}
