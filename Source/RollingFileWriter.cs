using Ada.Framework.Development.Log4Me.Entities;
using Ada.Framework.Development.Log4Me.Writers.RollingFileWrite.Entities;
using Ada.Framework.Util.FileMonitor;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Ada.Framework.Development.Log4Me.Writers.RollingFileWrite
{
    [Serializable]
    public class RollingFileWriter : ALogWriter
    {
        private static FileStream file;
        private static StreamWriter writer;
        public static DateTime FechaUltimoArchivoCreado { get; private set; }
        public static int NumeroArchivo { get; private set; }
        public static string RutaActualArchivo { get; private set; }

        [XmlElement("Rolling")]
        public RollingTag Rolling { get; set; }

        [XmlElement("Output")]
        public OutputTag Output { get; set; }

        protected override void Agregar(RegistroInLineTO registro)
        {
            Inicializar();
            writer.WriteLine(Formatear(registro));
        }

        public override void Inicializar()
        {
            IMonitorArchivo monitor = MonitorArchivoFactory.ObtenerArchivo();

            if (NumeroArchivo == 0)
            {
                NumeroArchivo = 1;
            }

            if (Rolling.By.Equals("Date", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Rolling.Unit.Equals("Day", StringComparison.InvariantCultureIgnoreCase))
                {
                    checked
                    {
                        if (FechaUltimoArchivoCreado.AddDays((int)Rolling.Value) >= DateTime.Now)
                        {
                            FechaUltimoArchivoCreado = DateTime.Now;
                            NumeroArchivo++;
                        }
                    }
                }
                else if (Rolling.Unit.Equals("Week", StringComparison.InvariantCultureIgnoreCase))
                {
                    checked
                    {
                        int diferenciaDias = (int)Rolling.Value;
                        diferenciaDias *= 7;

                        if (FechaUltimoArchivoCreado.AddDays(diferenciaDias) >= DateTime.Now)
                        {
                            FechaUltimoArchivoCreado = DateTime.Now;
                            NumeroArchivo++;
                        }
                    }
                }
                else if (Rolling.Unit.Equals("Month", StringComparison.InvariantCultureIgnoreCase))
                {
                    checked
                    {
                        if (FechaUltimoArchivoCreado.AddMonths((int)Rolling.Value) >= DateTime.Now)
                        {
                            FechaUltimoArchivoCreado = DateTime.Now;
                            NumeroArchivo++;
                        }
                    }
                }
                else if (Rolling.Unit.Equals("Year", StringComparison.InvariantCultureIgnoreCase))
                {
                    checked
                    {
                        if (FechaUltimoArchivoCreado.AddYears((int)Rolling.Value) >= DateTime.Now)
                        {
                            FechaUltimoArchivoCreado = DateTime.Now;
                            NumeroArchivo++;
                        }
                    }
                }
            }
            else if (Rolling.By.Equals("Size", StringComparison.InvariantCultureIgnoreCase))
            {
                if (monitor.Existe(RutaActualArchivo))
                {
                    //Tamaño en MB.
                    double tamañoArchivo = (new FileInfo(RutaActualArchivo).Length / 1048576.0);

                    double tamañoLimite = Rolling.Value;

                    if (Rolling.Unit.Equals("GB", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tamañoLimite *= 1024;
                    }

                    if (Rolling.Unit.Equals("MB", StringComparison.InvariantCultureIgnoreCase)
                        || Rolling.Unit.Equals("GB", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (tamañoArchivo > tamañoLimite)
                        {
                            FechaUltimoArchivoCreado = DateTime.Now;
                            NumeroArchivo++;
                        }
                    }
                }
            }

            string ruta = Output.ObtenerRuta(NumeroArchivo);
            if (RutaActualArchivo != ruta)
            {
                file = new FileStream(ruta, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                writer = new StreamWriter(file) { AutoFlush = true };
            }

            RutaActualArchivo = ruta;
        }

        public override void AgregarParametros()
        {
            Inicializar();
            writer.WriteLine(FormatoSalida);
            writer.WriteLine(SeparadorSalida);
        }
    }
}
