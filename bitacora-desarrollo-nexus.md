# Bitácora técnica — NEXUS: Expedición Eos

## 1. Relación con el compromiso
La síntesis de reunión indica que GitHub debía reflejar los avances de NEXUS, mostrar las acciones y cambios realizados y servir como respaldo del trabajo y guía para estudiar POO. Esta bitácora se limita exclusivamente a NEXUS.

**Criterio de veracidad:** se documenta lo comprobable en el código entregado y en `CORRECCIONES.md`. No se inventan fechas, commits ni etapas históricas no registradas.

## 2. Versión documentada
- Juego de acción táctica 2D en tiempo real.
- C# y Windows Forms.
- .NET 8 para Windows (`net8.0-windows`).
- Solución `NEXUS.sln`; proyecto `NEXUS/NEXUS.csproj`.
- Persistencia JSON y SQL Server.

## 3. Estructura y responsabilidades
| Archivo | Responsabilidad |
|---|---|
| `Program.cs` | Inicializa Windows Forms y abre `MainForm` |
| `MainForm.cs` | Ventana, interfaz, presentación y entradas |
| `GameEngine.cs` | Estado, reglas, movimiento, enemigos, misiones y progresión |
| `SoundFX.cs` | Reproducción de audio |
| `SqlSaveRepository.cs` | Operaciones de persistencia SQL |
| `SQL/CrearBaseNEXUS.sql` | Script SQL incluido |

## 4. Los cuatro escenarios implementados
1. **EOS — Jardín de Cristal:** reunir tres Fragmentos Prismáticos y activar el portal.
2. **NÉBULA — Archivo Sumergido:** descargar cuatro Núcleos de Datos antes de que termine la Tormenta Iónica; estabilizadores amplían el tiempo.
3. **SYLVARA — Santuario de las Lunas:** sincronizar tres Altares Lunares y derrotar al Acechador Lunar.
4. **ASTRALIS — Corazón del Vacío:** destruir dos Pilares de Escudo, derrotar al jefe final y cruzar el portal.

El estado de partida contempla nombre, vida, energía, experiencia, nivel, misión, núcleos, créditos, mejoras, botiquines, eliminaciones, racha, fragmentos, misiones completadas e inventario.

## 5. Controles
- WASD / flechas: movimiento.
- Ratón: apuntado.
- Clic izquierdo / Espacio: disparar.
- Shift / clic derecho: impulso espectral.
- Q: onda prismática.
- E: activar portal cuando el objetivo esté cumplido.
- H: botiquín.
- U / B: Armería Estelar.
- M: silenciar o activar sonido.
- F5: guardar.
- Esc: pausa.

## 6. Recursos
El proyecto contiene imágenes de planetas en `Assets/Planets/`, interfaz en `Assets/UI/` y audio WAV en `Assets/Audio/`. El archivo de proyecto configura estos recursos para copiarse al directorio de salida.

## 7. Persistencia
Se incluyen guardado JSON, `SqlSaveRepository` y `SQL/CrearBaseNEXUS.sql`. La conexión efectiva depende de que SQL Server y la base de datos estén configurados localmente. GitHub Actions no utiliza una instancia SQL real.

## 8. Correcciones registradas
Según `CORRECCIONES.md`:
1. HUD: evitar dimensiones cero en barras vacías y corregir el radio de figuras redondeadas.
2. Flechas: permitir que lleguen al control de juego.
3. Apuntado en bordes: usar la posición real del jugador cuando la cámara deja de centrarlo.
4. Teclas pegadas: limpiar teclas al volver de ventanas/pausa y al perder foco.
5. Partidas inválidas: sanear misión, nombre, valores y lista de misiones completadas; conservar guardados con vida cero.
6. Enemigos: evitar apariciones dentro de rocas y empujes fuera del mapa o dentro de obstáculos.
7. Repetición de teclas: impedir ráfagas en acciones puntuales como silenciar y usar botiquines.
8. Guardado: mostrar el resultado real de la operación.
9. SQL: limitar el nombre al tamaño de columna, usar tipos explícitos, acotar misión y reducir timeout.
10. Audio: conservar y reutilizar reproductores y detener ambiente al silenciar.

También se registran liberación de recursos gráficos, controles y texturas, reducción de parpadeo y ajuste del tamaño inicial de ventana para portátiles.

## 9. Límites conocidos registrados
- `SoundPlayer` reproduce un sonido a la vez; un efecto puede interrumpir la música ambiental.
- `nexus_splash.png` se copia al compilar, pero `CORRECCIONES.md` indica que no está utilizado por el código.

## 10. GitHub Actions
El workflow `.github/workflows/nexus-ci.yml` se ejecuta en Windows cuando hay push o pull request a `main` que afecte al proyecto, o manualmente desde Actions. Descarga el código, configura .NET 8, restaura dependencias, compila en Release e intenta conservar los archivos de salida como artefacto.

Una ejecución verde demuestra que restauración y compilación finalizaron correctamente en ese runner. No demuestra que todas las mecánicas hayan sido probadas ni valida SQL Server.

## 11. Cómo registrar avances futuros
- Realizar cambios pequeños y relacionados.
- Crear commits descriptivos, por ejemplo `Corrige validación del guardado de partidas`.
- Revisar Actions después de cada push.
- Añadir correcciones a esta bitácora solo después de incorporarlas y verificarlas.
- No atribuir al workflow pruebas que no ejecuta.

## 12. Correspondencia con el compromiso
| Compromiso de NEXUS | Evidencia |
|---|---|
| Actualizar GitHub | Código vigente, documentación y commits |
| Mostrar acciones y cambios | Historial de commits y esta bitácora |
| Respaldar el trabajo | Repositorio y artefactos de compilación |
| Tener guía de POO | `guia-estudio-poo-nexus.md` |
