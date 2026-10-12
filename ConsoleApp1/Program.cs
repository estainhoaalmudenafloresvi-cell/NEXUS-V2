using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ProyectoNexus
{
    /// <summary>
    /// Clase abstracta base para el jerárquico de entidades del sistema NEXUS.
    /// </summary>
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

        /// <summary>
        /// Muestra en consola un análisis detallado de la entidad.
        /// </summary>
        public abstract void AnalizarEntidad();
    }

    /// <summary>
    /// Representa al usuario bajo la figura de Cadete Explorer.
    /// </summary>
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

        public Cadete() { }

        public Cadete(string nombre, int edad, int energiaInicial, int conexionInicial)
            : base(nombre, 100, 25)
        {
            Edad = edad;
            NivelEnergia = energiaInicial;
            EstabilidadConexion = conexionInicial;
            RiesgoRadiacion = 10;

            Inventario.Add("🔋 Célula de Energía (+30 E)");
            Inventario.Add("🛡️ Escudo Nano-Protector (+25 V)");
            Inventario.Add("🔬 Analizador Cuántico");
        }

        public bool PuedeDespegar() => Edad >= 18 && NivelEnergia > 20;

        public void ModificarEnergia(int delta) => NivelEnergia = Math.Clamp(NivelEnergia + delta, 0, 100);
        public void ModificarConexion(int delta) => EstabilidadConexion = Math.Clamp(EstabilidadConexion + delta, 0, 100);
        public void AumentarRadiacion(int delta) => RiesgoRadiacion = Math.Clamp(RiesgoRadiacion + delta, 0, 100);

        public void EscanearIris()
        {
            ModificarEnergia(-10);
            AumentarRadiacion(15);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n👁 [EVENTO]: Escáner activo. Radiación: {RiesgoRadiacion}%. Batería consumida.");
            Console.ResetColor();
        }

        public void Recargar()
        {
            ModificarEnergia(100);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n⚡ Estabilizando traje... Batería restablecida al 100%.");
            Console.ResetColor();
        }

        public bool EvaluarAnomaliaCritica()
        {
            if (NivelEnergia <= 0 || Vida <= 0)
            {
                EjecutarDesconexionEmergencia("🚨 SISTEMA DAÑADO: Pérdida total de soporte vital o energía.");
                return true;
            }
            return false;
        }

        public void EjecutarDesconexionEmergencia(string motivo)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=================================================================");
            Console.WriteLine("          🚨 PROTOCOLO DE DESCONEXIÓN DE EMERGENCIA 🚨          ");
            Console.WriteLine("=================================================================");
            Console.ResetColor();
            Console.WriteLine($"\nMOTIVO: {motivo}\n");

            string[] fases = {
                "Aislando canal cuántico...",
                "Respaldando inventario del Cadete...",
                "Extrayendo cápsula de escape..."
            };

            foreach (var fase in fases)
            {
                Thread.Sleep(600);
                Console.WriteLine($" ⚙️ {fase}");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✨ Cadete retornado a la base con éxito.");
            Console.ResetColor();
        }

        public override void AnalizarEntidad()
        {
            Console.WriteLine($"\n📋 REGISTRO DEL CADETE: {Nombre}");
            Console.WriteLine($"   • Salud vital: {Vida}/{VidaMaxima}");
            Console.WriteLine($"   • Batería del traje: {NivelEnergia}%");
            Console.WriteLine($"   • Experiencia acumulada: {ExperienciaXP} XP");
            Console.WriteLine($"   • Radiación actual: {RiesgoRadiacion}%");
        }
    }

    /// <summary>
    /// Representa a un Robot autómata de defensa.
    /// </summary>
    public class Robot : Personaje
    {
        public Robot() { }
        public Robot(string modelo) : base($"Robot Sentinel-{modelo}", 65, 10) { }

        public override void AnalizarEntidad()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n🔬 [ANALIZADOR]: {Nombre}. Sistema defensivo automatizado actiivo. Blindaje alto.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Representa a un Enemigo de naturaleza hostil.
    /// </summary>
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
            Console.WriteLine($"\n🔬 [ANALIZADOR]: {Nombre} (Clase: {TipoAnomalia}). Entidad altamente inestable.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Gestiona la representación bidimensional del terreno de exploración.
    /// </summary>
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

        public void Renderizar(int jugadorX, int jugadorY)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n🌐 REALIDAD ACTUAL: {NombreRealidad.ToUpper()}");
            Console.ResetColor();

            for (int i = 0; i < Filas; i++)
            {
                Console.Write("   ");
                for (int j = 0; j < Columnas; j++)
                {
                    if (i == jugadorY && j == jugadorX)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" @ ");
                        Console.ResetColor();
                    }
                    else
                    {
                        string celda = matrizTerreno[i, j];
                        if (celda.Contains("⚠") || celda.Contains("●")) Console.ForegroundColor = ConsoleColor.Red;
                        else if (celda.Contains("◆")) Console.ForegroundColor = ConsoleColor.Yellow;
                        else if (celda.Contains("⌂")) Console.ForegroundColor = ConsoleColor.Magenta;
                        else Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.Write(celda);
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine(" 🎮 Controles [W/A/S/D] | Referencias: [@ Cadete | ◆ Objeto/Núcleo | ⚠ Entidad | ⌂ Extracción]\n");
        }
    }

    /// <summary>
    /// Control central del motor de juego NEXUS v1.0.
    /// </summary>
    public class SistemaNexus
    {
        private Cadete cadete;
        private Mapa mapaActual;
        private Dictionary<string, Mapa> mapaRealidades;
        private bool jugando;
        private bool nucleoEncontrado = false;
        private Random random = new Random();
        private const string ArchivoGuardado = "partida_nexus.json";

        public SistemaNexus()
        {
            jugando = true;
            mapaRealidades = new Dictionary<string, Mapa>(StringComparer.OrdinalIgnoreCase);
            CargarRealidades();
        }

        private void CargarRealidades()
        {
            mapaRealidades["A-01"] = new Mapa("Realidad Alfa-01", new string[5, 6] {
                { " ⌂ ", " · ", " · ", " ╦ ", " · ", " ◆ " },
                { " · ", " ⚠ ", " · ", " ║ ", " · ", " · " },
                { " ╠ ", " ═ ", " ═ ", " ╬ ", " ═ ", " ╣ " },
                { " · ", " · ", " ⚠ ", " ║ ", " · ", " ● " },
                { " · ", " ◆ ", " · ", " ╩ ", " ═ ", " ⌂ " }
            });
        }

        public void Iniciar()
        {
            Console.Clear();
            MostrarBanner();

            if (File.Exists(ArchivoGuardado))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("💾 Partida detectada. ¿Cargar progreso? (S/N): ");
                Console.ResetColor();
                if (Console.ReadLine()?.Trim().ToUpper() == "S")
                {
                    if (CargarPartida())
                    {
                        BuclePrincipal();
                        return;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--- REGISTRO DE NUEVO CADETE ---");
            Console.Write("Ingresa tu nombre/código de Cadete: ");
            Console.ResetColor();
            string nombre = Console.ReadLine();

            int edad = LeerEntero("Ingresa tu edad: ");
            int energia = LeerEntero("Batería inicial del traje (ej. 100): ");
            int conexion = LeerEntero("Conexión cuántica inicial (ej. 100): ");

            cadete = new Cadete(nombre, edad, energia, conexion);
            mapaActual = mapaRealidades["A-01"];

            if (!cadete.PuedeDespegar())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ REQUISITOS NO CUMPLIDOS: Para explorar debes ser mayor de 18 años y tener más de 20% de batería.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✨ Cadete {nombre} registrado. Desplegando en {mapaActual.NombreRealidad}...");
            Console.ResetColor();
            Thread.Sleep(1200);

            BuclePrincipal();
        }

        private void BuclePrincipal()
        {
            while (jugando)
            {
                Console.Clear();
                MostrarEstadoCadete();

                Console.WriteLine("🎯 MISIÓN ACTUAL: \"La anomalía perdida\"");
                Console.WriteLine("   1. Encontrar el núcleo energético (◆)");
                Console.WriteLine("   2. Sobrevivir a las entidades (⚠/●)");
                Console.WriteLine("   3. Regresar al punto de extracción (⌂)\n");

                Console.WriteLine("Panel de Control:");
                Console.WriteLine(" 1. 🗺️ Explorar zona");
                Console.WriteLine(" 2. 👁 Activar escáner de iris");
                Console.WriteLine(" 3. 📋 Consultar datos de la misión");
                Console.WriteLine(" 4. ⚡ Recargar traje");
                Console.WriteLine(" 5. 🎒 Administrar inventario");
                Console.WriteLine(" 6. 💾 Guardar partida");
                Console.WriteLine(" 7. 🚀 Abortar misión");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nSelecciona opción (1-7): ");
                Console.ResetColor();

                string op = Console.ReadLine();

                switch (op)
                {
                    case "1": ExplorarZona(); break;
                    case "2": cadete.EscanearIris(); Console.ReadKey(); break;
                    case "3": cadete.AnalizarEntidad(); Console.ReadKey(); break;
                    case "4": cadete.Recargar(); Console.ReadKey(); break;
                    case "5": AdministrarInventario(); break;
                    case "6": GuardarPartida(); Console.ReadKey(); break;
                    case "7":
                        cadete.EjecutarDesconexionEmergencia("Misión abortada voluntariamente.");
                        jugando = false;
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("⚠️️ Opción inválida. Intenta de nuevo.");
                        Console.ReadKey();
                        break;
                }

                if (jugando && cadete.EvaluarAnomaliaCritica())
                {
                    jugando = false;
                    Console.ReadKey();
                }
            }
        }

        private void ExplorarZona()
        {
            bool explorando = true;

            while (explorando && jugando)
            {
                Console.Clear();
                MostrarEstadoCadete();
                mapaActual.Renderizar(cadete.PosX, cadete.PosY);

                Console.Write("Moverse [W/A/S/D] | Regresar al menú [Q]: ");
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                int nx = cadete.PosX;
                int ny = cadete.PosY;

                if (tecla.Key == ConsoleKey.W) ny--;
                else if (tecla.Key == ConsoleKey.S) ny++;
                else if (tecla.Key == ConsoleKey.A) nx--;
                else if (tecla.Key == ConsoleKey.D) nx++;
                else if (tecla.Key == ConsoleKey.Q) { explorando = false; continue; }

                if (nx < 0 || nx >= mapaActual.Columnas || ny < 0 || ny >= mapaActual.Filas)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n🚨 LÍMITE ALCANZADO: No puedes desplazarte fuera del mapa.");
                    Console.ResetColor();
                    Thread.Sleep(900);
                    continue;
                }

                cadete.PosX = nx;
                cadete.PosY = ny;
                cadete.ModificarEnergia(-2);

                ProcesarEventoCasilla(nx, ny);

                if (cadete.EvaluarAnomaliaCritica())
                {
                    jugando = false;
                    explorando = false;
                }
            }
        }

        private void ProcesarEventoCasilla(int x, int y)
        {
            string casilla = mapaActual.ObtenerCasilla(y, x);

            if (casilla.Contains("◆"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n🎁 [OBJETO ENCONTRADO]: ¡Has recuperado el Núcleo Energético!");
                Console.ResetColor();
                cadete.Inventario.Add("⚛️ Núcleo Energético de la Anomalía");
                nucleoEncontrado = true;
                mapaActual.LimpiarCasilla(y, x);
                Thread.Sleep(1200);
            }
            else if (casilla.Contains("⚠") || casilla.Contains("●"))
            {
                Personaje entidad = (random.Next(0, 2) == 0)
                    ? (Personaje)new Robot("DEF-01")
                    : new Enemigo("Anomalía Espectral", "Especial", 60, 15);

                IniciarEncuentroEntidad(entidad);
                mapaActual.LimpiarCasilla(y, x);
            }
            else if (casilla.Contains("⌂"))
            {
                if (nucleoEncontrado)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n🟢 MISIÓN COMPLETADA");
                    Console.WriteLine(" +100 XP");
                    Console.WriteLine(" +1 módulo desconocido (Arma de Plasma)");
                    Console.ResetColor();

                    cadete.ExperienciaXP += 100;
                    cadete.Inventario.Add("🔫 Módulo Desconocido: Rifle de Plasma");
                    jugando = false;
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n🛸 Estás en la zona de extracción. Encuentra el núcleo (◆) antes de retirarte.");
                    Console.ResetColor();
                    Thread.Sleep(1200);
                }
            }
        }

        private void IniciarEncuentroEntidad(Personaje entidad)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n👾 ¡ENTIDAD DETECTADA!: {entidad.Nombre}");
            Console.ResetColor();

            bool combate = true;

            while (combate && cadete.Vida > 0 && entidad.Vida > 0)
            {
                Console.WriteLine($"\nSalud Cadete: [{cadete.Vida}/{cadete.VidaMaxima}] | Salud {entidad.Nombre}: [{entidad.Vida}/{entidad.VidaMaxima}]");
                Console.WriteLine("1. ⚔️ Atacar");
                Console.WriteLine("2. 🏃 Escapar");
                Console.WriteLine("3. 🎒 Utilizar objeto");
                Console.WriteLine("4. 🔬 Analizar entidad");
                Console.Write("Decisión: ");

                string op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        Console.WriteLine($"\n⚔️ Atacaste a {entidad.Nombre} causando {cadete.AtaqueBase} de daño.");
                        entidad.Vida -= cadete.AtaqueBase;

                        if (entidad.Vida > 0)
                        {
                            Console.WriteLine($"💥 {entidad.Nombre} respondió el ataque causando {entidad.AtaqueBase} de daño.");
                            cadete.Vida -= entidad.AtaqueBase;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✨ Neutralizaste la amenaza. Recompensa: +30 XP");
                            Console.ResetColor();
                            cadete.ExperienciaXP += 30;
                            combate = false;
                        }
                        break;

                    case "2":
                        Console.WriteLine("\n🏃 Lograste escapar a una casilla segura.");
                        combate = false;
                        break;

                    case "3":
                        AdministrarInventario();
                        break;

                    case "4":
                        entidad.AnalizarEntidad();
                        break;

                    default:
                        Console.WriteLine("⚠️ Opción no válida.");
                        break;
                }
            }
            Console.WriteLine("\nPresiona una tecla para continuar...");
            Console.ReadKey();
        }

        private void AdministrarInventario()
        {
            Console.WriteLine("\n--- 🎒 INVENTARIO DEL CADETE ---");
            if (cadete.Inventario.Count == 0)
            {
                Console.WriteLine("El inventario está vacío.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < cadete.Inventario.Count; i++)
                Console.WriteLine($" {i + 1}. {cadete.Inventario[i]}");

            Console.Write("\nSelecciona un número para utilizar (o 0 para volver): ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= cadete.Inventario.Count)
            {
                string item = cadete.Inventario[idx - 1];
                cadete.Inventario.RemoveAt(idx - 1);

                if (item.Contains("Energía"))
                {
                    cadete.ModificarEnergia(30);
                    Console.WriteLine("🔋 Batería restaurada +30.");
                }
                else if (item.Contains("Escudo"))
                {
                    cadete.Vida = Math.Min(cadete.VidaMaxima, cadete.Vida + 25);
                    Console.WriteLine("🛡️ Salud de traje restaurada +25.");
                }
                else
                {
                    Console.WriteLine($"Usaste {item}.");
                }
            }
            else if (idx != 0)
            {
                Console.WriteLine("⚠️ Intentaste seleccionar un objeto inexistente.");
            }
            Console.ReadKey();
        }

        private void GuardarPartida()
        {
            try
            {
                string json = JsonSerializer.Serialize(cadete, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ArchivoGuardado, json);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n💾 Estado guardado en 'partida_nexus.json'.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al guardar: {ex.Message}");
            }
            Console.ReadKey();
        }

        private bool CargarPartida()
        {
            try
            {
                string json = File.ReadAllText(ArchivoGuardado);
                cadete = JsonSerializer.Deserialize<Cadete>(json);
                mapaActual = mapaRealidades["A-01"];
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✨ ¡Bienvenido/a de nuevo, Cadete {cadete.Nombre}!");
                Console.ResetColor();
                Thread.Sleep(1000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void MostrarBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 ╔═════════════════════════════════════════════════════════════╗
 ║   _  _ _____  _  _   ____    ___  _     _   _  _ _____ _____ ║
 ║  | \| |  __|  \ \/ /  / U  |  |  _ \| |   / \  | \| | ____|_   _|║
 ║  | .` |  __|   >  <   | |_| |  | ___/| |__/ _ \ | .` |  _|   | |  ║
 ║  |_|\_|_____| /_/\_\  \____/   |_|   |____/_/ \_\|_|\_|____| |_|  ║
 ╚═════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        private void MostrarEstadoCadete()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($" CADETE: {cadete.Nombre} | XP: {cadete.ExperienciaXP} | ZONA: {mapaActual.NombreRealidad}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($" [Salud: {cadete.Vida}/{cadete.VidaMaxima}] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" [Energía: {cadete.NivelEnergia}%] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" [Radiación: {cadete.RiesgoRadiacion}%]");
            Console.ResetColor();
            Console.WriteLine("------------------------------------------------------------------");
        }

        private int LeerEntero(string msg)
        {
            int r;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(msg);
            Console.ResetColor();
            while (!int.TryParse(Console.ReadLine(), out r))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Ingresa un número válido: ");
                Console.ResetColor();
            }
            return r;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "NEXUS v1.0 - Cadete Explorer";
            SistemaNexus juego = new SistemaNexus();
            juego.Iniciar();
        }
    }
}