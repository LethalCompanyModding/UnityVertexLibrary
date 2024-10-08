﻿namespace VertexLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum LogType
{
    None = 0,
    Fatal = 1 << 1,
    Error = 1 << 2,
    Warning = 1 << 3,
    Info1 = 1 << 4,
    Info2 = 1 << 5,
    Info3 = 1 << 6,
    Info4 = 1 << 7,
    Info = Info1 | Info2 | Info3 | Info4,
    Debug1 = 1 << 8,
    Debug2 = 1 << 9,
    Debug3 = 1 << 10,
    Debug4 = 1 << 11,
    Debug = Debug1 | Debug2 | Debug3 | Debug4,
    All = Debug | Info | Warning | Error | Fatal
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member