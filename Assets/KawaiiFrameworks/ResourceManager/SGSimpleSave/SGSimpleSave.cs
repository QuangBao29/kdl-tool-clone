using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Kawaii.ResourceManager
{
    public class FileSaving
    {
        public static void Save(string path, string txt)
        {
            File.WriteAllText(path, txt, System.Text.Encoding.Unicode);
        }

        public static string Load(string path)
        {
            if(File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return null;
        }
    }

}
