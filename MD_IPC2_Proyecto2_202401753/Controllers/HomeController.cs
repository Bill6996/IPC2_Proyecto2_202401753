using MD_IPC2_Proyecto2_202401753.Models.Entidades;
using MD_IPC2_Proyecto2_202401753.Models.TDA;
using MD_IPC2_Proyecto2_202401753.Services;
using Microsoft.AspNetCore.Mvc;

namespace MD_IPC2_Proyecto2_202401753.Controllers
{
    public class HomeController : Controller
    {
        private readonly SistemaService _servicio;

        public HomeController()
        {
            _servicio = SistemaService.ObtenerInstancia();
        }

        // ─────────────────────────────────────────
        // INICIO
        // ─────────────────────────────────────────
        public IActionResult Index()
        {
            return View();
        }

        // ─────────────────────────────────────────
        // CARGAR XML
        // ─────────────────────────────────────────
        public IActionResult CargarXML()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CargarXML(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Mensaje = "selecciona un archivo XML.";
                return View();
            }

            string ruta = Path.Combine(Path.GetTempPath(), archivo.FileName);
            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            string resultado = _servicio.CargarXML(ruta);
            ViewBag.Mensaje = resultado;
            return View();
        }

        // ─────────────────────────────────────────
        // GENERAR XML SALIDA
        // ─────────────────────────────────────────
        public IActionResult GenerarXML()
        {
            string ruta = Path.Combine(Path.GetTempPath(), "salida.xml");
            string resultado = _servicio.GenerarXMLSalida(ruta);
            ViewBag.Mensaje = resultado;
            ViewBag.Ruta = ruta;
            return View();
        }

        public IActionResult DescargarXML()
        {
            string ruta = Path.Combine(Path.GetTempPath(), "salida.xml");
            if (!System.IO.File.Exists(ruta))
            {
                TempData["Error"] = "Primero genera el XML de salida.";
                return RedirectToAction("GenerarXML");
            }
            byte[] bytes = System.IO.File.ReadAllBytes(ruta);
            return File(bytes, "application/xml", "salida.xml");
        }

        public IActionResult Inicializar()
        {
            _servicio.Inicializar();
            TempData["Mensaje"] = "Sistema inicializado correctamente.";
            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────
        // GESTIÓN DE DRONES
        // ─────────────────────────────────────────
        public IActionResult Drones()
        {
            ListaEnlazada lista = _servicio.GetListaDrones();
            lista.OrdenarAlfabeticamente(d => ((Dron)d).Nombre);
            ViewBag.Lista = lista;
            return View();
        }

        [HttpPost]
        public IActionResult AgregarDron(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Error"] = "El nombre no puede estar vacío.";
                return RedirectToAction("Drones");
            }

            bool agregado = _servicio.AgregarDron(nombre.Trim());
            TempData["Mensaje"] = agregado
                ? $"Dron '{nombre}' agregado correctamente."
                : $"Ya existe un dron con el nombre '{nombre}', cambia el nombre.";

            return RedirectToAction("Drones");
        }

        // ─────────────────────────────────────────
        // GESTIÓN DE SISTEMAS DE DRONES
        // ─────────────────────────────────────────
        public IActionResult SistemasDrones()
        {
            ListaEnlazada lista = _servicio.GetListaSistemasDrones();
            ViewBag.Lista = lista;
            return View();
        }

        public IActionResult VisualizarSistema(string nombre)
        {
            SistemaDrones sistema = _servicio.BuscarSistemaDrones(nombre);
            if (sistema == null)
            {
                TempData["Error"] = "Sistema no encontrado.";
                return RedirectToAction("SistemasDrones");
            }

            string dot = GenerarDotSistema(sistema);
            string imagenBase64 = GenerarImagenGraphviz(dot);
            ViewBag.Imagen = imagenBase64;
            ViewBag.Nombre = nombre;
            return View();
        }

        // ─────────────────────────────────────────
        // GESTIÓN DE MENSAJES
        // ─────────────────────────────────────────
        public IActionResult Mensajes()
        {
            ListaEnlazada lista = _servicio.GetListaMensajes();
            lista.OrdenarAlfabeticamente(m => ((Mensaje)m).Nombre);
            ViewBag.Lista = lista;
            return View();
        }

        public IActionResult VerMensaje(string nombre)
        {
            ResultadoMensaje resultado = _servicio.CalcularTiempoOptimo(nombre);
            if (resultado == null)
            {
                TempData["Error"] = "Mensaje no encontrado.";
                return RedirectToAction("Mensajes");
            }

            string dot = GenerarDotMensaje(resultado);
            string imagenBase64 = GenerarImagenGraphviz(dot);
            ViewBag.Imagen = imagenBase64;
            ViewBag.Resultado = resultado;
            return View();
        }

        // ─────────────────────────────────────────
        // AYUDA
        // ─────────────────────────────────────────
        public IActionResult Ayuda()
        {
            return View();
        }

        // ─────────────────────────────────────────
        // GRAPHVIZ - GENERAR DOT SISTEMA
        // ─────────────────────────────────────────
        private string GenerarDotSistema(SistemaDrones sistema)
        {
            string dot = "digraph G {\n";
            dot += "  rankdir=LR;\n";
            dot += "  node [shape=record, style=filled, fillcolor=lightblue];\n";
            dot += $"  Sistema [label=\"{sistema.Nombre}\\nAltura Max: {sistema.AlturaMaxima}m\", fillcolor=orange];\n";

            Nodo nodoContenido = sistema.Contenido.ObtenerCabeza();
            while (nodoContenido != null)
            {
                ContenidoDron cd = (ContenidoDron)nodoContenido.Dato;
                string alturas = "";
                Nodo nodoAltura = cd.Alturas.ObtenerCabeza();
                while (nodoAltura != null)
                {
                    Altura a = (Altura)nodoAltura.Dato;
                    alturas += $"{a.Valor}m={a.Letra}\\n";
                    nodoAltura = nodoAltura.Siguiente;
                }
                dot += $"  {cd.NombreDron} [label=\"{cd.NombreDron}\\n{alturas}\"];\n";
                dot += $"  Sistema -> {cd.NombreDron};\n";
                nodoContenido = nodoContenido.Siguiente;
            }

            dot += "}";
            return dot;
        }

        // ─────────────────────────────────────────
        // GRAPHVIZ - GENERAR DOT MENSAJE
        // ─────────────────────────────────────────
        private string GenerarDotMensaje(ResultadoMensaje resultado)
        {
            string dot = "digraph G {\n";
            dot += "  rankdir=LR;\n";
            dot += "  node [shape=box, style=filled, fillcolor=lightyellow];\n";

            Nodo nodoTiempo = resultado.AccionesPorTiempo.ObtenerCabeza();
            string nodoPrev = "";
            while (nodoTiempo != null)
            {
                AccionTiempo at = (AccionTiempo)nodoTiempo.Dato;
                string acciones = "";
                Nodo nodoAccion = at.Acciones.ObtenerCabeza();
                while (nodoAccion != null)
                {
                    ParDronAccion pda = (ParDronAccion)nodoAccion.Dato;
                    acciones += $"{pda.NombreDron}: {pda.Accion}\\n";
                    nodoAccion = nodoAccion.Siguiente;
                }
                string nodoId = $"T{at.Tiempo}";
                dot += $"  {nodoId} [label=\"t={at.Tiempo}\\n{acciones}\"];\n";
                if (!string.IsNullOrEmpty(nodoPrev))
                    dot += $"  {nodoPrev} -> {nodoId};\n";
                nodoPrev = nodoId;
                nodoTiempo = nodoTiempo.Siguiente;
            }

            dot += "}";
            return dot;
        }

        // ─────────────────────────────────────────
        // GRAPHVIZ - GENERAR IMAGEN BASE64
        // ─────────────────────────────────────────
        private string GenerarImagenGraphviz(string dot)
        {
            try
            {
                string tempDot = Path.Combine(Path.GetTempPath(), "grafo.dot");
                string tempPng = Path.Combine(Path.GetTempPath(), "grafo.png");

                System.IO.File.WriteAllText(tempDot, dot);

                var proceso = new System.Diagnostics.Process();
                proceso.StartInfo.FileName = "dot";
                proceso.StartInfo.Arguments = $"-Tpng \"{tempDot}\" -o \"{tempPng}\"";
                proceso.StartInfo.UseShellExecute = false;
                proceso.StartInfo.CreateNoWindow = true;
                proceso.Start();
                proceso.WaitForExit();

                if (System.IO.File.Exists(tempPng))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(tempPng);
                    return Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Graphviz: " + ex.Message);
            }
            return "";
        }
    }
}