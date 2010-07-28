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
        private sealed class AssemblyProxy : MarshalByRefObject
        {
            public void ExtractUpdate(Version version, DirectoryInfo outputDirectory, string publicToken)
            {
                Assembly update = Assembly.ReflectionOnlyLoad(String.Format("{0}, Version={1}, Culture=neutral, PublicKeyToken={2}", string.Format(UpdatePackage.AssemblyName, version), version, publicToken));

                foreach (string name in update.GetManifestResourceNames())
                {
                    using (var fs = new FileStream(Path.Combine(outputDirectory.FullName, name), FileMode.Create))
                    {
                        update.GetManifestResourceStream(name).CopyTo(fs);
                    }
                }
            }
        }

        private readonly string _certificatePath;
        private readonly Version _version;
        private readonly bool _isDifferential;
        private readonly List<FileSearchTemplate> _included = new List<FileSearchTemplate>();
        private readonly List<FileSearchTemplate> _excluded = new List<FileSearchTemplate>();
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
        public IList<FileSearchTemplate> IncludedFiles
        {
            get
            {
                return _included;
            }
        }

        /// <summary>
        /// List of templates for files that are NOT included in update.
        /// </summary>
        public IList<FileSearchTemplate> ExcludedFiles
        {
            get
            {
                return _excluded;
            }
        }

        /// <summary>
        /// Creates normalized Update Package for BitsUpdater library.
        /// </summary>
        /// <param name="outputDirectory">Directory where to save files. It is recommended to use one directory because of program looking for Last Version files during differential update.</param>
        /// <returns>Assembly Thumbprint as string for retrieving assembly content during unpacking phase.</returns>
        public string Create(DirectoryInfo outputDirectory)
        {
            using (var certificate = new FileStream(_certificatePath, FileMode.Open, FileAccess.Read))
            {
                FileInfo fileName = new FileInfo(string.Format(AssemblyName + AssemblySuffix, _version));
                var name = new AssemblyName(string.Format(AssemblyName, _version))
                {
                    Version = _version,
                    KeyPair = new StrongNameKeyPair(certificate),
                };
                var builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save, outputDirectory.FullName);
                var moduleBuilder = builder.DefineDynamicModule(name.Name, fileName.Name);

                IEnumerable<FileStream> files;
                if (_isDifferential && Directory.Exists(Path.Combine(outputDirectory.FullName, LatestVersionDirectory)))
                {
                    files = CreateDifferentialPackage(moduleBuilder, outputDirectory);
                }
                else
                {
                    files = CreatePackage(moduleBuilder, outputDirectory);
                }

                builder.Save(fileName.Name);

                foreach (var item in files)
                {
                    item.Dispose();
                }

                Compress(outputDirectory, fileName.Name);
                return RetrievePublicTokenString(name);
            }
        }

        internal static void Extract(DirectoryInfo outputDirectory, Version version, string publicToken)
        {
            FileInfo packageLocation = new FileInfo(Path.Combine(outputDirectory.Parent.FullName, string.Format(AssemblyName, version) + AssemblySuffix + PackageSuffix));
            FileInfo assemblyLocation = new FileInfo(Path.Combine(Assembly.GetEntryAssembly().GetDirectory(), string.Format(AssemblyName, version) + AssemblySuffix));
            CopyPreviousFiles(outputDirectory);
            Decompress(packageLocation, assemblyLocation);
            ExtractFiles(version, outputDirectory, publicToken, assemblyLocation);
        }

        private static void CopyPreviousFiles(DirectoryInfo outputDirectory)
        {
            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            DirectoryInfo versionDirectory = outputDirectory.Parent;
            Version max = new Version();
            foreach (var item in versionDirectory.GetDirectories())
            {
                try
                {
                    var current = new Version(item.Name);
                    if (current > max)
                    {
                        max = current;
                    }
                }
                catch (ArgumentException) { /* Version try parse */ }
                catch (OverflowException) { /* Version try parse */ }
                catch (FormatException) { /* Version try parse */ }
            }

            outputDirectory.Create();

            if (max > new Version())
            {
                foreach (var item in Directory.GetFiles(Path.Combine(versionDirectory.FullName, max.ToString())))
                {
                    if (!item.EndsWith(UpdateStatus.UpdateStatusFileName, true, CultureInfo.InvariantCulture))
                    {
                        using (FileStream input = new FileStream(item, FileMode.Open, FileAccess.Read))
                        {
                            using (FileStream output = new FileStream(Path.Combine(outputDirectory.FullName, Path.GetFileName(item)), FileMode.OpenOrCreate))
                            {
                                input.CopyTo(output);
                            }
                        }
                    }
                }
            }
        }

        private static void ExtractFiles(Version version, DirectoryInfo outputDirectory, string publicToken, FileInfo assemblyLocation)
        {
            var domain = AppDomain.CreateDomain("TemporaryDomain");
            var proxy = domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(typeof(AssemblyProxy)).FullName, typeof(AssemblyProxy).ToString()) as AssemblyProxy;
            if (proxy != null)
            {
                proxy.ExtractUpdate(version, outputDirectory, publicToken);
            }
            AppDomain.Unload(domain);
            assemblyLocation.Delete();
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

        private IEnumerable<FileStream> CreatePackage(ModuleBuilder moduleBuilder, DirectoryInfo outputDirectory)
        {
            var files = new List<FileStream>();

            foreach (var item in IncludedFiles)
            {
                files.AddRange(AddFiles(moduleBuilder, item, false, null));
            }

            DirectoryInfo lastVersionDirectoryPath = new DirectoryInfo(Path.Combine(outputDirectory.FullName, LatestVersionDirectory));
            if (lastVersionDirectoryPath.Exists)
            {
                lastVersionDirectoryPath.Delete(true);
            }

            lastVersionDirectoryPath.Create();

            FillLatestVersionDirectory(files, lastVersionDirectoryPath);

            return files;
        }

        private IEnumerable<FileStream> CreateDifferentialPackage(ModuleBuilder moduleBuilder, DirectoryInfo outputDirectory)
        {
            var files = new List<FileStream>();
            DirectoryInfo lastVersionDirectoryPath = new DirectoryInfo(Path.Combine(outputDirectory.FullName, LatestVersionDirectory));

            foreach (var item in IncludedFiles)
            {
                files.AddRange(AddFiles(moduleBuilder, item, true, lastVersionDirectoryPath));
            }

            FillLatestVersionDirectory(files, lastVersionDirectoryPath);

            return files;
        }

        private void FillLatestVersionDirectory(List<FileStream> files, DirectoryInfo lastVersionDirectoryPath)
        {
            foreach (FileStream file in files)
            {
                using (FileStream copy = new FileStream(Path.Combine(lastVersionDirectoryPath.FullName, Path.GetFileName(file.Name)), FileMode.Create))
                {
                    file.CopyTo(copy);
                }
            }
        }

        private IEnumerable<FileStream> AddFiles(ModuleBuilder moduleBuilder, FileSearchTemplate template, bool compareLatest, DirectoryInfo lastVersionDirectoryPath)
        {
            var files = new List<FileStream>();
            var excludedFileNames = GetExcludedFileNames(ExcludedFiles);
            if (template.Directory.Exists)
            {
                foreach (var fileInfo in template.Directory.GetFiles(template.Pattern, template.SearchOption))
                {
                    if (!ExcludedContains(fileInfo, excludedFileNames))
                    {
                        var file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                        if (compareLatest)
                        {
                            FileInfo latestFilePath = new FileInfo(Path.Combine(lastVersionDirectoryPath.FullName, fileInfo.Name));
                            if (latestFilePath.Exists)
                            {
                                using (var latestFile = new FileStream(latestFilePath.FullName, FileMode.Open, FileAccess.Read))
                                {
                                    if (!file.AreEqual(latestFile))
                                    {
                                        AddFile(moduleBuilder, fileInfo, file, files);
                                    }
                                    else
                                    {
                                        file.Close();
                                    }
                                }
                            }
                            else
                            {
                                AddFile(moduleBuilder, fileInfo, file, files);
                            }
                        }
                        else
                        {
                            AddFile(moduleBuilder, fileInfo, file, files);
                        }
                    }
                }
            }

            return files;
        }

        private bool ExcludedContains(FileInfo fileInfo, IEnumerable<FileInfo> excluded)
        {
            foreach (var item in excluded)
            {
                if (fileInfo.FullName == item.FullName)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddFile(ModuleBuilder moduleBuilder, FileInfo filePath, FileStream file, List<FileStream> files)
        {
            moduleBuilder.DefineManifestResource(filePath.Name, file, ResourceAttributes.Public);
            files.Add(file);
        }

        private IEnumerable<FileInfo> GetExcludedFileNames(IEnumerable<FileSearchTemplate> filesNotForUpdate)
        {
            var result = new List<FileInfo>();

            foreach (var item in filesNotForUpdate)
            {
                if (item.Directory.Exists)
                {
                    result.AddRange(item.Directory.GetFiles(item.Pattern, item.SearchOption));
                }
            }

            return result;
        }

        private void Compress(DirectoryInfo directory, string fileName)
        {
            string inFilePath = Path.Combine(directory.FullName, fileName);
            string outFilePath = Path.Combine(directory.FullName, fileName + PackageSuffix);
            using (var inFile = new FileStream(inFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var outFile = new FileStream(outFilePath, FileMode.Create))
                {
                    using (var compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        inFile.CopyTo(compress);
                    }
                }
            }
            File.Delete(inFilePath);
        }

        private static void Decompress(FileInfo packageName, FileInfo assemblyLocation)
        {
            using (var inFile = new FileStream(packageName.FullName, FileMode.Open))
            {
                using (var outFile = new FileStream(assemblyLocation.FullName, FileMode.Create))
                {
                    using (var decompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        decompress.CopyTo(outFile);
                    }
                }
            }
            packageName.Delete();
        }
    }
}
