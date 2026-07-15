using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReverseCollider_Gabu : MonoBehaviour
{

    public bool removeExistingColliders = true;
    public bool isConvex = false;

    private void Start()
    {
        CreateInvertedMeshCollider();
        MeshCollider mc = GetComponent<MeshCollider>();
        if (isConvex)
        {

            mc.convex = true;
            mc.isTrigger = true;
        }
    }

    public void CreateInvertedMeshCollider()
    {
        if (removeExistingColliders)
            RemoveExistingColliders();

        InvertMesh();

        gameObject.AddComponent<MeshCollider>();
    }

    private void RemoveExistingColliders()
    {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);
    }

    private void InvertMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}