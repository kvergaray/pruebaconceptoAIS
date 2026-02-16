using IBM.WMQ;
using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace pruebaconceptoAIS
{
    class Program
    {
        protected MQQueueManager mqQueueManager;
        static string strQueueManagerName = ConfigurationManager.AppSettings["strQueueManagerName"];
        static string strChannelName = ConfigurationManager.AppSettings["strChannelName"];
        static string strQueueName = ConfigurationManager.AppSettings["strQueueName"];
        static string strServerName = ConfigurationManager.AppSettings["strServerName"];
        static int intPort = Convert.ToInt32(ConfigurationManager.AppSettings["intPort"]);
        static string user = ConfigurationManager.AppSettings["user"];
        static string password = ConfigurationManager.AppSettings["password"];
        static string rutaArchivo = ConfigurationManager.AppSettings["xmlPath"]+ConfigurationManager.AppSettings["fileName"] +  ".xml"; 
        static void Main(string[] args)
        {
            string strMsgAux = @"<NS1:IATA_AIDX_FlightLegNotifRQ xmlns:NS1=""http://www.iata.org/IATA/2007/00"" TimeStamp=""2022-01-05T15:55:43Z"" Version=""17.1"">
                                    <NS1:FlightLeg>
                                        <NS1:LegIdentifier>
                                            <NS1:Airline CodeContext=""3"">4A</NS1:Airline>
                                            <NS1:FlightNumber>035</NS1:FlightNumber>
                                            <NS1:DepartureAirport CodeContext=""3"">PIS</NS1:DepartureAirport>
                                            <NS1:ArrivalAirport CodeContext=""3"">LIM</NS1:ArrivalAirport>
                                            <NS1:OriginDate>2022-01-04T17:05:00Z</NS1:OriginDate>
                                        </NS1:LegIdentifier>
                                        <NS1:SpecialAction>Delete</NS1:SpecialAction>
                                        <NS1:LegData>
                                            <NS1:ServiceType/>
                                            <NS1:EstFlightDuration>PT1H00M.000S</NS1:EstFlightDuration>
                                            <NS1:CodeShareInfo RepeatIndex=""1"">
                                                <NS1:Airline CodeContext=""3"">IB</NS1:Airline>
                                                <NS1:FlightNumber>2002</NS1:FlightNumber>
                                            </NS1:CodeShareInfo>
                                            <NS1:AssociatedFlightLegSchedule FlightSequence=""upline"" RepeatIndex=""1"">
                                                <NS1:DepartureAirport CodeContext=""3"">YYZ</NS1:DepartureAirport>
                                                <NS1:ArrivalAirport CodeContext=""3"">JFK</NS1:ArrivalAirport>
                                            </NS1:AssociatedFlightLegSchedule>
                                            <NS1:RemarkTextCode CodeContext=""9750"" Qualifier=""PUB"" RepeatIndex=""1"">DX</NS1:RemarkTextCode>
                                            <NS1:RemarkTextCode CodeContext=""9750"" Qualifier=""BAG"" RepeatIndex=""2"">DX</NS1:RemarkTextCode>
                                            <NS1:RemarkTextCode CodeContext=""9750"" Qualifier=""LND"" RepeatIndex=""3"">DX</NS1:RemarkTextCode>
                                            <NS1:RemarkTextCode CodeContext=""9750"" Qualifier=""STF"" RepeatIndex=""4"">DX</NS1:RemarkTextCode>
                                            <NS1:AirportResources Usage=""Actual"">
                                                <NS1:Resource DepartureOrArrival=""Arrival"">
                                                    <NS1:Runway/>
                                                    <NS1:AircraftTerminal>1</NS1:AircraftTerminal>
                                                    <NS1:PublicTerminal>1</NS1:PublicTerminal>
                                                </NS1:Resource>
                                            </NS1:AirportResources>
                                            <NS1:OperationTime CodeContext=""9750"" OperationQualifier=""ONB"" RepeatIndex=""1"" TimeType=""SCT"">2022-01-04T17:06:00Z</NS1:OperationTime>
                                            <NS1:OperationTime CodeContext=""9750"" OperationQualifier=""ONB"" RepeatIndex=""2"" TimeType=""EST"">2022-01-04T16:50:00Z</NS1:OperationTime>
                                            <NS1:OperationTime CodeContext=""9750"" OperationQualifier=""ONB"" RepeatIndex=""3"" TimeType=""ACT"">2022-01-04T18:55:00Z</NS1:OperationTime>
                                            <NS1:AircraftInfo>
                                                <NS1:AircraftSubType/>
                                                <NS1:Registration/>
                                            </NS1:AircraftInfo>
                                            <NS1:ClearanceAgreement>DOM</NS1:ClearanceAgreement>
                                            <NS1:PublicFlightDisplay>
                                                <NS1:AirlineType>4A</NS1:AirlineType>
                                                <NS1:FlightNumber>035</NS1:FlightNumber>
                                            </NS1:PublicFlightDisplay>
                                            <NS1:FlightCrewAirline/>
                                            <NS1:CabinCrewAirline/>
                                        </NS1:LegData>
                                        <NS1:TPA_Extension>
                                            <String Context=""1"" Name=""ExitDoor"" Usage=""DepartureOrArrival"">A</String>
                                            <String Context=""2"" Name=""name2"" Usage=""DepartureOrArrival""/>
                                            <String Context=""3"" Name=""name3"" Usage=""DepartureOrArrival""/>
                                            <Number Context=""4"" Name=""name4"" Usage=""DepartureOrArrival""/>
                                        </NS1:TPA_Extension>
                                    </NS1:FlightLeg>
                                </NS1:IATA_AIDX_FlightLegNotifRQ>";


            string strMsg = "";// ObtenerCola();

            strMsg = strMsgAux;
            //!string.IsNullOrEmpty(strMsg) ? strMsg : strMsgAux;

            XmlSerializer serializer = new XmlSerializer(typeof(IATA_AIDX_FlightLegNotifRQ));

            using (StringReader reader = new StringReader(strMsgAux))
            {
                IATA_AIDX_FlightLegNotifRQ flightLegNotif = (IATA_AIDX_FlightLegNotifRQ)serializer.Deserialize(reader);

                foreach (var flightLeg in flightLegNotif.FlightLegs)
                {
                    var vueloObj = ConvertToVueloInfo(flightLeg);
                    if (vueloObj != null)
                    {
                        InsertVuelo(vueloObj, rutaArchivo);
                    }
                    else {

                        Console.WriteLine($"Vuelo { flightLeg.LegIdentifier.Airline}{ flightLeg.LegIdentifier.FlightNumber} no cumple con las condiciones.");
                    }
                    
                }
            }

                Console.WriteLine("Fin del proceso, archivo XML creado: " + rutaArchivo);
        }
         static MQQueueManager Open()
        {
            Hashtable queueProperties = new Hashtable
                {
                    { MQC.HOST_NAME_PROPERTY, strServerName },
                    { MQC.CHANNEL_PROPERTY, strChannelName },
                    { MQC.PORT_PROPERTY, intPort },
                    { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
                    { MQC.USER_ID_PROPERTY,user },
                    { MQC.PASSWORD_PROPERTY, password}
                };

            try
            {
                MQQueueManager myQM = new MQQueueManager(strQueueManagerName, queueProperties);
                Console.WriteLine("Connected to MQ");
                return myQM;
            }
            catch (MQException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("MQ Queue Manager creation Error:   ", e);
                throw e;
            }
        }

         static string ObtenerCola()
        { 
            MQMessage queueMessage;
            MQQueue mqQ = null;
            MQQueueManager mqQM = Open();
            string strReturn = "";

            try
            {
                mqQ = mqQM.AccessQueue(strQueueName,
                       MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
                queueMessage = new MQMessage();
                var queueGetMessageOptions = new MQGetMessageOptions { WaitInterval = 15 * 10000 };// 15 seconds

                queueGetMessageOptions.Options |= MQC.MQGMO_WAIT;

                mqQ.Get(queueMessage, queueGetMessageOptions);
                strReturn = queueMessage.ReadString(queueMessage.MessageLength);
                Console.WriteLine("*****Lectura de cola*****");
                Console.WriteLine(strReturn);
            }
            catch (MQException mqex)
            {
                if (mqex.Reason == MQC.MQRC_NO_MSG_AVAILABLE)
                {
                    Console.WriteLine("No hay mensajes disponibles en la cola.");
                }
                else
                {
                    Console.WriteLine("Error al obtener cola: " + mqex.Message);
                    Console.WriteLine(mqex.ToString());
                }
            }
            finally
            {
                if (mqQ != null)
                    mqQ.Close();

                if (mqQM != null)
                    mqQM.Disconnect();
            }
            return strReturn;
        }

        public static VueloEntity ConvertToVueloInfo(FlightLeg flightLeg)
        {
            // Diccionario para mapear los códigos a los estados
            var estadosMap = new Dictionary<string, string>
            {
                { "SH", "PROGRAMADO" },
                { "OT", "CONFIRMADO" },
                { "DL", "DEMORADO" },
                { "GC", "FIN EMBARQ" },
                { "DX", "CANCELADO" }
            };
            
            var vueloEntity = new VueloEntity
            {
                cod_vuelo = $"{flightLeg.LegIdentifier.Airline}{flightLeg.LegIdentifier.FlightNumber}",
                tip_ope = flightLeg.LegIdentifier.ArrivalAirport,
                tip_trafico = flightLeg.LegData.ClearanceAgreement,
                fch_hra_prog = Convert.ToDateTime(flightLeg.LegData.OperationTimes
                    .FirstOrDefault(o => o.TimeType == "SCT")?.Time),
                fch_hra_ult = Convert.ToDateTime(flightLeg.LegData.OperationTimes
                    .FirstOrDefault(o => o.TimeType == "ACT")?.Time),
                dsc_estado = flightLeg.LegData.RemarkTextCodes
                .Where(s => s.Qualifier == "PUB")
                .Select(s => estadosMap.ContainsKey(s.Value) ? estadosMap[s.Value] : s.Value) // Traducir el valor usando el diccionario
                .FirstOrDefault(),
                num_puerta = flightLeg.LegData.AirportResources?.Resource?.PassengerGates
                            .Where(pg => !string.IsNullOrEmpty(pg.Value)) 
                            .Select(pg => pg.Value)
                            .FirstOrDefault(),

                TipMq = "TiempoReal",
                VueloEstado = "EN ESPERA"
            };

            // Validar condiciones
            DateTime fch_hra24 = DateTime.Now.AddMinutes(1410);
            //           PROGRAMADO  CONFIRMADO  DEMORADO  FIN EMBARQ  CANCELADO
            if (new[] { "PROGRAMADO", "CONFIRMADO", "DEMORADO", "FIN EMBARQ", "CANCELADO" }.Contains(vueloEntity.dsc_estado)
                && vueloEntity.VueloEstado == "EN ESPERA"
               // && vueloEntity.fch_hra_ult <= fch_hra24
                && vueloEntity.TipMq == "TiempoReal")
            {
          
                return vueloEntity;
            }

       
            return null;

        }
        
        // Método para insertar o actualizar un vuelo en el archivo XML
        public static void InsertVuelo(VueloEntity vuelo, string rutaXml)
        {
            XDocument doc;
            if (File.Exists(rutaXml))
            {
                doc = XDocument.Load(rutaXml);
            }
            else
            {
                doc = new XDocument(new XElement("Vuelos"));
            }
            DateTime fch_hra24 = DateTime.Now.AddMinutes(1410); // 23 hrs 30 mins desde la hora actual

            // Verificar si el vuelo ya existe en el archivo XML
            var vueloExistente = doc.Descendants("Vuelo")
                               .FirstOrDefault(x =>
                                   x.Element("cod_vuelo").Value.Trim() == vuelo.cod_vuelo.Trim() && // Eliminar espacios en blanco
                                   x.Element("tip_ope").Value == vuelo.tip_ope &&
                                   x.Element("tip_trafico").Value == vuelo.tip_trafico &&
                                   DateTime.Parse(x.Element("fch_hra_prog").Value).ToUniversalTime() == vuelo.fch_hra_prog.ToUniversalTime() // Comparación precisa de fechas
                               );


           

            //if (vuelo.fch_hra_ult >= DateTime.Now && vuelo.fch_hra_ult <= fch_hra24)
            //{
                if (vueloExistente != null) // Vuelo ya existe, actualización
                {
                    DateTime fch_hra_ultExistente = DateTime.Parse(vueloExistente.Element("fch_hra_ult").Value);

                    // Actualizar los datos si hay cambios
                    if (vuelo.fch_hra_ult != fch_hra_ultExistente)
                    {
                        vueloExistente.Element("dsc_estado").Value = vuelo.dsc_estado;
                        vueloExistente.Element("fch_hra_ult").Value = vuelo.fch_hra_ult.ToString();
                        vueloExistente.Add(new XElement("num_puerta", vuelo.num_puerta?.ToString() ?? string.Empty));
                        
                        Console.WriteLine("Vuelo actualizado correctamente en el XML.");
                    }
                }
                else // No existe, inserción
                {
                    // Crear un nuevo elemento <Vuelo> y agregarlo al archivo XML
                        XElement vueloElement = new XElement("Vuelo",
                        new XElement("cod_vuelo", vuelo.cod_vuelo),
                        new XElement("tip_ope", vuelo.tip_ope),
                        new XElement("tip_trafico", vuelo.tip_trafico),
                        new XElement("dsc_estado", vuelo.dsc_estado),
                        new XElement("num_puerta", vuelo.num_puerta?.ToString() ?? string.Empty),
                        new XElement("fch_hra_prog", vuelo.fch_hra_prog.ToString()),
                        new XElement("fch_hra_ult", vuelo.fch_hra_ult.ToString())
                        
                    );

                    doc.Root.Add(vueloElement);

                    Console.WriteLine("Vuelo insertado correctamente en el XML.");
                }

                doc.Save(rutaXml); // Guardar el archivo XML
            //}
            //else if (vuelo.fch_hra_ult > fch_hra24)
            //{
            //    // Vuelo en espera
            //    Console.WriteLine($"Vuelo {vuelo.cod_vuelo} está 'EN ESPERA'.");
            //}
            //else if (vuelo.fch_hra_ult <= DateTime.Now)
            //{
            //    // Vuelo vencido
            //    Console.WriteLine($"Vuelo {vuelo.cod_vuelo} está 'VENCIDO'.");
            //}
        }
    }
}
