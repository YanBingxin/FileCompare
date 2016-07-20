using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;

namespace FilesCompare.CompareHelper
{
    public class ZipHelper : IDisposable
    {
        /// <summary>
        /// 源文件夹路径
        /// </summary>
        public string SourceFolderPath { set; get; }

        public ZipHelper(string path)
        {
            SourceFolderPath = path;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="path"></param>
        public void ZipFolder(string path)
        {
            using (Package package = Package.Open(path, FileMode.Create))
            {
                DirectoryInfo di = new DirectoryInfo(SourceFolderPath);
                ZipDirectory(di, package);
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="di"></param>
        /// <param name="package"></param>
        private void ZipDirectory(DirectoryInfo di, Package package)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                string relativePath = fi.FullName.Replace(SourceFolderPath, string.Empty);
                relativePath = relativePath.Replace("\\", "/");

                Uri uri = new Uri(relativePath, UriKind.Relative);
                PackagePart part = package.CreatePart(uri, System.Net.Mime.MediaTypeNames.Application.Zip);

                using (FileStream fs = fi.OpenRead())
                {
                    CopyStream(fs, part.GetStream());
                }
            }

            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                ZipDirectory(subDi, package);
            }
        }

        private void CopyStream(FileStream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            this.Dispose();
        }

        #endregion
    }
}
