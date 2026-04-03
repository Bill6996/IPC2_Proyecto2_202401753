namespace MD_IPC2_Proyecto2_202401753.Models.TDA
{
    public class ListaEnlazada
    {
        private Nodo cabeza;
        private int tamanio;

        public ListaEnlazada()
        {
            cabeza = null;
            tamanio = 0;
        }

        public int Tamanio() => tamanio;
        public bool EstaVacia() => cabeza == null;

        public void Agregar(object dato)
        {
            Nodo nuevo = new Nodo(dato);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                Nodo actual = cabeza;
                while (actual.Siguiente != null)
                    actual = actual.Siguiente;
                actual.Siguiente = nuevo;
            }
            tamanio++;
        }

        public object Obtener(int indice)
        {
            if (indice < 0 || indice >= tamanio) return null;
            Nodo actual = cabeza;
            for (int i = 0; i < indice; i++)
                actual = actual.Siguiente;
            return actual.Dato;
        }

        public void Eliminar(int indice)
        {
            if (indice < 0 || indice >= tamanio) return;
            if (indice == 0)
            {
                cabeza = cabeza.Siguiente;
            }
            else
            {
                Nodo actual = cabeza;
                for (int i = 0; i < indice - 1; i++)
                    actual = actual.Siguiente;
                actual.Siguiente = actual.Siguiente.Siguiente;
            }
            tamanio--;
        }

        public bool Contiene(string nombre, System.Func<object, string> obtenerNombre)
        {
            Nodo actual = cabeza;
            while (actual != null)
            {
                if (obtenerNombre(actual.Dato) == nombre)
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }

        public object BuscarPorNombre(string nombre, System.Func<object, string> obtenerNombre)
        {
            Nodo actual = cabeza;
            while (actual != null)
            {
                if (obtenerNombre(actual.Dato) == nombre)
                    return actual.Dato;
                actual = actual.Siguiente;
            }
            return null;
        }

        public void OrdenarAlfabeticamente(System.Func<object, string> obtenerNombre)
        {
            if (tamanio <= 1) return;
            bool cambio;
            do
            {
                cambio = false;
                Nodo actual = cabeza;
                while (actual.Siguiente != null)
                {
                    string a = obtenerNombre(actual.Dato);
                    string b = obtenerNombre(actual.Siguiente.Dato);
                    if (string.Compare(a, b) > 0)
                    {
                        object temp = actual.Dato;
                        actual.Dato = actual.Siguiente.Dato;
                        actual.Siguiente.Dato = temp;
                        cambio = true;
                    }
                    actual = actual.Siguiente;
                }
            } while (cambio);
        }

        public Nodo ObtenerCabeza() => cabeza;
    }
}