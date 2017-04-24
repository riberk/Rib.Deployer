namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using JetBrains.Annotations;

    internal static class FsHelper
    {
        public static void CopyDirectory([NotNull] DirectoryInfo src, [NotNull] DirectoryInfo dst)
        {
            src.Refresh();
            dst.Refresh();
            var dirs = src.GetDirectories();
            if (!dst.Exists)
            {
                dst.Create();
            }

            var files = src.GetFiles();
            foreach (var file in files)
            {
                var filePath = Path.Combine(dst.FullName, file.Name);
                file.CopyTo(filePath, false);
            }

            foreach (var subdir in dirs)
            {
                var dirPath = Path.Combine(dst.FullName, subdir.Name);
                CopyDirectory(subdir, new DirectoryInfo(dirPath));
            }
        }
    }
}