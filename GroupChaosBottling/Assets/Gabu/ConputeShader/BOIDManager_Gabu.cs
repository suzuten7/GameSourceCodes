using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Random = Unity.Mathematics.Random;

[RequireComponent(typeof(VisualEffect))]
public class BOIDManager_Gabu : MonoBehaviour
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct BoidState
    {
        public Vector3 Position;
        public Vector3 Forward;
    }

    [Serializable]
    public class BoidConfig
    {
        public float moveSpeed = 1f;

        [Range(0f, 1f)] public float separationWeight = .5f;
        [Range(0f, 1f)] public float alignmentWeight = .5f;
        [Range(0f, 1f)] public float targetWeight = .5f;

        public Transform boidTarget;
    }

    public int adder = 1;
    public int startCount = 1;
    public int endCount = 100;
    public int boidCount;
    public float instanceTime = 60f; // 60秒ごとにBOID数を更新

    public float3 boidExtent = new(32f, 32f, 32f);
    public ComputeShader BoidComputeShader;
    public BoidConfig boidConfig;

    private VisualEffect _boidVisualEffect;
    private GraphicsBuffer _boidBuffer;
    private int _kernelIndex;
    private float _timer;

    private void Awake()
    {
        _timer = 0;
        boidCount = startCount;
        InitializeBoids();
    }

    private void InitializeBoids()
    {
        _boidBuffer?.Dispose();
        _boidBuffer = PopulateBoids(boidCount, boidExtent, null); // 既存データなしで初期化

        _kernelIndex = BoidComputeShader.FindKernel("CSMain");
        BoidComputeShader.SetBuffer(_kernelIndex, "boidBuffer", _boidBuffer);
        BoidComputeShader.SetInt("numBoids", boidCount);

        _boidVisualEffect = GetComponent<VisualEffect>();
        _boidVisualEffect.SetGraphicsBuffer("Boids", _boidBuffer);
    }

    void OnEnable() => InitializeBoids();

    void OnDisable() => _boidBuffer?.Dispose();

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= instanceTime && boidCount < endCount)
        {
            _timer = 0;
            int newBoidCount = Mathf.Min(boidCount + adder, endCount); // 10体ずつ増やす
            UpdateBoidsCount(newBoidCount);
        }
        UpdateBoids();
    }

    void UpdateBoids()
    {
        var boidTarget = boidConfig.boidTarget ? boidConfig.boidTarget.position : transform.position;
        BoidComputeShader.SetFloat("deltaTime", Time.deltaTime);
        BoidComputeShader.SetFloat("separationWeight", boidConfig.separationWeight);
        BoidComputeShader.SetFloat("alignmentWeight", boidConfig.alignmentWeight);
        BoidComputeShader.SetFloat("targetWeight", boidConfig.targetWeight);
        BoidComputeShader.SetFloat("moveSpeed", boidConfig.moveSpeed);
        BoidComputeShader.SetVector("targetPosition", boidTarget);

        BoidComputeShader.GetKernelThreadGroupSizes(_kernelIndex, out var x, out _, out _);
        BoidComputeShader.Dispatch(_kernelIndex, math.max(1, boidCount / (int)x), 1, 1);
    }

    /// <summary>
    /// BOID の数を増やす (位置をリセットせず、新しい BOID だけ追加)
    /// </summary>
    void UpdateBoidsCount(int newBoidCount)
    {
        if (newBoidCount <= boidCount)
        {
            return;
        }

        // 現在の BOID データを取得
        var oldBoidArray = new NativeArray<BoidState>(boidCount, Allocator.Temp);
        BoidState[] tempArray = new BoidState[boidCount];
        _boidBuffer.GetData(tempArray);
        NativeArray<BoidState>.Copy(tempArray, oldBoidArray);


        // 新しい BOID を追加
        _boidBuffer?.Dispose();
        _boidBuffer = PopulateBoids(newBoidCount, boidExtent, oldBoidArray);
        oldBoidArray.Dispose();

        BoidComputeShader.SetBuffer(_kernelIndex, "boidBuffer", _boidBuffer);
        BoidComputeShader.SetInt("numBoids", newBoidCount);
        _boidVisualEffect.SetGraphicsBuffer("Boids", _boidBuffer);

        boidCount = newBoidCount;
    }

    /// <summary>
    /// BOID を生成する。`oldBoids` が指定されていれば、既存データを維持しながら新しい BOID を追加
    /// </summary>
    public static GraphicsBuffer PopulateBoids(int boidCount, float3 boidExtent, NativeArray<BoidState>? oldBoids)
    {
        var random = new Random(256);
        var boidArray = new NativeArray<BoidState>(boidCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

        int oldBoidCount = oldBoids?.Length ?? 0;

        // 既存 BOID をコピー
        if (oldBoids.HasValue)
        {
            NativeArray<BoidState> oldBoidArray = oldBoids.Value;
            for (int i = 0; i < oldBoidCount; i++)
            {
                boidArray[i] = oldBoidArray[i];
            }
        }

        // 新しい BOID を追加
        for (int i = oldBoidCount; i < boidArray.Length; i++)
        {
            boidArray[i] = new BoidState
            {
                Position = random.NextFloat3(-boidExtent, boidExtent),
                Forward = math.rotate(random.NextQuaternionRotation(), Vector3.forward),
            };
        }

        var boidBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, boidArray.Length, Marshal.SizeOf<BoidState>());
        boidBuffer.SetData(boidArray);
        boidArray.Dispose();
        return boidBuffer;
    }
}
