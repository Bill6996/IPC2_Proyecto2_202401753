namespace MD_IPC2_Proyecto2_202401753.Models.Entidades
{
    public class Altura
    {
        public int Valor { get; set; }
        public string Letra { get; set; }

        public Altura(int valor, string letra)
        {
            Valor = valor;
            Letra = letra;
        }
    }
}