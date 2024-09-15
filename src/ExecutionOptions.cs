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