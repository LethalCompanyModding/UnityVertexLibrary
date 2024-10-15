/*
 * VertexLibrary: Provides extension methods for obtaining
 * the oriented bounding box (OOBB) of Unity objects.
 * Copyright (C) 2024 mattymatty97 (GitHub)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System.Collections.Generic;
using UnityEngine;

namespace VertexLibrary.Caches;

internal class FullVertexCache : IVertexCache
{
    private IVertexCache? _complementaryCache;

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
