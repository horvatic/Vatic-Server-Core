﻿using System.IO;

namespace Server.Core
{
    class FileProxy : IFileProxy
    {
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
