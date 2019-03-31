using ColaApp.Core.DI.Interfaces;
using Dna;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColaApp.Core.DI
{
    public class CoreDI
    {
        /// <summary>
        /// A shortcut to access the <see cref="IFileManager"/>
        /// </summary>
        public static IFileManager FileManager => Framework.Service<IFileManager>();
    }
}
