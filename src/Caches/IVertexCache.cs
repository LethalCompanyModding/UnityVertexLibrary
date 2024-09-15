using System.Collections.Generic;
using UnityEngine;

namespace VertexLibrary.Caches;

public interface IVertexCache
{
    public bool IgnoreSkinnedRenders { get; }

    public int CacheSize => Cache.Count;

    internal IDictionary<Mesh, Vector3[]> Cache { get; }

    public Vector3[] this[Mesh mesh]
    {
        get => Cache[mesh];
        internal set => Cache[mesh] = value;
    }

    public bool ContainsKey(Mesh mesh)
    {
        return Cache.ContainsKey(mesh);
    }

    public bool TryGetValue(Mesh mesh, out Vector3[] vertices)
    {
        return Cache.TryGetValue(mesh, out vertices);
    }

    public IVertexCache AsPartial();
    public IVertexCache AsFull();

    public static IVertexCache CreateCache(bool partial)
    {
        if (partial)
            return new PartialVertexCache();
        return new FullVertexCache();
    }
}