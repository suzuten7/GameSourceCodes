using UnityEngine;

public class DataSetUp : MonoBehaviour
{
    [SerializeField] Data_Base db;
    private void Awake()
    {
        Start();
    }
    void Start()
    {
        Data_Base.DB = db;
    }
}
