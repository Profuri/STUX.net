using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturePart : PoolableMono
{
    private readonly List<Vector3> _verticies = new List<Vector3>();
    private readonly List<Vector3> _normals = new List<Vector3>();
    private readonly List<List<int>> _triangles = new List<List<int>>();
    private readonly List<Vector2> _uvs = new List<Vector2>();

    public Vector3[] Vertices;
    public Vector3[] Normal;
    public int[][] Triangle;
    public Vector2[] UV;
    public Bounds Bounds;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Rigidbody _rigidbody;

    private FractureObject _owner;

    private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");
    private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");
    private readonly int _baseColorHash = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    public void Broke(FractureObject origin, Vector3 force)
    {
        var originalTransform = origin.transform;
        transform.position = originalTransform.position;
        transform.rotation = originalTransform.rotation;
        transform.localScale = originalTransform.localScale;

        _rigidbody.AddForceAtPosition(force, originalTransform.position);
        
        StartSafeCoroutine("FracturePartLifeRoutine", LifeRoutine());
    }

    public void CreateMesh(FractureObject origin)
    {
        var mesh = new Mesh
        {
            name = origin.MeshFilter.mesh.name,
            vertices = Vertices,
            normals = Normal,
            uv = UV
        };

        for (var i = 0; i < Triangle.Length; i++)
        {
            mesh.SetTriangles(Triangle[i], i, true);
        }
        Bounds = mesh.bounds;
        Bounds.Expand(0.5f);
        
        _meshRenderer.material = Instantiate(origin.MeshRenderer.material);
        _meshFilter.mesh = mesh;
        
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        if (_triangles.Count - 1 < submesh)
            _triangles.Add(new List<int>());

        _triangles[submesh].Add(_verticies.Count);
        _verticies.Add(vert1);
        
        _triangles[submesh].Add(_verticies.Count);
        _verticies.Add(vert2);
        
        _triangles[submesh].Add(_verticies.Count);
        _verticies.Add(vert3);
        
        _normals.Add(normal1);
        _normals.Add(normal2);
        _normals.Add(normal3);
        
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);

        Bounds.min = Vector3.Min(Bounds.min, vert1);
        Bounds.min = Vector3.Min(Bounds.min, vert2);
        Bounds.min = Vector3.Min(Bounds.min, vert3);
        Bounds.max = Vector3.Min(Bounds.max, vert1);
        Bounds.max = Vector3.Min(Bounds.max, vert2);
        Bounds.max = Vector3.Min(Bounds.max, vert3);
    }

    public void FillArrays()
    {
        Vertices = _verticies.ToArray();
        Normal = _normals.ToArray();
        UV = _uvs.ToArray();
        Triangle = new int[_triangles.Count][];
        for (var i = 0; i < _triangles.Count; i++)
            Triangle[i] = _triangles[i].ToArray();
    }

    private IEnumerator LifeRoutine()
    {
        _meshRenderer.material.SetFloat(_visibleProgressHash, 0f);
        _meshRenderer.material.SetFloat(_dissolveProgressHash, 0f);
        
        var duration = 1f;
        var currentTime = 0f;
        var percent = 0f;
        var originAlpha = _meshRenderer.material.GetColor(_baseColorHash).a;

        while (percent <= 1f)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / duration;

            var color = _meshRenderer.material.GetColor(_baseColorHash);
            color.a = 1f * originAlpha - percent * originAlpha;
            _meshRenderer.material.SetColor(_baseColorHash, color);
        
            yield return null;
        }

        PoolManager.Instance.Push(this);
    }

    public override void OnPop()
    {
        _verticies.Clear();
        _triangles.Clear();
        _normals.Clear();
        _uvs.Clear();
    }

    public override void OnPush()
    {
    }
}
