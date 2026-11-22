# NEXUS · Expedición Eos — Registro de correcciones

> Revisión hecha leyendo todo el código a mano. En el entorno de revisión no había SDK de .NET,
> así que **no se pudo compilar ni ejecutar**. Compila en tu equipo (`COMPILAR_Y_EJECUTAR.bat`)
> y, si aparece algún error de compilación, mándame el mensaje exacto.

## Errores graves
1. **Fallo al dibujar el HUD (MainForm.cs, `GraphicsExtensions`)**: `FillRoundedRectangle` llamaba a
   `GraphicsPath.AddArc` con ancho 0 cuando una barra estaba vacía (por ejemplo la de XP, que
   empieza en 0, o la de energía), lo que lanza `ArgumentException` al pintar. Ahora se omite el
   dibujo si el ancho/alto es menor que 1 y el radio nunca es 0.
2. **Flechas del teclado (GameCanvas)**: WinForms puede interceptar las flechas como teclas de
   navegación y no entregarlas a `KeyDown`. Se añadió `IsInputKey` para que lleguen siempre.
3. **Apuntado erróneo cerca de los bordes del mapa**: la dirección se calculaba desde el centro de
   la pantalla, pero la cámara se detiene en los bordes y el jugador deja de estar centrado.
   Ahora se calcula desde la posición real del jugador en pantalla y se actualiza cada fotograma.
4. **Teclas "pegadas"**: tras abrir Pausa, Armería o un cuadro de mensaje, WinForms no entrega el
   `KeyUp` y el personaje seguía caminando solo. Se limpian las teclas al volver y al perder el foco.
5. **Partidas guardadas inválidas** (`GameEngine.Load` / `LoadFromState`, JSON y SQL): no se
   validaban `Mision`, el tamaño de `MisionesCompletadas`, nombre nulo, valores negativos, etc.
   Podía dar `IndexOutOfRange` o `NullReference`. Se añadió `Sanitize`. Además, una partida
   autoguardada al morir (Vida 0) se cargaba con 1 de vida; ahora se restaura completa.

## Errores medios
6. Enemigos que **nacían dentro de una roca** quedaban atascados para siempre
   (`SpawnEnemyAt`); la **Onda prismática** podía empujarlos dentro de rocas o fuera del mapa.
7. La **auto-repetición de teclas** disparaba en ráfaga acciones como silenciar (M) o usar botiquines (H).
8. **F5** guardaba sin avisar y el botón "Guardar partida (JSON)" decía "guardada" aunque fallara.
   Ahora se informa del resultado real.
9. **SQL**: `NombreExplorador` es `NVARCHAR(60)` y no se limitaba (error de truncado); parámetros
   ahora con tipo explícito, misión acotada a 1–4 y `Connect Timeout=5` para no congelar la ventana.
10. **Audio**: los `SoundPlayer` se creaban sin conservar referencia (el GC podía cortar el sonido) y
    se creaba uno nuevo por cada disparo. Ahora se cachean; M también detiene el ambiente.

## Mejoras de estabilidad
- Fugas de pinceles/fuentes GDI+ por fotograma (HUD, portal, altares, inicio).
- Los controles (y la textura del planeta) ahora se liberan al volver al menú.
- Textura del planeta cargada como copia en memoria (no bloquea el archivo y se dibuja más rápido).
- Pantalla de inicio con doble búfer (menos parpadeo).
- Ventana inicial limitada al tamaño de la pantalla (1380×860 no cabía en portátiles de 1366×768).
- Nombre del explorador limitado a 40 caracteres; Enter confirma el diálogo.

## Pendiente / límites conocidos (no son errores de compilación)
- `SoundPlayer` solo reproduce **un sonido a la vez**: el primer disparo corta la música ambiente.
  Para música de fondo y efectos simultáneos haría falta una librería como NAudio.
- `Assets/UI/nexus_splash.png` se copia al compilar pero ningún código la usa.
