using System;
using System.Collections.Generic;
using System.Text;

namespace ColaApp.Core.DI.Interfaces
{
    /// <summary>
    /// Handles reading/writing and querying the file system
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// 读取n个文本文件，把文件内容合并到一个文本文件中
        /// </summary>
        /// <param name="infileNames">输入文件数组</param>
        /// <param name="outfileName">输出文件</param>
        void CombineFile(string[] infileNames, string outfileName);
    }
}
