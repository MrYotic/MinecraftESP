﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESP.Utils;
public class LogManger
{
    public static string Path { get; private set; }
    public static Encoding Encoding { get; private set; } = Encoding.UTF8;

    private static FileStream stream;

    public static void SetFile(string path)
    {
        Path = path;
        stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public static void Clear() => stream.SetLength(0);
    public static void Write(object obj)
    {
        byte[] buffer = Encoding.GetBytes(obj.ToString());
        stream.Write(buffer, 0, buffer.Length);
    }
    public static void WriteLine(object obj)
    {
        Write(obj);
        Write('\n');
    }
}