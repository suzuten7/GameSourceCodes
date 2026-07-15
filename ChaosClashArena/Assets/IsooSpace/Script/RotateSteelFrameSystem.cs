using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSteelFrameSystem_Gabu : MonoBehaviour
{
    [SerializeField] private float speed = 1000f;
    [SerializeField] private Vector3 vector;
    [SerializeField] private Rigidbody rigidBody;

    private void Start()
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        if (rigidBody == null)
        {
            return;
        }

        vector = new Vector3(Random.Range(0f, vector.x),
            Random.Range(0f, vector.y), Random.Range(0f, vector.z));

        //rigidBody.AddTorque(vector * speed);
    }

    void Update()
    {
        Vector3 newVector = vector * speed * Time.time;
        transform.rotation = Quaternion.Euler(newVector);
    }
}
