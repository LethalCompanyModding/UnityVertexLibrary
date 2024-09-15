using System;
using System.Collections.Generic;
using UnityEngine;
using VertexLibrary.Caches;

namespace VertexLibrary;

public struct ExecutionOptions
{
    public ExecutionOptions()
    {}

    public Action<LogType, Func<string>> LogHandler { get; set; } = null;
    public ISet<Type> FilteredComponents { get; set; } = new HashSet<Type>();
    public Matrix4x4? OverrideMatrix { get; set; } = null;
    public IVertexCache VertexCache { get; set; } = VerticesExtensions.GlobalPartialCache;
}