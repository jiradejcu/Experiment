//
//  CompressionManager.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using System.Diagnostics;
using System.IO;
using Jemast.Utils;
using UnityEditor;
using UnityEngine;

namespace Jemast.LocalCache
{
    public static class CompressionManager
    {
        public static void PerformCompression(string path, bool silent = false)
        {
            if (!silent)
                EditorUtility.DisplayProgressBar("Hold on", "Compressing cache...", 0.5f);

            // Delete old compressed cache
            File.Delete(path + ".jcf");
            File.Delete(path + ".jcf.lz4");

            string compressedFilePath = path + ".jcf.lz4";

            Jcf.Concatenate(path, path + ".jcf");

            Process process;

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // Make lz4 process executable
                process = new Process
                {
                    StartInfo =
                    {
                        FileName = "chmod",
                        Arguments = "+x \"" + Shared.ProjectPath + Shared.UtilsPaths + "lz4\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }

            // Start lz4 process
            process = new Process
            {
                StartInfo =
                {
                    FileName = Shared.ProjectPath + Shared.UtilsPaths + "lz4" +
                               (Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : ""),
                    Arguments = string.Format("-{0} \"{1}\" \"{2}\"",
                        Preferences.CompressionQualityLz4 == 0 ? "1" : "9", path + ".jcf", compressedFilePath),
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            // Fix times
            File.SetCreationTime(compressedFilePath, Directory.GetCreationTime(path));
            File.SetLastAccessTime(compressedFilePath, Directory.GetLastAccessTime(path));
            File.SetLastWriteTime(compressedFilePath, Directory.GetLastWriteTime(path));

            // Cleanup
            Shared.DeleteDirectory(path);
            File.Delete(path + ".jcf");

            if (!silent)
                EditorUtility.ClearProgressBar();
        }

        public static void PerformDecompression(string path, bool silent = false)
        {
            string compressedPath = path + ".jcf.lz4";

            if (!File.Exists(compressedPath))
                return;

            if (Directory.Exists(path))
                Shared.DeleteDirectory(path);

            if (!silent)
                EditorUtility.DisplayProgressBar("Hold on", "Decompressing cache...", 0.5f);

            Process process;

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // Make lz4 process executable
                process = new Process
                {
                    StartInfo =
                    {
                        FileName = "chmod",
                        Arguments = "+x \"" + Shared.ProjectPath + Shared.UtilsPaths + "lz4\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }

            // Start lz4 process
            process = new Process
            {
                StartInfo =
                {
                    FileName = Shared.ProjectPath + Shared.UtilsPaths + "lz4" +
                               (Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : ""),
                    Arguments = string.Format("-d \"{0}\" \"{1}\"", compressedPath, path + ".jcf"),
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            // Unconcatenate
            Jcf.Unconcatenate(path + ".jcf", path);

            // Fix times
            Directory.SetCreationTime(path, File.GetCreationTime(path));
            Directory.SetLastAccessTime(path, File.GetLastAccessTime(path));
            Directory.SetLastWriteTime(path, File.GetLastWriteTime(path));

            // Cleanup
            File.Delete(path + ".jcf");
            File.Delete(path + ".jcf.lz4");

            if (!silent)
                EditorUtility.ClearProgressBar();
        }
    }
}