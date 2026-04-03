using MD_IPC2_Proyecto2_202401753.Models.TDA;

namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class ResultadoMensaje
    {
        public string NombreMensaje { get; set; }
        public string NombreSistemaDrones { get; set; }
        public int TiempoOptimo { get; set; }
        public string MensajeRecibido { get; set; }
        public ListaEnlazada AccionesPorTiempo { get; set; } // Lista de AccionTiempo

        public ResultadoMensaje(string nombreMensaje, string nombreSistema)
        {
            NombreMensaje = nombreMensaje;
            NombreSistemaDrones = nombreSistema;
            TiempoOptimo = 0;
            MensajeRecibido = "";
            AccionesPorTiempo = new ListaEnlazada();
        }

        public void AgregarAccionTiempo(AccionTiempo accion)
        {
            AccionesPorTiempo.Agregar(accion);
        }
    }
}