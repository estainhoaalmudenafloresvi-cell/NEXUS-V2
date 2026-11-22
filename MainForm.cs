using System.Drawing.Drawing2D;

namespace NEXUS;

public sealed class MainForm : Form
{
    private readonly GameEngine game = new();
    private readonly SqlSaveRepository sqlRepo = new();
    private readonly string saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NEXUS");
    private string SavePath => Path.Combine(saveDir, "expedicion.json");

    private GameCanvas? canvas;
    private readonly System.Windows.Forms.Timer timer = new() { Interval = 16 };
    private readonly HashSet<Keys> held = new();
    private DateTime last = DateTime.UtcNow;
    private bool inGame;
    private float autosave;
    private float homeAnim;
    private readonly System.Windows.Forms.Timer homeTimer = new() { Interval = 33 };
    private Panel? homeRoot;

    private readonly Color ink = Color.FromArgb(18, 11, 34);
    private readonly Color violet = Color.FromArgb(46, 28, 72);
    private readonly Color lavender = Color.FromArgb(204, 178, 245);
    private readonly Color pink = Color.FromArgb(250, 177, 220);
    private readonly Color cyan = Color.FromArgb(133, 228, 255);
    private readonly Color white = Color.FromArgb(250, 241, 255);

    public MainForm()
    {
        Text = "NEXUS · Expedición Eos (Edición Galáctica)";
        var wa = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1380, 860);
        ClientSize = new Size(Math.Min(1380, wa.Width - 40), Math.Min(860, wa.Height - 80));
        MinimumSize = new Size(Math.Min(1100, wa.Width - 20), Math.Min(720, wa.Height - 40));
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ink;
        ForeColor = white;
        Font = new Font("Segoe UI", 10);
        KeyPreview = true;
        DoubleBuffered = true;

        timer.Tick += Tick;
        homeTimer.Tick += (_, _) => { homeAnim += 0.04f; homeRoot?.Invalidate(); };
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
        Deactivate += (_, _) => held.Clear(); // evita teclas "pegadas" al hacer Alt+Tab o abrir diálogos

        BuildHome();
    }

    private void ClearControls()
    {
        var old = Controls.Cast<Control>().ToList();
        foreach (var c in old) Controls.Remove(c);
        canvas = null;
        homeRoot = null;

        void DisposeOld()
        {
            foreach (var c in old) c.Dispose();
        }

        // Si hay ventana, se libera después (esto puede llamarse desde un evento del propio control);
        // en el constructor todavía no existe el handle y BeginInvoke fallaría.
        if (IsHandleCreated) BeginInvoke(new Action(DisposeOld));
        else DisposeOld();
    }

    private Button MakeButton(string text, Action click, int width = 290, Color? bg = null)
    {
        var baseColor = bg ?? Color.FromArgb(197, 161, 233);
        var b = new Button
        {
            Text = text,
            Width = width,
            Height = 48,
            Margin = new Padding(0, 0, 0, 10),
            FlatStyle = FlatStyle.Flat,
            BackColor = baseColor,
            ForeColor = ink,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 0;
        b.FlatAppearance.MouseOverBackColor = pink;
        b.Click += (_, _) => click();
        return b;
    }

    private void BuildHome()
    {
        timer.Stop();
        inGame = false;
        held.Clear();
        ClearControls();

        var root = new BufferedPanel { Dock = DockStyle.Fill, BackColor = ink };
        homeRoot = root;
        root.Paint += (_, e) => PaintHome(e.Graphics, root.ClientSize);

        var left = new Panel { Dock = DockStyle.Left, Width = 540, BackColor = Color.Transparent };
        var title = new Label
        {
            Text = "N E X U S",
            ForeColor = pink,
            Font = new Font("Segoe UI Light", 46),
            AutoSize = true,
            Location = new Point(56, 72)
        };
        var eyebrow = new Label
        {
            Text = "EXPEDICIÓN EOS  /  ACCIÓN TÁCTICA EN 4 PLANETAS",
            ForeColor = cyan,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(62, 152)
        };
        var desc = new Label
        {
            Text = string.Join(Environment.NewLine,
                "Cuatro mundos hostiles. Cuatro misiones únicas.",
                "1. EOS · Jardín de Cristal (Fragmentos)",
                "2. NÉBULA · Archivo Sumergido (Tormenta Iónica)",
                "3. SYLVARA · Santuario de las Lunas (Altares + Jefe)",
                "4. ASTRALIS · Corazón del Vacío (Coloso Final)"),
            ForeColor = white,
            Font = new Font("Segoe UI", 10.5f),
            AutoSize = true,
            Location = new Point(62, 188)
        };

        var menu = new FlowLayoutPanel
        {
            Location = new Point(62, 345),
            Size = new Size(340, 400),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        menu.Controls.Add(MakeButton("INICIAR NUEVA EXPEDICIÓN", NewGame, 310, pink));
        menu.Controls.Add(MakeButton("CONTINUAR PARTIDA GUARDADA", LoadGame, 310));
        menu.Controls.Add(MakeButton("MAPA ESTELAR (ELEGIR PLANETA)", ShowPlanetSelector, 310, cyan));
        menu.Controls.Add(MakeButton("ARCHIVO Y BASE DE DATOS SQL", ShowArchive, 310));
        menu.Controls.Add(MakeButton("SALIR", Close, 310, Color.FromArgb(140, 115, 175)));

        left.Controls.AddRange(new Control[] { title, eyebrow, desc, menu });
        root.Controls.Add(left);
        Controls.Add(root);
        homeTimer.Start();
    }

    private void PaintHome(Graphics g, Size s)
    {
        if (s.Width <= 0 || s.Height <= 0) return;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var b = new LinearGradientBrush(
            new Rectangle(Point.Empty, s),
            Color.FromArgb(16, 10, 35),
            Color.FromArgb(92, 44, 114),
            LinearGradientMode.ForwardDiagonal);
        g.FillRectangle(b, 0, 0, s.Width, s.Height);

        for (int i = 0; i < 180; i++)
        {
            float x = (i * 173) % Math.Max(1, s.Width);
            float y = (i * 97) % Math.Max(1, s.Height);
            int alpha = 85 + (int)(95 * (0.5 + 0.5 * Math.Sin(homeAnim + i)));
            using var p = new SolidBrush(Color.FromArgb(alpha, 255, 231, 255));
            g.FillEllipse(p, x, y, 1 + i % 3, 1 + i % 3);
        }

        // Órbita decorativa con 4 planetas miniatura
        float cx = s.Width * 0.72f, cy = s.Height * 0.45f;
        using (var halo = new SolidBrush(Color.FromArgb(30, 246, 164, 223)))
            g.FillEllipse(halo, cx - 260, cy - 260, 520, 520);

        using (var orbitPen = new Pen(Color.FromArgb(70, 210, 180, 255), 1.5f) { DashStyle = DashStyle.Dash })
        {
            g.DrawEllipse(orbitPen, cx - 220, cy - 220, 440, 440);
            g.DrawEllipse(orbitPen, cx - 165, cy - 165, 330, 330);
        }

        using (var moon = new LinearGradientBrush(
            new RectangleF(cx - 125, cy - 125, 250, 250),
            Color.FromArgb(248, 226, 250),
            Color.FromArgb(106, 73, 155),
            LinearGradientMode.ForwardDiagonal))
        {
            g.FillEllipse(moon, cx - 125, cy - 125, 250, 250);
        }

        // 4 satélites orbitando
        Color[] satColors = { pink, cyan, Color.Aquamarine, Color.Gold };
        for (int i = 0; i < 4; i++)
        {
            float ang = homeAnim * (0.5f + i * 0.12f) + i * 1.57f;
            float r = i % 2 == 0 ? 220f : 165f;
            float sx = cx + MathF.Cos(ang) * r;
            float sy = cy + MathF.Sin(ang) * r;
            using var sb = new SolidBrush(satColors[i]);
            g.FillEllipse(sb, sx - 11, sy - 11, 22, 22);
        }

        using var mountain = new SolidBrush(Color.FromArgb(185, 26, 16, 48));
        g.FillPolygon(mountain, new[]
        {
            new Point((int)(s.Width * 0.42), s.Height),
            new Point((int)(s.Width * 0.56), s.Height - 260),
            new Point((int)(s.Width * 0.68), s.Height - 150),
            new Point((int)(s.Width * 0.83), s.Height - 330),
            new Point(s.Width, s.Height - 160),
            new Point(s.Width, s.Height)
        });

        using var caption = new Font("Segoe UI", 10, FontStyle.Bold);
        using var captionBrush = new SolidBrush(Color.FromArgb(200, 255, 220, 250));
        g.DrawString("SISTEMA NEXUS  ·  4 SECTORES PLANETARIOS OPERATIVOS", caption,
            captionBrush, s.Width - 460, s.Height - 48);
    }

    private void NewGame()
    {
        using var f = new Form
        {
            Text = "Identidad de exploradora",
            Size = new Size(430, 230),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = violet,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var t = new Label
        {
            Text = "Nombre de la Comandante:",
            ForeColor = white,
            AutoSize = true,
            Location = new Point(24, 24),
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        var input = new TextBox
        {
            Text = "Nova",
            MaxLength = 40,
            Location = new Point(24, 58),
            Width = 360,
            Font = new Font("Segoe UI", 12)
        };
        var go = MakeButton("DESPEGAR HACIA EOS", () =>
        {
            if (string.IsNullOrWhiteSpace(input.Text)) return;
            game.Nueva(input.Text);
            f.DialogResult = DialogResult.OK;
            f.Close();
        }, 230, pink);
        go.Location = new Point(24, 112);
        f.AcceptButton = go;
        f.Controls.AddRange(new Control[] { t, input, go });
        if (f.ShowDialog(this) == DialogResult.OK)
            StartGame();
    }

    private void ShowPlanetSelector()
    {
        using var f = new Form
        {
            Text = "Mapa Estelar · Selección de Planeta",
            Size = new Size(640, 470),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = violet,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var header = new Label
        {
            Text = "SELECCIONA DESTINO PLANETARIO",
            ForeColor = pink,
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 20)
        };
        f.Controls.Add(header);

        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            bool done = game.S.MisionesCompletadas[i];
            var card = new Panel
            {
                Location = new Point(24, 66 + i * 88),
                Size = new Size(575, 76),
                BackColor = Color.FromArgb(32, 19, 54)
            };
            var lblTitle = new Label
            {
                Text = game.Missions[i] + (done ? "  [COMPLETADO ✓]" : ""),
                ForeColor = done ? Color.Aquamarine : cyan,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblLore = new Label
            {
                Text = game.PlanetLore[i],
                ForeColor = white,
                Font = new Font("Segoe UI", 8.5f),
                Size = new Size(430, 36),
                Location = new Point(12, 33)
            };
            var btnGo = MakeButton("VIAJAR", () =>
            {
                game.SelectMission(idx);
                f.DialogResult = DialogResult.OK;
                f.Close();
            }, 105, done ? Color.Aquamarine : pink);
            btnGo.Height = 40;
            btnGo.Location = new Point(455, 18);

            card.Controls.AddRange(new Control[] { lblTitle, lblLore, btnGo });
            f.Controls.Add(card);
        }

        if (f.ShowDialog(this) == DialogResult.OK)
            StartGame();
    }

    private void LoadGame()
    {
        try
        {
            if (!game.Load(SavePath))
            {
                MessageBox.Show("Todavía no hay una expedición guardada localmente.", "NEXUS");
                return;
            }
            StartGame();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo cargar la expedición: " + ex.Message, "NEXUS");
        }
    }

    private void StartGame()
    {
        homeTimer.Stop();
        ClearControls();
        inGame = true;
        held.Clear();

        var cv = new GameCanvas(game) { Dock = DockStyle.Fill };
        cv.MouseMove += (_, e) =>
        {
            cv.SetMousePos(e.X, e.Y);
            cv.UpdateAim(); // el apuntado se calcula desde la posición real del jugador en pantalla
        };
        cv.MouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Left) game.Shoot();
            else if (e.Button == MouseButtons.Right) game.Dash();
        };

        canvas = cv;
        Controls.Add(cv);
        last = DateTime.UtcNow;
        autosave = 0;
        timer.Start();
        cv.Focus();
    }

    private void Tick(object? sender, EventArgs e)
    {
        if (!inGame || canvas == null) return;
        float dt = (float)(DateTime.UtcNow - last).TotalSeconds;
        last = DateTime.UtcNow;

        bool up = held.Contains(Keys.W) || held.Contains(Keys.Up);
        bool down = held.Contains(Keys.S) || held.Contains(Keys.Down);
        bool left = held.Contains(Keys.A) || held.Contains(Keys.Left);
        bool right = held.Contains(Keys.D) || held.Contains(Keys.Right);
        bool firing = held.Contains(Keys.Space) || Control.MouseButtons.HasFlag(MouseButtons.Left);

        canvas.UpdateAim();
        game.Update(dt, up, down, left, right, firing, canvas.HasMouseMoved);

        autosave += dt;
        if (autosave > 12f)
        {
            SaveQuietly();
            autosave = 0f;
        }

        canvas.Invalidate();

        if (game.Dead || game.Complete)
        {
            timer.Stop();
            SaveQuietly();
            string summary = game.Complete
                ? string.Join(Environment.NewLine,
                    game.Message,
                    "",
                    $"Exploradora: {game.S.Nombre}",
                    $"Nivel alcanzado: {game.S.Nivel}",
                    $"Eliminaciones: {game.S.Eliminaciones}",
                    $"Mejor racha: x{game.S.MejorRacha}",
                    "",
                    "¿Volver al menú principal?")
                : string.Join(Environment.NewLine, game.Message, "", "¿Reintentar este planeta?");
            var result = MessageBox.Show(summary, "NEXUS · Estado de Misión", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            held.Clear();
            if (game.Dead && result == DialogResult.OK)
            {
                game.Retry();
                last = DateTime.UtcNow;
                timer.Start();
            }
            else
            {
                BuildHome();
            }
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!inGame) return;
        if (!held.Add(e.KeyCode)) return; // auto-repetición de tecla: la acción ya se ejecutó

        if (e.KeyCode == Keys.Escape)
        {
            game.Paused = !game.Paused;
            if (game.Paused) { timer.Stop(); ShowPause(); }
            else { last = DateTime.UtcNow; timer.Start(); }
        }
        else if (e.KeyCode == Keys.ShiftKey) game.Dash();
        else if (e.KeyCode == Keys.Q) game.Pulse();
        else if (e.KeyCode == Keys.E) game.Interact();
        else if (e.KeyCode == Keys.H) game.UseMedkit();
        else if (e.KeyCode == Keys.U || e.KeyCode == Keys.B)
        {
            game.Paused = true;
            timer.Stop();
            ShowArmory();
        }
        else if (e.KeyCode == Keys.M)
        {
            SoundFX.Muted = !SoundFX.Muted;
        }
        else if (e.KeyCode == Keys.F5)
        {
            game.Notify(SaveQuietly() ? "Partida guardada (F5)." : "No se pudo guardar la partida.");
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e) => held.Remove(e.KeyCode);

    private void ShowArmory()
    {
        using var f = new Form
        {
            Text = "Armería Estelar · Mejoras de Traje",
            Size = new Size(560, 490),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = violet,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var lblCredits = new Label
        {
            Text = $"CRÉDITOS DISPONIBLES: {game.S.Creditos} CR",
            ForeColor = Color.Gold,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 18)
        };
        f.Controls.Add(lblCredits);

        void RefreshLabels() => lblCredits.Text = $"CRÉDITOS DISPONIBLES: {game.S.Creditos} CR";

        void AddUpgradeRow(int row, string title, Func<string> statusFn, Func<int> costFn, Func<bool> buyFn)
        {
            var card = new Panel
            {
                Location = new Point(24, 58 + row * 68),
                Size = new Size(495, 58),
                BackColor = Color.FromArgb(32, 19, 54)
            };
            var lbl = new Label
            {
                Text = $"{title}  ({statusFn()})",
                ForeColor = white,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 18)
            };
            var btn = new Button
            {
                Width = 145,
                Height = 36,
                Location = new Point(336, 11),
                FlatStyle = FlatStyle.Flat,
                BackColor = pink,
                ForeColor = ink,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            void UpdateBtn()
            {
                lbl.Text = $"{title}  ({statusFn()})";
                int c = costFn();
                btn.Text = c == 0 ? "MÁXIMO" : $"MEJORAR · {c} CR";
                btn.Enabled = c > 0;
            }
            UpdateBtn();

            btn.Click += (_, _) =>
            {
                if (buyFn())
                {
                    RefreshLabels();
                    UpdateBtn();
                }
            };

            card.Controls.AddRange(new Control[] { lbl, btn });
            f.Controls.Add(card);
        }

        AddUpgradeRow(0, "Cañón Prismático", () => $"Niv {game.S.ArmaNivel}/5", game.GetWeaponCost, game.UpgradeWeapon);
        AddUpgradeRow(1, "Blindaje de Traje", () => $"Niv {game.S.EscudoNivel}/5", game.GetShieldCost, game.UpgradeShield);
        AddUpgradeRow(2, "Propulsores Dash", () => $"Niv {game.S.PropulsorNivel}/5", game.GetThrusterCost, game.UpgradeThruster);
        AddUpgradeRow(3, "Dron Orbital Autónomo", () => game.S.DronNivel == 0 ? "Inactivo" : $"Niv {game.S.DronNivel}/4", game.GetDroneCost, game.UpgradeDrone);
        AddUpgradeRow(4, "Botiquín Táctico", () => $"Tienes {game.S.Botiquines}", () => 25, game.BuyMedkit);

        var closeBtn = MakeButton("VOLVER AL COMBATE", () => f.Close(), 240, cyan);
        closeBtn.Location = new Point(150, 402);
        f.Controls.Add(closeBtn);

        f.ShowDialog(this);
        held.Clear();
        if (inGame)
        {
            game.Paused = false;
            last = DateTime.UtcNow;
            timer.Start();
            canvas?.Focus();
        }
    }

    private void ShowPause()
    {
        if (!inGame) return;
        using var f = new Form
        {
            Text = "Expedición pausada",
            Size = new Size(410, 450),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = violet,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var title = new Label
        {
            Text = "PAUSA",
            ForeColor = pink,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(28, 18)
        };
        var flow = new FlowLayoutPanel
        {
            Location = new Point(28, 80),
            Size = new Size(340, 320),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        flow.Controls.Add(MakeButton("VOLVER A LA EXPEDICIÓN", () => f.Close(), 325, pink));
        flow.Controls.Add(MakeButton("ARMERÍA Y MEJORAS (U)", () => { f.Tag = "armory"; f.Close(); }, 325, cyan));
        flow.Controls.Add(MakeButton("GUARDAR PARTIDA (JSON)", () => MessageBox.Show(SaveQuietly() ? "Partida guardada localmente." : "No se pudo guardar la partida en disco.", "NEXUS"), 325));
        flow.Controls.Add(MakeButton("GUARDAR EN SQL SERVER", SaveToSql, 325));
        flow.Controls.Add(MakeButton("VOLVER AL MENÚ PRINCIPAL", () => { f.Tag = "home"; f.Close(); }, 325, Color.FromArgb(140, 115, 175)));

        f.Controls.AddRange(new Control[] { title, flow });
        f.ShowDialog(this);
        held.Clear();

        if (Equals(f.Tag, "home"))
        {
            SaveQuietly();
            BuildHome();
        }
        else if (Equals(f.Tag, "armory"))
        {
            ShowArmory();
        }
        else if (inGame)
        {
            game.Paused = false;
            last = DateTime.UtcNow;
            timer.Start();
            canvas?.Focus();
        }
    }

    private bool SaveQuietly()
    {
        try
        {
            Directory.CreateDirectory(saveDir);
            game.Save(SavePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SaveToSql()
    {
        try
        {
            sqlRepo.Guardar(game.S);
            MessageBox.Show("¡Partida guardada correctamente en SQL Server (dbo.Partidas)!", "NEXUS · SQL Server");
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo conectar a SQL Server (ejecuta SQL/CrearBaseNEXUS.sql primero):\n" + ex.Message, "NEXUS · SQL Server");
        }
    }

    private void ShowArchive()
    {
        string completedList = string.Join(", ",
            Enumerable.Range(0, 4).Where(i => game.S.MisionesCompletadas[i]).Select(i => $"Planeta {i + 1}"));
        if (string.IsNullOrEmpty(completedList)) completedList = "Ninguno aún";

        var res = MessageBox.Show(
            $"EXPLORADORA: {game.S.Nombre}\n" +
            $"NIVEL: {game.S.Nivel}  (XP: {game.S.XP}/{game.S.Nivel * 100})\n" +
            $"ELIMINACIONES: {game.S.Eliminaciones}  ·  MEJOR RACHA: x{game.S.MejorRacha}\n" +
            $"FRAGMENTOS REUNIDOS: {game.S.Fragmentos}\n" +
            $"CRÉDITOS: {game.S.Creditos} CR\n" +
            $"EQUIPO: Cañón Niv.{game.S.ArmaNivel} | Blindaje Niv.{game.S.EscudoNivel} | Dron Niv.{game.S.DronNivel}\n" +
            $"PLANETAS COMPLETADOS: {completedList}\n\n" +
            "¿Deseas intentar cargar la última partida desde SQL Server?",
            "Archivo de Exploradora · NEXUS",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

        if (res == DialogResult.Yes)
        {
            try
            {
                var loaded = sqlRepo.CargarUltima();
                if (loaded == null)
                {
                    MessageBox.Show("No hay partidas registradas en SQL Server.", "NEXUS · SQL");
                    return;
                }
                game.LoadFromState(loaded);
                StartGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar SQL Server:\n" + ex.Message, "NEXUS · SQL");
            }
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (inGame) SaveQuietly();
        timer.Stop();
        homeTimer.Stop();
        base.OnFormClosing(e);
    }
}

internal sealed class GameCanvas : Control
{
    private readonly GameEngine game;
    private float time;
    private Image? planetTexture;
    private int textureMission = -1;
    private PointF mousePos;
    public bool HasMouseMoved { get; private set; }

    public GameCanvas(GameEngine engine)
    {
        game = engine;
        DoubleBuffered = true;
        TabStop = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        LoadPlanetTexture();
    }

    public void SetMousePos(float x, float y)
    {
        mousePos = new PointF(x, y);
        HasMouseMoved = true;
    }

    private void LoadPlanetTexture()
    {
        int mission = Math.Clamp(game.S.Mision, 0, 3);
        if (textureMission == mission) return;
        planetTexture?.Dispose();
        planetTexture = null;
        textureMission = mission;
        string file = Path.Combine(AppContext.BaseDirectory, "Assets", "Planets", $"planet-{mission + 1}.png");
        if (!File.Exists(file)) return;
        try
        {
            using var src = Image.FromFile(file);
            var bmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var gg = Graphics.FromImage(bmp))
                gg.DrawImage(src, 0, 0, src.Width, src.Height);
            planetTexture = bmp;
        }
        catch
        {
            planetTexture = null; // textura corrupta: se juega con el fondo de degradado
        }
    }

    // Origen de la cámara en el mundo (el jugador centrado, salvo cerca de los bordes del mapa)
    private PointF CameraOrigin()
    {
        float camX = Math.Clamp(game.PlayerX - Width / 2f, 0f, Math.Max(0f, GameEngine.WorldW - Width));
        float camY = Math.Clamp(game.PlayerY - Height / 2f, 0f, Math.Max(0f, GameEngine.WorldH - Height));
        return new PointF(camX, camY);
    }

    // Apunta desde la posición real del jugador en pantalla hacia el ratón
    public void UpdateAim()
    {
        if (!HasMouseMoved) return;
        var cam = CameraOrigin();
        game.SetAim(mousePos.X - (game.PlayerX - cam.X), mousePos.Y - (game.PlayerY - cam.Y));
    }

    // Sin esto, las flechas pueden ser interceptadas como teclas de navegación y no llegar a KeyDown
    protected override bool IsInputKey(Keys keyData)
    {
        return (keyData & Keys.KeyCode) switch
        {
            Keys.Up or Keys.Down or Keys.Left or Keys.Right => true,
            _ => base.IsInputKey(keyData)
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        time += 0.03f;

        int w = Width, h = Height;
        if (w <= 0 || h <= 0) return;
        float shakeX = game.ScreenShake > 0 ? MathF.Sin(time * 95f) * game.ScreenShake : 0f;
        float shakeY = game.ScreenShake > 0 ? MathF.Cos(time * 83f) * game.ScreenShake : 0f;

        var cam = CameraOrigin();
        float camX = cam.X + shakeX;
        float camY = cam.Y + shakeY;

        g.TranslateTransform(-camX, -camY);
        DrawWorld(g, w, h, camX, camY);
        g.ResetTransform();

        DrawHud(g, w, h);
        DrawRadar(g, w, h);
        if (HasMouseMoved) DrawCrosshair(g);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) planetTexture?.Dispose();
        base.Dispose(disposing);
    }

    private void DrawWorld(Graphics g, int w, int h, float cx, float cy)
    {
        LoadPlanetTexture();

        Color[] topTints =
        {
            Color.FromArgb(28, 20, 58),
            Color.FromArgb(14, 34, 66),
            Color.FromArgb(20, 48, 52),
            Color.FromArgb(38, 14, 48)
        };
        Color[] botTints =
        {
            Color.FromArgb(91, 53, 111),
            Color.FromArgb(35, 82, 125),
            Color.FromArgb(44, 96, 88),
            Color.FromArgb(98, 28, 76)
        };

        int mIdx = Math.Clamp(game.S.Mision, 0, 3);
        using (var bg = new LinearGradientBrush(new RectangleF(cx, cy, w, h), topTints[mIdx], botTints[mIdx], LinearGradientMode.Vertical))
            g.FillRectangle(bg, cx, cy, w, h);

        if (planetTexture != null)
            g.DrawImage(planetTexture, new RectangleF(0, 0, GameEngine.WorldW, GameEngine.WorldH));

        // Partículas ambientales estelares
        for (int i = 0; i < 240; i++)
        {
            float x = (i * 197) % GameEngine.WorldW;
            float y = (i * 113) % GameEngine.WorldH;
            using var b = new SolidBrush(Color.FromArgb(75 + (int)(65 * (0.5 + 0.5 * Math.Sin(time + i))), 255, 224, 255));
            g.FillEllipse(b, x, y, 2.2f, 2.2f);
        }

        // Cuadrícula holográfica sutil
        using (var grid = new Pen(Color.FromArgb(22, 225, 190, 255), 1))
        {
            for (int x = 0; x < GameEngine.WorldW; x += 100) g.DrawLine(grid, x, 0, x, GameEngine.WorldH);
            for (int y = 0; y < GameEngine.WorldH; y += 100) g.DrawLine(grid, 0, y, GameEngine.WorldW, y);
        }

        // Altares Lunares (Planeta 3)
        foreach (var altar in game.Altars)
            DrawLunarAltar(g, altar);

        // Obstáculos cristalinos
        foreach (var r in game.Obstacles)
        {
            using var sh = new SolidBrush(Color.FromArgb(70, 10, 5, 30));
            g.FillEllipse(sh, r.X - 5, r.Y + r.Height - 4, r.Width + 12, 20);
            using var rock = new LinearGradientBrush(r, Color.FromArgb(185, 134, 146, 189), Color.FromArgb(255, 48, 34, 78), LinearGradientMode.ForwardDiagonal);
            g.FillPolygon(rock, new[]
            {
                new PointF(r.Left, r.Bottom),
                new PointF(r.Left + r.Width * 0.12f, r.Top + r.Height * 0.42f),
                new PointF(r.Left + r.Width * 0.52f, r.Top),
                new PointF(r.Right, r.Top + r.Height * 0.3f),
                new PointF(r.Right - r.Width * 0.1f, r.Bottom)
            });
            using var edge = new Pen(Color.FromArgb(110, 241, 202, 255), 2);
            g.DrawLine(edge, r.Left + r.Width * 0.12f, r.Top + r.Height * 0.42f, r.Left + r.Width * 0.52f, r.Top);
        }

        // Pickups (Fragmentos, Estabilizadores, Curación, Energía, Créditos)
        foreach (var p in game.Pickups)
        {
            float bob = MathF.Sin(time * 3.4f + p.Phase) * 6f;
            Color c = p.Kind switch
            {
                "fragment" => Color.FromArgb(255, 255, 195, 246),
                "stabilizer" => Color.DeepSkyBlue,
                "heal" => Color.LightGreen,
                "credits" => Color.Gold,
                _ => Color.Cyan
            };
            float rad = p.Kind == "fragment" ? 34f : 24f;
            using var aura = new SolidBrush(Color.FromArgb(55, c));
            g.FillEllipse(aura, p.X - rad, p.Y - rad + bob, rad * 2, rad * 2);

            float sz = p.Kind == "fragment" ? 17f : 12f;
            using var crystal = new SolidBrush(c);
            g.FillPolygon(crystal, new[]
            {
                new PointF(p.X, p.Y - sz + bob),
                new PointF(p.X + sz * 0.72f, p.Y + bob),
                new PointF(p.X, p.Y + sz + bob),
                new PointF(p.X - sz * 0.72f, p.Y + bob)
            });
        }

        // Portal de Extracción
        DrawPortal(g);

        // Rayos de enlace entre Pilares de Escudo y el Jefe Final (Planeta 4)
        var boss = game.Enemies.FirstOrDefault(e => e.Kind == EnemyKind.VoidBoss);
        if (boss != null)
        {
            foreach (var pylon in game.Enemies.Where(e => e.Kind == EnemyKind.ShieldPylon))
            {
                using var beam = new Pen(Color.FromArgb(165, 0, 235, 255), 4f) { DashStyle = DashStyle.Dash };
                g.DrawLine(beam, pylon.X, pylon.Y, boss.X, boss.Y);
            }
        }

        // Proyectiles
        foreach (var s in game.Shots)
        {
            using var b = new SolidBrush(s.Tint);
            g.FillEllipse(b, s.X - s.Size, s.Y - s.Size, s.Size * 2, s.Size * 2);
            using var glow = new Pen(Color.FromArgb(120, s.Tint), s.Size);
            g.DrawLine(glow, s.X - s.VX * 0.025f, s.Y - s.VY * 0.025f, s.X, s.Y);
        }

        // Enemigos
        foreach (var en in game.Enemies)
            DrawEnemy(g, en);

        // Onda prismática
        if (game.PulseWaveRadius > 0)
        {
            int alpha = Math.Clamp((int)(220 * (1f - game.PulseWaveRadius / 300f)), 0, 220);
            using var wavePen = new Pen(Color.FromArgb(alpha, 250, 150, 255), 6f);
            g.DrawEllipse(wavePen, game.PlayerX - game.PulseWaveRadius, game.PlayerY - game.PulseWaveRadius, game.PulseWaveRadius * 2, game.PulseWaveRadius * 2);
        }

        // Estela de Dash
        foreach (var gh in game.Ghosts)
        {
            using var gb = new SolidBrush(Color.FromArgb((int)(gh.Alpha * 110), 230, 175, 255));
            g.FillEllipse(gb, gh.X - 16, gh.Y - 24, 32, 46);
        }

        // Línea de apuntado láser sutil
        using (var laser = new Pen(Color.FromArgb(55, 165, 245, 255), 1.5f) { DashStyle = DashStyle.Dot })
            g.DrawLine(laser, game.PlayerX, game.PlayerY, game.PlayerX + game.AimX * 150f, game.PlayerY + game.AimY * 150f);

        // Astronauta Nova + Dron Orbital
        DrawAstronaut(g, game.PlayerX, game.PlayerY, time, game.Invulnerable > 0);
        if (game.S.DronNivel > 0)
        {
            float dx = game.PlayerX + MathF.Cos(game.DroneAngle) * 44f;
            float dy = game.PlayerY + MathF.Sin(game.DroneAngle) * 44f;
            using var dGlow = new SolidBrush(Color.FromArgb(70, 130, 255, 210));
            g.FillEllipse(dGlow, dx - 14, dy - 14, 28, 28);
            using var dCore = new SolidBrush(Color.FromArgb(255, 130, 255, 210));
            g.FillEllipse(dCore, dx - 7, dy - 7, 14, 14);
        }

        // Flecha guía holográfica hacia objetivo activo o portal
        DrawWaypointArrow(g);

        // Partículas y números flotantes
        foreach (var pt in game.Particles)
        {
            int a = Math.Clamp((int)(255 * (pt.Life / pt.MaxLife)), 0, 255);
            using var pb = new SolidBrush(Color.FromArgb(a, pt.Color));
            g.FillEllipse(pb, pt.X - pt.Size / 2f, pt.Y - pt.Size / 2f, pt.Size, pt.Size);
        }

        using var ftFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        foreach (var ft in game.Texts)
        {
            int a = Math.Clamp((int)(255 * ft.Life), 0, 255);
            using var fb = new SolidBrush(Color.FromArgb(a, ft.Color));
            g.DrawString(ft.Text, ftFont, fb, ft.X - 24, ft.Y);
        }
    }

    private void DrawLunarAltar(Graphics g, LunarAltar altar)
    {
        Color c = altar.Completed ? Color.Aquamarine : Color.FromArgb(215, 175, 255);
        using var zone = new SolidBrush(Color.FromArgb(altar.Completed ? 45 : 28, c));
        g.FillEllipse(zone, altar.X - altar.Radius, altar.Y - altar.Radius, altar.Radius * 2, altar.Radius * 2);

        using var ring = new Pen(Color.FromArgb(190, c), 3f) { DashStyle = altar.Completed ? DashStyle.Solid : DashStyle.Dash };
        g.DrawEllipse(ring, altar.X - altar.Radius, altar.Y - altar.Radius, altar.Radius * 2, altar.Radius * 2);

        if (altar.Progress > 0 && !altar.Completed)
        {
            using var arcPen = new Pen(Color.Aquamarine, 6f);
            g.DrawArc(arcPen, altar.X - altar.Radius, altar.Y - altar.Radius, altar.Radius * 2, altar.Radius * 2, -90f, 360f * (altar.Progress / 100f));
        }

        using var f = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        using var labelBrush = new SolidBrush(c);
        string txt = altar.Completed ? $"{altar.Label} ✓" : $"{altar.Label} ({(int)altar.Progress}%)";
        var sz = g.MeasureString(txt, f);
        g.DrawString(txt, f, labelBrush, altar.X - sz.Width / 2f, altar.Y - altar.Radius - 22f);
    }

    private void DrawPortal(Graphics g)
    {
        float px = game.PortalX, py = game.PortalY;
        bool ready = game.PortalReady;
        Color c = ready ? Color.FromArgb(140, 255, 225) : Color.FromArgb(165, 130, 210);

        using var portalGlow = new SolidBrush(Color.FromArgb(ready ? 65 : 25, c));
        g.FillEllipse(portalGlow, px - 115, py - 115, 230, 230);

        using var portalPen = new Pen(Color.FromArgb(ready ? 245 : 130, c), ready ? 8f : 4f);
        g.DrawEllipse(portalPen, px - 72, py - 78, 144, 156);

        using var portalCore = new SolidBrush(Color.FromArgb(ready ? 155 : 65, c));
        g.FillEllipse(portalCore, px - 52, py - 58, 104, 116);

        using var f = new Font("Segoe UI", 9, FontStyle.Bold);
        using var labelBrush = new SolidBrush(ready ? Color.Aquamarine : Color.Lavender);
        string label = ready ? "PORTAL ACTIVO [PULSA E]" : "PORTAL SELLADO";
        var sz = g.MeasureString(label, f);
        g.DrawString(label, f, labelBrush, px - sz.Width / 2f, py - 102);
    }

    private void DrawWaypointArrow(Graphics g)
    {
        float tx = game.PortalX, ty = game.PortalY;
        Color arrowColor = Color.Aquamarine;

        if (!game.PortalReady)
        {
            var frag = game.Pickups.FirstOrDefault(p => p.Kind == "fragment");
            var altar = game.Altars.FirstOrDefault(a => !a.Completed);
            var targetEnemy = game.Enemies.FirstOrDefault(e => e.Kind == EnemyKind.ShieldPylon)
                              ?? game.Enemies.FirstOrDefault(e => e.Elite);

            if (frag != null) { tx = frag.X; ty = frag.Y; arrowColor = Color.FromArgb(255, 195, 246); }
            else if (altar != null) { tx = altar.X; ty = altar.Y; arrowColor = Color.Cyan; }
            else if (targetEnemy != null) { tx = targetEnemy.X; ty = targetEnemy.Y; arrowColor = Color.Gold; }
        }

        float dx = tx - game.PlayerX, dy = ty - game.PlayerY;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        if (dist < 140f) return;

        float ang = MathF.Atan2(dy, dx);
        float ax = game.PlayerX + MathF.Cos(ang) * 58f;
        float ay = game.PlayerY + MathF.Sin(ang) * 58f;

        PointF tip = new(ax + MathF.Cos(ang) * 14f, ay + MathF.Sin(ang) * 14f);
        PointF left = new(ax + MathF.Cos(ang + 2.4f) * 10f, ay + MathF.Sin(ang + 2.4f) * 10f);
        PointF right = new(ax + MathF.Cos(ang - 2.4f) * 10f, ay + MathF.Sin(ang - 2.4f) * 10f);

        using var b = new SolidBrush(Color.FromArgb(215, arrowColor));
        g.FillPolygon(b, new[] { tip, left, right });
    }

    private void DrawEnemy(Graphics g, Enemy e)
    {
        using var aura = new SolidBrush(Color.FromArgb(40, e.Elite ? 255 : 220, 105, 185));
        g.FillEllipse(aura, e.X - e.Radius * 1.65f, e.Y - e.Radius * 1.65f, e.Radius * 3.3f, e.Radius * 3.3f);

        Color c1 = e.Kind switch
        {
            EnemyKind.VoidBoss => Color.FromArgb(255, 255, 120, 200),
            EnemyKind.LunarStalker => Color.FromArgb(255, 140, 255, 215),
            EnemyKind.ShieldPylon => Color.FromArgb(255, 90, 225, 255),
            EnemyKind.TorpedoDrone => Color.FromArgb(255, 255, 175, 110),
            _ => Color.FromArgb(255, 172, 143, 213)
        };

        using var b = new LinearGradientBrush(
            new RectangleF(e.X - e.Radius, e.Y - e.Radius, e.Radius * 2, e.Radius * 2),
            c1,
            Color.FromArgb(255, 42, 24, 74),
            LinearGradientMode.ForwardDiagonal);
        g.FillEllipse(b, e.X - e.Radius, e.Y - e.Radius, e.Radius * 2, e.Radius * 2);

        if (e.InvulnerableShield)
        {
            using var shPen = new Pen(Color.Cyan, 3.5f);
            g.DrawEllipse(shPen, e.X - e.Radius - 7, e.Y - e.Radius - 7, (e.Radius + 7) * 2, (e.Radius + 7) * 2);
        }

        using var eye = new SolidBrush(Color.FromArgb(255, 255, 228, 248));
        g.FillEllipse(eye, e.X - e.Radius * 0.38f, e.Y - e.Radius * 0.18f, e.Radius * 0.76f, e.Radius * 0.3f);

        // Barra de vida
        float bw = Math.Max(42f, e.Radius * 2.1f);
        using var back = new SolidBrush(Color.FromArgb(190, 16, 10, 30));
        g.FillRectangle(back, e.X - bw / 2f, e.Y - e.Radius - 14, bw, 6);
        using var hp = new SolidBrush(e.InvulnerableShield ? Color.Cyan : Color.FromArgb(255, 255, 125, 185));
        g.FillRectangle(hp, e.X - bw / 2f, e.Y - e.Radius - 14, bw * Math.Clamp(e.HP / e.MaxHP, 0f, 1f), 6);

        if (e.Elite || e.Kind == EnemyKind.ShieldPylon)
        {
            using var nf = new Font("Segoe UI", 8f, FontStyle.Bold);
            var sz = g.MeasureString(e.Name, nf);
            g.DrawString(e.Name, nf, Brushes.White, e.X - sz.Width / 2f, e.Y - e.Radius - 30);
        }
    }

    private void DrawAstronaut(Graphics g, float x, float y, float phase, bool hit)
    {
        float step = MathF.Sin(phase * 8f) * 3f;
        using (var aura = new SolidBrush(Color.FromArgb(50, 246, 168, 225)))
            g.FillEllipse(aura, x - 34, y - 34, 68, 72);
        using (var shadow = new SolidBrush(Color.FromArgb(100, 8, 5, 24)))
            g.FillEllipse(shadow, x - 20, y + 19, 40, 13);
        using (var pack = new SolidBrush(Color.FromArgb(255, 160, 123, 193)))
            g.FillRoundedRectangle(pack, x - 22, y - 9, 12, 29, 5);
        using (var limbs = new Pen(Color.FromArgb(255, 226, 215, 244), 7) { StartCap = LineCap.Round, EndCap = LineCap.Round })
        {
            g.DrawLine(limbs, x - 9, y + 8, x - 12, y + 25 + step);
            g.DrawLine(limbs, x + 9, y + 8, x + 13, y + 25 - step);
            g.DrawLine(limbs, x - 10, y - 3, x - 20, y + 9);
            g.DrawLine(limbs, x + 10, y - 3, x + 20, y + 9);
        }
        using (var suit = new SolidBrush(hit ? Color.FromArgb(255, 255, 150, 185) : Color.FromArgb(255, 245, 235, 250)))
            g.FillRoundedRectangle(suit, x - 13, y - 13, 26, 34, 9);
        using (var trim = new Pen(Color.FromArgb(255, 186, 151, 221), 2))
            g.DrawRoundedRectangle(trim, x - 13, y - 13, 26, 34, 9);
        using (var helmet = new SolidBrush(Color.FromArgb(255, 247, 238, 250)))
            g.FillEllipse(helmet, x - 16, y - 39, 32, 32);
        using (var rim = new Pen(Color.FromArgb(255, 185, 151, 221), 2))
            g.DrawEllipse(rim, x - 16, y - 39, 32, 32);
        using (var visor = new LinearGradientBrush(new RectangleF(x - 12, y - 35, 24, 24), Color.FromArgb(255, 74, 64, 113), Color.FromArgb(255, 224, 149, 205), LinearGradientMode.ForwardDiagonal))
            g.FillEllipse(visor, x - 12, y - 35, 24, 24);
    }

    private void DrawHud(Graphics g, int w, int h)
    {
        using var top = new LinearGradientBrush(new Rectangle(0, 0, w, 116), Color.FromArgb(240, 20, 12, 38), Color.FromArgb(180, 42, 26, 68), LinearGradientMode.Vertical);
        g.FillRectangle(top, 0, 0, w, 116);

        using var title = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        using var big = new Font("Segoe UI", 13.5f, FontStyle.Bold);
        using var small = new Font("Segoe UI", 9f, FontStyle.Bold);

        DrawText(g, $"N E X U S   ·   COMANDANTE {game.S.Nombre.ToUpperInvariant()}", title, Color.FromArgb(255, 250, 177, 220), 22, 14);
        g.DrawString(game.MissionName, big, Brushes.White, 22, 36);
        DrawText(g, game.Objective, small, game.PortalReady ? Color.Aquamarine : Color.FromArgb(255, 218, 194, 245), 22, 68);

        int barX = Math.Max(430, w - 640);
        DrawBar(g, barX, 24, 190, 13, (float)game.S.Vida / game.S.MaxVida, Color.FromArgb(255, 255, 125, 180), $"INTEGRIDAD {game.S.Vida}/{game.S.MaxVida}");
        DrawBar(g, barX, 56, 190, 10, (float)game.S.Energia / game.S.MaxEnergia, Color.FromArgb(255, 130, 228, 255), $"ENERGÍA {game.S.Energia}/{game.S.MaxEnergia}");
        DrawBar(g, barX, 84, 190, 7, (float)game.S.XP / (game.S.Nivel * 100), Color.Gold, $"NIVEL {game.S.Nivel}  ·  XP {game.S.XP}/{game.S.Nivel * 100}");

        int statX = barX + 215;
        DrawText(g, $"CRÉDITOS: {game.S.Creditos} CR", small, Color.Gold, statX, 18);
        DrawText(g, $"BOTIQUINES: {game.S.Botiquines} [H]", small, Color.LightGreen, statX, 40);
        g.DrawString($"CAÑÓN NIV.{game.S.ArmaNivel}  ·  DRON NIV.{game.S.DronNivel}", small, Brushes.White, statX, 62);
        if (game.Combo > 1)
            DrawText(g, $"RACHA DE COMBATE x{game.Combo}!", small, Color.FromArgb(255, 255, 180, 240), statX, 84);

        // Barra inferior de controles
        using var bottom = new SolidBrush(Color.FromArgb(200, 20, 12, 38));
        g.FillRoundedRectangle(bottom, 18, h - 50, Math.Min(w - 36, 930), 34, 10);
        using var controlsFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        g.DrawString("WASD MOVER  ·  RATÓN APUNTAR  ·  CLIC/ESPACIO DISPARAR  ·  SHIFT IMPULSO  ·  Q ONDA  ·  E PORTAL  ·  H BOTIQUÍN  ·  U ARMERÍA  ·  M AUDIO  ·  ESC PAUSA",
            controlsFont, Brushes.White, 30, h - 40);

        // Mensaje táctico flotante
        if (!string.IsNullOrEmpty(game.Message))
        {
            using var msgbg = new SolidBrush(Color.FromArgb(185, 22, 12, 42));
            int bw = Math.Min(w - 40, 680);
            g.FillRoundedRectangle(msgbg, w / 2 - bw / 2, h - 96, bw, 32, 10);
            using var mf = new Font("Segoe UI", 9f, FontStyle.Bold);
            var size = g.MeasureString(game.Message, mf);
            DrawText(g, game.Message, mf, Color.FromArgb(255, 248, 218, 250), w / 2f - size.Width / 2f, h - 88);
        }
    }

    private void DrawRadar(Graphics g, int w, int h)
    {
        float rw = 165f, rh = 108f;
        float rx = w - rw - 18f, ry = 128f;

        using var bg = new SolidBrush(Color.FromArgb(185, 16, 10, 32));
        g.FillRoundedRectangle(bg, rx, ry, rw, rh, 10);
        using var border = new Pen(Color.FromArgb(140, 204, 178, 245), 1.5f);
        g.DrawRoundedRectangle(border, rx, ry, rw, rh, 10);

        float sx = rw / GameEngine.WorldW, sy = rh / GameEngine.WorldH;

        // Portal en radar
        using (var pb = new SolidBrush(game.PortalReady ? Color.Aquamarine : Color.MediumPurple))
            g.FillEllipse(pb, rx + game.PortalX * sx - 4, ry + game.PortalY * sy - 4, 8, 8);

        // Altares en radar
        foreach (var a in game.Altars)
        {
            using var ab = new SolidBrush(a.Completed ? Color.Aquamarine : Color.Cyan);
            g.FillEllipse(ab, rx + a.X * sx - 3.5f, ry + a.Y * sy - 3.5f, 7, 7);
        }

        // Fragmentos en radar
        foreach (var p in game.Pickups.Where(p => p.Kind == "fragment"))
        {
            using var fb = new SolidBrush(Color.FromArgb(255, 255, 180, 245));
            g.FillEllipse(fb, rx + p.X * sx - 3.5f, ry + p.Y * sy - 3.5f, 7, 7);
        }

        // Enemigos en radar
        foreach (var en in game.Enemies)
        {
            using var eb = new SolidBrush(en.Elite ? Color.Gold : Color.FromArgb(255, 255, 95, 145));
            float sz = en.Elite ? 6f : 4f;
            g.FillEllipse(eb, rx + en.X * sx - sz / 2f, ry + en.Y * sy - sz / 2f, sz, sz);
        }

        // Jugador en radar
        using (var jb = new SolidBrush(Color.White))
            g.FillEllipse(jb, rx + game.PlayerX * sx - 3.5f, ry + game.PlayerY * sy - 3.5f, 7, 7);
    }

    private static void DrawText(Graphics g, string text, Font font, Color color, float x, float y)
    {
        using var brush = new SolidBrush(color);
        g.DrawString(text, font, brush, x, y);
    }

    private void DrawCrosshair(Graphics g)
    {
        using var pen = new Pen(Color.FromArgb(210, 165, 245, 255), 1.8f);
        g.DrawEllipse(pen, mousePos.X - 11, mousePos.Y - 11, 22, 22);
        g.DrawLine(pen, mousePos.X - 16, mousePos.Y, mousePos.X - 6, mousePos.Y);
        g.DrawLine(pen, mousePos.X + 6, mousePos.Y, mousePos.X + 16, mousePos.Y);
        g.DrawLine(pen, mousePos.X, mousePos.Y - 16, mousePos.X, mousePos.Y - 6);
        g.DrawLine(pen, mousePos.X, mousePos.Y + 6, mousePos.X, mousePos.Y + 16);
    }

    private static void DrawBar(Graphics g, float x, float y, float w, float h, float value, Color color, string label)
    {
        using var f = new Font("Segoe UI", 8f, FontStyle.Bold);
        g.DrawString(label, f, Brushes.White, x, y - 14);
        using var bg = new SolidBrush(Color.FromArgb(145, 10, 7, 24));
        g.FillRoundedRectangle(bg, x, y, w, h, 5);
        using var fg = new SolidBrush(color);
        g.FillRoundedRectangle(fg, x, y, w * Math.Clamp(value, 0f, 1f), h, 5);
    }
}

internal sealed class BufferedPanel : Panel
{
    public BufferedPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}

internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, float x, float y, float w, float h, float radius)
    {
        if (w < 1f || h < 1f) return; // AddArc lanza ArgumentException con ancho/alto 0
        using var p = Rounded(new RectangleF(x, y, w, h), radius);
        g.FillPath(brush, p);
    }

    public static void DrawRoundedRectangle(this Graphics g, Pen pen, float x, float y, float w, float h, float radius)
    {
        if (w < 1f || h < 1f) return;
        using var p = Rounded(new RectangleF(x, y, w, h), radius);
        g.DrawPath(pen, p);
    }

    private static GraphicsPath Rounded(RectangleF r, float radius)
    {
        var p = new GraphicsPath();
        float d = Math.Max(1f, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}
