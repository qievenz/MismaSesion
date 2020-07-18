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
            string proceso = "cierre";
            #if (!DEBUG)
                proceso = args[0];
            #endif

            NavegadoresConfigurationSection navegadoresSeccion = ConfigurationManager.GetSection("Navegadores") as NavegadoresConfigurationSection;
            for (int i = 0; i < navegadoresSeccion.Navegadores.Count; i++)
            {
                NavegadorConfigurationElement nav = navegadoresSeccion.Navegadores[i];
                var manager = new SesionManager(nav, proceso);
                manager.CopiarArchivos();
            }
        }
    }
}
