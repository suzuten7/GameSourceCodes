using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCharaInStart_Gabu : MonoBehaviour
{
    [SerializeField] private Suzuten_DataBase _suzutenDB;
    [SerializeField] private int _instantiateQuantity;
    private List<GameObject> _gameObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _instantiateQuantity; i++)
        {
            Instantiate(_suzutenDB.Charas[_instantiateQuantity]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
