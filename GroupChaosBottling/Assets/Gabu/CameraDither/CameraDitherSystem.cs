using UnityEngine;

public class CameraDitherSystem : MonoBehaviour
{
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Transform _target;
    [SerializeField] float Hight;
    [SerializeField]
    private string _cameraPositionValue = "_PointA";

    private void Start()
    {

    }

    void Update()
    {
        Debug.Log($"やってるよ");
        _material.SetVector(_cameraPositionValue, _target.position + Vector3.up * Hight);
    }
}
