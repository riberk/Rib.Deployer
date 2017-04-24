namespace TestInfrastructure
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class TestFsHelper
    {
        public static string AbsPath(string relativePath)
        {
            var sourceCodeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            return Path.Combine(Path.GetDirectoryName(sourceCodeBase), relativePath);
        }

        public static void Copy(string relativeSource)
        {
            CopyTo(relativeSource, relativeSource);
        }

        public static void CopyTo(string relativeSource, string relativeDest)
        {
            var source = AbsPath(relativeSource);
            var dest = Path.Combine(Directory.GetCurrentDirectory(), relativeDest);
            if (File.Exists(source))
            {
                CopyFile(source, dest);
            }
            else if (Directory.Exists(source))
            {
                CopyDirectory(source, dest);
            }
        }

        public static void CopyToDirectory(string relativeSource, string relativeDestDir)
        {
            var source = AbsPath(relativeSource);
            if (File.Exists(source))
            {
                var dest = Path.Combine(Directory.GetCurrentDirectory(), relativeDestDir, Path.GetFileName(source));
                CopyFile(source, dest);
            }
            else if (Directory.Exists(source))
            {
                var srcDi = new DirectoryInfo(source);
                var dest = Path.Combine(Directory.GetCurrentDirectory(), relativeDestDir, srcDi.Name);
                CopyDirectory(source, dest);
            }

        }

        private static void CopyFile(string source, string dest)
        {
            var directoryName = Path.GetDirectoryName(dest);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Copy(source, dest, true);
        }

        private static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);
            if (diTarget.Exists)
            {
                diTarget.Delete(true);
            }
            diTarget.Refresh();
            CopyAll(diSource, diTarget);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Directory.CreateDirectory(target.FullName);

            foreach (var fi in source.GetFiles())
            {
                CopyFile(fi.FullName, Path.Combine(target.FullName, fi.Name));
            }

            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}