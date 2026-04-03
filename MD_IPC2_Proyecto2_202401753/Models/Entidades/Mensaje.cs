using MD_IPC2_Proyecto2_202401753.Models.TDA;
using System.Reflection;

namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class Mensaje
    {
        public string Nombre { get; set; }
        public string NombreSistemaDrones { get; set; }
        public ListaEnlazada Instrucciones { get; set; }  // Lista de Instruccion

        public Mensaje(string nombre, string nombreSistemaDrones)
        {
            Nombre = nombre;
            NombreSistemaDrones = nombreSistemaDrones;
            Instrucciones = new ListaEnlazada();
        }

        public void AgregarInstruccion(string nombreDron, int altura)
        {
            Instrucciones.Agregar(new Instruccion(nombreDron, altura));
        }
    }
}
