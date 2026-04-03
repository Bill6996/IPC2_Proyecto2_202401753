using MD_IPC2_Proyecto2_202401753.Models.Entidades;
using MD_IPC2_Proyecto2_202401753.Models.TDA;
using System;
using System.Xml;

namespace MD_IPC2_Proyecto2_202401753.Services
{
    public class SistemaService
    {
        private ListaEnlazada listaDrones;
        private ListaEnlazada listaSistemasDrones;
        private ListaEnlazada listaMensajes;

        private static SistemaService instancia;

        public static SistemaService ObtenerInstancia()
        {
            if (instancia == null)
                instancia = new SistemaService();
            return instancia;
        }

        private SistemaService()
        {
            listaDrones = new ListaEnlazada();
            listaSistemasDrones = new ListaEnlazada();
            listaMensajes = new ListaEnlazada();
        }

        // ─────────────────────────────────────────
        // GETTERS
        // ─────────────────────────────────────────
        public ListaEnlazada GetListaDrones() => listaDrones;
        public ListaEnlazada GetListaSistemasDrones() => listaSistemasDrones;
        public ListaEnlazada GetListaMensajes() => listaMensajes;

        // ─────────────────────────────────────────
        // GESTIÓN DE DRONES
        // ─────────────────────────────────────────
        public bool AgregarDron(string nombre)
        {
            if (listaDrones.Contiene(nombre, d => ((Dron)d).Nombre))
                return false;
            listaDrones.Agregar(new Dron(nombre));
            return true;
        }

        public Dron BuscarDron(string nombre)
        {
            object resultado = listaDrones.BuscarPorNombre(nombre, d => ((Dron)d).Nombre);
            return resultado != null ? (Dron)resultado : null;
        }

        // ─────────────────────────────────────────
        // GESTIÓN DE SISTEMAS DE DRONES
        // ─────────────────────────────────────────
        public bool AgregarSistemaDrones(SistemaDrones sistema)
        {
            if (listaSistemasDrones.Contiene(sistema.Nombre, s => ((SistemaDrones)s).Nombre))
                return false;
            listaSistemasDrones.Agregar(sistema);
            return true;
        }

        public SistemaDrones BuscarSistemaDrones(string nombre)
        {
            object resultado = listaSistemasDrones.BuscarPorNombre(nombre, s => ((SistemaDrones)s).Nombre);
            return resultado != null ? (SistemaDrones)resultado : null;
        }

        // ─────────────────────────────────────────
        // GESTIÓN DE MENSAJES
        // ─────────────────────────────────────────
        public bool AgregarMensaje(Mensaje mensaje)
        {
            if (listaMensajes.Contiene(mensaje.Nombre, m => ((Mensaje)m).Nombre))
                return false;
            listaMensajes.Agregar(mensaje);
            return true;
        }

        public Mensaje BuscarMensaje(string nombre)
        {
            object resultado = listaMensajes.BuscarPorNombre(nombre, m => ((Mensaje)m).Nombre);
            return resultado != null ? (Mensaje)resultado : null;
        }

        // ─────────────────────────────────────────
        // CARGA DE XML ENTRADA
        // ─────────────────────────────────────────
        public string CargarXML(string rutaArchivo)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(rutaArchivo);

                int dronesAgregados = 0;
                XmlNodeList drones = doc.SelectNodes("//listaDrones/dron");
                foreach (XmlNode dron in drones)
                {
                    string nombre = dron.InnerText.Trim();
                    if (AgregarDron(nombre))
                        dronesAgregados++;
                }

                int sistemasAgregados = 0;
                XmlNodeList sistemas = doc.SelectNodes("//listaSistemasDrones/sistemaDrones");
                foreach (XmlNode sistemaNode in sistemas)
                {
                    string nombreSistema = sistemaNode.Attributes["nombre"].Value.Trim();
                    int alturaMaxima = int.Parse(sistemaNode.SelectSingleNode("alturaMaxima").InnerText.Trim());
                    int cantidadDrones = int.Parse(sistemaNode.SelectSingleNode("cantidadDrones").InnerText.Trim());

                    SistemaDrones sistema = new SistemaDrones(nombreSistema, alturaMaxima, cantidadDrones);

                    XmlNodeList contenidos = sistemaNode.SelectNodes("contenido");
                    foreach (XmlNode contenidoNode in contenidos)
                    {
                        string nombreDron = contenidoNode.SelectSingleNode("dron").InnerText.Trim();
                        ContenidoDron contenido = new ContenidoDron(nombreDron);

                        XmlNodeList alturas = contenidoNode.SelectNodes("alturas/altura");
                        foreach (XmlNode alturaNode in alturas)
                        {
                            int valorAltura = int.Parse(alturaNode.Attributes["valor"].Value.Trim());
                            string letra = alturaNode.InnerText.Trim();
                            contenido.AgregarAltura(valorAltura, letra);
                        }

                        sistema.AgregarContenidoDron(contenido);
                    }

                    if (AgregarSistemaDrones(sistema))
                        sistemasAgregados++;
                }

                int mensajesAgregados = 0;
                XmlNodeList mensajes = doc.SelectNodes("//listaMensajes/Mensaje");
                foreach (XmlNode mensajeNode in mensajes)
                {
                    string nombreMensaje = mensajeNode.Attributes["nombre"].Value.Trim();
                    string nombreSistema = mensajeNode.SelectSingleNode("sistemaDrones").InnerText.Trim();

                    Mensaje mensaje = new Mensaje(nombreMensaje, nombreSistema);

                    XmlNodeList instrucciones = mensajeNode.SelectNodes("instrucciones/instruccion");
                    foreach (XmlNode instrNode in instrucciones)
                    {
                        string nombreDron = instrNode.Attributes["dron"].Value.Trim();
                        int altura = int.Parse(instrNode.InnerText.Trim());
                        mensaje.AgregarInstruccion(nombreDron, altura);
                    }

                    if (AgregarMensaje(mensaje))
                        mensajesAgregados++;
                }

                return $"XML cargado correctamente. Drones: {dronesAgregados}, Sistemas: {sistemasAgregados}, Mensajes: {mensajesAgregados}";
            }
            catch (Exception ex)
            {
                return $"Error al cargar XML: {ex.Message}";
            }
        }

        // ─────────────────────────────────────────
        // ALGORITMO TIEMPO ÓPTIMO
        // ─────────────────────────────────────────
        public ResultadoMensaje CalcularTiempoOptimo(string nombreMensaje)
        {
            Mensaje mensaje = BuscarMensaje(nombreMensaje);
            if (mensaje == null) return null;

            SistemaDrones sistema = BuscarSistemaDrones(mensaje.NombreSistemaDrones);
            if (sistema == null) return null;

            ListaEnlazada instrucciones = mensaje.Instrucciones;
            int totalInstrucciones = instrucciones.Tamanio();

            // Inicializar alturas actuales de cada dron en 0
            ListaEnlazada nombresDrones = new ListaEnlazada();
            ListaEnlazada alturasDrones = new ListaEnlazada();

            Nodo nodoContenido = sistema.Contenido.ObtenerCabeza();
            while (nodoContenido != null)
            {
                ContenidoDron cd = (ContenidoDron)nodoContenido.Dato;
                nombresDrones.Agregar(cd.NombreDron);
                alturasDrones.Agregar(0);
                nodoContenido = nodoContenido.Siguiente;
            }

            ResultadoMensaje resultado = new ResultadoMensaje(nombreMensaje, mensaje.NombreSistemaDrones);
            int tiempoActual = 0;

            for (int i = 0; i < totalInstrucciones; i++)
            {
                Instruccion instr = (Instruccion)instrucciones.Obtener(i);
                string dronObjetivo = instr.NombreDron;
                int alturaObjetivo = instr.Altura;

                int indice = ObtenerIndiceDron(nombresDrones, dronObjetivo);
                int alturaActual = (int)alturasDrones.Obtener(indice);

                // Calcular cuántos segundos necesita el dron objetivo para llegar
                int diferencia = Math.Abs(alturaObjetivo - alturaActual);
                // Si ya está en la altura correcta solo necesita 1 segundo (emitir luz)
                // Si necesita moverse: diferencia segundos moviéndose + 1 emitir luz
                int segundosNecesarios = diferencia + 1;

                for (int t = 0; t < segundosNecesarios; t++)
                {
                    tiempoActual++;
                    AccionTiempo accionTiempo = new AccionTiempo(tiempoActual);

                    Nodo nodoDron = nombresDrones.ObtenerCabeza();
                    int idx = 0;
                    while (nodoDron != null)
                    {
                        string nomDron = (string)nodoDron.Dato;
                        int altDron = (int)alturasDrones.Obtener(idx);
                        string accion;

                        if (nomDron == dronObjetivo)
                        {
                            bool esUltimoSegundo = (t == segundosNecesarios - 1);
                            if (esUltimoSegundo)
                            {
                                accion = "Emitir luz";
                                ActualizarAltura(alturasDrones, idx, alturaObjetivo);
                            }
                            else
                            {
                                if (altDron < alturaObjetivo)
                                {
                                    accion = "Subir";
                                    ActualizarAltura(alturasDrones, idx, altDron + 1);
                                }
                                else if (altDron > alturaObjetivo)
                                {
                                    accion = "Bajar";
                                    ActualizarAltura(alturasDrones, idx, altDron - 1);
                                }
                                else
                                {
                                    // Ya está en la altura, solo espera antes de emitir
                                    accion = "Esperar";
                                }
                            }
                        }
                        else
                        {
                            // Los demás drones se mueven anticipándose a su próxima instrucción
                            accion = MoverDronAnticipado(nombresDrones, alturasDrones,
                                                         instrucciones, i, nomDron,
                                                         idx, altDron);
                        }

                        accionTiempo.AgregarAccion(nomDron, accion);
                        nodoDron = nodoDron.Siguiente;
                        idx++;
                    }

                    resultado.AgregarAccionTiempo(accionTiempo);
                }
            }

            resultado.TiempoOptimo = tiempoActual;
            resultado.MensajeRecibido = DecodificarMensaje(mensaje, sistema);
            return resultado;
        }

        // Mueve un dron hacia su próxima instrucción anticipadamente
        private string MoverDronAnticipado(ListaEnlazada nombresDrones,
                                            ListaEnlazada alturasDrones,
                                            ListaEnlazada instrucciones,
                                            int instruccionActual,
                                            string nomDron,
                                            int idx,
                                            int altDron)
        {
            // Buscar la próxima instrucción de este dron
            int proxAltura = -1;
            for (int j = instruccionActual + 1; j < instrucciones.Tamanio(); j++)
            {
                Instruccion sig = (Instruccion)instrucciones.Obtener(j);
                if (sig.NombreDron == nomDron)
                {
                    proxAltura = sig.Altura;
                    break;
                }
            }

            if (proxAltura == -1)
                return "Esperar"; // No tiene más instrucciones

            // Moverse anticipadamente hacia la próxima altura
            if (altDron < proxAltura)
            {
                ActualizarAltura(alturasDrones, idx, altDron + 1);
                return "Subir";
            }
            else if (altDron > proxAltura)
            {
                ActualizarAltura(alturasDrones, idx, altDron - 1);
                return "Bajar";
            }

            return "Esperar";
        }

        private int ObtenerIndiceDron(ListaEnlazada nombres, string nombreBuscado)
        {
            Nodo actual = nombres.ObtenerCabeza();
            int idx = 0;
            while (actual != null)
            {
                if ((string)actual.Dato == nombreBuscado)
                    return idx;
                actual = actual.Siguiente;
                idx++;
            }
            return -1;
        }

        private void ActualizarAltura(ListaEnlazada alturas, int indice, int nuevaAltura)
        {
            Nodo actual = alturas.ObtenerCabeza();
            for (int i = 0; i < indice; i++)
                actual = actual.Siguiente;
            actual.Dato = nuevaAltura;
        }

        private string DecodificarMensaje(Mensaje mensaje, SistemaDrones sistema)
        {
            string mensajeDecodificado = "";
            Nodo actual = mensaje.Instrucciones.ObtenerCabeza();
            while (actual != null)
            {
                Instruccion instr = (Instruccion)actual.Dato;
                string letra = sistema.ObtenerLetra(instr.NombreDron, instr.Altura);
                if (letra != null)
                    mensajeDecodificado += letra;
                actual = actual.Siguiente;
            }
            return mensajeDecodificado;
        }

        // ─────────────────────────────────────────
        // GENERAR XML SALIDA
        // ─────────────────────────────────────────
        public string GenerarXMLSalida(string rutaSalida)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration declaracion = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(declaracion);

                XmlElement respuesta = doc.CreateElement("respuesta");
                doc.AppendChild(respuesta);

                XmlElement listaMensajesElem = doc.CreateElement("listaMensajes");
                respuesta.AppendChild(listaMensajesElem);

                Nodo nodoMensaje = listaMensajes.ObtenerCabeza();
                while (nodoMensaje != null)
                {
                    Mensaje msg = (Mensaje)nodoMensaje.Dato;
                    ResultadoMensaje resultado = CalcularTiempoOptimo(msg.Nombre);

                    if (resultado != null)
                    {
                        XmlElement mensajeElem = doc.CreateElement("mensaje");
                        mensajeElem.SetAttribute("nombre", resultado.NombreMensaje);
                        listaMensajesElem.AppendChild(mensajeElem);

                        XmlElement sistemaElem = doc.CreateElement("sistemaDrones");
                        sistemaElem.InnerText = resultado.NombreSistemaDrones;
                        mensajeElem.AppendChild(sistemaElem);

                        XmlElement tiempoElem = doc.CreateElement("tiempoOptimo");
                        tiempoElem.InnerText = resultado.TiempoOptimo.ToString();
                        mensajeElem.AppendChild(tiempoElem);

                        XmlElement mensajeRecibidoElem = doc.CreateElement("mensajeRecibido");
                        mensajeRecibidoElem.InnerText = resultado.MensajeRecibido;
                        mensajeElem.AppendChild(mensajeRecibidoElem);

                        XmlElement instruccionesElem = doc.CreateElement("instrucciones");
                        mensajeElem.AppendChild(instruccionesElem);

                        Nodo nodoTiempo = resultado.AccionesPorTiempo.ObtenerCabeza();
                        while (nodoTiempo != null)
                        {
                            AccionTiempo at = (AccionTiempo)nodoTiempo.Dato;

                            XmlElement tiempoValElem = doc.CreateElement("tiempo");
                            tiempoValElem.SetAttribute("valor", at.Tiempo.ToString());
                            instruccionesElem.AppendChild(tiempoValElem);

                            XmlElement accionesElem = doc.CreateElement("acciones");
                            tiempoValElem.AppendChild(accionesElem);

                            Nodo nodoAccion = at.Acciones.ObtenerCabeza();
                            while (nodoAccion != null)
                            {
                                ParDronAccion pda = (ParDronAccion)nodoAccion.Dato;

                                XmlElement dronElem = doc.CreateElement("dron");
                                dronElem.SetAttribute("nombre", pda.NombreDron);
                                dronElem.InnerText = pda.Accion;
                                accionesElem.AppendChild(dronElem);

                                nodoAccion = nodoAccion.Siguiente;
                            }

                            nodoTiempo = nodoTiempo.Siguiente;
                        }
                    }

                    nodoMensaje = nodoMensaje.Siguiente;
                }

                doc.Save(rutaSalida);
                return $"XML generado correctamente en: {rutaSalida}";
            }
            catch (Exception ex)
            {
                return $"Error al generar XML: {ex.Message}";
            }
        }
    }
}