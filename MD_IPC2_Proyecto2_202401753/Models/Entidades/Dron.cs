namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class Dron
    {
        public string Nombre { get; set; }
        public int AlturaActual { get; set; }

        public Dron(string nombre)
        {
            Nombre = nombre;
            AlturaActual = 0;
        }
    }
}