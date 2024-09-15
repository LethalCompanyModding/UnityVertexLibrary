using System.Collections.Generic;
using UnityEngine;

namespace VertexLibrary.Caches;

internal class FullVertexCache : IVertexCache
{
    private IVertexCache _complementaryCache;

    public FullVertexCache()
    {
    }

    public FullVertexCache(PartialVertexCache complementaryCache)
    {
        _complementaryCache = complementaryCache;
        Cache = complementaryCache.Cache;
    }

    public bool IgnoreSkinnedRenders => false;
    public IDictionary<Mesh, Vector3[]> Cache { get; } = new Dictionary<Mesh, Vector3[]>();

    public IVertexCache AsPartial()
    {
        _complementaryCache ??= new PartialVertexCache(this);
        return _complementaryCache;
    }

    public IVertexCache AsFull()
    {
        return this;
    }
}