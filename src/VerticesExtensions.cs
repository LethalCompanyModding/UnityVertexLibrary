using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using VertexLibrary.Caches;
using Object = UnityEngine.Object;

namespace VertexLibrary;

public static class VerticesExtensions
{
    public static IVertexCache GlobalCache { get; } = IVertexCache.CreateCache(false);
    public static IVertexCache GlobalPartialCache { get; } = GlobalCache.AsPartial();
    public static event Action<LogType, Func<string>> logEvent;


    //GameObject Extensions
    /// <summary>
    /// Find the OOBB of this object
    /// Optionally use the Override matrix to translate the result
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>the bounds encapsulating the object</returns>
    public static Bounds GetBounds(this GameObject target, ExecutionOptions executionOptions = default)
    {
        return GetBounds(target.transform, executionOptions);
    }

    /// <summary>
    /// Find the OOBB of this object
    /// Optionally use the Override matrix to translate the result
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>true if any vertex has been found, false otherwise</returns>
    public static bool TryGetBounds(this GameObject target, out Bounds bounds,
        ExecutionOptions executionOptions = default)
    {
        return TryGetBounds(target.transform, out bounds, executionOptions);
    }

    /// <summary>
    /// Find the AABB of this object
    /// the Override matrix will be ignored
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>the bounds encapsulating the object</returns>
    public static Bounds GetWorldBounds(this GameObject target, ExecutionOptions executionOptions = default)
    {
        return GetWorldBounds(target.transform, executionOptions);
    }

    /// <summary>
    /// Find the AABB of this object
    /// the Override matrix will be ignored
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>true if any vertex has been found, false otherwise</returns>
    public static bool TryGetWorldBounds(this GameObject target, out Bounds bounds,
        ExecutionOptions executionOptions = default)
    {
        return TryGetWorldBounds(target.transform, out bounds, executionOptions);
    }
    
    /// <summary>
    /// compute the largest distance from the bounding box center
    /// the Override matrix will be ignored
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>largest distance from the bounding box center</returns>
    public static float GetRadius(this GameObject target, ExecutionOptions executionOptions = default)
    {
        return GetRadius(target.transform, executionOptions);
    }

    /// <summary>
    /// Obtain all the vertexes of the object in local space
    /// Optionally use the Override matrix to translate the result
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>all the vertexes found</returns>
    public static Vector3[] GetVertexes(this GameObject target, ExecutionOptions executionOptions = default)
    {
        return GetVertexes(target.transform, executionOptions);
    }
    
    /// <summary>
    /// Pre-Fill the cache from <param name="executionOptions"></param>
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers and the cache to be filled
    public static void CacheVertexes(GameObject tartgetObject, ExecutionOptions executionOptions)
    {
        CacheVertexes(tartgetObject.transform, executionOptions);
    }

    //Transform Extensions
    
    /// <summary>
    /// Find the OOBB of this object
    /// Optionally use the Override matrix to translate the result
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>the bounds encapsulating the object</returns>
    public static Bounds GetBounds(this Transform target, ExecutionOptions executionOptions = default)
    {
        logEvent += executionOptions.LogHandler;
        try
        {
            string Logfunc(List<Vector3> vertexes)
            {
                var bounds = GetBounds(vertexes);
                return bounds == default ? "" : $"{bounds} Min {bounds.min} Max {bounds.max}";
            }

            var vertexes = ListPool<Vector3>.Get();

            var matrix = executionOptions.OverrideMatrix ?? Matrix4x4.identity;

            target.GetChildVertexes(vertexes, matrix, "", executionOptions, Logfunc);
            var bounds = GetBounds(vertexes);
            ListPool<Vector3>.Release(vertexes);
            return bounds;
        }
        finally
        {
            logEvent -= executionOptions.LogHandler;
        }
    }

    /// <summary>
    /// Find the OOBB of this object
    /// Optionally use the Override matrix to translate the result
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>true if any vertex has been found, false otherwise</returns>
    public static bool TryGetBounds(this Transform target, out Bounds bounds,
        ExecutionOptions executionOptions = default)
    {
        bounds = GetBounds(target, executionOptions);
        return bounds != default;
    }

    /// <summary>
    /// Find the AABB of this object
    /// the Override matrix will be ignored
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>the bounds encapsulating the object</returns>
    public static Bounds GetWorldBounds(this Transform target, ExecutionOptions executionOptions = default)
    {
        logEvent += executionOptions.LogHandler;
        try
        {
            string Logfunc(List<Vector3> vertexes)
            {
                var bounds = GetBounds(vertexes);
                return bounds == default ? "" : $"{bounds} Min {bounds.min} Max {bounds.max}";
            }

            var vertexes = ListPool<Vector3>.Get();

            var localMatrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
            target.GetChildVertexes(vertexes, localMatrix, "", executionOptions, Logfunc);

            var bounds = GetBounds(vertexes);
            ListPool<Vector3>.Release(vertexes);
            return bounds;
        }
        finally
        {
            logEvent -= executionOptions.LogHandler;
        }
    }
    
    /// <summary>
    /// Find the AABB of this object
    /// the Override matrix will be ignored
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>true if any vertex has been found, false otherwise</returns>
    public static bool TryGetWorldBounds(this Transform target, out Bounds bounds,
        ExecutionOptions executionOptions = default)
    {
        bounds = GetWorldBounds(target, executionOptions);
        return bounds != default;
    }
    
    /// <summary>
    /// compute the largest distance from the bounding box center
    /// the Override matrix will be ignored
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>largest distance from the bounding box center</returns>
    public static float GetRadius(this Transform target, ExecutionOptions executionOptions = default)
    {
        logEvent += executionOptions.LogHandler;
        try
        {
            string Logfunc(List<Vector3> vertexes)
            {
                var bounds = GetBounds(vertexes);
                return bounds == default ? "Radius: 0" : $"{bounds} Min {bounds.min} Max {bounds.max} Radius: {GetFarthestPoint(vertexes, bounds.center)}";
            }

            var vertexes = ListPool<Vector3>.Get();

            var localMatrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
            target.GetChildVertexes(vertexes, localMatrix, "", executionOptions, Logfunc);

            var bounds = GetBounds(vertexes);

            var (_, radius) = GetFarthestPoint(vertexes, bounds.center);

            ListPool<Vector3>.Release(vertexes);

            return radius;
        }
        finally
        {
            logEvent -= executionOptions.LogHandler;
        }
    }

    /// <summary>
    /// Obtain all the vertexes of the object in local space
    /// Optionally use the Override matrix to translate the result
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers
    /// <returns>all the vertexes found</returns>
    public static Vector3[] GetVertexes(this Transform target, ExecutionOptions executionOptions = default)
    {
        logEvent += executionOptions.LogHandler;
        try
        {
            string Logfunc(List<Vector3> vertexes)
            {
                var bounds = GetBounds(vertexes);
                return bounds == default ? "" : $"{bounds} Min {bounds.min} Max {bounds.max}";
            }

            var vertexes = ListPool<Vector3>.Get();

            var matrix = executionOptions.OverrideMatrix ?? Matrix4x4.identity;

            target.GetChildVertexes(vertexes, matrix, "", executionOptions, Logfunc);
            var output = vertexes.ToArray();
            ListPool<Vector3>.Release(vertexes);
            return output;
        }
        finally
        {
            logEvent -= executionOptions.LogHandler;
        }
    }
    
    /// <summary>
    /// Pre-Fill the cache from <param name="executionOptions"></param>
    /// </summary>
    /// <param name="executionOptions"></param> optional modifiers and the cache to be filled
    public static void CacheVertexes(this Transform target, ExecutionOptions executionOptions = default)
    {
        if (executionOptions.VertexCache == null)
            throw new ArgumentNullException($"{nameof(executionOptions)}.{nameof(executionOptions.VertexCache)}");

        CacheChildVertexes(target, "", executionOptions);
    }

    // Utility Extensions

    /// <summary>
    /// Get bounds encapsulating all <param name="vertexes"></param>
    /// </summary>
    /// <returns>bounds encapsulating all <param name="vertexes"></param></returns>
    public static Bounds GetBounds(this IEnumerable<Vector3> vertexes)
    {
        var bounds = new Bounds();
        foreach (var v in vertexes)
            bounds.Encapsulate(v);
        return bounds;
    }

    /// <summary>
    /// Get the farthest point from <param name="origin"></param>
    /// </summary>
    /// <param name="vertexes"></param>
    /// <returns>farthest point from <param name="origin"></param> and relative numeric distance</returns>
    public static (Vector3? point, float distance) GetFarthestPoint(this IEnumerable<Vector3> vertexes,
        Vector3 origin = default)
    {
        Vector3? found = null;
        var max = float.NegativeInfinity;
        foreach (var v in vertexes)
        {
            var distance = Vector3.Distance(v, origin);
            if (distance > max)
            {
                found = v;
                max = distance;
            }
        }

        return (found, max);
    }

    //INTERNAL
    
    private static void GetChildVertexes(this Transform target, List<Vector3> outVertices, Matrix4x4 localMatrix,
        string path, ExecutionOptions executionOptions, Func<List<Vector3>, string> logFunc)
    {
        logEvent?.Invoke(LogType.Log, () => $"Processing {path}/{target.name}");

        if (executionOptions.FilteredComponents?.Any(c => target.TryGetComponent(c, out _)) ?? false)
        {
            logEvent?.Invoke(LogType.Log, () => $"Skipping {path}/{target.name}!");
            return;
        }

        using var pooledRenderers = ListPool<Renderer>.Get(out var renderers);
        target.GetComponents(renderers);

        using (ListPool<Vector3>.Get(out var vertexes))
        {
            foreach (var renderer in renderers.Where(r => r.enabled))
                using (ListPool<Vector3>.Get(out var rVertices))
                {
                    switch (renderer)
                    {
                        case SkinnedMeshRenderer skinnedMeshRenderer:
                        {
                            var mesh = skinnedMeshRenderer.sharedMesh;
                            if (mesh == null)
                            {
                                logEvent?.Invoke(LogType.Warning,
                                    () => $"{renderer.GetType()} in {path} is missing a mesh");
                                continue;
                            }

                            if (executionOptions.VertexCache is { IgnoreSkinnedRenders: false } &&
                                executionOptions.VertexCache.TryGetValue(mesh, out var cached))
                            {
                                logEvent?.Invoke(LogType.Warning,
                                    () => $"Cache hit {path}/{target.name} renderer {renderer.GetType().Name}");
                                rVertices.AddRange(cached);
                            }
                            else
                            {
                                var tmpMesh = new Mesh();

                                skinnedMeshRenderer.BakeMesh(tmpMesh, true);

                                if (tmpMesh.isReadable)
                                    tmpMesh.GetVertices(rVertices);
                                else
                                    tmpMesh.GetNonReadableVertices(rVertices);

                                Object.Destroy(tmpMesh);
                                if (executionOptions.VertexCache != null)
                                    executionOptions.VertexCache[mesh] = rVertices.ToArray();
                            }

                            break;
                        }
                        case MeshRenderer:
                        {
                            var filter = renderer.GetComponent<MeshFilter>();
                            if (filter == null)
                            {
                                logEvent?.Invoke(LogType.Warning,
                                    () => $"{renderer.GetType()} in {path} is missing a MeshFilter");
                                continue;
                            }

                            var mesh = filter.sharedMesh;

                            if (mesh == null)
                            {
                                logEvent?.Invoke(LogType.Warning,
                                    () => $"{renderer.GetType()} in {path} is missing a mesh");
                                continue;
                            }

                            if (executionOptions.VertexCache != null &&
                                executionOptions.VertexCache.TryGetValue(mesh, out var cached))
                            {
                                logEvent?.Invoke(LogType.Log,
                                    () => $"Cache hit {path}/{target.name} renderer {renderer.GetType().Name}");
                                rVertices.AddRange(cached);
                            }
                            else
                            {
                                if (mesh.isReadable)
                                    mesh.GetVertices(rVertices);
                                else
                                    mesh.GetNonReadableVertices(rVertices);

                                if (executionOptions.VertexCache != null)
                                    executionOptions.VertexCache[mesh] = rVertices.ToArray();
                            }

                            break;
                        }
                        case ParticleSystemRenderer:
                            break;
                        default:
                        {
                            var bounds = renderer.bounds;
                            rVertices.Add(bounds.min);
                            rVertices.Add(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
                            rVertices.Add(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
                            rVertices.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
                            rVertices.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
                            rVertices.Add(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
                            rVertices.Add(bounds.max);
                            break;
                        }
                    }

                    logEvent?.Invoke(LogType.Log,
                        () =>
                            $"Processing {path}/{target.name} renderer {renderer.GetType().Name} {logFunc?.Invoke(rVertices)}");

                    vertexes.AddRange(rVertices);
                }

            foreach (Transform child in target.transform)
            {
                if (!child.gameObject.activeSelf)
                    continue;

                var childMatrix = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale);

                var childVertices = ListPool<Vector3>.Get();
                GetChildVertexes(child, childVertices, childMatrix, path + "/" + target.name, executionOptions,
                    logFunc);

                vertexes.AddRange(childVertices);
                ListPool<Vector3>.Release(childVertices);
            }

            outVertices.AddRange(vertexes.Select(localMatrix.MultiplyPoint3x4));
        }

        logEvent?.Invoke(LogType.Log, () => $"Found {path}/{target.name} {logFunc?.Invoke(outVertices)}");
    }

    
    
    private static void CacheChildVertexes(this Transform target, string path, ExecutionOptions executionOptions)
    {
        logEvent?.Invoke(LogType.Log, () => $"Caching {path}/{target.name}");

        if (executionOptions.FilteredComponents?.Any(c => target.TryGetComponent(c, out _)) ?? false)
        {
            logEvent?.Invoke(LogType.Log, () => $"Skipping {path}/{target.name}!");
            return;
        }

        using var pooledRenderers = ListPool<Renderer>.Get(out var renderers);
        target.GetComponents(renderers);

        foreach (var renderer in renderers.Where(r => r.enabled))
        {
            switch (renderer)
            {
                case SkinnedMeshRenderer skinnedMeshRenderer:
                {
                    var mesh = skinnedMeshRenderer.sharedMesh;
                    if (mesh == null)
                    {
                        logEvent?.Invoke(LogType.Warning, () => $"{renderer.GetType()} in {path} is missing a mesh");
                        continue;
                    }

                    if (!executionOptions.VertexCache.Cache.ContainsKey(mesh))
                    {
                        var tmpMesh = new Mesh();

                        skinnedMeshRenderer.BakeMesh(tmpMesh, true);

                        if (tmpMesh.isReadable)
                            tmpMesh.CacheVertices(executionOptions, mesh);
                        else
                            tmpMesh.CacheNonReadableVertices(executionOptions, mesh);

                        Object.Destroy(tmpMesh);
                    }

                    break;
                }
                case MeshRenderer:
                {
                    var filter = renderer.GetComponent<MeshFilter>();
                    if (filter == null)
                    {
                        logEvent?.Invoke(LogType.Warning,
                            () => $"{renderer.GetType()} in {path} is missing a MeshFilter");
                        continue;
                    }

                    var mesh = filter.sharedMesh;

                    if (mesh == null)
                    {
                        logEvent?.Invoke(LogType.Warning, () => $"{renderer.GetType()} in {path} is missing a mesh");
                        continue;
                    }

                    if (!executionOptions.VertexCache.Cache.ContainsKey(mesh))
                    {
                        if (mesh.isReadable)
                            mesh.CacheVertices(executionOptions);
                        else
                            mesh.CacheNonReadableVertices(executionOptions);
                    }

                    break;
                }
                case ParticleSystemRenderer:
                    break;
            }


            logEvent?.Invoke(LogType.Log, () => $"Caching {path}/{target.name} renderer {renderer.GetType().Name}");

            foreach (Transform child in target.transform)
                CacheChildVertexes(child, path + "/" + target.name, executionOptions);
        }
    }

    
    
    private static void GetNonReadableVertices(this Mesh nonReadableMesh, List<Vector3> vertexes)
    {
        Mesh meshCopy = new();

        // Handle vertexes
        nonReadableMesh.vertexBufferTarget = GraphicsBuffer.Target.Vertex;
        if (nonReadableMesh.vertexBufferCount > 0)
        {
            var verticesBuffer = nonReadableMesh.GetVertexBuffer(0);
            var totalSize = verticesBuffer.stride * verticesBuffer.count;

            var data = ArrayPool<byte>.Shared.Rent(totalSize);
            verticesBuffer.GetData(data, 0, 0, totalSize);

            var vertexAttributeCount = nonReadableMesh.vertexAttributeCount;
            var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(vertexAttributeCount, Allocator.Temp);
            for (var i = 0; i < vertexAttributeCount; i++) vertexAttributes[i] = nonReadableMesh.GetVertexAttribute(i);

            meshCopy.SetVertexBufferParams(nonReadableMesh.vertexCount, vertexAttributes);
            meshCopy.SetVertexBufferData(data, 0, 0, totalSize);

            ArrayPool<byte>.Shared.Return(data);
            vertexAttributes.Dispose();
            verticesBuffer.Dispose();
        }

        meshCopy.GetVertices(vertexes);

        Object.Destroy(meshCopy);
    }

    
    
    private static void CacheNonReadableVertices(this Mesh nonReadableMesh, ExecutionOptions executionOptions,
        Mesh cacheKey = null)
    {
        if (executionOptions.VertexCache.ContainsKey(nonReadableMesh))
            return;

        // Handle vertexes
        nonReadableMesh.vertexBufferTarget = GraphicsBuffer.Target.Vertex;
        if (nonReadableMesh.vertexBufferCount > 0)
        {
            var verticesBuffer = nonReadableMesh.GetVertexBuffer(0);
            var totalSize = verticesBuffer.stride * verticesBuffer.count;
            var attributes = nonReadableMesh.GetVertexAttributes();
            var count = nonReadableMesh.vertexCount;

            var vertexCache = executionOptions.VertexCache.Cache;

            logEvent?.Invoke(LogType.Warning, () => $"Requesting vertexes for {nonReadableMesh} from GPU");
            AsyncGPUReadback.Request(verticesBuffer, totalSize, 0, request =>
            {
                verticesBuffer.Release();
                if (vertexCache.ContainsKey(nonReadableMesh))
                    return;

                Mesh meshCopy = new();
                var data = request.GetData<byte>();
                meshCopy.SetVertexBufferParams(count, attributes);
                meshCopy.SetVertexBufferData(data, 0, 0, totalSize);

                using (ListPool<Vector3>.Get(out var tmp))
                {
                    meshCopy.GetVertices(tmp);
                    logEvent?.Invoke(LogType.Log, () => $"Cached {tmp.Count} vertexes for {nonReadableMesh}");
                    executionOptions.VertexCache[cacheKey != null ? cacheKey : nonReadableMesh] = tmp.ToArray();
                }

                Object.Destroy(meshCopy);
            });
        }
    }

    private static void CacheVertices(this Mesh readableMesh, ExecutionOptions executionOptions, Mesh cacheKey = null)
    {
        if (executionOptions.VertexCache.ContainsKey(readableMesh))
            return;

        using (ListPool<Vector3>.Get(out var tmp))
        {
            readableMesh.GetVertices(tmp);
            logEvent?.Invoke(LogType.Log, () => $"Cached {tmp.Count} vertexes for {readableMesh}");
            executionOptions.VertexCache[cacheKey != null ? cacheKey : readableMesh] = tmp.ToArray();
        }
    }
}