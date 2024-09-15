using System.Collections.Generic;
using UnityEngine;

namespace VertexLibrary.Caches;

internal class PartialVertexCache : IVertexCache
{
    private IVertexCache _complementaryCache;

    public PartialVertexCache()
    {
    }

    public PartialVertexCache(FullVertexCache complementaryCache)
    {
        _complementaryCache = complementaryCache;
        Cache = complementaryCache.Cache;
    }

    public bool IgnoreSkinnedRenders => true;
    public IDictionary<Mesh, Vector3[]> Cache { get; } = new Dictionary<Mesh, Vector3[]>();


    public IVertexCache AsPartial()
    {
        return this;
    }

    public IVertexCache AsFull()
    {
        _complementaryCache ??= new FullVertexCache(this);
        return _complementaryCache;
    }
}