using System;
using System.IO;

namespace Common
{
    public static class Constants
    {
        public static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "titanfall2-rp"
        );
    }
}