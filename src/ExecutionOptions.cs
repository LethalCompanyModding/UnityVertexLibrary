/*
 * VertexLibrary: Provides extension methods for obtaining and manipulating
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

using System;
using System.Collections.Generic;
using UnityEngine;
using VertexLibrary.Caches;
// ReSharper disable CollectionNeverUpdated.Global

namespace VertexLibrary;

public struct ExecutionOptions
{
    public ExecutionOptions()
    {}

    /// <summary>
    /// Function to process logs from the library.
    /// </summary>
    public Action<LogType, Func<string>> LogHandler { get; set; } = null;

    /// <summary>
    /// Set of Unity component types that, if present on a GameObject, will cause it to be skipped.
    /// </summary>
    public ISet<Type> FilteredComponents { get; set; } = new HashSet<Type>();
    
    /// <summary>
    /// Translation matrix to be used to convert the vertexes from local space
    /// </summary>
    public Matrix4x4 OverrideMatrix { get; set; } = Matrix4x4.identity;
    
    /// <summary>
    /// Cache used to speed up computation. If <value>null</value> no cache will be used.
    /// </summary>
    public IVertexCache VertexCache { get; set; } = VerticesExtensions.GlobalPartialCache;

}