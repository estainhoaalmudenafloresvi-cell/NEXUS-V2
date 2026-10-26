# Arquitectura actual de NEXUS (H3)

## 1. Objetivo

Este documento describe la arquitectura de la versión actual de NEXUS, desarrollada en C#. Se identifican sus clases principales, las funcionalidades implementadas, el flujo de ejecución y las limitaciones detectadas en el código.

El propósito es establecer una base técnica verificable para planificar la evolución del videojuego, sin confundir las funciones existentes con las que todavía se deben desarrollar.

## 2. Tecnologías y estructura general

La versión actual de NEXUS es un videojuego de exploración por consola.

- **Lenguaje de programación:** C#.
- **Interfaz:** consola de Windows.
- **Paradigma:** programación orientada a objetos.
- **Representación del escenario:** matriz bidimensional de cadenas de texto.
- **Persistencia:** archivo JSON mediante `System.Text.Json`.
- **Archivo de guardado:** `partida_nexus.json`.
- **Espacio de nombres:** `ProyectoNexus`.

El código utiliza las bibliotecas `System`, `System.Collections.Generic`, `System.IO`, `System.Text.Json` y `System.Threading` para gestionar operaciones básicas, colecciones, archivos, serialización y pausas durante la ejecución.

La aplicación no utiliza actualmente Windows Forms ni SQL Server. La interacción se realiza mediante texto, colores, símbolos, menús numéricos y controles por teclado.

## 3. Clases principales

### 3.1. `Personaje`

Es una clase abstracta que establece los atributos comunes de los personajes y entidades del juego.

**Atributos:**
- `Nombre`: nombre de la entidad.
- `Vida`: puntos de vida actuales.
- `VidaMaxima`: límite de vida.
- `AtaqueBase`: daño base del personaje.

**Método principal:**
- `AnalizarEntidad()`: método abstracto que las clases derivadas implementan para mostrar información sobre la entidad.

Esta clase permite compartir propiedades y comportamientos comunes entre el cadete, los robots y los enemigos.

### 3.2. `Cadete`

Hereda de `Personaje` y representa al jugador dentro del universo NEXUS.

**Atributos adicionales:**
- `Edad`
- `NivelEnergia`
- `EstabilidadConexion`
- `RiesgoRadiacion`
- `ExperienciaXP`
- `PosX`
- `PosY`
- `Inventario`

El constructor inicializa la vida en 100 puntos, el ataque base en 25, la radiación en 10 y agrega tres objetos iniciales al inventario: una célula de energía, un escudo nano-protector y un analizador cuántico.

**Métodos y comportamientos implementados:**

- `PuedeDespegar()`: comprueba que la edad sea igual o superior a 18 y que la energía sea mayor que 20.
- `ModificarEnergia(int delta)`: modifica la energía y limita su valor al intervalo de 0 a 100.
- `ModificarConexion(int delta)`: modifica la estabilidad de conexión dentro del mismo intervalo.
- `AumentarRadiacion(int delta)`: actualiza el riesgo de radiación con límites de 0 a 100.
- `EscanearIris()`: consume 10 puntos de energía y aumenta la radiación en 15.
- `Recargar()`: restaura la energía al 100 %, respetando el límite establecido.
- `EvaluarAnomaliaCritica()`: comprueba si la vida o la energía han llegado a cero.
- `EjecutarDesconexionEmergencia(string motivo)`: presenta una secuencia de desconexión en la consola.
- `AnalizarEntidad()`: muestra información del estado del cadete.

La clase concentra buena parte de las variables que describen el estado del jugador y algunas de las reglas relacionadas con sus recursos.

### 3.3. `Robot`

Hereda de `Personaje` y representa un robot de defensa.

Su constructor recibe un modelo y establece un nombre con el prefijo `Robot Sentinel-`. También configura 65 puntos de vida y 10 puntos de ataque.

El método `AnalizarEntidad()` muestra información sobre el sistema defensivo y su blindaje.

### 3.4. `Enemigo`

También hereda de `Personaje` y representa una entidad hostil.

**Atributo adicional:**
- `TipoAnomalia`: identifica la clase o naturaleza de la anomalía.

Su constructor permite establecer el nombre, el tipo de anomalía, la vida y el ataque. Su método `AnalizarEntidad()` muestra información de la entidad detectada.

### 3.5. `Mapa`

Administra la representación del terreno de exploración.

**Atributos y propiedades:**
- `NombreRealidad`: nombre del escenario.
- `matrizTerreno`: matriz privada de cadenas que contiene las casillas.
- `Filas`: cantidad de filas de la matriz.
- `Columnas`: cantidad de columnas de la matriz.

**Métodos principales:**
- `ObtenerCasilla(int f, int c)`: devuelve el contenido de una casilla.
- `LimpiarCasilla(int f, int c)`: reemplaza una casilla por un símbolo de terreno vacío.
- `Renderizar(int jugadorX, int jugadorY)`: muestra el mapa en la consola, representa al jugador con `@` y aplica colores según el contenido de cada casilla.

El mapa se representa mediante símbolos de texto. Entre ellos se encuentran `@` para el jugador, `◆` para el núcleo u objeto, `⚠` y `●` para elementos que activan encuentros, y `⌂` para las zonas de extracción.

### 3.6. `SistemaNexus`

Es la clase que coordina el funcionamiento general del videojuego.

**Atributos principales:**
- `cadete`: referencia al jugador actual.
- `mapaActual`: escenario en el que se encuentra.
- `mapaRealidades`: diccionario que almacena los mapas disponibles.
- `jugando`: controla el bucle principal.
- `nucleoEncontrado`: indica si se ha recuperado el núcleo.
- `random`: permite seleccionar aleatoriamente el tipo de entidad durante un encuentro.
- `ArchivoGuardado`: constante con el nombre `partida_nexus.json`.

**Métodos y responsabilidades identificados:**

- `SistemaNexus()`: inicializa el estado del juego y carga las realidades disponibles.
- `CargarRealidades()`: registra el mapa `A-01`, denominado «Realidad Alfa-01».
- `Iniciar()`: presenta el inicio del juego, ofrece cargar una partida o registrar un nuevo cadete y comprueba los requisitos para comenzar.
- `BuclePrincipal()`: muestra el menú principal y dirige las acciones seleccionadas.
- `ExplorarZona()`: gestiona el movimiento del cadete por el mapa.
- `ProcesarEventoCasilla(int x, int y)`: procesa los objetos, encuentros y zonas de extracción.
- `IniciarEncuentroEntidad(Personaje entidad)`: gestiona las opciones de combate.
- `AdministrarInventario()`: muestra los objetos y permite utilizarlos.
- `GuardarPartida()`: serializa el cadete en un archivo JSON.
- `CargarPartida()`: recupera el cadete desde el archivo JSON.
- `MostrarBanner()`: presenta el título del videojuego.
- `MostrarEstadoCadete()`: muestra los indicadores principales del jugador.
- `LeerEntero(string msg)`: solicita valores enteros y vuelve a pedirlos si la entrada no es numérica.

Esta clase concentra la mayor parte de la coordinación entre el menú, la exploración, los encuentros, el inventario y la persistencia.

### 3.7. `Program`

Contiene el método `Main()`, punto de entrada de la aplicación.

Su funcionamiento consiste en establecer el título de la consola, crear una instancia de `SistemaNexus` y ejecutar su método `Iniciar()`.

## 4. Flujo de ejecución actual

El funcionamiento general de NEXUS sigue esta secuencia:

1. Se ejecuta `Main()` y se crea una instancia de `SistemaNexus`.
2. El constructor carga la definición del mapa disponible.
3. `Iniciar()` muestra el banner y comprueba si existe `partida_nexus.json`.
4. Si el jugador decide no cargar una partida, se solicitan su nombre, edad, energía inicial y conexión cuántica.
5. Se comprueban los requisitos para iniciar la exploración.
6. Se presenta el menú principal con las opciones de explorar, escanear, consultar datos, recargar, administrar el inventario, guardar y abortar.
7. Durante la exploración, el jugador se desplaza por el mapa con las teclas W, A, S y D.
8. Cada movimiento válido consume dos puntos de energía y procesa la casilla correspondiente.
9. Si encuentra una entidad, puede atacarla, escapar, utilizar un objeto o analizarla.
10. Al recuperar el núcleo energético y regresar a una casilla de extracción, se completa la misión.
11. El sistema comprueba las condiciones de desconexión de emergencia y finaliza el bucle cuando corresponde.

## 5. Funcionalidades implementadas

### 5.1. Registro e inicio del cadete

El juego solicita los datos iniciales del jugador y verifica los requisitos de edad y energía mediante `PuedeDespegar()`.

El método `LeerEntero()` comprueba que los datos numéricos sean válidos como enteros, aunque no verifica que todos estén dentro de un intervalo permitido.

### 5.2. Exploración

El escenario inicial tiene cinco filas y seis columnas. El jugador puede moverse mediante W, A, S y D, regresar al menú con Q y visualizar su posición en el mapa.

El código impide desplazarse fuera de los límites del escenario.

### 5.3. Administración de recursos

El sistema gestiona la vida, la energía, la estabilidad de conexión, la radiación y la experiencia. El escáner consume energía y aumenta la radiación; la recarga restablece la energía al 100 %.

La desconexión de emergencia se activa cuando se detecta que la vida o la energía han llegado a cero en los puntos donde se evalúa esa condición.

### 5.4. Inventario

El inventario se representa mediante una `List<string>`.

Los objetos iniciales son:
- Célula de energía: restaura 30 puntos de energía.
- Escudo nano-protector: restaura hasta 25 puntos de vida sin superar la vida máxima.
- Analizador cuántico: actualmente no tiene un efecto específico implementado al utilizarlo; se muestra un mensaje genérico.

Al seleccionar un objeto, este se elimina de la lista antes de aplicar su efecto. Por ello, el comportamiento actual no permite conservar un objeto después de usarlo.

### 5.5. Combate

Los encuentros pueden involucrar un robot de defensa o un enemigo de tipo anomalía.

El jugador dispone de cuatro acciones: atacar, escapar, utilizar un objeto o analizar la entidad. Un ataque reduce la vida del enemigo y, si este continúa con vida, responde con un ataque contra el cadete.

Al neutralizar una entidad se otorgan 30 puntos de experiencia.

### 5.6. Misión principal

La misión implementada se denomina **«La anomalía perdida»**.

Sus objetivos mostrados en el menú son:
1. Encontrar el núcleo energético.
2. Sobrevivir a las entidades.
3. Regresar al punto de extracción.

Cuando el jugador encuentra un núcleo, este se agrega al inventario, se activa el indicador `nucleoEncontrado` y la casilla se limpia.

La misión termina cuando el jugador llega a una casilla de extracción después de encontrar el núcleo. La recompensa es de 100 puntos de experiencia y un objeto denominado módulo desconocido: rifle de plasma.

Aunque el menú menciona sobrevivir a las entidades, la condición de finalización no exige derrotarlas todas.

### 5.7. Guardado y carga

El método `GuardarPartida()` serializa el objeto `Cadete` en `partida_nexus.json` mediante `JsonSerializer`.

El método `CargarPartida()` lee ese archivo, reconstruye el objeto `Cadete` y selecciona nuevamente el mapa `A-01`.

Este mecanismo permite recuperar los atributos serializados del cadete, pero no guarda el estado completo de la partida.

## 6. Limitaciones técnicas detectadas

### 6.1. Dependencia de la consola

La presentación, la entrada de datos y los mensajes dependen directamente de `Console`. Esto limita la posibilidad de construir una interfaz gráfica sin reorganizar las responsabilidades del código.

### 6.2. Concentración de responsabilidades

`SistemaNexus` administra muchas funciones diferentes: menú, exploración, eventos, combate, inventario y guardado. Esta concentración puede dificultar las pruebas y la incorporación de nuevas misiones.

### 6.3. Persistencia incompleta

El guardado serializa el cadete, pero no conserva explícitamente:
- El valor de `nucleoEncontrado`.
- El estado modificado del mapa.
- El progreso detallado de la misión.
- El estado de los encuentros.

Al cargar, se vuelve a seleccionar el mapa `A-01`, mientras que `nucleoEncontrado` comienza con su valor inicial `false`. Por lo tanto, una partida guardada después de recoger el núcleo no necesariamente puede reanudarse desde el mismo estado de la misión.

### 6.4. Escape del combate

La opción de escapar muestra el mensaje de que el cadete llegó a una casilla segura y termina el encuentro. Sin embargo, no modifica `PosX` ni `PosY`; el desplazamiento a una casilla segura no está implementado.

### 6.5. Validación de datos iniciales

`LeerEntero()` verifica que la entrada sea numérica, pero no impide introducir valores negativos o valores superiores a 100 para la energía y la conexión iniciales.

Además, el mensaje de inicio indica que se debe ser «mayor de 18 años», mientras que la condición real `Edad >= 18` acepta también a quien tiene exactamente 18 años.

### 6.6. Consumo de energía durante la exploración

La energía se reduce en dos puntos después de comprobar los límites del mapa. Sin embargo, una tecla distinta de W, A, S, D o Q puede dejar la posición sin cambios y aun así continuar por el flujo de movimiento, consumiendo energía y procesando la casilla actual.

### 6.7. Gestión del estado del juego

La misión está controlada mediante una variable booleana y lógica concentrada en `SistemaNexus`. No existe todavía un sistema general de misiones que permita administrar estados, objetivos y recompensas diferenciados para varias misiones.

### 6.8. Alcance limitado del contenido

La implementación identificada dispone de un mapa inicial y una misión principal. Las misiones adicionales, la interfaz gráfica y la persistencia mediante SQL Server todavía deben desarrollarse.

## 7. Conclusión

NEXUS H3 cuenta con una base funcional de programación orientada a objetos, exploración por mapa, administración de recursos, inventario, encuentros y una misión principal. También incorpora una primera solución de persistencia basada en JSON.

Las limitaciones principales se encuentran en la concentración de responsabilidades dentro de `SistemaNexus`, la dependencia directa de la consola y el guardado incompleto del estado de la misión.

La evolución técnica deberá conservar las mecánicas útiles, corregir los comportamientos inconsistentes y separar progresivamente la interfaz, la lógica del juego y el acceso a datos. De esta manera, se podrá incorporar una interfaz gráfica y guardar el progreso de forma más completa sin perder la base desarrollada en H3.

## 8. Base de verificación

Este documento se elaboró a partir del código C# de NEXUS H3 compartido durante el proyecto, junto con la descripción técnica disponible. Las funcionalidades indicadas como implementadas corresponden a los métodos y comportamientos identificados en ese material; no se afirma que se hayan probado de forma independiente en un entorno de ejecución.

Antes de la entrega, se recomienda comparar este documento con la versión definitiva del repositorio para confirmar que el código no haya cambiado.
