using System;
using System.Configuration;

namespace MismaSesion
{
    public class NavegadorConfigurationElement : ConfigurationElement
    {
        public NavegadorConfigurationElement()
        {
        }
 
        [ConfigurationProperty("Nombre")]
        public string Nombre { get { return this["Nombre"] as string; } }
 
        [ConfigurationProperty("Perfiles", DefaultValue = "Default")]
        public string Perfiles { get { return this["Perfiles"] as string; } }
 
        [ConfigurationProperty("RutaLocal")]
        public string RutaLocal { get { return this["RutaLocal"] as string; } }
 
        [ConfigurationProperty("RutaServidor")]
        public string RutaServidor { get { return this["RutaServidor"] as string; } }
 
        [ConfigurationProperty("Archivos")]
        public string Archivos { get { return this["Archivos"] as string; } }
        [ConfigurationProperty("RutaServidorAux")]
        public string RutaServidorAux { get { return this["RutaServidorAux"] as string; } }
        
    }
 
    public class NavegadorConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("Navegador1")]
        public NavegadorConfigurationElement Navegador1
        {
            get { return this["Navegador1"] as NavegadorConfigurationElement; }
            set { this["Navegador1"] = value; }
        }
        [ConfigurationProperty("Navegador2")]
        public NavegadorConfigurationElement Navegador2
        {
            get { return this["Navegador2"] as NavegadorConfigurationElement; }
            set { this["Navegador2"] = value; }
        }
    }
}