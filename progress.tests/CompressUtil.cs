using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace progress.tests
{
    /// <summary>
    /// 文件压缩解压工具类
    /// </summary>
    public static class CompressUtil
    {
        #region 压缩

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩文件路径(包含文件名称)</param>
        /// <param name="zipFilePath">压缩文件路径(包含压缩后的文件名称)</param>
        /// <param name="comment">说明</param>
        /// <param name="pwd">密码</param>
        /// <param name="compressionLevel">压缩等级(0-9)</param>
        /// <returns></returns>
        public static void CompressFile(string sourceFilePath, string zipFilePath, string comment = null, string pwd = null, int compressionLevel = 9)
        {
            try
            {
                //检查源文件是否存在
                if (!File.Exists(sourceFilePath))
                    throw new FileNotFoundException($"该文件{sourceFilePath}不存在！");

                using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    zipOutputStream.Password = pwd;
                    zipOutputStream.SetComment(comment);
                    zipOutputStream.SetLevel(compressionLevel);

                    FileInfo fileInfo = new FileInfo(sourceFilePath);

                    using (FileStream readStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        ZipEntry zipEntry = new ZipEntry(Path.GetFileName(sourceFilePath));
                        zipEntry.DateTime = fileInfo.LastWriteTime;
                        zipEntry.Size = readStream.Length;
                        zipOutputStream.PutNextEntry(zipEntry);

                        var readLength = 0;
                        var bufferSize = 4096;
                        byte[] buffer = new byte[bufferSize];

                        do
                        {
                            readLength = readStream.Read(buffer, 0, bufferSize);
                            zipOutputStream.Write(buffer, 0, readLength);
                        }
                        while (readLength == bufferSize);

                        readStream.Close();
                    }

                    zipOutputStream.Flush();
                    zipOutputStream.Finish();
                    zipOutputStream.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("文件压缩异常", e);
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="zipFilePath"></param>
        /// <param name="comment"></param>
        /// <param name="pwd"></param>
        /// <param name="compressionLevel"></param>
        public static void CompressFolder(string sourcePath, string zipFilePath, string comment = null, string pwd = null, int compressionLevel = 9)
        {
            try
            {
                //检测源文件所属的文件夹是否存在
                var srcDirectory = Path.GetDirectoryName(sourcePath);
                if (!Directory.Exists(srcDirectory))
                    throw new FileNotFoundException($"该文件夹{sourcePath}不存在！");

                var dict = PrepareFiles(sourcePath);

                using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    zipStream.Password = pwd;
                    zipStream.SetComment(comment);
                    zipStream.SetLevel(compressionLevel);

                    foreach (string key in dict.Keys)
                    {
                        if (!Directory.Exists(key))
                        {
                            FileInfo fileInfo = new FileInfo(key);

                            using (FileStream readStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                ZipEntry zipEntry = new ZipEntry(dict[key]);
                                zipEntry.DateTime = fileInfo.LastWriteTime;
                                zipEntry.Size = readStream.Length;
                                zipStream.PutNextEntry(zipEntry);

                                var readLength = 0;
                                var bufferSize = 4096;
                                byte[] buffer = new byte[bufferSize];

                                do
                                {
                                    readLength = readStream.Read(buffer, 0, bufferSize);
                                    zipStream.Write(buffer, 0, readLength);
                                }
                                while (readLength == bufferSize);

                                readStream.Close();
                            }
                        }
                        else
                        {
                            ZipEntry zipEntry = new ZipEntry(dict[key] + "/");
                            zipStream.PutNextEntry(zipEntry);
                        }
                    }

                    zipStream.Flush();
                    zipStream.Finish();
                    zipStream.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("文件压缩异常", e);
            }
        }

        /// <summary>
        /// 准备待压缩文件对象(该方法遍历此文件夹下的所有文件)
        /// </summary>
        /// <param name="sourcePath">待压缩文件路径</param>
        /// <returns></returns>
        private static Dictionary<string, string> PrepareFiles(string sourcePath)
        {
            Dictionary<string, string> fileDict = new Dictionary<string, string>();

            if (sourcePath.EndsWith(@"\"))
                sourcePath = sourcePath.Remove(sourcePath.LastIndexOf(@"\"));

            var parentDirectoryPath = Path.GetDirectoryName(sourcePath) + @"\";

            //防止根目录下把盘符压入的错误
            if (parentDirectoryPath.EndsWith(@":\\"))
            {
                parentDirectoryPath = parentDirectoryPath.Replace(@"\\", @"\");
            }

            //获取目录中所有的文件系统对象
            var subDictionary = GetAllFiles(sourcePath, parentDirectoryPath);

            //将文件系统对象添加到总的文件字典中
            foreach (string key in subDictionary.Keys)
            {
                if (!fileDict.ContainsKey(key))//检测重复项
                {
                    fileDict.Add(key, subDictionary[key]);
                }
            }
            return fileDict;
        }

        /// <summary>
        /// 获取所有文件系统对象
        /// </summary>
        /// <param name="source">源路径</param>
        /// <param name="topDirectory">顶级文件夹</param>
        /// <returns>字典中Key为完整路径，Value为文件(夹)名称</returns>
        private static Dictionary<string, string> GetAllFiles(string source, string topDirectory)
        {
            var filesDict = new Dictionary<string, string>();
            filesDict.Add(source, source.Replace(topDirectory, ""));

            if (Directory.Exists(source))
            {
                //一次性获取下级所有目录，避免递归
                string[] directories = Directory.GetDirectories(source, "*.*", SearchOption.AllDirectories);
                foreach (string directory in directories)
                {
                    //Value为顶级文件夹的相对路径，使用此路径使压缩文件存在层级结构。
                    filesDict.Add(directory, directory.Replace(topDirectory, ""));
                    //filesDict.Add(directory, Path.GetDirectoryName(directory));
                }

                string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    filesDict.Add(file, file.Replace(topDirectory, ""));
                    //filesDict.Add(file, Path.GetFileName(file));
                }
            }

            return filesDict;
        }

        #endregion

        #region 解压

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFilePath">待解压文件全路径</param>
        /// <param name="targetFilePath">目标文件夹</param>
        /// <param name="pwd">密码</param>
        public static void DecompressFile(string zipFilePath, string targetFilePath = null, string pwd = null)
        {
            if (string.IsNullOrEmpty(zipFilePath) || !File.Exists(zipFilePath))
                throw new FileNotFoundException("压缩文件不存在！");

            try
            {
                if (string.IsNullOrEmpty(targetFilePath))
                    targetFilePath = Path.GetDirectoryName(zipFilePath);
                if (!Directory.Exists(targetFilePath))
                    Directory.CreateDirectory(targetFilePath);

                using (ZipInputStream zipStream = new ZipInputStream(File.Open(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    zipStream.Password = pwd;
                    ZipEntry zipEntry = zipStream.GetNextEntry();

                    while (zipEntry != null)
                    {
                        //如果是文件夹则创建
                        if (zipEntry.IsDirectory)
                        {
                            Directory.CreateDirectory(Path.Combine(targetFilePath, Path.GetDirectoryName(zipEntry.Name)));
                        }
                        else
                        {
                            string fileName = Path.GetFileName(zipEntry.Name);
                            if (!string.IsNullOrEmpty(fileName) && fileName.Trim().Length > 0)
                            {
                                FileInfo fileItem = new FileInfo(Path.Combine(targetFilePath, zipEntry.Name));
                                using (FileStream writeStream = fileItem.Create())
                                {
                                    var bufferSize = 4096;
                                    byte[] buffer = new byte[bufferSize];
                                    int readLength = 0;

                                    do
                                    {
                                        readLength = zipStream.Read(buffer, 0, bufferSize);
                                        writeStream.Write(buffer, 0, readLength);
                                    } while (readLength == bufferSize);

                                    writeStream.Flush();
                                    writeStream.Close();
                                }
                                fileItem.LastWriteTime = zipEntry.DateTime;
                            }
                        }
                        //获取下一个文件
                        zipEntry = zipStream.GetNextEntry();
                    }

                    zipStream.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("文件压缩异常", e);
            }
        }

        #endregion
    }
}
