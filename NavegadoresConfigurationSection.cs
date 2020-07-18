using System;
using System.Configuration;

namespace MismaSesion
{
    public class NavegadoresConfigurationElementCollection : ConfigurationElementCollection
    {
 
        /* overrides */       
        protected override ConfigurationElement CreateNewElement()
        {
            return new NavegadorConfigurationElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as NavegadorConfigurationElement).Nombre;
        }
 
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName
        {
            get { return "Navegador"; }
        }
 
        /* public */
        public int IndexOf(string Nombre)
        {
            Nombre = Nombre.ToLower();
 
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Nombre.ToLower() == Nombre)
                    return i;
            }
            return -1;
        }
 
        /* properties */
        public new NavegadorConfigurationElement this[string Code]
        {
            get
            {
                int Index = IndexOf(Code);
                return Index >= 0 ? BaseGet(Index) as NavegadorConfigurationElement : null;
            }
        }
        public NavegadorConfigurationElement this[int Index] { get { return BaseGet(Index) as NavegadorConfigurationElement; } }
 
    }
 
    public class NavegadoresConfigurationSection : ConfigurationSection
    {
        public NavegadoresConfigurationSection()
        {
        }
 
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public NavegadoresConfigurationElementCollection Navegadores { get { return base[""] as NavegadoresConfigurationElementCollection; } } 
    }
}