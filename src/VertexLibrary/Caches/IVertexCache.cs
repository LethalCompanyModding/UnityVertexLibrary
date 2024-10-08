﻿/*
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
/// <summary>
/// </summary>
public interface IVertexCache
{
    /// <summary>
    /// Gets a value indicating whether this cache will ignore hits for <see cref="UnityEngine.SkinnedMeshRenderer"/>.
    /// </summary>
    public bool IgnoreSkinnedRenders { get; }

    /// <summary>
    /// Gets the number of entries in the cache.
    /// </summary>
    public int CacheSize => Cache.Count;

    /// <summary>
    /// The internal dictionary used to store vertex data for each mesh.
    /// </summary>
    internal IDictionary<Mesh, Vector3[]> Cache { get; }

    /// <summary>
    /// Indexer for accessing vertex data in the cache by a specific <see cref="Mesh"/>.
    /// </summary>
    /// <param name="mesh">The mesh to retrieve or assign vertex data for.</param>
    /// <returns>The cached array of vertex positions for the mesh.</returns>
    public Vector3[] this[Mesh mesh]
    {
        get => Cache[mesh];
        internal set => Cache[mesh] = value;
    }

    /// <summary>
    /// Determines if the cache contains vertex data for a specified <see cref="Mesh"/>.
    /// </summary>
    /// <param name="mesh">The mesh to check for in the cache.</param>
    /// <returns><c>true</c> if the cache contains the mesh; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(Mesh mesh)
    {
        return Cache.ContainsKey(mesh);
    }

    /// <summary>
    /// Attempts to retrieve vertex data for a specified <see cref="Mesh"/>.
    /// </summary>
    /// <param name="mesh">The mesh to retrieve vertex data for.</param>
    /// <param name="vertices">The array of vertex positions, if found.</param>
    /// <returns><c>true</c> if the vertex data is found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(Mesh mesh, out Vector3[] vertices)
    {
        return Cache.TryGetValue(mesh, out vertices);
    }

    /// <summary>
    /// Returns a cache object that uses the same internal dictionary but ignores <see cref="UnityEngine.SkinnedMeshRenderer"/> components.
    /// </summary>
    /// <returns>An <see cref="IVertexCache"/> instance representing a partial cache.</returns>
    public IVertexCache AsPartial();

    /// <summary>
    /// Returns a cache object that uses the same internal dictionary and includes <see cref="UnityEngine.SkinnedMeshRenderer"/> components.
    /// </summary>
    /// <returns>An <see cref="IVertexCache"/> instance representing a full cache.</returns>
    public IVertexCache AsFull();

    /// <summary>
    /// Factory method for creating a new vertex cache, either partial or full.
    /// </summary>
    /// <param name="partial">If <c>true</c>, a partial cache is created; otherwise, a full cache is created.</param>
    /// <returns>A new instance of an <see cref="IVertexCache"/>.</returns>
    public static IVertexCache CreateCache(bool partial)
    {
        if (partial)
            return new PartialVertexCache();
        return new FullVertexCache();
    }
}