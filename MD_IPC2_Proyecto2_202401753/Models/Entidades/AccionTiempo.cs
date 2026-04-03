using MD_IPC2_Proyecto2_202401753.Models.TDA;

namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class AccionTiempo
    {
        public int Tiempo { get; set; }
        public ListaEnlazada Acciones { get; set; } // Lista de ParDronAccion

        public AccionTiempo(int tiempo)
        {
            Tiempo = tiempo;
            Acciones = new ListaEnlazada();
        }

        public void AgregarAccion(string nombreDron, string accion)
        {
            Acciones.Agregar(new ParDronAccion(nombreDron, accion));
        }
    }

    public class ParDronAccion
    {
        public string NombreDron { get; set; }
        public string Accion { get; set; }

        public ParDronAccion(string nombreDron, string accion)
        {
            NombreDron = nombreDron;
            Accion = accion;
        }
    }
}