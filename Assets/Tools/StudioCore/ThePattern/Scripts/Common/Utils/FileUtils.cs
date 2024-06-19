using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern.Utils
{
    public static class FileUtils
    {
        public static void EmptyDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            } 
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
            }
        }

        private const string TEMP_FILE = "tempFile.tmp";

        public static bool HasWritePermissionOnDir(string path)
        {
            bool flag = false;
            string path1 = Path.Combine(path, Guid.NewGuid().ToString() + "tempFile.tmp");
            if (Directory.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = new FileStream(path1, FileMode.CreateNew, FileAccess.Write))
                        fileStream.WriteByte(byte.MaxValue);
                    if (File.Exists(path1))
                    {
                        File.Delete(path1);
                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                }
            }
            return flag;
        }
    }
}

