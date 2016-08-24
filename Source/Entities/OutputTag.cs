using Ada.Framework.Util.FileMonitor;
using System;
using System.Xml.Serialization;

namespace Ada.Framework.Development.Log4Me.Writers.RollingFileWrite.Entities
{
    public class OutputTag
    {
        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public string FileNameFormat { get; set; }
        
        [XmlAttribute]
        public string DateTimeFormat { get; set; }

        public string ObtenerRuta(int numeroArchivo)
        {
            string retorno = FileNameFormat;
            string fechaActual = string.Empty;

            if (!string.IsNullOrEmpty(DateTimeFormat) && !string.IsNullOrEmpty(DateTimeFormat.Trim()))
            {
                fechaActual = DateTime.Now.ToString(DateTimeFormat);
            }
            
            if (numeroArchivo > 1)
            {
                if (retorno.Contains("[FileNumber]"))
                {
                    retorno = retorno.Replace("[FileNumber]", numeroArchivo.ToString());
                }
                else
                {
                    if (retorno.LastIndexOf(".") != -1)
                    {
                        retorno = retorno.Substring(0, retorno.LastIndexOf(".")) + numeroArchivo + retorno.Substring(retorno.LastIndexOf("."));
                    }
                    else
                    {
                        retorno += numeroArchivo;
                    }
                }
            }
            else
            {
                retorno = retorno.Replace("[FileNumber]", string.Empty);
            }
            
            if (retorno.Contains("[DateTime]"))
            {
                retorno = retorno.Replace("[DateTime]", fechaActual);
            }

            IMonitorArchivo monitor = MonitorArchivoFactory.ObtenerArchivo();
            if(!Path.EndsWith(monitor.SEPARADOR_CARPETAS))
            {
                Path += monitor.SEPARADOR_CARPETAS;
            }

            monitor.PrepararDirectorio(Path);
            retorno = monitor.ObtenerRutaAbsoluta(Path + retorno);

            return retorno;
        }
    }
}