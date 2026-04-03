#  IPC2- Proyecto 2: Sistema de Drones Encriptados
**Universidad San Carlos de Guatemala**  
**Facultad de Ingeniería - Escuela de Ciencias y Sistemas**  
**Introducción a la Programación y Computación 2**  

**Estudiante:** Bily Estuardo Vallecidos Folgar 
**Carnet:** 202401753  
**Año:** 2026, Primer Semestre

---

##  Descripción General

Sistema de comunicación encriptada mediante drones desarrollado para el Ministerio de Defensa de Guatemala. El sistema permite enviar mensajes secretos usando drones que suben a diferentes alturas y emiten luz, donde cada combinación de dron y altura representa una letra del alfabeto.

El software controla el sistema emisor de drones y calcula el tiempo óptimo para transmitir cualquier mensaje, permitiendo que los drones se muevan anticipadamente hacia su próxima posición mientras otro dron está emitiendo su señal.

---

##  Tecnologías Utilizadas

- **Lenguaje:** C#
- **Framework:** ASP.NET Core MVC
- **Visualización:** Graphviz
- **Formato de datos:** XML
- **IDE:** Visual Studio Community
- **Versionamiento:** GitHub

---

##  Restricciones cumplidas

- ✅ TDAs implementados por el estudiante (sin List, Queue, Stack de C#)
- ✅ Nodos sin genéricos(hechos por mi)
- ✅ POO completa implementada por el estudiante
- ✅ XML de entrada no modificado
- ✅ XML de salida con formato exacto del enunciado
- ✅ 4 releases mínimos en GitHub

---

##  Estructura del Proyecto
```
MD_IPC2_Proyecto2_202401753/
├── Controllers/
│   └── HomeController.cs
├── Models/
│   ├── TDA/
│   │   ├── Nodo.cs
│   │   └── ListaEnlazada.cs
│   └── Entidades/
│       ├── Dron.cs
│       ├── Altura.cs
│       ├── ContenidoDron.cs
│       ├── SistemaDrones.cs
│       ├── Instruccion.cs
│       ├── Mensaje.cs
│       ├── AccionTiempo.cs
│       └── ResultadoMensaje.cs
├── Services/
│   └── SistemaService.cs
└── Views/
    └── Home/
        ├── Index.cshtml
        ├── CargarXML.cshtml
        ├── GenerarXML.cshtml
        ├── Drones.cshtml
        ├── SistemasDrones.cshtml
        ├── VisualizarSistema.cshtml
        ├── Mensajes.cshtml
        ├── VerMensaje.cshtml
        └── Ayuda.cshtml
```

---

## Clases del Sistema

###  TDA — Tipos de Datos Abstractos

#### `Nodo.cs`
Nodo básico de la lista enlazada. Contiene un `object Dato` para almacenar cualquier tipo de objeto sin usar genéricos, y una referencia `Nodo Siguiente` para apuntar al siguiente nodo de la lista.

#### `ListaEnlazada.cs`
Lista enlazada simple implementada desde cero sin usar estructuras de C#. Implementa las siguientes operaciones:
- `Agregar(dato)` — inserta al final de la lista
- `Obtener(indice)` — retorna el elemento en la posición indicada
- `Eliminar(indice)` — elimina el elemento en la posición indicada
- `Contiene(nombre, func)` — verifica si existe un elemento con ese nombre
- `BuscarPorNombre(nombre, func)` — retorna el elemento con ese nombre
- `OrdenarAlfabeticamente(func)` — ordena la lista usando bubble sort
- `ObtenerCabeza()` — retorna el primer nodo para iteración manual

---

###  Entidades

#### `Dron.cs`
Representa un dron del sistema. Almacena su nombre único y su altura actual. Todos los drones comienzan en altura 0 al iniciar la simulación.

#### `Altura.cs`
Representa la relación entre un valor numérico de altura y la letra del alfabeto que representa en esa posición. Por ejemplo, altura 3 puede representar la letra "H" para un dron específico.

#### `ContenidoDron.cs`
Asocia un dron con su tabla de alturas dentro de un sistema de drones específico. Contiene una lista de objetos `Altura` y permite buscar qué letra corresponde a una altura dada mediante `ObtenerLetraPorAltura(valor)`.

#### `SistemaDrones.cs`
Representa un sistema completo de drones con su nombre, altura máxima permitida y cantidad de drones. Contiene una lista de `ContenidoDron` y permite consultar qué letra emite un dron específico a una altura dada mediante `ObtenerLetra(nombreDron, altura)`.

#### `Instruccion.cs`
Representa una instrucción individual dentro de un mensaje. Indica qué dron debe emitir luz y a qué altura debe hacerlo para representar una letra específica.

#### `Mensaje.cs`
Representa un mensaje a transmitir. Contiene su nombre, el sistema de drones que debe usar y la lista de instrucciones ordenadas que forman el mensaje completo.

#### `AccionTiempo.cs`
Representa lo que hace cada dron en un segundo específico de la simulación. Contiene el número de segundo y una lista de `ParDronAccion` que indica la acción de cada dron en ese instante (Subir, Bajar, Esperar o Emitir luz).

#### `ParDronAccion.cs`
Par simple que asocia el nombre de un dron con la acción que realiza en un segundo dado. Se usa dentro de `AccionTiempo`.

#### `ResultadoMensaje.cs`
Almacena el resultado completo del cálculo de tiempo óptimo para un mensaje. Contiene el nombre del mensaje, el sistema usado, el tiempo óptimo calculado, el mensaje decodificado y la lista completa de acciones por cada segundo de simulación.

---

###  Servicios

#### `SistemaService.cs`
Clase principal del sistema implementada como Singleton. Es el cerebro del proyecto y contiene toda la lógica de negocio:

- **Gestión de Drones:** agregar y buscar drones sin duplicados
- **Gestión de Sistemas:** agregar y buscar sistemas de drones
- **Gestión de Mensajes:** agregar y buscar mensajes
- **CargarXML:** lee el archivo XML de entrada de forma incremental y carga todos los datos al sistema
- **CalcularTiempoOptimo:** algoritmo principal que simula segundo a segundo el movimiento de los drones. Los drones que están esperando se mueven anticipadamente hacia su próxima instrucción para reducir el tiempo total
- **GenerarXMLSalida:** genera el archivo XML de salida con el formato exacto requerido, incluyendo el tiempo óptimo y las instrucciones detalladas por segundo
- **Inicializar:** reinicia todas las listas del sistema

---

###  Controlador

#### `HomeController.cs`
Controlador MVC que maneja todas las peticiones HTTP de la interfaz web. Gestiona las vistas de inicio, carga de XML, generación de XML, drones, sistemas, mensajes y ayuda. También contiene la lógica de generación de grafos Graphviz en formato DOT y su conversión a imágenes PNG en base64 para mostrarlas directamente en el navegador.

---

##  Algoritmo de Tiempo Óptimo

El algoritmo funciona de la siguiente manera:

1. Todos los drones inician en altura 0
2. Se procesa cada instrucción secuencialmente
3. Para cada instrucción, se calcula cuántos segundos necesita el dron objetivo para llegar a su altura (diferencia de alturas + 1 segundo para emitir luz)
4. Durante esos segundos, los drones que no están emitiendo se mueven anticipadamente hacia su próxima instrucción futura
5. Esto reduce el tiempo total porque cuando le toca a un dron emitir, ya está más cerca de su altura objetivo

---

##  Formato XML de Entrada
```xml
<?xml version="1.0"?>
<config>
  <listaDrones>
    <dron>NombreDron</dron>
  </listaDrones>
  <listaSistemasDrones>
    <sistemaDrones nombre="NombreSistema">
      <alturaMaxima>7</alturaMaxima>
      <cantidadDrones>3</cantidadDrones>
      <contenido>
        <dron>NombreDron</dron>
        <alturas>
          <altura valor="1">A</altura>
        </alturas>
      </contenido>
    </sistemaDrones>
  </listaSistemasDrones>
  <listaMensajes>
    <Mensaje nombre="NombreMensaje">
      <sistemaDrones>NombreSistema</sistemaDrones>
      <instrucciones>
        <instruccion dron="NombreDron">3</instruccion>
      </instrucciones>
    </Mensaje>
  </listaMensajes>
</config>
```

---

##  Formato XML de Salida
```xml
<?xml version="1.0" encoding="UTF-8"?>
<respuesta>
  <listaMensajes>
    <mensaje nombre="NombreMensaje">
      <sistemaDrones>NombreSistema</sistemaDrones>
      <tiempoOptimo>7</tiempoOptimo>
      <mensajeRecibido>IPC2</mensajeRecibido>
      <instrucciones>
        <tiempo valor="1">
          <acciones>
            <dron nombre="DronX">Subir</dron>
            <dron nombre="DronY">Subir</dron>
          </acciones>
        </tiempo>
      </instrucciones>
    </mensaje>
  </listaMensajes>
</respuesta>
```

---

##  Repositorio

[https://github.com/Bill6996/IPC2_Proyecto2_202401753]

##  CONCLUSION PROPIA:
Realmente me gusto este proyecto, ya que se utilizaron diferentes metodos vistos en clase y aparte la interfaz grafica que nunca habia trabajado conjunto a C#, puedo decir que si me gusto trabajarlo y tambien me encanto como quedo mi trabajo, aunque realmente hay que pulirlo esta decente para la entrega de esta clase...