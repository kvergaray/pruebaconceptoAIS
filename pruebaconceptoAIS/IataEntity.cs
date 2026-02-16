using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(Namespace = "http://www.iata.org/IATA/2007/00", ElementName = "IATA_AIDX_FlightLegNotifRQ")]
public class IATA_AIDX_FlightLegNotifRQ
{
    public string TimeStamp { get; set; } // Marca de tiempo en que se generó la solicitud
    public string Version { get; set; }   // Versión del formato de mensaje

    [XmlElement("FlightLeg")]
    public List<FlightLeg> FlightLegs { get; set; } // Información específica de la pierna del vuelo
}

// Clase que representa un (tramo) del vuelo
public class FlightLeg
{
    public LegIdentifier LegIdentifier { get; set; } 
    public string SpecialAction { get; set; } 
    public LegData LegData { get; set; } 
}

// Identificador del vuelo, incluye información de aerolínea y aeropuertos
public class LegIdentifier
{
    public string Airline { get; set; } // Información de la aerolínea que opera el vuelo 
    public string FlightNumber { get; set; } // Número de vuelo asignado por la aerolínea 
    public string DepartureAirport { get; set; } // Código del aeropuerto de salida
    public string ArrivalAirport { get; set; } // Código del aeropuerto de llegada 
    public DateTime OriginDate { get; set; } // Fecha y hora de salida del vuelo 
}

// Datos adicionales sobre la pierna del vuelo
public class LegData
{
    public string OperationalStatus { get; set; }

    public string ClearanceAgreement { get; set; } // Acuerdo de autorización del vuelo //tip_trafico

    [XmlElement("RemarkTextCode")]
    public List<RemarkTextCode> RemarkTextCodes { get; set; }
    public AirportResources AirportResources { get; set; } // Recursos utilizados en el aeropuerto (ej. terminales)
    [XmlElement("OperationTime")]
    public List<OperationTime> OperationTimes { get; set; } // Tiempos de operación (estimados, reales, programados)

    public PublicFlightDisplay PublicFlightDisplay { get; set; } // Información pública del vuelo
}

// Observaciones o comentarios del vuelo
public class RemarkTextCode
{
    [XmlAttribute("CodeContext")]
    public string CodeContext { get; set; }

    [XmlAttribute("Qualifier")]
    public string Qualifier { get; set; }

    [XmlAttribute("RepeatIndex")]
    public int RepeatIndex { get; set; }

    [XmlText]
    public string Value { get; set; }
}

// Recursos del aeropuerto, como terminales o pistas
public class AirportResources
{
    public Resource Resource { get; set; } // Recursos utilizados en el aeropuerto (ej. terminales)
}

// Detalles de recursos específicos como terminales y pistas
public class Resource
{
    [XmlAttribute("DepartureOrArrival")]
    public string DepartureOrArrival { get; set; }

    [XmlElement("PassengerGate")]
    public List<PassengerGate> PassengerGates { get; set; }

    public string Runway { get; set; }

    public string AircraftTerminal { get; set; }

    public string PublicTerminal { get; set; }

    [XmlElement("CheckInInfo")]
    public CheckInInfo CheckInInfo { get; set; }
}

public class PassengerGate
{
    [XmlAttribute("RepeatIndex")]
    public int RepeatIndex { get; set; }

    [XmlText]
    public string Value { get; set; }
}

public class CheckInInfo
{
    [XmlAttribute("Qualifier")]
    public string Qualifier { get; set; }

    [XmlAttribute("RepeatIndex")]
    public int RepeatIndex { get; set; }

    [XmlElement("FirstPosition")]
    public string FirstPosition { get; set; }

    [XmlElement("LastPosition")]
    public string LastPosition { get; set; }
}

// Tiempos de operación del vuelo (programados, estimados, reales)
public class OperationTime
{
    [XmlAttribute("CodeContext")]
    public string CodeContext { get; set; }

    [XmlAttribute("OperationQualifier")]
    public string OperationQualifier { get; set; }// Calificador de la operación (ej. embarque, aterrizaje)

    [XmlAttribute("TimeType")]
    public string TimeType { get; set; } // Tipo de tiempo (SCT = programado, EST = estimado, ACT = real)

    [XmlText]
    public DateTime Time { get; set; } // Hora de la operación
}

// Información pública del vuelo, como tipo de aerolínea y número de vuelo
public class PublicFlightDisplay
{
    public string AirlineType { get; set; } // Tipo de aerolínea mostrada públicamente
    public string FlightNumber { get; set; } // Número de vuelo mostrado públicamente
}




