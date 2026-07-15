using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class SpotMask : MonoBehaviour
{
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public int rayCount = 100;
    public float addDis;
    public LayerMask wallLayer;
    public bool useViewMask;
    Mesh mesh;

    private void OnValidate()
    {
        GenerateFOV();
    }

    void LateUpdate()
    {
        GenerateFOV();
    }
    private void Start()
    {
        if(useViewMask) Obj_LocalObjects.ViewMasks.Add(this);
    }
    void GenerateFOV()
    {
        float startAngle = -viewAngle / 2f;
        float angleStep = viewAngle / rayCount;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero);

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + angleStep * i;

            // ローカル方向
            Vector3 localDir = DirFromAngle(angle);

            // ワールド方向に変換
            Vector3 worldDir = transform.TransformDirection(localDir);

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                worldDir,
                viewDistance,
                wallLayer
            );

            Vector3 point;

            if (hit.collider != null)
            {
                point = transform.InverseTransformPoint(hit.point);
                point += point.normalized * addDis;
            }
            else
            {
                point = localDir * viewDistance;
            }
            vertices.Add(point);
            if (i > 0)
            {
                int baseIndex = vertices.Count - 1;

                triangles.Add(0);
                triangles.Add(baseIndex - 1);
                triangles.Add(baseIndex);
            }
        }

        if (mesh == null)
            mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    Vector3 DirFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
