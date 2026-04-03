namespace MD_IPC2_Proyecto2_202401753.Models.TDA
{
    public class Nodo
    {
        public object Dato { get; set; }
        public Nodo Siguiente { get; set; }

        public Nodo(object dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}