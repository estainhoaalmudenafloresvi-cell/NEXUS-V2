namespace WinFormsApp1
{
    partial class FormPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitulo = new Label();
            btnJugar = new Button();
            btnExploradores = new Button();
            btnMisiones = new Button();
            btnSalir = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Impact", 25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Plum;
            lblTitulo.Location = new Point(293, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(166, 63);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "NEXUS ";
            lblTitulo.Click += lblTitulo_Click;
            // 
            // btnJugar
            // 
            btnJugar.BackColor = Color.LightSeaGreen;
            btnJugar.Location = new Point(163, 118);
            btnJugar.Name = "btnJugar";
            btnJugar.Size = new Size(414, 62);
            btnJugar.TabIndex = 1;
            btnJugar.Text = "INICIAR PARTIDA ";
            btnJugar.UseVisualStyleBackColor = false;
            btnJugar.Click += btnJugar_Click;
            // 
            // btnExploradores
            // 
            btnExploradores.BackColor = Color.CadetBlue;
            btnExploradores.Location = new Point(163, 202);
            btnExploradores.Name = "btnExploradores";
            btnExploradores.Size = new Size(414, 62);
            btnExploradores.TabIndex = 2;
            btnExploradores.Text = "EXPLORADORES ";
            btnExploradores.UseVisualStyleBackColor = false;
            // 
            // btnMisiones
            // 
            btnMisiones.BackColor = Color.MediumPurple;
            btnMisiones.Location = new Point(163, 270);
            btnMisiones.Name = "btnMisiones";
            btnMisiones.Size = new Size(414, 62);
            btnMisiones.TabIndex = 3;
            btnMisiones.Text = "MISIONES ";
            btnMisiones.UseVisualStyleBackColor = false;
            // 
            // btnSalir
            // 
            btnSalir.BackColor = Color.DarkMagenta;
            btnSalir.Location = new Point(163, 347);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(414, 62);
            btnSalir.TabIndex = 4;
            btnSalir.Text = "SALIR";
            btnSalir.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 5;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Javanese Text", 8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.LightCoral;
            label2.Location = new Point(242, 63);
            label2.Name = "label2";
            label2.Size = new Size(247, 36);
            label2.TabIndex = 6;
            label2.Text = "MENU PRINCIPAL CADETE";
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            BackgroundImage = Properties.Resources.planetitas_kawai;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(778, 444);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnSalir);
            Controls.Add(btnMisiones);
            Controls.Add(btnExploradores);
            Controls.Add(btnJugar);
            Controls.Add(lblTitulo);
            Name = "FormPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NEXUS-Cadete Explorer";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Button btnJugar;
        private Button btnExploradores;
        private Button btnMisiones;
        private Button btnSalir;
        private Label label1;
        private Label label2;
    }
}

