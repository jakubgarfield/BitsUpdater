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
using System.Globalization;

namespace BitsUpdater
{
    /// <summary>
    /// Class for creating and unpacking normalised BitsUpdater update packages that are used in whole update process.
    /// </summary>
    public sealed class UpdatePackage
    {
        private readonly string _certificatePath;
        private readonly Version _version;
        private readonly bool _isDifferential;
        internal const string LatestVersionDirectory = "LatestVersion\\";
        public const string AssemblyName = "Update.{0}";
        public const string AssemblySuffix = ".dll";
        public const string PackageSuffix = ".gz";

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
        /// Creates normalized Update Package for BitsUpdater library.
        /// </summary>
        /// <param name="outputDirectory">Directory where to save files. It is recommended to use one directory because of program looking for Last Version files during differential update.</param>
        /// <returns>Assembly Thumbprint as string for retrieving assembly content during unpacking phase.</returns>
        public string Create(string outputDirectory)
        {
            using (var certificate = new FileStream(_certificatePath, FileMode.Open, FileAccess.Read))
            {
                string fileName = string.Format(AssemblyName + AssemblySuffix, _version);
                var name = new AssemblyName(string.Format(AssemblyName, _version))
                {
                    Version = _version,
                    KeyPair = new StrongNameKeyPair(certificate),
                };
                var builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save, outputDirectory);
                var moduleBuilder = builder.DefineDynamicModule(name.Name, fileName);

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
                return RetrievePublicTokenString(name);
            }
        }

        public static void Extract(string outputDirectory, Version version, string publicToken)
        {
            CopyPreviousFiles(outputDirectory);
            Decompress(string.Format(AssemblyName, version) + AssemblySuffix);
            ExtractFiles(version, outputDirectory, publicToken);
        }

        private static void CopyPreviousFiles(string outputDirectory)
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }

            string versionDirectory = Path.GetDirectoryName(outputDirectory);
            Version max = new Version();
            foreach (var item in Directory.GetDirectories(versionDirectory))
            {
                try
                {
                    var current = new Version(Path.GetFileName(item));
                    if (current > max)
                    {
                        max = current;
                    }
                }
                catch (ArgumentException) { /* Version try parse */ }
                catch (OverflowException) { /* Version try parse */ }
                catch (FormatException) { /* Version try parse */ }

            }

            Directory.CreateDirectory(outputDirectory);

            if (max > new Version())
            {
                foreach (var item in Directory.GetFiles(Path.Combine(versionDirectory, max.ToString())))
                {
                    using (FileStream input = new FileStream(item, FileMode.Open, FileAccess.Read))
                    {
                        using (FileStream output = new FileStream(Path.Combine(outputDirectory, Path.GetFileName(item)), FileMode.OpenOrCreate))
                        {
                            input.CopyTo(output);
                        }
                    }
                }
            }
        }

        private static void ExtractFiles(Version version, string outputDirectory, string publicToken)
        {
            var domain = AppDomain.CreateDomain("TemporaryDomain");
            var proxy = domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(typeof(AssemblyProxy)).FullName, typeof(AssemblyProxy).ToString()) as AssemblyProxy;
            if (proxy != null)
            {
                proxy.ExtractUpdate(version, outputDirectory, publicToken);
            }
            AppDomain.Unload(domain);
            File.Delete(string.Format(AssemblyName, version) + AssemblySuffix);
        }

        private string RetrievePublicTokenString(AssemblyName name)
        {
            var builder = new StringBuilder();
            var token = name.GetPublicKeyToken();
            for (int i = 0; i < token.GetLength(0); i++)
            {
                builder.Append(token[i].ToString("x"));
            }
            return builder.ToString();
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
                        var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        if (compareLatest)
                        {
                            string latestFilePath = Path.Combine(lastVersionDirectoryPath, Path.GetFileName(filePath));
                            if (File.Exists(latestFilePath))
                            {
                                using (var latestFile = new FileStream(latestFilePath, FileMode.Open, FileAccess.Read))
                                {
                                    if (!file.AreEqual(latestFile))
                                    {
                                        AddFile(moduleBuilder, filePath, file, files);
                                    }
                                    else
                                    {
                                        file.Close();
                                    }
                                }
                            }
                            else
                            {
                                AddFile(moduleBuilder, filePath, file, files);
                            }
                        }
                        else
                        {
                            AddFile(moduleBuilder, filePath, file, files);
                        }
                    }
                }
            }

            return files;
        }

        private void AddFile(ModuleBuilder moduleBuilder, string filePath, FileStream file, List<FileStream> files)
        {
            moduleBuilder.DefineManifestResource(Path.GetFileName(filePath), file, ResourceAttributes.Public);
            files.Add(file);
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
            File.Delete(Path.Combine(directory, fileName));
        }

        private static void Decompress(string fileName)
        {
            using (var inFile = new FileStream(fileName + PackageSuffix, FileMode.Open))
            {
                using (var outFile = new FileStream(fileName, FileMode.Create))
                {
                    using (var decompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        decompress.CopyTo(outFile);
                    }
                }
            }
            File.Delete(fileName + PackageSuffix);
        }
    }
}
