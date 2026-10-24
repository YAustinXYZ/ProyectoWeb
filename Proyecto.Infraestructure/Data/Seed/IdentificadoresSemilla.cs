namespace Proyecto.Infraestructure.Data.Seed;

// Identificadores fijos de los datos semilla: HasData exige llaves conocidas y así las
// relaciones se leen por nombre en lugar de números sueltos.
public static class IdentificadoresSemilla
{
    public static class CategoriasActivos
    {
        public const int Servidores = 1;
        public const int Redes = 2;
        public const int Laptops = 3;
        public const int Escritorio = 4;
        public const int Impresoras = 5;
        public const int Energia = 6;
        public const int Almacenamiento = 7;
        public const int Climatizacion = 8;
    }

    public static class Ubicaciones
    {
        public const int DataCenterRackA = 1;
        public const int DataCenterSalaUps = 2;
        public const int OficinasPiso1 = 3;
        public const int OficinasPiso2 = 4;
        public const int SalaReuniones = 5;
        public const int BodegaTi = 6;
        public const int LaboratorioSoporte = 7;
        public const int SedeAntigua = 8;           // Inactiva: no aparece en GET /api/locations
    }

    public static class Proveedores
    {
        public const int SolucionesDataCenter = 1;
        public const int RedesCableado = 2;
        public const int DistribuidoraComputo = 3;
        public const int EnergiaContinua = 4;
        public const int ImpresionTotal = 5;
        public const int ClimaTech = 6;
        public const int AlmacenamientoSeguro = 7;
        public const int TecnoRepuestos = 8;        // Inactivo
    }

    public static class Especialidades
    {
        public const int ServidoresVirtualizacion = 1;
        public const int RedesTelecomunicaciones = 2;
        public const int SoporteUsuarioFinal = 3;
        public const int ImpresionPerifericos = 4;
        public const int SistemasElectricosUps = 5;
        public const int Climatizacion = 6;
        public const int AlmacenamientoRespaldo = 7;
        public const int Ciberseguridad = 8;
    }

    public static class Tecnicos
    {
        public const int LauraMora = 1;              // Caso borde: tres especialidades
        public const int DiegoSolano = 2;
        public const int AndreaVargas = 3;
        public const int JoseQuesada = 4;
        public const int MarianaCastro = 5;
        public const int RicardoJimenez = 6;
        public const int SofiaArias = 7;
        public const int PabloHerrera = 8;           // Sin especialidades
    }

    public static class TiposMantenimiento
    {
        public const int Preventivo = 1;
        public const int Correctivo = 2;
        public const int Inspeccion = 3;
        public const int ActualizacionFirmware = 4;
        public const int LimpiezaFisica = 5;
        public const int Emergencia = 6;
        public const int Calibracion = 7;
        public const int ReemplazoComponente = 8;
    }

    public static class Activos
    {
        public const int ServidorBaseDatos = 1;
        public const int ServidorAplicaciones = 2;   // UnderMaintenance con una orden abierta
        public const int SwitchCore = 3;
        public const int Firewall = 4;
        public const int LaptopFinanzas = 5;
        public const int LaptopVentas = 6;           // OutOfService
        public const int EscritorioRecepcion = 7;    // Decommissioned
        public const int ImpresoraPiso2 = 8;
        public const int UpsDataCenter = 9;
        public const int NasRespaldos = 10;
        public const int AirePrecision = 11;
    }

    public static class OrdenesMantenimiento
    {
        public const int PreventivoServidorBaseDatos = 1;
        public const int CorrectivoServidorAplicaciones = 2;
        public const int FirmwareFirewall = 3;
        public const int DiagnosticoLaptopVentas = 4;
        public const int InspeccionEscritorio = 5;
        public const int LimpiezaImpresora = 6;
        public const int PreventivoUps = 7;
        public const int InspeccionAire = 8;
        public const int InspeccionCanceladaServidor = 9;
        public const int ReemplazoDiscoNas = 10;
    }

    public static class Repuestos
    {
        public const int DiscoSsd960 = 1;            // Bajo el mínimo
        public const int DiscoHdd4Tb = 2;
        public const int MemoriaRam32 = 3;
        public const int FuentePoder800 = 4;
        public const int Toner89A = 5;               // Stock 0: bajo el mínimo
        public const int KitFusor = 6;
        public const int BateriaUps = 7;             // Stock igual al mínimo: no está bajo el mínimo
        public const int ModuloSfp10G = 8;
        public const int CableCat6A = 9;
        public const int FiltroAire = 10;
        public const int PantallaLaptop = 11;        // Bajo el mínimo y sin proveedores
    }
}
