using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Build
{
    public static class ProjectCreator
    {
        public static Microsoft.Build.Evaluation.Project Create(string path)
        {
            var proj = new Project { DefaultTargets = "Build", ToolsVersion = "12.0" };

            //var baseImports = new Import();
            //baseImports["Project"] = @"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props";
            //baseImports["Condition"] = @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')";
            //proj.Add(baseImports);

            proj.Add(new PropertyGroup
            {
                Children = new List<XmlObject>
                {
                    //new Conditional("Configuration", "Debug", @" '$(Configuration)' == '' "),
                    //new Conditional("Platform", "AnyCPU", @" '$(Platform)' == '' "),
                    //new Property("ProjectGuid", "{" + Guid.NewGuid().ToString() + "}"),
                    new Property("OutputType", "Exe"),
                    new Property("AppDesignerFolder", "Properties"),
                    new Property("AssemblyName", new DirectoryInfo(path).Name),
                    new Property("TargetFrameworkVersion", "v4.51"),
                    //new Property("FileAlignment", "512"),

                }
            });

            var config = new PropertyGroup
            {
                Children = new List<XmlObject>
                {
                    new Property("PlatformTarget", "AnyCPU"),
                    new Property("DebugSymbols", "true"),
                    new Property("DebugType", "full"),
                    new Property("Optimize", "false"),
                    new Property("OutputPath", @"bin\Debug"),
                    new Property("DefineConstants", "DEBUG"),
                    new Property("ErrorReport", "prompt"),
                    new Property("WarningLevel", "4")
                }
            };

            //config["Condition"] = @"'$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
            proj.Add(config);

            string refsPath = Path.Combine(path, "refs");
            if (Directory.Exists(refsPath))
            {
                var references = Directory.EnumerateFiles(refsPath);
                var refs = new ItemGroup();
                foreach (var r in references)
                {
                    var reference = GetReference(r);
                    if (reference != null)
                        refs.Add(reference);
                }
                proj.Children.Add(refs);
            }

            var compiles = new ItemGroup();
            proj.Add(compiles);
            FileUtils.GetFilesRelative(path, "*.cs").ForEach(f => compiles.Add(new Compile(f)));

            var includes = new ItemGroup();
            proj.Add(includes);
            FileUtils.GetFilesRelative(path, "*.config", SearchOption.TopDirectoryOnly).ForEach(f => includes.Add(new None(f)));

            var targets = new Import();
            targets["Project"] = @"$(MSBuildToolsPath)\Microsoft.CSharp.targets";
            proj.Add(targets);


            string xml = proj.GetXml();
            File.WriteAllText(@"C:\Junk\proj.txt", xml);

            Microsoft.Build.Evaluation.Project project;
            ProjectCollection collection = new ProjectCollection();
            using (var stream = new StringReader(xml))
            using (var xmlNode = XmlReader.Create(stream))
            {
                var projRoot = ProjectRootElement.Create(xmlNode, collection);

                project = new Microsoft.Build.Evaluation.Project(projRoot, null, null, collection) { FullPath = path };
            }

            return project;
        }

        private static Reference GetReference(string fileReference)
        {
            string extension = Path.GetExtension(fileReference);
            string referencePath = null;
            if (String.Equals(extension, ".ref", StringComparison.InvariantCultureIgnoreCase))
            {
                var fileInfo = new FileInfo(fileReference);
                if (fileInfo.Length == 0)
                {
                    referencePath = Path.GetFileNameWithoutExtension(fileReference);
                }
                else
                {
                    var reference = Deserialize<ReferenceFile>(File.ReadAllText(fileReference));
                    if (reference != null)
                        referencePath = reference.ProjectPath ?? reference.FilePath;
                }
            }

            else if (String.Equals(extension, ".dll", StringComparison.InvariantCultureIgnoreCase))
            {
                referencePath = fileReference;
            }

            if (referencePath != null)
                return new Reference(referencePath);
            return null;
        }

        private static T Deserialize<T>(string val)
        {
            using (var reader = new StringReader(val))
            using (var j = new JsonTextReader(reader))
                return new JsonSerializer().Deserialize<T>(j);
        }

        private class ReferenceFile
        {

            public string ProjectPath { get; set; }

            public string FilePath { get; set; }
        }
    }
}
