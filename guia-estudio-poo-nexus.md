# Guía de estudio de POO aplicada a NEXUS

Esta guía se basa en clases presentes en NEXUS — Expedición Eos.

## 1. Clases principales
| Clase | Responsabilidad |
|---|---|
| `GameEngine` | Reglas, estado, misiones, movimiento y progreso |
| `GameState` | Datos del progreso de la partida |
| `Enemy` | Enemigo y atributos de combate |
| `Shot` | Proyectil, movimiento y daño |
| `Pickup` | Objeto recogible |
| `LunarAltar` | Altar y progreso de sincronización |
| `Particle` | Partícula visual |
| `FloatingText` | Texto temporal |
| `GhostTrail` | Rastro visual |
| `MainForm` | Ventana, presentación e interacción |
| `SqlSaveRepository` | Operaciones de guardado SQL |
| `SoundFX` | Reproducción de sonido |

## 2. Conceptos de POO

### Clase y objeto
Una clase define datos y comportamiento; un objeto es una instancia concreta. `Enemy` es la clase y cada enemigo creado durante una partida es un objeto con posición, vida, velocidad y tipo propios.

### Atributos y propiedades
Describen el estado. En `GameState` aparecen `Nombre`, `Vida`, `MaxVida`, `Energia`, `XP`, `Nivel`, `Mision`, `Creditos`, `MisionesCompletadas` e `Inventario`.

### Métodos
Son operaciones de una clase. `Program.Main` inicia la aplicación y `GameEngine.Nueva` inicia una partida nueva. Para explicar un método, identifica qué hace, qué datos recibe, qué estado modifica y qué devuelve.

### Encapsulamiento
Controla el acceso al estado. En `GameEngine`, `S` tiene setter privado (`private set`): se puede consultar, pero no reemplazar directamente desde cualquier clase. También hay propiedades como `PlayerX`, `PlayerY`, `Complete` y `Dead` con setter privado.

### Enumeraciones
`EnemyKind` limita los tipos de enemigo: `CrystalSentinel`, `MistMoth`, `TorpedoDrone`, `LunarStalker`, `ShieldPylon` y `VoidBoss`.

### Colecciones y composición
`GameEngine` mantiene listas de enemigos, disparos, objetos, obstáculos, altares, partículas, textos y rastros. El motor coordina esos objetos como partes del mundo.

### Separación de responsabilidades
El motor contiene reglas; el formulario presenta el juego y recibe entradas; el repositorio SQL gestiona persistencia; el componente de sonido agrupa audio.

## 3. Ejemplo: condición de portal
`GoalRequired` determina la cantidad de objetivos: tres fragmentos en EOS, cuatro núcleos en NÉBULA, tres altares en SYLVARA y un objetivo final en ASTRALIS. `PortalReady` comprueba el cumplimiento y, en SYLVARA, considera que no quede un enemigo élite.

## 4. Preguntas de repaso
1. ¿Qué diferencia hay entre clase y objeto? Da un ejemplo.
2. ¿Qué información contiene `GameState`?
3. ¿Por qué separar `MainForm` de `GameEngine`?
4. ¿Qué significa `private set`?
5. ¿Para qué se usa `EnemyKind`?
6. ¿Qué ventaja tiene guardar enemigos y proyectiles en listas?
7. ¿Qué responsabilidad tiene `SqlSaveRepository`?
8. ¿Qué condiciones permiten activar el portal?
9. ¿Qué diferencia existe entre atributo y método?
10. ¿Cómo ayuda la separación de responsabilidades al corregir errores?

## 5. Método de estudio
Para cada clase explica propósito, datos, comportamiento, relaciones y ejemplo observable durante el juego. Sigue el flujo desde `Program.Main` a `MainForm` y después a `GameEngine`. Para persistencia, revisa `SqlSaveRepository` y las operaciones de carga/guardado.
