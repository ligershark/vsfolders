using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class LaunchIISExpressCommand : CommandBase
    {
        private static readonly Lazy<string> IISExpressBinaryPath = new Lazy<string>(GetIISExpressBinaryPath);

        private readonly Random _rnd = new Random();
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>; ;
            string args;
            string launchBrowserAt = null;
            string relPath;
            var host = FindLikelySiteRoot(obj.Value, out relPath);

            if (host == null)
            {
                return;
            }

            if (host.Name.Equals("applicationhost.config", StringComparison.OrdinalIgnoreCase))
            {
                args = "/config:\"" + host.FullPath + "\" /systray:true";
            }
            else if (host.IsDirectory)
            {
                var port = _rnd.Next(8081, 16384);
                args = "/path:\"" + host.FullPath + "\" /systray:true /port:" + port;
                launchBrowserAt = "http://localhost:" + port + relPath.Replace('\\', '/');
            }
            else
            {
                var port = _rnd.Next(8081, 16384);
                args = "/path:\"" + host.FullPath + "\" /systray:true /port:" + port;
                launchBrowserAt = "http://localhost:" + port + relPath.Replace('\\', '/');
            }

            var psi = new ProcessStartInfo(IISExpressBinaryPath.Value)
            {
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);

            if (launchBrowserAt != null)
            {
                Process.Start(launchBrowserAt);
            }
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && IISExpressBinaryPath.Value != null;
        }

        private FileData FindLikelySiteRoot(FileData source, out string relativePath)
        {
            var target = FindOwnedRoot(source, out relativePath);
            var current = source;

            while (current != null && !current.FullPath.Equals(target.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                var appHost = current.TreeNode.FirstOrDefault<TreeNode<FileData>>(x => x.Value.Name.Equals("applicationhost.config", StringComparison.OrdinalIgnoreCase));

                if (appHost != null)
                {
                    relativePath = MakeRelativePath(current.FullPath, source.FullPath);
                    return appHost.Value;
                }

                current = current.Parent;
            }

            return target;
        }


        private FileData FindOwnedRoot(FileData source, out string relativePath)
        {
            foreach (var fd in ((IEnumerable<TreeNode<FileData>>)source.TreeNode.Root).Where(x => x.Value.IsDirectory).OrderBy(x => x.Value.FullPath.Length))
            {
                if (source.FullPath.StartsWith(fd.Value.FullPath, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = MakeRelativePath(fd.Value.FullPath, source.FullPath);
                    return fd.Value;
                }
            }

            relativePath = source.IsDirectory ? "\\" : source.Name;
            return source;
        }

        private static string MakeRelativePath(string basis, string fullPath)
        {
            return "\\" + fullPath.Substring(basis.Length).TrimStart('\\');
        }

        private static string GetIISExpressBinaryPath()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (Directory.Exists(programFiles))
            {
                var iisExpressDir = System.IO.Path.Combine(programFiles, "IIS Express");

                if (Directory.Exists(iisExpressDir))
                {
                    var bin = System.IO.Path.Combine(iisExpressDir, "iisexpress.exe");

                    if (File.Exists(bin))
                    {
                        return bin;
                    }
                }
            }

            programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (Directory.Exists(programFiles))
            {
                var iisExpressDir = System.IO.Path.Combine(programFiles, "IIS Express");

                if (Directory.Exists(iisExpressDir))
                {
                    var bin = System.IO.Path.Combine(iisExpressDir, "iisexpress.exe");

                    if (File.Exists(bin))
                    {
                        return bin;
                    }
                }
            }

            return null;
        }
    }
}
