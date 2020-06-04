using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;

namespace MismaSesion
{
    public class SesionManager
    {
        public string Origen { get; }
        public string Destino { get; }
        public string RutaAux { get; }
        public string[] Archivos { get; }
        public string[] Perfiles { get; }
        public string Proceso { get; set; }


        public SesionManager(string proceso)
        {
            this.RutaAux = ConfigurationManager.AppSettings.Get("RutaServidorAux");
            this.Perfiles = ConfigurationManager.AppSettings.Get("Perfiles").Split(";");
            this.Archivos = ConfigurationManager.AppSettings.Get("Archivos").Split(";");

            if (proceso.ToLower().Trim() == "cierre")
            {
                Origen = ConfigurationManager.AppSettings.Get("RutaLocal");
                Destino = ConfigurationManager.AppSettings.Get("RutaServidor");
                Proceso = "cierre";
            }
            else if (proceso.ToLower().Trim() == "inicio")
            {
                Origen = ConfigurationManager.AppSettings.Get("RutaServidor");
                Destino = ConfigurationManager.AppSettings.Get("RutaLocal");
                Proceso = "inicio";
            }
            else
            {
                throw new Exception($"Argumento invalido: {proceso.ToLower().Trim()}");
            }
        }
        public void CopiarArchivos()
        {
            Log("--------Inicia el proceso de copiado");
            foreach (string perfil in Perfiles)
            {
                foreach (string archivo in this.Archivos)
                {
                    var archivoOrigen = ArmarRutaArchivo(this.Origen, perfil, archivo);
                    var archivoDestino = ArmarRutaArchivo(this.Destino, perfil, archivo);
                    var archivoRutaAux = ArmarRutaArchivo(this.RutaAux, perfil, archivo);

                    if (Proceso == "inicio")
                    {
                        Inicio(archivoOrigen, archivoDestino, archivoRutaAux);
                    }
                    else
                    {
                        Cierre(archivoOrigen, archivoDestino, archivoRutaAux);
                    }
                }
            }
            Log("--------Finaliza el proceso de copiado");
        }

        private void Cierre(string origen, string destino, string aux)
        {
            Log($"Cierre - Origen: {origen}, Destino: {destino}, Aux: {aux}");
            try
            {
                Copiar(origen, destino);
            }
            catch (System.IO.IOException)
            {
                Copiar(origen, aux);
            }
        }

        private void Inicio(string origen, string destino, string aux)
        {
            Log($"Inicio - Origen: {origen}, Destino: {destino}, Aux: {aux}");
            DateTime ultModOrigen = UltimaModificacion(origen);
            DateTime ultModDestino = UltimaModificacion(destino);
            DateTime ultModRutaAux = UltimaModificacion(aux);
           
            if (ultModOrigen > ultModRutaAux)
            {
                if (ultModOrigen > ultModDestino)
                {
                    Copiar(origen, destino);
                    return;
                }
            }
            else
            {
                if (ultModRutaAux > ultModDestino)
                {
                    Copiar(aux, destino);
                    return;
                }
            }
            Log("No se realiza la copia");
        }

        private DateTime UltimaModificacion(string ruta)
        {
            DateTime resultado;
            try
            {
                resultado = File.GetLastWriteTime(ruta);
            }
            catch (System.ArgumentException)
            {
                resultado = DateTime.MinValue;
            }
            return resultado;
        }

        private void Copiar(string origen, string destino)
        {
            File.Copy(origen, destino, true);
            Log($"Copia finalizada {origen} -> {destino}");
        }

        private string ArmarRutaArchivo(string ruta, string perfil, string archivo)
        {
            string resultado = string.Empty;
            if (!string.IsNullOrEmpty(ruta))
            {
                resultado = Path.Combine(ruta.Trim(), perfil.Trim());
                if (!Directory.Exists(resultado))
                {
                    Log($"Creando directorio {resultado}");
                    Directory.CreateDirectory(resultado);
                }
                resultado = Path.Combine(resultado, archivo.Trim());
            }
            return resultado;
        }

        private void SistemaOperativo()
        {
            //Validar sistema operativo
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotImplementedException();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception("Sistema operativo no encontrado");
            }
        }

        private void Log(string mensaje)
        {
            Console.WriteLine($"{DateTime.Now}\t{mensaje}");
        }

    }
}
