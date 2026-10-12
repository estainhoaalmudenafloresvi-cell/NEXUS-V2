MANUAL TÉCNICO Y DE USUARIO
PROYECTO NEXUS v1.0 - CADETE EXPLORER

Lenguaje: C#
Tipo de aplicación: Juego de exploración por consola
Archivo de guardado: partida_nexus.json

============================================================
ÍNDICE
============================================================

1. Introducción
2. Objetivos
3. Descripción general del sistema
4. Requisitos e instalación
5. Cómo ejecutar el programa
6. Manual de usuario
7. Estructura del código
8. Explicación de las clases
9. Explicación de métodos importantes
10. Conceptos de C# y POO utilizados
11. Flujo general del programa
12. Archivos y persistencia de datos
13. Validaciones y manejo de errores
14. Limitaciones y observaciones técnicas
15. Pruebas recomendadas
16. Preguntas de estudio para la defensa
17. Conclusión


============================================================
1. INTRODUCCIÓN
============================================================

NEXUS v1.0 es una aplicación de consola desarrollada en C# que simula una misión de exploración en una realidad desconocida. El usuario controla a un personaje llamado Cadete Explorer, que debe recorrer un mapa, recuperar un núcleo energético, enfrentarse a posibles entidades hostiles y regresar al punto de extracción.

El programa incorpora un menú de opciones, control de salud y energía, inventario, encuentros con entidades, análisis de personajes y guardado de datos en un archivo JSON. El proyecto aplica conceptos de programación orientada a objetos, como clases, herencia, abstracción, propiedades y polimorfismo.


============================================================
2. OBJETIVOS
============================================================

2.1. Objetivo general

Desarrollar y utilizar un juego de exploración por consola en el que el usuario gestione los recursos del cadete y complete una misión mediante distintas acciones.

2.2. Objetivos específicos

- Registrar los datos iniciales del cadete.
- Representar una zona de exploración mediante un mapa bidimensional.
- Permitir el movimiento del jugador mediante las teclas W, A, S y D.
- Administrar salud, energía, radiación, experiencia e inventario.
- Crear encuentros con robots o enemigos.
- Permitir guardar y cargar los datos del cadete.
- Aplicar conceptos de programación orientada a objetos en C#.


============================================================
3. DESCRIPCIÓN GENERAL DEL SISTEMA
============================================================

La misión principal se llama "La anomalía perdida". El jugador debe encontrar el núcleo energético identificado con el símbolo ◆ y después regresar a una casilla de extracción identificada con ⌂.

Durante la exploración, el cadete puede encontrar entidades señaladas con ⚠ o ●. Al entrar en una casilla de este tipo, el programa inicia un encuentro. El jugador puede atacar, escapar, utilizar un objeto o analizar la entidad.

El sistema muestra el estado del cadete y ofrece un menú con siete opciones: explorar, escanear, consultar datos, recargar, administrar inventario, guardar y abortar la misión.


============================================================
4. REQUISITOS E INSTALACIÓN
============================================================

Para trabajar con el proyecto se recomienda:

- Una computadora compatible con .NET.
- Tener instalado el SDK de .NET.
- Utilizar Visual Studio o Visual Studio Code con las herramientas de C#.
- Tener acceso a una terminal o consola.
- Usar una versión de .NET que admita Math.Clamp y System.Text.Json, por ejemplo, .NET 6 o posterior.

4.1. Crear o abrir el proyecto

Si el código todavía no está dentro de un proyecto, se puede crear desde una terminal con estos comandos:

dotnet new console -n ProyectoNexus
cd ProyectoNexus

Después, se reemplaza el contenido de Program.cs por el código del proyecto.

Para compilar:

dotnet build

Para ejecutar:

dotnet run

Nota: si el proyecto ya existe en Visual Studio o Visual Studio Code, no es necesario crearlo de nuevo. Basta con abrir la carpeta que contiene el archivo .csproj y ejecutar el programa.


============================================================
5. CÓMO EJECUTAR EL PROGRAMA
============================================================

Al iniciar, el programa establece el título de la consola como "NEXUS v1.0 - Cadete Explorer", crea un objeto SistemaNexus y llama a su método Iniciar().

El sistema muestra el banner de bienvenida. Si encuentra el archivo partida_nexus.json, pregunta si se desea cargar el progreso guardado.

- Escribir S para intentar cargar la partida.
- Escribir N o cualquier otra respuesta para continuar con el registro de un nuevo cadete.

Si no se carga una partida, se solicitan el nombre, la edad, la batería inicial y la conexión cuántica inicial.

Para poder despegar, el código exige que la edad sea de 18 años o más y que la energía sea superior a 20. Si no se cumplen ambas condiciones, se muestra un mensaje y el inicio termina.


============================================================
6. MANUAL DE USUARIO
============================================================

6.1. Registro del cadete

El sistema solicita:

1. Nombre o código: identifica al cadete.
2. Edad: se utiliza para comprobar el requisito de inicio.
3. Batería inicial: energía con la que comienza el traje.
4. Conexión cuántica inicial: valor almacenado como característica del cadete.

Los valores numéricos se leen mediante el método LeerEntero(), que vuelve a solicitar el dato si el usuario escribe algo que no sea un número entero. El método no comprueba que el número esté dentro del rango de 0 a 100; por eso, conviene ingresar valores razonables.

6.2. Menú principal

OPCIÓN 1. EXPLORAR ZONA
Abre el mapa y permite desplazarse.

OPCIÓN 2. ACTIVAR ESCÁNER DE IRIS
Consume 10 puntos de energía y aumenta la radiación en 15.

OPCIÓN 3. CONSULTAR DATOS DE LA MISIÓN
Muestra nombre, salud, energía, experiencia y radiación.

OPCIÓN 4. RECARGAR TRAJE
Restaura la energía al 100 %.

OPCIÓN 5. ADMINISTRAR INVENTARIO
Muestra y permite utilizar objetos.

OPCIÓN 6. GUARDAR PARTIDA
Guarda los datos del cadete en un archivo JSON.

OPCIÓN 7. ABORTAR MISIÓN
Ejecuta la secuencia de desconexión de emergencia y termina el bucle principal.

Para seleccionar una opción, se escribe su número y se presiona Enter.

6.3. Controles del mapa

W: moverse hacia arriba.
A: moverse hacia la izquierda.
S: moverse hacia abajo.
D: moverse hacia la derecha.
Q: salir de la exploración y volver al menú principal.

Cada movimiento válido reduce la energía en 2 puntos. Si el movimiento intenta salir de los límites del mapa, el programa muestra un aviso y no realiza ese desplazamiento.

6.4. Significado de los símbolos

@: posición del cadete.
◆: objeto o núcleo energético.
⚠ o ●: casilla que puede activar un encuentro con una entidad.
⌂: punto de extracción.
·: casilla vacía o ya limpiada.

El mapa inicial se llama "Realidad Alfa-01" y está formado por 5 filas y 6 columnas. Algunos símbolos de paredes y caminos son decorativos: el código solamente impide salir de los límites de la matriz, no bloquea el movimiento por cada pared dibujada.

6.5. Estado del cadete

SALUD: vida actual y vida máxima. El cadete empieza con 100 puntos de vida.
ENERGÍA: batería disponible del traje.
RADIACIÓN: nivel de riesgo que aumenta al utilizar el escáner.
XP: experiencia acumulada.
ZONA: realidad actual.

La energía y la radiación se limitan a un rango de 0 a 100 mediante Math.Clamp. La salud, en cambio, se modifica directamente durante el combate.

6.6. Escáner de iris

Al elegir la opción 2, el programa llama a EscanearIris():

- Resta 10 puntos de energía.
- Suma 15 puntos de radiación.
- Muestra un mensaje de evento.

Si la energía llega a cero, el programa puede activar la desconexión de emergencia al volver a evaluar el estado del cadete.

6.7. Recarga

La opción 4 llama al método Recargar(), que aumenta la energía en 100 puntos usando el método de modificación. Como la energía está limitada entre 0 y 100, el resultado es una batería al 100 %.

6.8. Inventario

El cadete comienza con tres objetos:

- Célula de Energía (+30 E): restaura 30 puntos de energía.
- Escudo Nano-Protector (+25 V): recupera hasta 25 puntos de salud, sin superar la vida máxima.
- Analizador Cuántico: el código actual no le asigna un efecto especial; al usarlo, solo muestra un mensaje y lo elimina del inventario.

Al abrir el inventario, se selecciona el número del objeto. El número 0 permite volver sin elegir un objeto. Cuando se utiliza un objeto válido, se elimina de la lista.

Importante: el inventario puede abrirse desde el menú y durante un combate. En el combate, el código no pausa ni reanuda automáticamente el turno enemigo después de usar un objeto; al regresar al ciclo del combate, se vuelve a solicitar una decisión.

6.9. Encuentros y combate

Al entrar en una casilla con ⚠ o ●, el sistema selecciona aleatoriamente un robot o una anomalía enemiga.

OPCIÓN 1. ATACAR
El enemigo pierde una cantidad de vida igual al ataque base del cadete. Si sigue vivo, responde y resta vida al cadete.

OPCIÓN 2. ESCAPAR
Finaliza el encuentro sin derrotar a la entidad.

OPCIÓN 3. UTILIZAR OBJETO
Abre el inventario.

OPCIÓN 4. ANALIZAR ENTIDAD
Muestra información de la entidad.

El cadete tiene un ataque base de 25. El robot tiene 65 puntos de vida y 10 de ataque. La anomalía tiene 60 puntos de vida y 15 de ataque. Al derrotar a una entidad, el cadete recibe 30 XP.

6.10. Completar la misión

Para terminar la misión correctamente:

1. Explorar el mapa.
2. Llegar a una casilla con ◆ y recuperar el núcleo energético.
3. Dirigirse a una casilla ⌂.
4. El sistema muestra el mensaje "MISIÓN COMPLETADA", suma 100 XP y añade el módulo de rifle de plasma al inventario.

Si el cadete llega al punto de extracción antes de encontrar el núcleo, el sistema le indica que debe buscarlo primero.

6.11. Guardar y cargar una partida

La opción 6 guarda el objeto Cadete como JSON en el archivo partida_nexus.json, utilizando JsonSerializer.Serialize() y File.WriteAllText().

Al comenzar otra ejecución, si el archivo existe, se puede elegir cargarlo. El sistema lee el archivo con File.ReadAllText() y convierte su contenido a un objeto Cadete con JsonSerializer.Deserialize().

El guardado actual conserva los datos serializados del cadete, pero no guarda el estado del mapa, la posición del núcleo, la variable de misión completada ni el estado completo del juego. Por eso, no equivale a guardar toda la partida.

6.12. Abortar la misión

La opción 7 llama a EjecutarDesconexionEmergencia(), muestra el motivo de la desconexión y presenta tres fases: aislar el canal cuántico, respaldar el inventario y extraer la cápsula. Entre cada fase hay una pausa breve. Finalmente, el bucle principal termina.


============================================================
7. ESTRUCTURA DEL CÓDIGO
============================================================

El programa se encuentra dentro del espacio de nombres ProyectoNexus. Sus clases principales son:

PERSONAJE
Clase abstracta base con propiedades comunes y el método abstracto de análisis.

CADETE
Representa al usuario y administra energía, conexión, radiación, experiencia, posición e inventario.

ROBOT
Representa un robot defensivo.

ENEMIGO
Representa una anomalía hostil y su tipo.

MAPA
Almacena y representa el terreno en una matriz bidimensional.

SISTEMANEXUS
Controla el menú, exploración, encuentros, inventario y guardado.

PROGRAM
Contiene Main(), el punto de entrada de la aplicación.

7.1. Bibliotecas utilizadas

using System;
Permite utilizar herramientas básicas como Console y Random.

using System.Collections.Generic;
Permite utilizar colecciones como List y Dictionary.

using System.IO;
Permite leer y escribir archivos.

using System.Text.Json;
Permite convertir datos a JSON y recuperarlos.

using System.Threading;
Permite utilizar pausas mediante Thread.Sleep().

7.2. Espacio de nombres

namespace ProyectoNexus
{
    // Clases del proyecto
}

namespace agrupa los elementos relacionados del proyecto bajo el nombre ProyectoNexus. Ayuda a organizar el código y a evitar conflictos entre nombres.


============================================================
8. EXPLICACIÓN DE LAS CLASES
============================================================

8.1. Clase abstracta Personaje

CÓDIGO:

public abstract class Personaje
{
    public string Nombre { get; set; }
    public int Vida { get; set; }
    public int VidaMaxima { get; set; }
    public int AtaqueBase { get; set; }

    protected Personaje() { }

    protected Personaje(string nombre, int vida, int ataque)
    {
        Nombre = nombre;
        Vida = vida;
        VidaMaxima = vida;
        AtaqueBase = ataque;
    }

    public abstract void AnalizarEntidad();
}

EXPLICACIÓN:

- public abstract class Personaje declara una clase base abstracta. No se puede crear directamente con new Personaje().
- Nombre, Vida, VidaMaxima y AtaqueBase son propiedades comunes a los personajes.
- { get; set; } permite obtener y asignar el valor de una propiedad.
- protected Personaje() es un constructor sin parámetros, accesible desde las clases derivadas.
- Personaje(string nombre, int vida, int ataque) inicializa los datos comunes.
- VidaMaxima = vida copia la vida inicial como vida máxima.
- public abstract void AnalizarEntidad() declara un método que las clases derivadas concretas deben implementar.

8.2. Clase Cadete

CÓDIGO:

public class Cadete : Personaje
{
    public int Edad { get; set; }
    public int NivelEnergia { get; set; }
    public int EstabilidadConexion { get; set; }
    public int RiesgoRadiacion { get; set; }
    public int ExperienciaXP { get; set; } = 0;

    public int PosX { get; set; } = 0;
    public int PosY { get; set; } = 0;
    public List<string> Inventario { get; set; } = new List<string>();
}

EXPLICACIÓN:

Cadete : Personaje indica que Cadete hereda de Personaje. Por eso tiene también las propiedades heredadas, como Nombre, Vida y AtaqueBase.

Las propiedades propias del cadete representan edad, energía, estabilidad de conexión, radiación, experiencia y posición. List<string> es una lista de textos que almacena los objetos del inventario. = 0 establece un valor inicial y new List<string>() crea una lista vacía.

Constructor principal:

public Cadete(string nombre, int edad, int energiaInicial, int conexionInicial)
    : base(nombre, 100, 25)
{
    Edad = edad;
    NivelEnergia = energiaInicial;
    EstabilidadConexion = conexionInicial;
    RiesgoRadiacion = 10;

    Inventario.Add("Célula de Energía (+30 E)");
    Inventario.Add("Escudo Nano-Protector (+25 V)");
    Inventario.Add("Analizador Cuántico");
}

El constructor recibe el nombre, edad, energía y conexión inicial. La instrucción : base(nombre, 100, 25) llama al constructor de Personaje, estableciendo la vida en 100 y el ataque base en 25. Luego inicializa los datos propios del cadete y agrega tres objetos al inventario.

Método PuedeDespegar():

public bool PuedeDespegar() => Edad >= 18 && NivelEnergia > 20;

Devuelve true solo cuando ambas condiciones son verdaderas: la edad es 18 o más y la energía supera 20. El operador && significa "y lógico".

8.3. Clase Robot

CÓDIGO:

public class Robot : Personaje
{
    public Robot() { }

    public Robot(string modelo)
        : base($"Robot Sentinel-{modelo}", 65, 10) { }

    public override void AnalizarEntidad()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n[ANALIZADOR]: {Nombre}. Sistema defensivo automatizado activo. Blindaje alto.");
        Console.ResetColor();
    }
}

Robot hereda de Personaje. Su constructor recibe un modelo y construye el nombre del robot. También establece 65 puntos de vida y 10 puntos de ataque. override implementa el método abstracto AnalizarEntidad() y muestra un mensaje específico del robot.

8.4. Clase Enemigo

CÓDIGO:

public class Enemigo : Personaje
{
    public string TipoAnomalia { get; set; }

    public Enemigo() { }

    public Enemigo(string nombre, string tipo, int vida, int ataque)
        : base(nombre, vida, ataque)
    {
        TipoAnomalia = tipo;
    }

    public override void AnalizarEntidad()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n[ANALIZADOR]: {Nombre} (Clase: {TipoAnomalia}). Entidad altamente inestable.");
        Console.ResetColor();
    }
}

Enemigo hereda de Personaje y agrega la propiedad TipoAnomalia. Su constructor recibe nombre, tipo, vida y ataque. Su implementación de AnalizarEntidad() muestra el tipo de anomalía.

8.5. Clase Mapa

CÓDIGO:

public class Mapa
{
    public string NombreRealidad { get; private set; }
    private string[,] matrizTerreno;

    public Mapa(string nombre, string[,] matriz)
    {
        NombreRealidad = nombre;
        matrizTerreno = matriz;
    }

    public int Filas => matrizTerreno.GetLength(0);
    public int Columnas => matrizTerreno.GetLength(1);

    public string ObtenerCasilla(int f, int c) => matrizTerreno[f, c];
    public void LimpiarCasilla(int f, int c) => matrizTerreno[f, c] = " · ";
}

EXPLICACIÓN:

- string[,] declara una matriz bidimensional de textos.
- private string[,] matrizTerreno limita el acceso directo a la matriz desde fuera de la clase.
- private set permite consultar NombreRealidad, pero solo asignarlo desde dentro de la clase.
- GetLength(0) devuelve la cantidad de filas y GetLength(1) la cantidad de columnas.
- ObtenerCasilla devuelve el contenido de una posición.
- LimpiarCasilla reemplaza una casilla por el símbolo de terreno vacío.

El método Renderizar(int jugadorX, int jugadorY) recorre la matriz con dos ciclos for. Muestra @ donde se encuentra el jugador y cambia el color de otros símbolos según su contenido.

8.6. Clase SistemaNexus

Es la clase que coordina el juego. Contiene referencias al cadete y al mapa actual, un diccionario de realidades, variables de control y métodos para iniciar el juego, explorar, procesar eventos, combatir, administrar el inventario y guardar o cargar datos.

Campos importantes:

private Cadete cadete;
private Mapa mapaActual;
private Dictionary<string, Mapa> mapaRealidades;
private bool jugando;
private bool nucleoEncontrado = false;
private Random random = new Random();
private const string ArchivoGuardado = "partida_nexus.json";

- cadete conserva el objeto que representa al jugador.
- mapaActual referencia el mapa que se está explorando.
- Dictionary<string, Mapa> relaciona una clave de texto con un objeto Mapa.
- jugando controla si el ciclo principal debe continuar.
- nucleoEncontrado indica si se recuperó el núcleo.
- Random permite elegir aleatoriamente una entidad.
- const declara una constante cuyo valor no cambia durante la ejecución.

8.7. Clase Program y método Main()

CÓDIGO:

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "NEXUS v1.0 - Cadete Explorer";
        SistemaNexus juego = new SistemaNexus();
        juego.Iniciar();
    }
}

Main() es el punto de entrada de la aplicación.

- static permite ejecutar el método sin crear un objeto Program.
- void indica que no devuelve un resultado.
- string[] args permite recibir argumentos de la línea de comandos.
- new SistemaNexus() crea el objeto que controla el juego.
- juego.Iniciar() comienza la ejecución del sistema.


============================================================
9. EXPLICACIÓN DE MÉTODOS IMPORTANTES
============================================================

9.1. Modificar energía, conexión y radiación

public void ModificarEnergia(int delta) =>
    NivelEnergia = Math.Clamp(NivelEnergia + delta, 0, 100);

public void ModificarConexion(int delta) =>
    EstabilidadConexion = Math.Clamp(EstabilidadConexion + delta, 0, 100);

public void AumentarRadiacion(int delta) =>
    RiesgoRadiacion = Math.Clamp(RiesgoRadiacion + delta, 0, 100);

delta representa cuánto se suma o resta. Math.Clamp(valor, 0, 100) impide que el resultado sea menor que 0 o mayor que 100.

Ejemplo: si la energía es 50 y delta es -10, el nuevo valor será 40. Si es 95 y se suman 30, el resultado queda limitado a 100.

9.2. Escanear y recargar

public void EscanearIris()
{
    ModificarEnergia(-10);
    AumentarRadiacion(15);
    Console.WriteLine("Escáner activo. Batería consumida.");
}

public void Recargar()
{
    ModificarEnergia(100);
    Console.WriteLine("Batería restablecida al 100%.");
}

El escáner modifica dos características del cadete. La recarga aumenta la energía y el límite de Math.Clamp evita superar el 100 %.

9.3. Evaluar una anomalía crítica

public bool EvaluarAnomaliaCritica()
{
    if (NivelEnergia <= 0 || Vida <= 0)
    {
        EjecutarDesconexionEmergencia(
            "Sistema dañado: pérdida total de soporte vital o energía.");
        return true;
    }

    return false;
}

if comprueba una condición. El operador || significa "o lógico": basta con que la energía llegue a cero o la vida llegue a cero para activar la emergencia. El método devuelve true cuando se activa y false si no hay una condición crítica.

9.4. Ciclo principal y menú

El método BuclePrincipal() utiliza:

while (jugando)
{
    // Mostrar estado y menú
    // Leer opción
    // Ejecutar la acción seleccionada
}

while repite las instrucciones mientras jugando sea verdadero. Dentro del ciclo se utiliza switch para ejecutar una acción según la opción ingresada. Por ejemplo, el caso "1" llama a ExplorarZona() y el caso "6" llama a GuardarPartida().

9.5. Movimiento y límites del mapa

ExplorarZona() lee una tecla, calcula la nueva posición y comprueba los límites:

if (nx < 0 || nx >= mapaActual.Columnas ||
    ny < 0 || ny >= mapaActual.Filas)
{
    Console.WriteLine("Límite alcanzado: no puedes salir del mapa.");
    continue;
}

nx representa la nueva coordenada horizontal y ny la vertical. || significa "o". continue termina la iteración actual y vuelve al inicio del ciclo de exploración.

Si el movimiento es válido, se actualizan PosX y PosY, se consume energía y se procesa la casilla.

9.6. Procesar una casilla

ProcesarEventoCasilla(int x, int y) consulta el contenido de la casilla:

string casilla = mapaActual.ObtenerCasilla(y, x);

if (casilla.Contains("◆"))
{
    // Recuperar el núcleo
}
else if (casilla.Contains("⚠") || casilla.Contains("●"))
{
    // Crear una entidad e iniciar el encuentro
}
else if (casilla.Contains("⌂"))
{
    // Comprobar si se puede completar la misión
}

Se utiliza y, x al consultar la matriz porque las matrices se indexan como fila y columna, mientras que el movimiento se maneja como coordenada horizontal y vertical. Contains() comprueba si el texto de la casilla contiene el símbolo indicado.

9.7. Combate

El método IniciarEncuentroEntidad(Personaje entidad) recibe un objeto del tipo base Personaje. Puede recibir un Robot o un Enemigo gracias a la herencia.

while (combate && cadete.Vida > 0 && entidad.Vida > 0)
{
    // Mostrar salud y opciones
    // Leer decisión
    // Atacar, escapar, usar objeto o analizar
}

El ciclo continúa mientras el combate siga activo y ambos personajes tengan vida mayor que cero. Al atacar, se resta cadete.AtaqueBase a la vida de la entidad. Si la entidad continúa viva, responde con su ataque base.

9.8. Administrar el inventario

El método AdministrarInventario():

1. Comprueba si el inventario está vacío.
2. Recorre la lista con un ciclo for.
3. Solicita el número del objeto.
4. Valida que el número sea válido.
5. Elimina el objeto seleccionado con RemoveAt().
6. Aplica el efecto correspondiente según el contenido del texto.

int.TryParse() intenta convertir el texto ingresado en un número entero sin lanzar una excepción si la conversión falla.

9.9. Guardar una partida

string json = JsonSerializer.Serialize(
    cadete,
    new JsonSerializerOptions { WriteIndented = true });

File.WriteAllText(ArchivoGuardado, json);

Serialize() convierte el objeto del cadete a texto JSON. WriteIndented = true hace que el JSON se escriba con sangría para facilitar su lectura. WriteAllText() escribe el texto en el archivo indicado.

El código usa try/catch para mostrar un mensaje si ocurre un error durante el guardado.

9.10. Cargar una partida

string json = File.ReadAllText(ArchivoGuardado);
cadete = JsonSerializer.Deserialize<Cadete>(json);
mapaActual = mapaRealidades["A-01"];

ReadAllText() lee el archivo. Deserialize<Cadete>() convierte el JSON en un objeto Cadete. Después, se asigna el mapa inicial al mapa actual.

El bloque catch de este método evita que un error de lectura o conversión se muestre como una excepción no controlada, pero actualmente no explica al usuario cuál fue el error.

9.11. Leer números enteros

private int LeerEntero(string msg)
{
    int r;
    Console.Write(msg);

    while (!int.TryParse(Console.ReadLine(), out r))
    {
        Console.Write("Ingresa un número válido: ");
    }

    return r;
}

El método recibe un mensaje, lo muestra y solicita un dato. Mientras el texto no pueda convertirse en un entero, vuelve a preguntar. Finalmente, return r devuelve el número válido.


============================================================
10. CONCEPTOS DE C# Y POO UTILIZADOS
============================================================

CLASE
Ejemplo: class Cadete.
Define las características y acciones de un tipo de objeto.

OBJETO
Ejemplo: new SistemaNexus().
Es una instancia creada a partir de una clase.

PROPIEDAD
Ejemplo: public int Edad { get; set; }.
Permite consultar y asignar un dato.

MÉTODO
Ejemplo: Recargar().
Agrupa instrucciones para realizar una tarea.

CONSTRUCTOR
Ejemplo: Cadete(string nombre, ...).
Inicializa un objeto al crearlo.

HERENCIA
Ejemplo: Cadete : Personaje.
Permite reutilizar elementos de una clase base.

ABSTRACCIÓN
Ejemplo: abstract class Personaje.
Define una base general sin crear directamente un personaje genérico.

POLIMORFISMO
Ejemplo: una referencia Personaje puede contener un Robot o un Enemigo.
Un tipo base puede referirse a objetos de clases derivadas.

SOBRESCRITURA
Ejemplo: override void AnalizarEntidad().
Implementa el método abstracto de forma específica.

ENCAPSULAMIENTO
Ejemplo: private string[,] matrizTerreno.
Restringe el acceso directo a un campo.

LISTA
Ejemplo: List<string> Inventario.
Guarda varios textos en una colección ordenada.

DICCIONARIO
Ejemplo: Dictionary<string, Mapa>.
Relaciona claves con objetos de mapa.

MATRIZ
Ejemplo: string[,].
Organiza datos en filas y columnas.

CONDICIONAL
Ejemplo: if / else if.
Ejecuta instrucciones según una condición.

SELECCIÓN MÚLTIPLE
Ejemplo: switch.
Elige una acción según el valor de una opción.

CICLO
Ejemplo: while y for.
Repite instrucciones.

EXCEPCIONES
Ejemplo: try/catch.
Permite gestionar errores durante ciertas operaciones.

JSON
Ejemplo: JsonSerializer.
Convierte datos a JSON y los recupera.

VALOR ALEATORIO
Ejemplo: Random.
Elige entre opciones al azar.

10.1. Diferencia entre public, private y protected

public: el elemento puede ser accesible desde otros códigos que tengan acceso al tipo.
private: el elemento solo puede usarse directamente desde la clase que lo declara.
protected: el elemento puede usarse desde la propia clase y desde sus clases derivadas.

10.2. Diferencia entre void, bool e int

void: el método no devuelve un valor.
bool: devuelve true o false.
int: devuelve un número entero.

Ejemplos:

public void Recargar() { /* instrucciones */ }

public bool PuedeDespegar() => Edad >= 18 && NivelEnergia > 20;

public int LeerEntero(string msg) { /* instrucciones */ }

10.3. ¿Qué significa =>?

En métodos como:

public bool PuedeDespegar() => Edad >= 18 && NivelEnergia > 20;

=> permite escribir una expresión de forma abreviada. Es equivalente a:

public bool PuedeDespegar()
{
    return Edad >= 18 && NivelEnergia > 20;
}


============================================================
11. FLUJO GENERAL DEL PROGRAMA
============================================================

1. Se ejecuta Main().
2. Se crea SistemaNexus.
3. Se carga la definición del mapa.
4. Se muestra el banner.
5. Si existe un archivo de guardado, se ofrece cargarlo.
6. Si no se carga una partida, se registra un cadete.
7. Se comprueban los requisitos para despegar.
8. Se muestra el menú principal.
9. El jugador explora, consulta datos, escanea, recarga, usa objetos, guarda o aborta.
10. Al moverse, se procesan los eventos de las casillas.
11. Si encuentra el núcleo y vuelve a la extracción, completa la misión.
12. Si la salud o la energía llegan a cero y se evalúa esa condición, se activa la desconexión de emergencia.


============================================================
12. ARCHIVOS Y PERSISTENCIA DE DATOS
============================================================

El archivo partida_nexus.json almacena una representación JSON del objeto Cadete. JSON es un formato de texto utilizado para organizar información en propiedades y valores, además de listas y objetos anidados.

La serialización se realiza al guardar; la deserialización se realiza al cargar.

Importante: el código guarda solamente los datos serializados del cadete. Para guardar la partida completa sería necesario almacenar también la posición y el estado del mapa, el núcleo encontrado, el estado del juego y cualquier otro dato necesario para continuar exactamente desde el mismo punto.


============================================================
13. VALIDACIONES Y MANEJO DE ERRORES
============================================================

El programa incorpora varias comprobaciones:

- LeerEntero() vuelve a pedir el dato si no es un entero.
- PuedeDespegar() comprueba la edad y energía mínima.
- ExplorarZona() evita salir de los límites de la matriz.
- AdministrarInventario() comprueba que el número seleccionado corresponda a un objeto existente.
- EvaluarAnomaliaCritica() comprueba si la vida o energía llegaron a cero.
- GuardarPartida() utiliza try/catch para gestionar errores al guardar.
- CargarPartida() utiliza try/catch para evitar que un fallo de carga interrumpa directamente la ejecución.

No todas las entradas están validadas por completo. Por ejemplo, LeerEntero() acepta números negativos o valores de energía superiores a 100, y el nombre del cadete puede quedar vacío. Estas son oportunidades de mejora.


============================================================
14. LIMITACIONES Y OBSERVACIONES TÉCNICAS
============================================================

Este apartado describe el comportamiento del código actual, no funciones que todavía no están implementadas.

1. CONDICIÓN DE EDAD
El código acepta 18 años (Edad >= 18), aunque el mensaje dice "mayor de 18 años". Para que coincida con el mensaje, habría que cambiar la condición a Edad > 18; si se quiere permitir desde los 18, conviene cambiar el mensaje.

2. CONEXIÓN CUÁNTICA
Se solicita y almacena, pero no afecta actualmente al juego.

3. GUARDADO PARCIAL
Solo se serializa el objeto Cadete. El mapa, la posición de la misión y nucleoEncontrado no se guardan.

4. POSICIÓN AL CARGAR
Al cargar, el mapa vuelve a asignarse a A-01, pero la posición del cadete se recupera del JSON. Como el mapa no se restaura, la partida puede no continuar en el mismo estado.

5. MAPA
Los símbolos que parecen paredes son principalmente visuales. La lógica solo limita el movimiento a filas y columnas existentes.

6. OBJETO ANALIZADOR CUÁNTICO
No tiene una acción especial programada; el inventario lo elimina y muestra un mensaje genérico al utilizarlo.

7. RADIACIÓN
El nivel aumenta y se muestra, pero no se usa para activar un efecto o condición de derrota.

8. ESTABILIDAD DE CONEXIÓN
Se almacena y puede modificarse mediante un método, pero no interviene en las reglas actuales.

9. SALUD
Se limita al curar, pero el daño se resta directamente y puede dejar la vida por debajo de cero.

10. INICIO DE SESIÓN
Al no cumplir los requisitos, el método Iniciar() termina, pero el objeto del sistema no se reinicia automáticamente.

11. ESTADO DE JUEGO
La variable jugando se inicializa en true; no se restablece al cargar otra partida en el mismo objeto.

12. CASILLAS DE EXTRACCIÓN
Existen dos casillas ⌂ en el mapa inicial. El programa puede activar la extracción al entrar en cualquiera de ellas, siempre que se haya encontrado el núcleo.


============================================================
15. PRUEBAS RECOMENDADAS
============================================================

PRUEBA 1. REGISTRO VÁLIDO
Acción: ingresar edad 18 y energía 100.
Resultado esperado: el cadete cumple los requisitos y comienza la exploración.

PRUEBA 2. EDAD INSUFICIENTE
Acción: ingresar edad 17.
Resultado esperado: el sistema indica que no se cumplen los requisitos.

PRUEBA 3. ENERGÍA INSUFICIENTE
Acción: ingresar energía 20.
Resultado esperado: el sistema no permite despegar porque exige más de 20.

PRUEBA 4. DATO NO NUMÉRICO
Acción: escribir letras al solicitar la edad.
Resultado esperado: el programa vuelve a pedir un número entero.

PRUEBA 5. LÍMITE DEL MAPA
Acción: intentar moverse fuera del mapa.
Resultado esperado: se muestra una alerta y el movimiento no se realiza.

PRUEBA 6. ESCÁNER
Acción: activar el escáner con energía suficiente.
Resultado esperado: la energía baja 10 y la radiación sube 15.

PRUEBA 7. RECARGA
Acción: seleccionar recargar.
Resultado esperado: la energía queda en 100.

PRUEBA 8. INVENTARIO
Acción: usar la célula de energía.
Resultado esperado: el objeto se elimina y la energía aumenta hasta un máximo de 100.

PRUEBA 9. GUARDADO
Acción: seleccionar la opción 6.
Resultado esperado: se crea o actualiza partida_nexus.json si la operación tiene éxito.

PRUEBA 10. NÚCLEO Y EXTRACCIÓN
Acción: recuperar el núcleo y entrar a una casilla de extracción.
Resultado esperado: se muestra el mensaje de misión completada y se otorgan recompensas.


============================================================
16. PREGUNTAS DE ESTUDIO PARA LA DEFENSA
============================================================

1. ¿Qué es NEXUS?
Es una aplicación de consola desarrollada en C# que simula una misión de exploración, recolección de un núcleo, encuentros con entidades y regreso a una zona de extracción.

2. ¿Qué es una clase?
Es una plantilla que define datos y comportamientos de objetos. En el proyecto, Cadete, Robot y Enemigo son clases.

3. ¿Qué es un objeto?
Es una instancia de una clase. Por ejemplo, new SistemaNexus() crea el objeto que controla la partida.

4. ¿Por qué Personaje es abstracta?
Porque representa características generales compartidas y declara el método abstracto AnalizarEntidad(). Las clases derivadas implementan ese método.

5. ¿Qué es la herencia?
Es un mecanismo para que una clase reutilice elementos de otra. Cadete, Robot y Enemigo heredan de Personaje.

6. ¿Dónde aparece el polimorfismo?
En el uso de una referencia Personaje para representar un Robot o un Enemigo, y en las diferentes implementaciones de AnalizarEntidad().

7. ¿Para qué sirve override?
Para implementar en una clase derivada un método heredado que es virtual o abstracto.

8. ¿Para qué se usa List<string>?
Para almacenar los nombres de los objetos del inventario en una lista.

9. ¿Para qué se usa Dictionary<string, Mapa>?
Para relacionar claves de texto, como A-01, con objetos de tipo Mapa.

10. ¿Qué es una matriz bidimensional?
Es una estructura de datos organizada en filas y columnas. En NEXUS se usa para representar el terreno.

11. ¿Qué hacen if, switch, while y for?
if ejecuta instrucciones si una condición es verdadera.
switch selecciona una opción entre varios casos.
while repite instrucciones mientras una condición sea verdadera.
for repite instrucciones controlando una variable de recorrido.

12. ¿Qué significa && y qué significa ||?
&& es "y": las dos condiciones deben ser verdaderas.
|| es "o": basta con que una condición sea verdadera.

13. ¿Para qué sirve Math.Clamp()?
Para mantener un valor dentro de un rango. En el proyecto, energía, conexión y radiación se limitan entre 0 y 100.

14. ¿Qué es JSON?
Es un formato de texto para representar datos estructurados. NEXUS lo utiliza para guardar y recuperar los datos del cadete.

15. ¿Qué hacen Serialize() y Deserialize()?
Serialize() convierte un objeto en texto JSON. Deserialize() convierte el JSON en un objeto del tipo indicado.

16. ¿Qué hace try/catch?
Permite intentar una operación y gestionar una excepción si ocurre un error. En NEXUS se utiliza al guardar y cargar archivos.

17. ¿Cuál es el punto de entrada del programa?
El método Main() de la clase Program.

18. ¿Qué ocurre si la energía llega a cero?
Cuando se ejecuta EvaluarAnomaliaCritica(), si la energía o la vida son cero o menos, se activa la desconexión de emergencia.

19. ¿Qué información guarda el programa?
Guarda el objeto Cadete en partida_nexus.json. No guarda el estado completo del mapa ni de la misión.

20. ¿Qué mejorarías en el proyecto?
Se podrían validar rangos y nombres, guardar el estado completo de la misión, implementar el efecto del Analizador Cuántico, aplicar reglas a la radiación y conexión, y mejorar los mensajes de error al cargar archivos.

CONSEJO PARA EXPLICAR EL CÓDIGO EN EL EXAMEN

Cuando te pidan explicar un fragmento, responde en este orden:

1. Qué es: clase, método, propiedad, condición, ciclo o colección.
2. Qué recibe o qué datos utiliza.
3. Qué hace paso a paso.
4. Qué resultado produce dentro de NEXUS.

Ejemplo:
"PuedeDespegar() es un método que devuelve un booleano. Comprueba que la edad sea al menos 18 y que la energía supere 20. Como usa &&, ambas condiciones deben cumplirse para permitir el inicio."


============================================================
17. CONCLUSIÓN
============================================================

NEXUS v1.0 integra distintas herramientas de C# para crear un juego de exploración por consola. Su estructura permite estudiar clases, objetos, herencia, abstracción, polimorfismo, matrices, listas, diccionarios, condicionales, ciclos y archivos JSON.

El manual describe tanto el uso del programa como la lógica de sus componentes principales. También identifica limitaciones del código actual que pueden considerarse en futuras mejoras. Comprender estas partes facilita el mantenimiento del proyecto y la explicación de su funcionamiento durante una evaluación.

FIN DEL MANUAL TÉCNICO Y DE USUARIO - NEXUS v1.0
