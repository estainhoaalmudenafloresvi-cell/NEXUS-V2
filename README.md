# NEXUS — Expedición Eos

Videojuego de acción táctica 2D en tiempo real desarrollado en **C# con Windows Forms y .NET 8 para Windows**.

## Los cuatro escenarios
1. **EOS — Jardín de Cristal:** reunir tres Fragmentos Prismáticos y activar el portal.
2. **NÉBULA — Archivo Sumergido:** descargar cuatro Núcleos de Datos antes de que la Tormenta Iónica agote el tiempo.
3. **SYLVARA — Santuario de las Lunas:** sincronizar tres Altares Lunares y derrotar al Acechador Lunar.
4. **ASTRALIS — Corazón del Vacío:** destruir dos Pilares de Escudo, derrotar al Vigía de Eos y cruzar el portal final.

## Controles
| Entrada | Acción |
|---|---|
| WASD / Flechas | Movimiento |
| Ratón | Apuntado |
| Clic izquierdo / Espacio | Disparar |
| Shift / Clic derecho | Impulso espectral |
| Q | Onda prismática |
| E | Activar portal cuando esté disponible |
| H | Usar botiquín |
| U / B | Abrir Armería Estelar |
| M | Silenciar / activar sonido |
| F5 | Guardar partida |
| Esc | Pausa |

## Tecnologías
- C#, Windows Forms y .NET 8 (`net8.0-windows`).
- SQL Server mediante `Microsoft.Data.SqlClient`.
- Guardado JSON y soporte de persistencia SQL.
- Recursos gráficos PNG y audio WAV.

## Archivos principales
- `NEXUS.sln`: solución de Visual Studio.
- `NEXUS/Program.cs`: punto de entrada.
- `NEXUS/MainForm.cs`: ventana e interfaz.
- `NEXUS/GameEngine.cs`: lógica y estado del videojuego.
- `NEXUS/SoundFX.cs`: audio.
- `NEXUS/SqlSaveRepository.cs`: persistencia SQL.
- `NEXUS/SQL/CrearBaseNEXUS.sql`: script SQL.
- `NEXUS/Assets/`: imágenes y audio.
- `Docs/`: bitácora, compromisos y guía de POO.
- `.github/workflows/nexus-ci.yml`: compilación automática.

## Cómo ejecutar en Windows
Requisitos: Visual Studio 2022 con la carga de trabajo de desarrollo de escritorio .NET, o .NET 8 SDK compatible con Windows Forms.

1. Abre `NEXUS.sln` en Visual Studio 2022.
2. Restaura los paquetes NuGet.
3. Selecciona el proyecto `NEXUS` como inicio.
4. Ejecuta con **F5**.

También se incluye `COMPILAR_Y_EJECUTAR.bat`. La conexión SQL requiere configurar SQL Server y preparar la base de datos con el script incluido.

## GitHub Actions
El workflow se ejecuta en Windows al realizar push o pull request hacia `main` cuando cambian los archivos del proyecto, y también puede iniciarse manualmente. Restaura y compila la solución en Release. Consulta la pestaña **Actions** para ver cada ejecución.

Una compilación correcta no reemplaza las pruebas manuales del juego ni valida una conexión real a SQL Server.

## Documentación
- [Bitácora técnica y correcciones](Docs/bitacora-desarrollo-nexus.md)
- [Acciones y compromisos de NEXUS](Docs/acciones-y-compromisos-nexus.md)
- [Guía de estudio de POO aplicada al código](Docs/guia-estudio-poo-nexus.md)
- [Correcciones registradas](CORRECCIONES.md)

## Estado
Versión Windows Forms con cuatro escenarios, motor de juego, recursos visuales y de audio, y componentes de guardado JSON/SQL. La compilación y las pruebas manuales deben verificarse en Windows antes de considerar la entrega validada.
