using UnityEngine;
[CreateAssetMenu(menuName ="DataCre/DColor")] 
public class DataColors : ScriptableObject
{
    static DataColors DCo;
    static public DataColors DCol
    {
        get
        {
            if (DCo == null)
            {
                if (DCo == null) DCo = (DataColors)Resources.Load("DataColor");
            }
            return DCo;
        }
    }
    [SerializeField] Color NSelectCol;
    [SerializeField] Color SelectCol;
    public Color ColSelects(bool Sels)
    {
        return Sels ? SelectCol : NSelectCol;
    }
    public Color F_HP_Base;
    public Color E_HP_Base;
    public Color HP_DBuf;
    public Color HP_Rem;
    public Color HP_Add;
    public Color Shild_Base;
    public Color Shild_Rem;
    public Color Shild_Add;
    public Color Break_Base;
    public Color Break_Add;

    public Color MP_Base;
    public Color MP_Low;
    public Color MP_Neg;
}
