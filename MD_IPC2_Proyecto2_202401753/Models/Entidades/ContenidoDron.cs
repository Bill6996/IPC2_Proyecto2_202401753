using MD_IPC2_Proyecto2_202401753.Models.TDA;

namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class ContenidoDron
    {
        public string NombreDron { get; set; }
        public ListaEnlazada Alturas { get; set; }  // Lista de objetos Altura

        public ContenidoDron(string nombreDron)
        {
            NombreDron = nombreDron;
            Alturas = new ListaEnlazada();
        }

        public void AgregarAltura(int valor, string letra)
        {
            Alturas.Agregar(new Altura(valor, letra));
        }

        // Busca la letra que corresponde a una altura dada
        public string ObtenerLetraPorAltura(int valor)
        {
            Nodo actual = Alturas.ObtenerCabeza();
            while (actual != null)
            {
                Altura a = (Altura)actual.Dato;
                if (a.Valor == valor)
                    return a.Letra;
                actual = actual.Siguiente;
            }
            return null;
        }
    }
}