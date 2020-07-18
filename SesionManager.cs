using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;

namespace MismaSesion
{
    public class SesionManager
    {
        public string Nombre { get; set; }
        public string Origen { get; }
        public string Destino { get; }
        public string RutaAux { get; }
        public string[] Archivos { get; }
        public string[] Perfiles { get; }
        public string Proceso { get; set; }

        public SesionManager(NavegadorConfigurationElement nav, string proceso)
        {
            this.Nombre = nav.Nombre;
            this.RutaAux = nav.RutaServidorAux;
            this.Perfiles = nav.Perfiles.Split(";");
            this.Archivos = nav.Archivos.Split(";");
            
            if (proceso.ToLower().Trim() == "cierre")
            {
                Origen = nav.RutaLocal;
                Destino = nav.RutaServidor;
                Proceso = "cierre";
            }
            else if (proceso.ToLower().Trim() == "inicio")
            {
                Origen = nav.RutaServidor;
                Destino = nav.RutaLocal;
                Proceso = "inicio";
            }
            else
            {
                throw new Exception($"Argumento invalido: {proceso.ToLower().Trim()}");
            }
        }
        public void CopiarArchivos()
        {
            Log($"--------Inicia el proceso de copiado {this.Nombre}");
            foreach (string perfil in Perfiles)
            {
                foreach (string archivo in this.Archivos)
                {
                    string archivoOrigen = "", archivoDestino = "", archivoRutaAux = "";
                    archivoOrigen = ArmarRutaArchivo(this.Origen, perfil, archivo);
                    try
                    {
                        archivoDestino = ArmarRutaArchivo(this.Destino, perfil, archivo);
                    }
                    catch (System.Exception)
                    {
                        Log($"Error al armar la ruta de destino: {this.Destino} Continúa con la auxiliar.");
                    }
                    archivoRutaAux = ArmarRutaArchivo(this.RutaAux, perfil, archivo);

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
            Log($"--------Finaliza el proceso de copiado {this.Nombre}");
        }

        private void Cierre(string origen, string destino, string aux)
        {
            Log($"###   Cierre - Origen: {origen}, Destino: {destino}, Aux: {aux}");
            if (!Copiar(origen, destino))
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
            catch (Exception e)
            {
                Log($"UltimaModificacion - Exception: {e}");
                resultado = DateTime.MinValue;
            }
            return resultado;
        }

        private bool Copiar(string origen, string destino)
        {
            bool resultado = true;
            try
            {
                File.Copy(origen, destino, true);
                Log($"Copiar - Finalizado: {origen} -> {destino}");
            }
            catch (System.ArgumentException)
            {
                Log($"Copiar - No se informa ruta destino");
                resultado = false;
            }
            catch (System.IO.FileNotFoundException)
            {
                Log($"Copiar - Archivo inexistente: {origen}");
                resultado = false;
            }
            catch (System.IO.IOException)
            {
                Log($"Copiar - Ruta destino protegido contra escritura: {destino}");
                resultado = false;
            }
            return resultado;
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
                    try
                    {
                        Directory.CreateDirectory(resultado);
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        Log($"Access to the path {resultado} is denied");
                    }
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
