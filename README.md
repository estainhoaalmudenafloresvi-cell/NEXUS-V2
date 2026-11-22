# NEXUS · Expedición Eos (Edición Galáctica Completa)

Juego de acción táctica 2D en tiempo real desarrollado en **C# / Windows Forms (.NET 8)** con 4 planetas únicos, sistema de mejoras, radar táctico, partículas, audio sintetizado y persistencia dual (JSON + SQL Server).

## Los 4 Planetas y sus Misiones

1. **Planeta 1 — EOS · Jardín de Cristal:** Explora las geodas cristalinas, derrota a los Centinelas de Cristal, recupera los **3 Fragmentos Prismáticos** y activa el Portal al este (`E`).
2. **Planeta 2 — NÉBULA · Archivo Sumergido:** Descarga los **4 Núcleos de Datos** antes de que la **Tormenta Iónica** agote la estabilidad de tu traje (recoge estabilizadores azules para ganar +26s).
3. **Planeta 3 — SYLVARA · Santuario de las Lunas:** Sincroniza los **3 Altares Lunares** permaneciendo dentro de sus círculos de energía y derrota al sub-jefe **Acechador Lunar**.
4. **Planeta 4 — ASTRALIS · Corazón del Vacío:** Destruye los **2 Pilares de Escudo** para desactivar la barrera del jefe final (**El Vigía de Eos**), derrótalo y cruza el portal final.

## Controles

- **WASD / Flechas:** Movimiento libre.
- **Ratón:** Apuntado 360° independiente con mira láser.
- **Clic Izquierdo / Espacio:** Disparar cañón prismático.
- **Shift / Clic Derecho:** Impulso espectral (esquiva con invulnerabilidad).
- **Q:** Onda prismática (repele enemigos y disipa proyectiles).
- **E:** Activar el Portal de extracción cuando la misión esté lista.
- **H:** Usar botiquín táctico.
- **U / B:** Abrir la **Armería Estelar** (mejorar Cañón, Blindaje, Propulsores, Dron Orbital y Botiquines).
- **M:** Silenciar / Activar efectos de sonido.
- **F5 / Esc:** Guardar partida / Menú de Pausa (incluye guardado en SQL Server).

## Cómo ejecutar

- **Opción rápida:** Haz doble clic en `COMPILAR_Y_EJECUTAR.bat` (requiere .NET 8 SDK Desktop).
- **Visual Studio 2022:** Abre `NEXUS.sln` y presiona **F5** (Iniciar).
