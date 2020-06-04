using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MismaSesion
{
    class Program
    {
        static void Main(string[] args)
        {
            string proceso = "inicio";
            #if (!DEBUG)
                proceso = args[0];
            #endif
            var a = new SesionManager(proceso);
            a.CopiarArchivos();
        }

    }
}
