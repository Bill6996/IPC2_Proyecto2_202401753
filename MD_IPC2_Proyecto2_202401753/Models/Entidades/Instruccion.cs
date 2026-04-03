namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class Instruccion
    {
        public string NombreDron { get; set; }
        public int Altura { get; set; }

        public Instruccion(string nombreDron, int altura)
        {
            NombreDron = nombreDron;
            Altura = altura;
        }
    }
}