using System.Text.Json;

namespace NEXUS;

public sealed class GameState
{
    public string Nombre { get; set; } = "Nova";
    public int Vida { get; set; } = 100;
    public int MaxVida { get; set; } = 100;
    public int Energia { get; set; } = 100;
    public int MaxEnergia { get; set; } = 100;
    public int XP { get; set; }
    public int Nivel { get; set; } = 1;
    public int Mision { get; set; }
    public int Nucleos { get; set; }
    public int Creditos { get; set; } = 45;
    public int ArmaNivel { get; set; } = 1;
    public int EscudoNivel { get; set; } = 1;
    public int PropulsorNivel { get; set; } = 1;
    public int DronNivel { get; set; } = 0;
    public int Botiquines { get; set; } = 2;
    public int Eliminaciones { get; set; }
    public int MejorRacha { get; set; }
    public int Fragmentos { get; set; }
    public bool[] MisionesCompletadas { get; set; } = new bool[4];
    public List<string> Inventario { get; set; } = new() { "Botiquín", "Celda de energía" };
}

public enum EnemyKind
{
    CrystalSentinel,
    MistMoth,
    TorpedoDrone,
    LunarStalker,
    ShieldPylon,
    VoidBoss
}

public sealed class Enemy
{
    public float X, Y, VX, VY, HP, MaxHP, Speed, Radius;
    public float AttackCooldown;
    public float SpecialTimer;
    public bool Elite;
    public bool InvulnerableShield;
    public EnemyKind Kind;
    public string Name = "Eco";
}

public sealed class Shot
{
    public float X, Y, VX, VY, Life, Damage;
    public bool Enemy;
    public Color Tint = Color.FromArgb(178, 240, 255);
    public float Size = 5f;
}

public sealed class Pickup
{
    public float X, Y;
    public string Kind = "fragment"; // fragment, heal, energy, credits, stabilizer
    public float Phase;
}

public sealed class LunarAltar
{
    public float X, Y, Radius = 115f;
    public float Progress; // 0..100
    public bool Completed => Progress >= 100f;
    public string Label = "Altar Lunar";
}

public sealed class Particle
{
    public float X, Y, VX, VY, Life, MaxLife, Size;
    public Color Color;
}

public sealed class FloatingText
{
    public float X, Y, VY, Life;
    public string Text = "";
    public Color Color;
}

public sealed class GhostTrail
{
    public float X, Y, Alpha;
}

/// <summary>Motor arcade-táctico en tiempo real con 4 planetas únicos, misiones exclusivas, jefes, mejoras y efectos visuales.</summary>
public sealed class GameEngine
{
    public GameState S { get; private set; } = new();
    public const float WorldW = 2600f, WorldH = 1700f;

    public float PlayerX { get; private set; } = 210f;
    public float PlayerY { get; private set; } = 850f;
    public float AimX { get; private set; } = 1f;
    public float AimY { get; private set; } = 0f;
    public float PlayerRadius => 18f;

    public float DashTime { get; private set; }
    public float Invulnerable { get; private set; }
    public float FireCooldown { get; private set; }
    public float PulseCooldown { get; private set; }
    public float SpecialCooldown { get; private set; }
    public float ScreenShake { get; private set; }
    public float PulseWaveRadius { get; private set; }
    public float DroneAngle { get; private set; }
    private float droneFireCooldown;

    // Planeta 2: Temporizador de tormenta iónica
    public float StormTimer { get; private set; } = 115f;
    public float MaxStormTimer => 115f;

    // Racha de combate
    public int Combo { get; private set; }
    private float comboTimer;

    public bool Paused { get; set; }
    public bool Complete { get; private set; }
    public bool Dead { get; private set; }
    public bool MissionJustCleared { get; private set; }

    public string Message { get; private set; } = "La señal de Eos te espera. Explora y cumple la misión planetaria.";

    public readonly string[] Missions =
    {
        "PLANETA 1 · EOS — JARDÍN DE CRISTAL",
        "PLANETA 2 · NÉBULA — ARCHIVO SUMERGIDO",
        "PLANETA 3 · SYLVARA — SANTUARIO DE LAS LUNAS",
        "PLANETA 4 · ASTRALIS — CORAZÓN DEL VACÍO"
    };

    public readonly string[] PlanetLore =
    {
        "Reúne los 3 Fragmentos Prismáticos dispersos entre las geodas de cristal y activa el Portal de Extracción al este.",
        "Descarga los 4 Núcleos de Datos antes de que la Tormenta Iónica colapse tu traje. Recoge estabilizadores azules.",
        "Sincroniza los 3 Altares Lunares permaneciendo dentro de sus anillos de energía y derrota al Acechador Lunar.",
        "Destruye los 2 Pilares de Escudo del Vacío para vulnerar al Coloso de Astralis, derrótalo y cruza el Portal."
    };

    public List<Enemy> Enemies { get; } = new();
    public List<Shot> Shots { get; } = new();
    public List<Pickup> Pickups { get; } = new();
    public List<RectangleF> Obstacles { get; } = new();
    public List<LunarAltar> Altars { get; } = new();
    public List<Particle> Particles { get; } = new();
    public List<FloatingText> Texts { get; } = new();
    public List<GhostTrail> Ghosts { get; } = new();

    public int GoalCount { get; private set; }
    public int GoalRequired => S.Mision switch
    {
        0 => 3, // 3 fragmentos
        1 => 4, // 4 núcleos de datos
        2 => 3, // 3 altares lunares
        _ => 1  // 1 jefe final derrotado
    };

    public bool PortalReady => S.Mision switch
    {
        2 => GoalCount >= GoalRequired && !Enemies.Any(e => e.Elite),
        _ => GoalCount >= GoalRequired
    };

    public bool BossAlive => Enemies.Any(e => e.Elite);
    public float PortalX => WorldW - 170f;
    public float PortalY => WorldH / 2f;

    private readonly Random rng = new();
    private float spawnClock, messageClock, elapsed, regenClock;
    private int wave = 1;

    public string Objective => S.Mision switch
    {
        0 => PortalReady
            ? "PORTAL ACTIVO · ACÉRCATE AL ESTE Y PULSA [E]"
            : $"RECOLECTA FRAGMENTOS PRISMÁTICOS: {GoalCount}/{GoalRequired}",
        1 => PortalReady
            ? "ARCHIVOS DESCARGADOS · EXTRAE EN EL PORTAL [E]"
            : $"NÚCLEOS DE DATOS: {GoalCount}/{GoalRequired}  ·  TORMENTA: {Math.Ceiling(StormTimer)}s",
        2 => PortalReady
            ? "SANTUARIO PURIFICADO · CRUZA EL PORTAL [E]"
            : (GoalCount < GoalRequired
                ? $"SINCRONIZA ALTARES LUNARES: {GoalCount}/{GoalRequired} (QUÉDATE EN EL CÍRCULO)"
                : "DERROTA AL ACECHADOR LUNAR"),
        _ => PortalReady
            ? "EL VIGÍA HA CAÍDO · ACTIVA EL PORTAL FINAL [E]"
            : (Enemies.Count(e => e.Kind == EnemyKind.ShieldPylon) > 0
                ? $"DESTRUYE PILARES DE ESCUDO ({Enemies.Count(e => e.Kind == EnemyKind.ShieldPylon)} RESTANTES)"
                : "DERROTA AL COLOSO: EL VIGÍA DE EOS")
    };

    public string MissionName => Missions[Math.Clamp(S.Mision, 0, 3)];

    public void Nueva(string name)
    {
        S = new GameState
        {
            Nombre = string.IsNullOrWhiteSpace(name) ? "Nova" : name.Trim()
        };
        StartMission();
        Message = $"Bienvenida, {S.Nombre}. {PlanetLore[0]}";
    }

    public void Notify(string text, float seconds = 2f)
    {
        Message = text;
        messageClock = seconds;
    }

    public void SelectMission(int missionIndex)
    {
        S.Mision = Math.Clamp(missionIndex, 0, 3);
        S.Vida = S.MaxVida;
        S.Energia = S.MaxEnergia;
        StartMission();
    }

    public void StartMission()
    {
        PlayerX = 210f;
        PlayerY = WorldH / 2f;
        GoalCount = 0;
        wave = 1;
        spawnClock = 1.2f;
        elapsed = 0f;
        StormTimer = MaxStormTimer;
        Combo = 0;
        comboTimer = 0f;
        Complete = false;
        Dead = false;
        Paused = false;
        MissionJustCleared = false;
        PulseWaveRadius = 0f;
        DashTime = 0f;
        Invulnerable = 0f;
        ScreenShake = 0f;
        FireCooldown = 0f;
        droneFireCooldown = 0f;

        Enemies.Clear();
        Shots.Clear();
        Pickups.Clear();
        Obstacles.Clear();
        Altars.Clear();
        Particles.Clear();
        Texts.Clear();
        Ghosts.Clear();

        // Generar obstáculos evitando zona inicial, portal y centros de objetivos
        int obsCount = S.Mision == 3 ? 22 : 34;
        for (int i = 0; i < obsCount; i++)
        {
            float x = 380f + (float)rng.NextDouble() * (WorldW - 760f);
            float y = 130f + (float)rng.NextDouble() * (WorldH - 260f);
            if (Distance(x, y, PlayerX, PlayerY) < 220f) continue;
            if (Distance(x, y, PortalX, PortalY) < 260f) continue;
            float size = 38f + rng.Next(48);
            Obstacles.Add(new RectangleF(x, y, size, size * 0.74f));
        }

        SetupPlanetMission();
        SoundFX.PlayPlanetCue(S.Mision);
    }

    private void SetupPlanetMission()
    {
        switch (S.Mision)
        {
            case 0: // PLANETA 1: EOS - Jardín de Cristal
                for (int i = 0; i < 3; i++)
                {
                    float fx = 680f + i * 560f + rng.Next(-90, 90);
                    float fy = 280f + ((i % 2 == 0) ? 240f : 920f) + rng.Next(-110, 110);
                    ClearObstaclesNear(fx, fy, 95f);
                    Pickups.Add(new Pickup { X = fx, Y = fy, Kind = "fragment", Phase = i * 1.4f });
                    SpawnEnemyAt(EnemyKind.CrystalSentinel, fx + 90f, fy - 50f, false);
                }
                for (int i = 0; i < 3; i++)
                {
                    Pickups.Add(new Pickup
                    {
                        X = 520f + i * 640f,
                        Y = 200f + rng.Next(1250),
                        Kind = i % 2 == 0 ? "energy" : "heal",
                        Phase = i
                    });
                }
                SpawnEnemy(false);
                SpawnEnemy(false);
                break;

            case 1: // PLANETA 2: NÉBULA - Archivo Sumergido (Contrarreloj + Estabilizadores)
                StormTimer = MaxStormTimer;
                for (int i = 0; i < 4; i++)
                {
                    float fx = 580f + i * 440f + rng.Next(-70, 70);
                    float fy = 260f + (i % 2) * 980f + rng.Next(-120, 120);
                    ClearObstaclesNear(fx, fy, 95f);
                    Pickups.Add(new Pickup { X = fx, Y = fy, Kind = "fragment", Phase = i * 1.1f });
                    SpawnEnemyAt(EnemyKind.TorpedoDrone, fx + 70f, fy + 60f, false);
                }
                for (int i = 0; i < 5; i++)
                {
                    float sx = 450f + i * 410f;
                    float sy = 350f + (i % 3) * 480f;
                    ClearObstaclesNear(sx, sy, 80f);
                    Pickups.Add(new Pickup { X = sx, Y = sy, Kind = "stabilizer", Phase = i * 0.8f });
                }
                SpawnEnemy(false);
                SpawnEnemy(false);
                break;

            case 2: // PLANETA 3: SYLVARA - Santuario de las Lunas (3 Altares de sincronización + Sub-jefe)
                (float ax, float ay, string lbl)[] altarCoords =
                {
                    (740f, 420f, "ALTAR DEL ALBA"),
                    (1340f, 1260f, "ALTAR DEL CENIT"),
                    (1920f, 520f, "ALTAR DEL OCASO")
                };
                foreach (var (ax, ay, lbl) in altarCoords)
                {
                    ClearObstaclesNear(ax, ay, 155f);
                    Altars.Add(new LunarAltar { X = ax, Y = ay, Label = lbl });
                    SpawnEnemyAt(EnemyKind.MistMoth, ax + 110f, ay, false);
                }
                for (int i = 0; i < 4; i++)
                {
                    Pickups.Add(new Pickup
                    {
                        X = 500f + i * 500f,
                        Y = 850f + rng.Next(-450, 450),
                        Kind = i % 2 == 0 ? "heal" : "energy",
                        Phase = i
                    });
                }
                break;

            case 3: // PLANETA 4: ASTRALIS - Corazón del Vacío (2 Pilares de escudo + Jefe Final)
                float bossX = 1860f, bossY = WorldH / 2f;
                ClearObstaclesNear(bossX, bossY, 340f);
                SpawnEnemyAt(EnemyKind.VoidBoss, bossX, bossY, true);

                float p1x = 1480f, p1y = 430f;
                float p2x = 1480f, p2y = WorldH - 430f;
                ClearObstaclesNear(p1x, p1y, 130f);
                ClearObstaclesNear(p2x, p2y, 130f);
                SpawnEnemyAt(EnemyKind.ShieldPylon, p1x, p1y, false);
                SpawnEnemyAt(EnemyKind.ShieldPylon, p2x, p2y, false);

                for (int i = 0; i < 4; i++)
                {
                    Pickups.Add(new Pickup
                    {
                        X = 650f + (i % 2) * 520f,
                        Y = 360f + (i / 2) * 980f,
                        Kind = i % 2 == 0 ? "heal" : "energy",
                        Phase = i
                    });
                }
                break;
        }
    }

    private void ClearObstaclesNear(float x, float y, float radius)
    {
        Obstacles.RemoveAll(o => Distance(o.X + o.Width / 2f, o.Y + o.Height / 2f, x, y) < radius);
    }

    public void SetAim(float dx, float dy)
    {
        float len = MathF.Sqrt(dx * dx + dy * dy);
        if (len > 0.01f)
        {
            AimX = dx / len;
            AimY = dy / len;
        }
    }

    public void Update(float dt, bool up, bool down, bool left, bool right, bool firing, bool hasMouseAim)
    {
        if (Paused || Dead || Complete) return;
        dt = Math.Clamp(dt, 0f, 0.045f);
        elapsed += dt;

        messageClock = Math.Max(0, messageClock - dt);
        FireCooldown = Math.Max(0, FireCooldown - dt);
        PulseCooldown = Math.Max(0, PulseCooldown - dt);
        SpecialCooldown = Math.Max(0, SpecialCooldown - dt);
        DashTime = Math.Max(0, DashTime - dt);
        Invulnerable = Math.Max(0, Invulnerable - dt);
        ScreenShake = Math.Max(0, ScreenShake - dt * 28f);
        droneFireCooldown = Math.Max(0, droneFireCooldown - dt);

        if (PulseWaveRadius > 0)
        {
            PulseWaveRadius += 920f * dt;
            if (PulseWaveRadius > 300f) PulseWaveRadius = 0f;
        }

        if (comboTimer > 0)
        {
            comboTimer -= dt;
            if (comboTimer <= 0) Combo = 0;
        }

        // Regeneración suave de energía
        regenClock += dt;
        if (regenClock >= 0.35f)
        {
            regenClock = 0f;
            if (S.Energia < S.MaxEnergia)
                S.Energia = Math.Min(S.MaxEnergia, S.Energia + 1);
        }

        // Tormenta iónica en Planeta 2
        if (S.Mision == 1 && !PortalReady)
        {
            StormTimer -= dt;
            if (StormTimer <= 0f)
            {
                StormTimer = 0f;
                S.Vida = 0;
                Dead = true;
                Message = "¡La tormenta iónica de Nébula colapsó tu traje! Recoge estabilizadores azules.";
                return;
            }
        }

        // Movimiento libre (no sobrescribe el apuntado del ratón)
        float dx = (right ? 1 : 0) - (left ? 1 : 0);
        float dy = (down ? 1 : 0) - (up ? 1 : 0);
        float len = MathF.Sqrt(dx * dx + dy * dy);
        if (len > 0)
        {
            dx /= len;
            dy /= len;
            if (!hasMouseAim)
                SetAim(dx, dy);

            float baseSpeed = 265f + (S.PropulsorNivel - 1) * 24f;
            float speed = DashTime > 0 ? 740f : baseSpeed;
            MovePlayer(dx * speed * dt, dy * speed * dt);

            if (DashTime > 0)
                Ghosts.Add(new GhostTrail { X = PlayerX, Y = PlayerY, Alpha = 0.75f });
        }

        if (firing)
            Shoot();

        // Actualizar Dron Orbital (si está desbloqueado)
        UpdateOrbitalDrone(dt);

        // Actualizar Altares Lunares (Planeta 3)
        if (S.Mision == 2)
            UpdateLunarAltars(dt);

        // Actualizar Proyectiles
        foreach (var s in Shots.ToArray())
        {
            s.X += s.VX * dt;
            s.Y += s.VY * dt;
            s.Life -= dt;

            if (s.Life <= 0 || Blocked(s.X, s.Y))
            {
                SpawnSparks(s.X, s.Y, s.Tint, 4, 80f);
                Shots.Remove(s);
                continue;
            }

            if (s.Enemy)
            {
                if (Distance(s.X, s.Y, PlayerX, PlayerY) < PlayerRadius + s.Size)
                {
                    Shots.Remove(s);
                    Hurt((int)s.Damage);
                }
            }
            else
            {
                var hit = Enemies.FirstOrDefault(e => Distance(s.X, s.Y, e.X, e.Y) < e.Radius + s.Size);
                if (hit != null)
                {
                    Shots.Remove(s);
                    if (hit.InvulnerableShield)
                    {
                        SpawnSparks(s.X, s.Y, Color.Cyan, 7, 130f);
                        AddFloatingText(hit.X, hit.Y - hit.Radius - 12, "ESCUDO ACTIVO", Color.Cyan);
                        SoundFX.PlayHit();
                    }
                    else
                    {
                        bool crit = rng.NextDouble() < 0.16;
                        float dmg = crit ? s.Damage * 1.5f : s.Damage;
                        hit.HP -= dmg;
                        SpawnSparks(s.X, s.Y, crit ? Color.Gold : s.Tint, crit ? 9 : 5, 140f);
                        AddFloatingText(hit.X, hit.Y - hit.Radius - 8,
                            crit ? $"¡{(int)dmg}!" : $"-{(int)dmg}",
                            crit ? Color.Gold : Color.FromArgb(255, 220, 250));
                        SoundFX.PlayHit();
                        if (hit.HP <= 0)
                            Kill(hit);
                    }
                }
            }
        }

        // Actualizar Enemigos
        foreach (var e in Enemies.ToArray())
            UpdateEnemy(e, dt);

        // Actualizar Pickups (con leve atracción magnética)
        foreach (var p in Pickups.ToArray())
        {
            float dist = Distance(PlayerX, PlayerY, p.X, p.Y);
            if (dist < 130f && dist > 1f)
            {
                p.X += (PlayerX - p.X) / dist * 240f * dt;
                p.Y += (PlayerY - p.Y) / dist * 240f * dt;
            }

            if (dist < 34f)
            {
                Pickups.Remove(p);
                CollectPickup(p);
            }
        }

        // Verificar derrota del jefe en Planeta 4
        if (S.Mision == 3 && !BossAlive && GoalCount == 0)
        {
            GoalCount = 1;
            Message = "¡EL VIGÍA DE EOS HA CAÍDO! Cruza el portal al este para completar la campaña.";
            SoundFX.PlayLevelUp();
        }

        // Oleadas dinámicas según planeta
        spawnClock -= dt;
        if (spawnClock <= 0f && !PortalReady)
        {
            int maxEnemies = S.Mision switch { 0 => 6, 1 => 7, 2 => 8, _ => 5 };
            if (S.Mision < 3 && Enemies.Count < maxEnemies)
            {
                SpawnEnemy(false);
                spawnClock = Math.Max(2.6f, 6.2f - wave * 0.45f);
                wave++;
            }
            else if (S.Mision == 3 && BossAlive && Enemies.Count < 4)
            {
                SpawnEnemyAt(EnemyKind.TorpedoDrone, PortalX - 180f, rng.Next(240, (int)WorldH - 240), false);
                spawnClock = 8.5f;
            }
        }

        // Actualizar partículas, textos flotantes y estelas
        foreach (var pt in Particles.ToArray())
        {
            pt.X += pt.VX * dt;
            pt.Y += pt.VY * dt;
            pt.Life -= dt;
            if (pt.Life <= 0) Particles.Remove(pt);
        }
        foreach (var ft in Texts.ToArray())
        {
            ft.Y += ft.VY * dt;
            ft.Life -= dt;
            if (ft.Life <= 0) Texts.Remove(ft);
        }
        foreach (var gh in Ghosts.ToArray())
        {
            gh.Alpha -= dt * 3.2f;
            if (gh.Alpha <= 0) Ghosts.Remove(gh);
        }

        if (S.Vida <= 0)
        {
            S.Vida = 0;
            Dead = true;
            SoundFX.PlayExplosion();
            Message = "Integridad crítica. La expedición se ha detenido.";
        }
    }

    private void UpdateLunarAltars(float dt)
    {
        int completedCount = 0;

        foreach (var altar in Altars)
        {
            if (altar.Completed)
            {
                completedCount++;
                continue;
            }

            float dist = Distance(PlayerX, PlayerY, altar.X, altar.Y);
            if (dist <= altar.Radius)
            {
                altar.Progress = Math.Min(100f, altar.Progress + 24f * dt);
                if (rng.NextDouble() < 0.35)
                    SpawnSparks(altar.X + rng.Next(-50, 50), altar.Y + rng.Next(-50, 50), Color.FromArgb(180, 255, 220), 1, 60f);

                if (altar.Completed)
                {
                    completedCount++;
                    S.XP += 45;
                    S.Creditos += 20;
                    SoundFX.PlayLevelUp();
                    ScreenShake = 6f;
                    AddFloatingText(altar.X, altar.Y - 40, $"{altar.Label} SINCRONIZADO", Color.Aquamarine);
                    Message = $"{altar.Label} activado ({completedCount}/{GoalRequired}).";
                    SpawnEnemy(false);
                    LevelUp();
                }
            }
            else if (altar.Progress > 0f)
            {
                altar.Progress = Math.Max(0f, altar.Progress - 4f * dt);
            }
        }

        GoalCount = completedCount;

        // Al completar los 3 altares, invocar al sub-jefe Acechador Lunar si aún no ha aparecido
        if (GoalCount >= GoalRequired && !Enemies.Any(e => e.Elite) && !MissionJustCleared)
        {
            MissionJustCleared = true;
            SpawnEnemyAt(EnemyKind.LunarStalker, PortalX - 240f, PortalY, true);
            Message = "¡EL ACECHADOR LUNAR HA DESPERTADO! Derrótalo para abrir el portal.";
            ScreenShake = 9f;
            SoundFX.PlayPulse();
        }
    }

    private void UpdateOrbitalDrone(float dt)
    {
        if (S.DronNivel <= 0) return;
        DroneAngle += (2.4f + S.DronNivel * 0.4f) * dt;
        if (droneFireCooldown > 0 || Enemies.Count == 0) return;

        float dxPos = PlayerX + MathF.Cos(DroneAngle) * 44f;
        float dyPos = PlayerY + MathF.Sin(DroneAngle) * 44f;

        var target = Enemies
            .Where(e => !e.InvulnerableShield)
            .OrderBy(e => Distance(dxPos, dyPos, e.X, e.Y))
            .FirstOrDefault();

        if (target != null && Distance(dxPos, dyPos, target.X, target.Y) < 460f)
        {
            droneFireCooldown = Math.Max(0.32f, 0.85f - S.DronNivel * 0.16f);
            float tx = target.X - dxPos, ty = target.Y - dyPos;
            float d = MathF.Max(1f, MathF.Sqrt(tx * tx + ty * ty));
            Shots.Add(new Shot
            {
                X = dxPos,
                Y = dyPos,
                VX = tx / d * 760f,
                VY = ty / d * 760f,
                Life = 0.85f,
                Damage = 10f + S.DronNivel * 5f,
                Tint = Color.FromArgb(130, 255, 210),
                Size = 4f
            });
        }
    }

    private void CollectPickup(Pickup p)
    {
        SoundFX.PlayPickup();
        switch (p.Kind)
        {
            case "fragment":
                GoalCount++;
                S.Fragmentos++;
                S.XP += 35;
                S.Creditos += 15;
                SpawnSparks(p.X, p.Y, Color.FromArgb(255, 190, 245), 16, 180f);
                AddFloatingText(p.X, p.Y - 15, S.Mision == 1 ? "+1 NÚCLEO DE DATOS" : "+1 FRAGMENTO", Color.FromArgb(255, 200, 250));
                Message = PortalReady
                    ? "¡Todos los objetivos reunidos! Dirígete al Portal al este y pulsa [E]."
                    : $"Objetivo recuperado ({GoalCount}/{GoalRequired}). +35 XP · +15 CR";
                LevelUp();
                break;

            case "stabilizer":
                StormTimer = Math.Min(MaxStormTimer, StormTimer + 26f);
                S.Energia = Math.Min(S.MaxEnergia, S.Energia + 20);
                SpawnSparks(p.X, p.Y, Color.DeepSkyBlue, 12, 140f);
                AddFloatingText(p.X, p.Y - 15, "+26s ESTABILIDAD", Color.DeepSkyBlue);
                Message = "Estabilizador atmosférico recogido: +26s frente a la tormenta.";
                break;

            case "heal":
                S.Vida = Math.Min(S.MaxVida, S.Vida + 28);
                SpawnSparks(p.X, p.Y, Color.LightPink, 10, 130f);
                AddFloatingText(p.X, p.Y - 15, "+28 INTEGRIDAD", Color.LightPink);
                Message = "Nanogel reparador: +28 de integridad.";
                break;

            case "credits":
                S.Creditos += 14;
                SpawnSparks(p.X, p.Y, Color.Gold, 10, 130f);
                AddFloatingText(p.X, p.Y - 15, "+14 CR", Color.Gold);
                break;

            default:
                S.Energia = Math.Min(S.MaxEnergia, S.Energia + 32);
                S.Creditos += 6;
                SpawnSparks(p.X, p.Y, Color.Cyan, 10, 130f);
                AddFloatingText(p.X, p.Y - 15, "+32 ENERGÍA", Color.Cyan);
                Message = "Cristales de energía absorbidos.";
                break;
        }
        messageClock = 3f;
    }

    private void MovePlayer(float dx, float dy)
    {
        float nx = Math.Clamp(PlayerX + dx, 36f, WorldW - 36f);
        float ny = Math.Clamp(PlayerY + dy, 36f, WorldH - 36f);
        if (!Blocked(nx, PlayerY)) PlayerX = nx;
        if (!Blocked(PlayerX, ny)) PlayerY = ny;
    }

    private bool Blocked(float x, float y) => Obstacles.Any(o => o.Contains(x, y));

    public void Shoot()
    {
        if (FireCooldown > 0 || Dead || Paused) return;
        FireCooldown = Math.Max(0.11f, 0.21f - (S.ArmaNivel - 1) * 0.02f);
        SoundFX.PlayLaser();

        float baseDmg = 18f + (S.ArmaNivel - 1) * 5f;
        float speed = 740f;
        float baseAngle = MathF.Atan2(AimY, AimX);

        if (S.ArmaNivel == 1)
        {
            AddPlayerShot(baseAngle, speed, baseDmg, 5f);
        }
        else if (S.ArmaNivel == 2)
        {
            float perpX = -AimY * 7f, perpY = AimX * 7f;
            Shots.Add(new Shot { X = PlayerX + AimX * 22 + perpX, Y = PlayerY + AimY * 22 + perpY, VX = AimX * speed, VY = AimY * speed, Life = 1.25f, Damage = baseDmg * 0.72f, Size = 4.5f });
            Shots.Add(new Shot { X = PlayerX + AimX * 22 - perpX, Y = PlayerY + AimY * 22 - perpY, VX = AimX * speed, VY = AimY * speed, Life = 1.25f, Damage = baseDmg * 0.72f, Size = 4.5f });
        }
        else if (S.ArmaNivel <= 4)
        {
            float spread = 0.11f;
            AddPlayerShot(baseAngle - spread, speed, baseDmg * 0.65f, 4.5f);
            AddPlayerShot(baseAngle, speed, baseDmg * 0.85f, 5.5f);
            AddPlayerShot(baseAngle + spread, speed, baseDmg * 0.65f, 4.5f);
        }
        else
        {
            for (int i = -2; i <= 2; i++)
                AddPlayerShot(baseAngle + i * 0.09f, speed, baseDmg * 0.62f, 5f);
        }
    }

    private void AddPlayerShot(float angle, float speed, float dmg, float size)
    {
        float cx = MathF.Cos(angle), cy = MathF.Sin(angle);
        Shots.Add(new Shot
        {
            X = PlayerX + cx * 22f,
            Y = PlayerY + cy * 22f,
            VX = cx * speed,
            VY = cy * speed,
            Life = 1.25f,
            Damage = dmg,
            Size = size,
            Tint = S.ArmaNivel >= 4 ? Color.FromArgb(255, 190, 250) : Color.FromArgb(165, 242, 255)
        });
    }

    public void Dash()
    {
        int cost = Math.Max(8, 16 - (S.PropulsorNivel - 1) * 2);
        if (S.Energia < cost || SpecialCooldown > 0) return;
        S.Energia -= cost;
        DashTime = 0.24f;
        Invulnerable = 0.40f;
        SpecialCooldown = Math.Max(0.65f, 1.15f - (S.PropulsorNivel - 1) * 0.14f);
        MovePlayer(AimX * 95f, AimY * 95f);
        SpawnSparks(PlayerX, PlayerY, Color.FromArgb(230, 175, 255), 14, 170f);
        SoundFX.PlayDash();
        Message = "Impulso espectral activado.";
        messageClock = 1.2f;
    }

    public void Pulse()
    {
        if (S.Energia < 28 || PulseCooldown > 0) return;
        S.Energia -= 28;
        PulseCooldown = 4.5f;
        PulseWaveRadius = 18f;
        ScreenShake = 8f;
        SoundFX.PlayPulse();

        // Neutralizar proyectiles enemigos cercanos
        Shots.RemoveAll(s => s.Enemy && Distance(PlayerX, PlayerY, s.X, s.Y) < 280f);

        foreach (var e in Enemies.ToArray())
        {
            float d = Distance(PlayerX, PlayerY, e.X, e.Y);
            if (d < 280f)
            {
                if (!e.InvulnerableShield)
                {
                    float dmg = 58f + S.Nivel * 6f;
                    e.HP -= dmg;
                    AddFloatingText(e.X, e.Y - e.Radius - 10, $"-{(int)dmg}", Color.Magenta);
                }
                if (!e.Elite && e.Kind != EnemyKind.ShieldPylon && d > 1f)
                {
                    float kx = Math.Clamp(e.X + (e.X - PlayerX) / d * 85f, 30f, WorldW - 30f);
                    float ky = Math.Clamp(e.Y + (e.Y - PlayerY) / d * 85f, 30f, WorldH - 30f);
                    if (!Blocked(kx, ky))
                    {
                        e.X = kx;
                        e.Y = ky;
                    }
                }
                if (e.HP <= 0) Kill(e);
            }
        }
        Message = "ONDA PRISMÁTICA · Proyectiles disipados y enemigos repelidos.";
        messageClock = 2f;
    }

    public void Interact()
    {
        if (PortalReady && Distance(PlayerX, PlayerY, PortalX, PortalY) < 210f)
        {
            FinishMission();
            return;
        }

        if (!PortalReady)
            Message = $"Portal bloqueado: completa primero la misión ({Objective}).";
        else
            Message = "El Portal está activo al este (sigue la flecha guía) y pulsa [E].";
        messageClock = 2.5f;
    }

    public void UseMedkit()
    {
        if (S.Vida >= S.MaxVida)
        {
            Message = "Integridad del traje al 100%.";
            messageClock = 1.6f;
            return;
        }

        if (S.Botiquines > 0)
        {
            S.Botiquines--;
            S.Vida = Math.Min(S.MaxVida, S.Vida + 45);
            SpawnSparks(PlayerX, PlayerY, Color.LightGreen, 14, 150f);
            AddFloatingText(PlayerX, PlayerY - 25, "+45 BOTIQUÍN", Color.LightGreen);
            SoundFX.PlayPickup();
            Message = $"Botiquín aplicado ({S.Botiquines} restantes).";
        }
        else if (S.Creditos >= 20)
        {
            S.Creditos -= 20;
            S.Vida = Math.Min(S.MaxVida, S.Vida + 40);
            SpawnSparks(PlayerX, PlayerY, Color.LightGreen, 14, 150f);
            AddFloatingText(PlayerX, PlayerY - 25, "+40 REPARACIÓN (-20 CR)", Color.LightGreen);
            SoundFX.PlayPickup();
            Message = "Reparación de emergencia aplicada (-20 CR).";
        }
        else
        {
            Message = "Sin botiquines ni créditos suficientes (20 CR).";
        }
        messageClock = 2f;
    }

    public void Retry()
    {
        S.Vida = S.MaxVida;
        S.Energia = S.MaxEnergia;
        if (S.Botiquines == 0) S.Botiquines = 1;
        StartMission();
        Message = $"Reintento iniciado en {MissionName}.";
    }

    private void UpdateEnemy(Enemy e, float dt)
    {
        // Actualizar invulnerabilidad del jefe final según pilares activos
        if (e.Kind == EnemyKind.VoidBoss)
        {
            e.InvulnerableShield = Enemies.Any(x => x.Kind == EnemyKind.ShieldPylon);
        }

        float dx = PlayerX - e.X, dy = PlayerY - e.Y;
        float d = MathF.Max(1f, MathF.Sqrt(dx * dx + dy * dy));
        e.AttackCooldown = Math.Max(0f, e.AttackCooldown - dt);
        e.SpecialTimer += dt;

        if (e.Kind == EnemyKind.ShieldPylon)
        {
            // Pilar estático que dispara pulsos defensivos periódicos
            if (d < 620f && e.AttackCooldown <= 0f)
            {
                e.AttackCooldown = 2.3f;
                Shots.Add(new Shot { X = e.X, Y = e.Y, VX = dx / d * 250f, VY = dy / d * 250f, Life = 2.4f, Damage = 10f, Enemy = true, Tint = Color.Cyan, Size = 6f });
            }
            return;
        }

        if (d > e.Radius + PlayerRadius + 10f)
        {
            float rage = (e.Elite && e.HP < e.MaxHP * 0.5f) ? 1.32f : 1f;
            float sp = e.Speed * rage * (d < 240f ? 1.15f : 1f);
            float nx = e.X + dx / d * sp * dt;
            float ny = e.Y + dy / d * sp * dt;
            if (!Blocked(nx, e.Y)) e.X = nx;
            if (!Blocked(e.X, ny)) e.Y = ny;
        }
        else if (e.AttackCooldown <= 0f)
        {
            e.AttackCooldown = e.Elite ? 0.95f : 1.2f;
            Hurt(e.Elite ? 16 : 9);
        }

        // Ataques a distancia según tipo de enemigo
        if (e.Kind == EnemyKind.VoidBoss && !e.InvulnerableShield && d < 780f && e.AttackCooldown <= 0f)
        {
            bool phase2 = e.HP < e.MaxHP * 0.5f;
            e.AttackCooldown = phase2 ? 1.25f : 1.65f;
            float baseAng = MathF.Atan2(dy, dx);
            int fan = phase2 ? 5 : 3;
            for (int i = -(fan / 2); i <= fan / 2; i++)
            {
                float a = baseAng + i * 0.22f;
                Shots.Add(new Shot
                {
                    X = e.X,
                    Y = e.Y,
                    VX = MathF.Cos(a) * 310f,
                    VY = MathF.Sin(a) * 310f,
                    Life = 2.5f,
                    Damage = 12f,
                    Enemy = true,
                    Tint = Color.FromArgb(255, 95, 175),
                    Size = 6.5f
                });
            }
        }
        else if (e.Kind == EnemyKind.LunarStalker && d < 650f && e.AttackCooldown <= 0f)
        {
            e.AttackCooldown = 1.45f;
            float baseAng = MathF.Atan2(dy, dx);
            for (int i = -1; i <= 1; i += 2)
            {
                float a = baseAng + i * 0.14f;
                Shots.Add(new Shot
                {
                    X = e.X,
                    Y = e.Y,
                    VX = MathF.Cos(a) * 330f,
                    VY = MathF.Sin(a) * 330f,
                    Life = 2.1f,
                    Damage = 11f,
                    Enemy = true,
                    Tint = Color.FromArgb(190, 255, 220),
                    Size = 5.5f
                });
            }
        }
        else if (e.Kind == EnemyKind.CrystalSentinel && d > 170f && d < 520f && e.AttackCooldown <= 0f)
        {
            e.AttackCooldown = 2.4f;
            Shots.Add(new Shot
            {
                X = e.X,
                Y = e.Y,
                VX = dx / d * 255f,
                VY = dy / d * 255f,
                Life = 2.0f,
                Damage = 8f,
                Enemy = true,
                Tint = Color.FromArgb(255, 130, 210),
                Size = 5f
            });
        }
    }

    private void Hurt(int amount)
    {
        if (Invulnerable > 0) return;
        int reduced = Math.Max(3, amount - (S.EscudoNivel - 1) * 2);
        S.Vida = Math.Max(0, S.Vida - reduced);
        Invulnerable = 0.58f;
        ScreenShake = 7f;
        Combo = 0;
        SpawnSparks(PlayerX, PlayerY, Color.FromArgb(255, 100, 150), 12, 160f);
        AddFloatingText(PlayerX, PlayerY - 26, $"-{reduced}", Color.FromArgb(255, 110, 150));
        SoundFX.PlayHit();
        Message = $"Impacto recibido (-{reduced}) · Integridad {S.Vida}/{S.MaxVida}";
        messageClock = 1.3f;
    }

    private void Kill(Enemy e)
    {
        Enemies.Remove(e);
        S.Eliminaciones++;
        Combo++;
        comboTimer = 4.2f;
        if (Combo > S.MejorRacha) S.MejorRacha = Combo;

        int bonusCr = Math.Min(15, Combo * 2);
        int xpGain = e.Elite ? 120 : (e.Kind == EnemyKind.ShieldPylon ? 50 : 24);
        int crGain = (e.Elite ? 55 : 9) + bonusCr;

        S.XP += xpGain;
        S.Creditos += crGain;
        ScreenShake = e.Elite ? 14f : 4.5f;
        SoundFX.PlayExplosion();

        SpawnSparks(e.X, e.Y, e.Elite ? Color.Gold : Color.FromArgb(255, 145, 215), e.Elite ? 34 : 16, e.Elite ? 260f : 170f);
        AddFloatingText(e.X, e.Y - 12, Combo > 1 ? $"RACHA x{Combo} +{crGain} CR" : $"+{crGain} CR", Color.Gold);

        if (e.Kind == EnemyKind.ShieldPylon)
        {
            int rem = Enemies.Count(x => x.Kind == EnemyKind.ShieldPylon);
            Message = rem > 0
                ? "¡Pilar de escudo destruido! Destruye el pilar restante."
                : "¡ESCUDO DEL COLOSO DESACTIVADO! El Vigía de Eos ahora es vulnerable.";
        }
        else if (e.Elite)
        {
            Message = $"¡{e.Name} NEUTRALIZADO! El portal de extracción está abierto.";
        }
        else
        {
            if (rng.NextDouble() < 0.42)
            {
                string kind = rng.Next(3) switch { 0 => "heal", 1 => "energy", _ => "credits" };
                Pickups.Add(new Pickup { X = e.X, Y = e.Y, Kind = kind });
            }
            Message = $"{e.Name} eliminado · +{xpGain} XP · +{crGain} CR";
        }
        messageClock = 2.4f;
        LevelUp();
    }

    private void SpawnEnemy(bool elite)
    {
        float x = PlayerX > WorldW / 2f ? 220f : WorldW - 240f;
        float y = rng.Next(150, (int)WorldH - 150);
        EnemyKind kind = S.Mision switch
        {
            0 => rng.Next(2) == 0 ? EnemyKind.CrystalSentinel : EnemyKind.MistMoth,
            1 => rng.Next(2) == 0 ? EnemyKind.TorpedoDrone : EnemyKind.MistMoth,
            2 => rng.Next(2) == 0 ? EnemyKind.MistMoth : EnemyKind.CrystalSentinel,
            _ => EnemyKind.TorpedoDrone
        };
        SpawnEnemyAt(kind, x, y, elite);
    }

    private void SpawnEnemyAt(EnemyKind kind, float x, float y, bool elite)
    {
        float mScale = 1f + S.Mision * 0.16f;
        var enemy = kind switch
        {
            EnemyKind.VoidBoss => new Enemy
            {
                X = x, Y = y, HP = 680f, MaxHP = 680f, Speed = 92f, Radius = 48f,
                Elite = true, InvulnerableShield = true, Kind = kind, Name = "EL VIGÍA DE EOS"
            },
            EnemyKind.LunarStalker => new Enemy
            {
                X = x, Y = y, HP = 360f, MaxHP = 360f, Speed = 132f, Radius = 36f,
                Elite = true, Kind = kind, Name = "ACECHADOR LUNAR"
            },
            EnemyKind.ShieldPylon => new Enemy
            {
                X = x, Y = y, HP = 180f, MaxHP = 180f, Speed = 0f, Radius = 30f,
                Elite = false, Kind = kind, Name = "PILAR DE ESCUDO"
            },
            EnemyKind.TorpedoDrone => new Enemy
            {
                X = x, Y = y, HP = 44f * mScale, MaxHP = 44f * mScale, Speed = 148f, Radius = 18f,
                Elite = false, Kind = kind, Name = "Dron Torpedo"
            },
            EnemyKind.CrystalSentinel => new Enemy
            {
                X = x, Y = y, HP = 64f * mScale, MaxHP = 64f * mScale, Speed = 94f, Radius = 22f,
                Elite = false, Kind = kind, Name = "Centinela de Cristal"
            },
            _ => new Enemy
            {
                X = x, Y = y, HP = 48f * mScale, MaxHP = 48f * mScale, Speed = 115f, Radius = 19f,
                Elite = false, Kind = kind, Name = "Polilla de Niebla"
            }
        };
        // Evita que el enemigo nazca incrustado en una roca (quedaría atascado para siempre)
        Obstacles.RemoveAll(o =>
        {
            var area = o;
            area.Inflate(enemy.Radius, enemy.Radius);
            return area.Contains(x, y);
        });
        Enemies.Add(enemy);
    }

    private void LevelUp()
    {
        while (S.XP >= S.Nivel * 100)
        {
            S.XP -= S.Nivel * 100;
            S.Nivel++;
            S.MaxVida += 10;
            S.MaxEnergia += 10;
            S.Vida = S.MaxVida;
            S.Energia = S.MaxEnergia;
            S.Creditos += 25;
            SoundFX.PlayLevelUp();
            SpawnSparks(PlayerX, PlayerY, Color.Gold, 24, 210f);
            AddFloatingText(PlayerX, PlayerY - 40, $"¡NIVEL {S.Nivel}!", Color.Gold);
            Message = $"¡NIVEL {S.Nivel} ALCANZADO! Integridad y energía máximas aumentadas.";
            messageClock = 3f;
        }
    }

    private void FinishMission()
    {
        S.MisionesCompletadas[S.Mision] = true;
        S.Creditos += 85;
        S.Botiquines++;
        S.Vida = S.MaxVida;
        S.Energia = S.MaxEnergia;
        SoundFX.PlayLevelUp();

        if (S.Mision == 3)
        {
            Complete = true;
            Message = "¡EOS HA DESPERTADO! Has completado los 4 planetas de la expedición.";
            return;
        }

        string clearedName = MissionName;
        S.Mision++;
        StartMission();
        Message = $"¡{clearedName} completado! (+85 CR, +1 Botiquín). Nuevo destino: {MissionName}.";
    }

    // Sistema de Armería y Mejoras (4 mejoras independientes)
    public int GetWeaponCost() => S.ArmaNivel >= 5 ? 0 : S.ArmaNivel * 50;
    public int GetShieldCost() => S.EscudoNivel >= 5 ? 0 : S.EscudoNivel * 45;
    public int GetThrusterCost() => S.PropulsorNivel >= 5 ? 0 : S.PropulsorNivel * 45;
    public int GetDroneCost() => S.DronNivel >= 4 ? 0 : (S.DronNivel + 1) * 65;

    public bool UpgradeWeapon()
    {
        int cost = GetWeaponCost();
        if (cost == 0 || S.Creditos < cost) { Message = cost == 0 ? "Cañón al máximo." : $"Necesitas {cost} CR."; return false; }
        S.Creditos -= cost;
        S.ArmaNivel++;
        SoundFX.PlayLevelUp();
        Message = $"Cañón Prismático mejorado a NIVEL {S.ArmaNivel}.";
        return true;
    }

    public bool UpgradeShield()
    {
        int cost = GetShieldCost();
        if (cost == 0 || S.Creditos < cost) { Message = cost == 0 ? "Blindaje al máximo." : $"Necesitas {cost} CR."; return false; }
        S.Creditos -= cost;
        S.EscudoNivel++;
        S.MaxVida += 20;
        S.Vida = S.MaxVida;
        SoundFX.PlayLevelUp();
        Message = $"Blindaje de Traje mejorado a NIVEL {S.EscudoNivel} (+20 Integridad Máx).";
        return true;
    }

    public bool UpgradeThruster()
    {
        int cost = GetThrusterCost();
        if (cost == 0 || S.Creditos < cost) { Message = cost == 0 ? "Propulsores al máximo." : $"Necesitas {cost} CR."; return false; }
        S.Creditos -= cost;
        S.PropulsorNivel++;
        S.MaxEnergia += 15;
        S.Energia = S.MaxEnergia;
        SoundFX.PlayLevelUp();
        Message = $"Propulsores mejorados a NIVEL {S.PropulsorNivel} (+Velocidad y Dash).";
        return true;
    }

    public bool UpgradeDrone()
    {
        int cost = GetDroneCost();
        if (cost == 0 || S.Creditos < cost) { Message = cost == 0 ? "Dron Orbital al máximo." : $"Necesitas {cost} CR."; return false; }
        S.Creditos -= cost;
        S.DronNivel++;
        SoundFX.PlayLevelUp();
        Message = S.DronNivel == 1 ? "¡Dron Orbital desplegado!" : $"Dron Orbital mejorado a NIVEL {S.DronNivel}.";
        return true;
    }

    public bool BuyMedkit()
    {
        if (S.Creditos < 25) { Message = "Necesitas 25 CR para un botiquín."; return false; }
        S.Creditos -= 25;
        S.Botiquines++;
        SoundFX.PlayPickup();
        Message = $"Botiquín comprado (Total: {S.Botiquines}).";
        return true;
    }

    private void SpawnSparks(float x, float y, Color c, int count, float speed)
    {
        for (int i = 0; i < count; i++)
        {
            float ang = (float)(rng.NextDouble() * Math.PI * 2);
            float sp = speed * (0.35f + (float)rng.NextDouble() * 0.75f);
            float life = 0.25f + (float)rng.NextDouble() * 0.35f;
            Particles.Add(new Particle
            {
                X = x,
                Y = y,
                VX = MathF.Cos(ang) * sp,
                VY = MathF.Sin(ang) * sp,
                Life = life,
                MaxLife = life,
                Size = 3f + (float)rng.NextDouble() * 4f,
                Color = c
            });
        }
    }

    private void AddFloatingText(float x, float y, string text, Color color)
    {
        Texts.Add(new FloatingText { X = x, Y = y, VY = -42f, Life = 0.95f, Text = text, Color = color });
    }

    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(S, new JsonSerializerOptions { WriteIndented = true }));

    public bool Load(string path)
    {
        if (!File.Exists(path)) return false;
        var v = JsonSerializer.Deserialize<GameState>(File.ReadAllText(path));
        if (v == null) return false;
        LoadFromState(v);
        return true;
    }

    public void LoadFromState(GameState state)
    {
        S = Sanitize(state);
        StartMission();
    }

    /// <summary>Valida una partida cargada (JSON o SQL) para que datos viejos o corruptos no rompan el juego.</summary>
    private static GameState Sanitize(GameState s)
    {
        s.Nombre = string.IsNullOrWhiteSpace(s.Nombre) ? "Nova" : s.Nombre.Trim();
        if (s.Nombre.Length > 40) s.Nombre = s.Nombre[..40];

        var done = new bool[4];
        if (s.MisionesCompletadas != null)
            Array.Copy(s.MisionesCompletadas, done, Math.Min(done.Length, s.MisionesCompletadas.Length));
        s.MisionesCompletadas = done;

        s.Inventario ??= new();
        s.Mision = Math.Clamp(s.Mision, 0, 3);
        s.MaxVida = Math.Max(100, s.MaxVida);
        s.MaxEnergia = Math.Max(100, s.MaxEnergia);
        // Una partida guardada justo al morir (Vida 0) se restaura con la integridad completa
        s.Vida = s.Vida <= 0 ? s.MaxVida : Math.Min(s.Vida, s.MaxVida);
        s.Energia = Math.Clamp(s.Energia, 0, s.MaxEnergia);
        s.Nivel = Math.Max(1, s.Nivel);
        s.XP = Math.Max(0, s.XP);
        s.Creditos = Math.Max(0, s.Creditos);
        s.Botiquines = Math.Max(0, s.Botiquines);
        s.ArmaNivel = Math.Clamp(s.ArmaNivel, 1, 5);
        s.EscudoNivel = Math.Clamp(s.EscudoNivel, 1, 5);
        s.PropulsorNivel = Math.Clamp(s.PropulsorNivel, 1, 5);
        s.DronNivel = Math.Clamp(s.DronNivel, 0, 4);
        return s;
    }

    public static float Distance(float x, float y, float a, float b) => MathF.Sqrt((x - a) * (x - a) + (y - b) * (y - b));
}
