using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.IO.Compression;
using BitsUpdater.Extensions;
using System.Text.RegularExpressions;

namespace BitsUpdater
{
    public sealed class UpdatePackage
    {
        private readonly string _certificatePath;
        private readonly Version _version;
        private readonly bool _isDifferential;
        internal const string LatestVersionDirectory = "LatestVersion\\";
        internal const string AssemblyName = "Update.{0}";
        internal const string AssemblySuffix = ".dll";
        internal const string PackageSuffix = ".gz";

        public UpdatePackage(string certificatePath, Version version, bool isDifferential)
        {
            _certificatePath = certificatePath;
            _version = version;
            _isDifferential = isDifferential;
        }

        /// <summary>
        /// List of templates specifying files that are included in update.
        /// </summary>
        public IEnumerable<FileSearchTemplate> IncludedFiles
        {
            get;
            set;
        }

        /// <summary>
        /// List of templates for files that are NOT included in update.
        /// </summary>
        public IEnumerable<FileSearchTemplate> ExcludedFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Creates normalized Update Package for BitsUpdater library
        /// </summary>
        /// <param name="outputDirectory"></param>
        public void Create(string outputDirectory)
        {
            using (FileStream certificate = new FileStream(_certificatePath, FileMode.Open, FileAccess.Read))
            {
                string fileName = string.Format(AssemblyName + AssemblySuffix, _version);
                var name = new AssemblyName(string.Format(AssemblyName, _version))
                {
                    Version = _version,
                    KeyPair = new StrongNameKeyPair(certificate),
                };
                AssemblyBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save, outputDirectory);
                ModuleBuilder moduleBuilder = builder.DefineDynamicModule(name.Name, fileName);

                IEnumerable<FileStream> files;
                if (_isDifferential && Directory.Exists(Path.Combine(outputDirectory, LatestVersionDirectory)))
                {
                    files = CreateDifferentialPackage(moduleBuilder, outputDirectory);
                }
                else
                {
                    files = CreatePackage(moduleBuilder, outputDirectory);
                }

                builder.Save(fileName);

                foreach (var item in files)
                {
                    item.Dispose();
                }
                Compress(outputDirectory, fileName);
            }
        }

        private IEnumerable<FileStream> CreatePackage(ModuleBuilder moduleBuilder, string outputDirectory)
        {
            var files = new List<FileStream>();

            foreach (var item in IncludedFiles)
            {
                files.AddRange(AddFiles(moduleBuilder, item, false, null));
            }

            string lastVersionDirectoryPath = Path.Combine(outputDirectory, LatestVersionDirectory);
            if (Directory.Exists(lastVersionDirectoryPath))
            {
                Directory.Delete(lastVersionDirectoryPath, true);
            }

            Directory.CreateDirectory(lastVersionDirectoryPath);

            FillLatestVersionDirectory(files, lastVersionDirectoryPath);

            return files;
        }

        private IEnumerable<FileStream> CreateDifferentialPackage(ModuleBuilder moduleBuilder, string outputDirectory)
        {
            var files = new List<FileStream>();
            string lastVersionDirectoryPath = Path.Combine(outputDirectory, LatestVersionDirectory);

            foreach (var item in IncludedFiles)
            {
                files.AddRange(AddFiles(moduleBuilder, item, true, lastVersionDirectoryPath));
            }

            FillLatestVersionDirectory(files, lastVersionDirectoryPath);

            return files;
        }

        private void FillLatestVersionDirectory(List<FileStream> files, string lastVersionDirectoryPath)
        {
            foreach (FileStream file in files)
            {
                using (FileStream copy = new FileStream(lastVersionDirectoryPath + Path.GetFileName(file.Name), FileMode.Create))
                {
                    file.CopyTo(copy);
                }
            }
        }

        private IEnumerable<FileStream> AddFiles(ModuleBuilder moduleBuilder, FileSearchTemplate template, bool compareLatest, string lastVersionDirectoryPath)
        {
            var files = new List<FileStream>();
            var excludedFileNames = GetExcludedFileNames(ExcludedFiles);
            if (Directory.Exists(template.Directory))
            {
                foreach (var filePath in Directory.GetFiles(template.Directory, template.Pattern, template.SearchOption))
                {
                    if (!excludedFileNames.Contains(filePath))
                    {
                        if (compareLatest)
                        {
                            string latestFilePath = Path.Combine(lastVersionDirectoryPath, Path.GetFileName(filePath));
                            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            if (File.Exists(latestFilePath))
                            {
                                using (var latestFile = new FileStream(latestFilePath, FileMode.Open, FileAccess.Read))
                                {
                                    if (!file.AreEqual(latestFile))
                                    {
                                        moduleBuilder.DefineManifestResource(Path.GetFileName(filePath), file, ResourceAttributes.Public);
                                        files.Add(file);
                                    }
                                    else
                                    {
                                        file.Close();
                                    }
                                }
                            }
                            else
                            {
                                moduleBuilder.DefineManifestResource(Path.GetFileName(filePath), file, ResourceAttributes.Public);
                                files.Add(file);
                            }
                        }
                        else
                        {
                            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            moduleBuilder.DefineManifestResource(Path.GetFileName(filePath), file, ResourceAttributes.Public);
                            files.Add(file);
                        }
                    }
                }
            }

            return files;
        }

        private IEnumerable<String> GetExcludedFileNames(IEnumerable<FileSearchTemplate> filesNotForUpdate)
        {
            var result = new List<String>();

            foreach (var item in filesNotForUpdate)
            {
                if (Directory.Exists(item.Directory))
                {
                    result.AddRange(Directory.GetFiles(item.Directory, item.Pattern, item.SearchOption));
                }
            }

            return result;
        }

        private void Compress(string directory, string fileName)
        {
            using (var inFile = new FileStream(Path.Combine(directory, fileName), FileMode.Open, FileAccess.Read))
            {
                using (var outFile = new FileStream(Path.Combine(directory, fileName + PackageSuffix), FileMode.Create))
                {
                    using (var compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        inFile.CopyTo(compress);
                    }
                }
            }
            File.Delete(directory + fileName);
        }
    }
}
