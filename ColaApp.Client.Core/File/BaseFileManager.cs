using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ColaApp.Core.File
{
    public class BaseFileManager : DI.Interfaces.IFileManager
    {
        public void CombineFile(string[] infileNames, string outfileName)
        {
            int b;
            int n = infileNames.Length;
            FileStream[] fileIn = new FileStream[n];
            using (FileStream fileOut = new FileStream(outfileName, FileMode.Create))
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        fileIn[i] = new FileStream(infileNames[i], FileMode.Open);
                        while ((b = fileIn[i].ReadByte()) != -1)
                            fileOut.WriteByte((byte)b);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        fileIn[i].Close();
                    }

                }
            }
        }
    }
}
