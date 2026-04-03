using MD_IPC2_Proyecto2_202401753.Models.TDA;

namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class SistemaDrones
    {
        public string Nombre { get; set; }
        public int AlturaMaxima { get; set; }
        public int CantidadDrones { get; set; }
        public ListaEnlazada Contenido { get; set; }  // Lista de ContenidoDron

        public SistemaDrones(string nombre, int alturaMaxima, int cantidadDrones)
        {
            Nombre = nombre;
            AlturaMaxima = alturaMaxima;
            CantidadDrones = cantidadDrones;
            Contenido = new ListaEnlazada();
        }

        public void AgregarContenidoDron(ContenidoDron contenido)
        {
            Contenido.Agregar(contenido);
        }

        // Dado un dron y altura, retorna la letra
        public string ObtenerLetra(string nombreDron, int altura)
        {
            Nodo actual = Contenido.ObtenerCabeza();
            while (actual != null)
            {
                ContenidoDron cd = (ContenidoDron)actual.Dato;
                if (cd.NombreDron == nombreDron)
                    return cd.ObtenerLetraPorAltura(altura);
                actual = actual.Siguiente;
            }
            return null;
        }
    }
}