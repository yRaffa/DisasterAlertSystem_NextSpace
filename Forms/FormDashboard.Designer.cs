namespace DisasterAlert.Forms
{
    partial class FormDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Form
            this.Text = "🛰️ Disaster Alert System — Monitoramento via Satélite";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.ForeColor = Color.White;
            this.MinimumSize = new Size(1100, 650);

            // Header Panel
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(20, 0, 20, 0)
            };

            lblTitulo = new Label
            {
                Text = "🛰️  DISASTER ALERT SYSTEM",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                AutoSize = true,
                Location = new Point(20, 18)
            };

            lblSubtitulo = new Label
            {
                Text = "Monitoramento de Desastres Naturais via Dados Orbitais (GOES-16 / INPE Simulado)",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(22, 48)
            };

            lblStatus = new Label
            {
                Text = "● Online",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(74, 222, 128),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            pnlHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, lblStatus });

            // Tab Control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Padding = new Point(20, 8)
            };

            // Tab 1: Dashboard
            tabDashboard = new TabPage
            {
                Text = "  📊 Dashboard  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White
            };

            // Tab 2: Cidades
            tabCidades = new TabPage
            {
                Text = "  🏙️ Cidades  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White
            };

            // Tab 3: Monitoramentos
            tabMonitoramentos = new TabPage
            {
                Text = "  🌡️ Monitoramentos  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White
            };

            // Tab 4: Alertas
            tabAlertas = new TabPage
            {
                Text = "  🚨 Alertas  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White
            };

            // Tab 5: Relatório
            tabRelatorio = new TabPage
            {
                Text = "  📋 Relatório  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White
            };

            tabControl.TabPages.AddRange(new TabPage[]
            {
                tabDashboard, tabCidades, tabMonitoramentos, tabAlertas, tabRelatorio
            });

            // === DASHBOARD TAB ===
            BuildDashboardTab();
            BuildCidadesTab();
            BuildMonitoramentosTab();
            BuildAlertasTab();
            BuildRelatorioTab();

            // Status bar
            statusBar = new StatusStrip
            {
                BackColor = Color.FromArgb(30, 41, 59)
            };
            lblStatusBar = new ToolStripStatusLabel
            {
                Text = "Sistema inicializado. Pronto para monitoramento.",
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9)
            };
            lblUltimaAtualizacao = new ToolStripStatusLabel
            {
                Text = "",
                ForeColor = Color.FromArgb(148, 163, 184),
                Alignment = ToolStripItemAlignment.Right
            };
            statusBar.Items.AddRange(new ToolStripItem[] { lblStatusBar, lblUltimaAtualizacao });

            this.Controls.Add(tabControl);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(statusBar);

            StyleTabControl();
        }

        private void BuildDashboardTab()
        {
            // Cards panel
            pnlCards = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                Padding = new Padding(15, 15, 15, 0)
            };

            cardCidades = CriarCard("🏙️", "Cidades\nMonitoradas", "0", Color.FromArgb(56, 189, 248));
            cardAlertas = CriarCard("🚨", "Alertas\nAtivos", "0", Color.FromArgb(251, 113, 133));
            cardCriticos = CriarCard("⚠️", "Nível\nCrítico", "0", Color.FromArgb(251, 191, 36));
            cardUltimaLeitura = CriarCard("🛰️", "Última\nLeitura", "--", Color.FromArgb(167, 243, 208));

            var flpCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                WrapContents = false
            };
            flpCards.Controls.AddRange(new Control[] { cardCidades, cardAlertas, cardCriticos, cardUltimaLeitura });
            pnlCards.Controls.Add(flpCards);

            // Grid alertas ativos
            lblAlertasAtivos = new Label
            {
                Text = "🚨  ALERTAS ATIVOS",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(251, 113, 133),
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(15, 8, 0, 0)
            };

            dgvDashboard = CriarDataGridView();
            dgvDashboard.Dock = DockStyle.Fill;
            dgvDashboard.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Cidade", HeaderText = "Cidade", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Nivel", HeaderText = "Nível", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Tipo", HeaderText = "Tipo", Width = 130 },
                new DataGridViewTextBoxColumn { Name = "Risco", HeaderText = "Índice Risco", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 400 },
                new DataGridViewTextBoxColumn { Name = "DataHora", HeaderText = "Data/Hora", Width = 160 }
            );

            // Botão executar monitoramento
            btnExecutarMonitoramento = CriarBotao("🛰️  EXECUTAR CICLO DE MONITORAMENTO (SIMULAÇÃO SATÉLITE)", Color.FromArgb(56, 189, 248));
            btnExecutarMonitoramento.Dock = DockStyle.Bottom;
            btnExecutarMonitoramento.Height = 48;
            btnExecutarMonitoramento.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            tabDashboard.Controls.Add(dgvDashboard);
            tabDashboard.Controls.Add(lblAlertasAtivos);
            tabDashboard.Controls.Add(pnlCards);
            tabDashboard.Controls.Add(btnExecutarMonitoramento);
        }

        private void BuildCidadesTab()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10, 10, 10, 0) };

            btnNovaCidade = CriarBotao("➕  Nova Cidade", Color.FromArgb(74, 222, 128));
            btnNovaCidade.Width = 160;
            btnNovaCidade.Location = new Point(10, 10);

            btnEditarCidade = CriarBotao("✏️  Editar", Color.FromArgb(251, 191, 36));
            btnEditarCidade.Width = 120;
            btnEditarCidade.Location = new Point(180, 10);

            btnExcluirCidade = CriarBotao("🗑️  Excluir", Color.FromArgb(251, 113, 133));
            btnExcluirCidade.Width = 120;
            btnExcluirCidade.Location = new Point(310, 10);

            btnSimularCidade = CriarBotao("🛰️  Simular Monitoramento", Color.FromArgb(167, 139, 250));
            btnSimularCidade.Width = 220;
            btnSimularCidade.Location = new Point(440, 10);

            pnlTop.Controls.AddRange(new Control[] { btnNovaCidade, btnEditarCidade, btnExcluirCidade, btnSimularCidade });

            dgvCidades = CriarDataGridView();
            dgvCidades.Dock = DockStyle.Fill;
            dgvCidades.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Cidade", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Latitude", HeaderText = "Latitude", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Longitude", HeaderText = "Longitude", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Populacao", HeaderText = "População", Width = 130 },
                new DataGridViewTextBoxColumn { Name = "DataCadastro", HeaderText = "Cadastro", Width = 150 }
            );

            tabCidades.Controls.Add(dgvCidades);
            tabCidades.Controls.Add(pnlTop);
        }

        private void BuildMonitoramentosTab()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10, 10, 10, 0) };

            lblFiltroCidade = new Label
            {
                Text = "Filtrar cidade:",
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(10, 18)
            };

            cboCidadesFiltro = new ComboBox
            {
                Width = 220,
                Location = new Point(110, 14),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };

            btnFiltrarMonitoramentos = CriarBotao("🔍 Filtrar", Color.FromArgb(56, 189, 248));
            btnFiltrarMonitoramentos.Width = 100;
            btnFiltrarMonitoramentos.Location = new Point(340, 12);

            btnTodosMonitoramentos = CriarBotao("📋 Todos", Color.FromArgb(148, 163, 184));
            btnTodosMonitoramentos.Width = 100;
            btnTodosMonitoramentos.Location = new Point(450, 12);

            pnlTop.Controls.AddRange(new Control[] { lblFiltroCidade, cboCidadesFiltro, btnFiltrarMonitoramentos, btnTodosMonitoramentos });

            dgvMonitoramentos = CriarDataGridView();
            dgvMonitoramentos.Dock = DockStyle.Fill;
            dgvMonitoramentos.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { Name = "Cidade", HeaderText = "Cidade", Width = 160 },
                new DataGridViewTextBoxColumn { Name = "Chuva", HeaderText = "Chuva (mm)", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "Temp", HeaderText = "Temp (°C)", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Umidade", HeaderText = "Umidade (%)", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "Vento", HeaderText = "Vento (km/h)", Width = 115 },
                new DataGridViewTextBoxColumn { Name = "Risco", HeaderText = "Índice Risco", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "Fonte", HeaderText = "Fonte", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "DataHora", HeaderText = "Data/Hora", Width = 160 }
            );

            tabMonitoramentos.Controls.Add(dgvMonitoramentos);
            tabMonitoramentos.Controls.Add(pnlTop);
        }

        private void BuildAlertasTab()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10, 10, 10, 0) };

            chkSomenteAtivos = new CheckBox
            {
                Text = "Somente alertas ativos",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Checked = true,
                AutoSize = true,
                Location = new Point(10, 17)
            };

            btnEncerrarAlerta = CriarBotao("✅  Encerrar Alerta Selecionado", Color.FromArgb(74, 222, 128));
            btnEncerrarAlerta.Width = 250;
            btnEncerrarAlerta.Location = new Point(230, 12);

            btnAtualizarAlertas = CriarBotao("🔄  Atualizar", Color.FromArgb(56, 189, 248));
            btnAtualizarAlertas.Width = 120;
            btnAtualizarAlertas.Location = new Point(490, 12);

            pnlTop.Controls.AddRange(new Control[] { chkSomenteAtivos, btnEncerrarAlerta, btnAtualizarAlertas });

            dgvAlertas = CriarDataGridView();
            dgvAlertas.Dock = DockStyle.Fill;
            dgvAlertas.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { Name = "Cidade", HeaderText = "Cidade", Width = 160 },
                new DataGridViewTextBoxColumn { Name = "Nivel", HeaderText = "Nível", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Tipo", HeaderText = "Tipo", Width = 130 },
                new DataGridViewTextBoxColumn { Name = "Risco", HeaderText = "Índice Risco", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 380 },
                new DataGridViewTextBoxColumn { Name = "Ativo", HeaderText = "Ativo", Width = 70 },
                new DataGridViewTextBoxColumn { Name = "DataHora", HeaderText = "Data/Hora", Width = 160 }
            );

            tabAlertas.Controls.Add(dgvAlertas);
            tabAlertas.Controls.Add(pnlTop);
        }

        private void BuildRelatorioTab()
        {
            var lblTitulo = new Label
            {
                Text = "📋  RESUMO POR CIDADE — Dados Consolidados de Monitoramento",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(167, 243, 208),
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(15, 10, 0, 0)
            };

            dgvRelatorio = CriarDataGridView();
            dgvRelatorio.Dock = DockStyle.Fill;
            dgvRelatorio.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Cidade", HeaderText = "Cidade", Width = 160 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "UF", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "TotalMon", HeaderText = "Monitoramentos", Width = 130 },
                new DataGridViewTextBoxColumn { Name = "MediaChuva", HeaderText = "Chuva Média (mm)", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "MediaTemp", HeaderText = "Temp. Média (°C)", Width = 145 },
                new DataGridViewTextBoxColumn { Name = "IndiceRisco", HeaderText = "Índice Risco Médio", Width = 155 },
                new DataGridViewTextBoxColumn { Name = "AlertasAtivos", HeaderText = "Alertas Ativos", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "NivelAtual", HeaderText = "Nível Atual", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "UltimaAtt", HeaderText = "Última Atualização", Width = 160 }
            );

            btnAtualizarRelatorio = CriarBotao("🔄  Atualizar Relatório", Color.FromArgb(167, 243, 208));
            btnAtualizarRelatorio.Dock = DockStyle.Bottom;
            btnAtualizarRelatorio.Height = 42;

            tabRelatorio.Controls.Add(dgvRelatorio);
            tabRelatorio.Controls.Add(lblTitulo);
            tabRelatorio.Controls.Add(btnAtualizarRelatorio);
        }

        private Panel CriarCard(string emoji, string titulo, string valor, Color cor)
        {
            var card = new Panel
            {
                Width = 200,
                Height = 100,
                BackColor = Color.FromArgb(30, 41, 59),
                Margin = new Padding(0, 0, 15, 0),
                Cursor = Cursors.Default
            };

            var lblEmoji = new Label
            {
                Text = emoji,
                Font = new Font("Segoe UI Emoji", 20),
                AutoSize = true,
                Location = new Point(14, 12),
                BackColor = Color.Transparent
            };

            var lblTit = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = false,
                Width = 110,
                Height = 36,
                Location = new Point(58, 10),
                BackColor = Color.Transparent
            };

            var lblVal = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = cor,
                AutoSize = true,
                Location = new Point(58, 50),
                BackColor = Color.Transparent,
                Tag = titulo // used to find the label later
            };

            // left border color indicator
            var border = new Panel
            {
                Width = 4,
                Dock = DockStyle.Left,
                BackColor = cor
            };

            card.Controls.AddRange(new Control[] { border, lblEmoji, lblTit, lblVal });
            return card;
        }

        private Button CriarBotao(string texto, Color cor)
        {
            return new Button
            {
                Text = texto,
                BackColor = cor,
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Height = 34,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private DataGridView CriarDataGridView()
        {
            var dgv = new DataGridView
            {
                BackgroundColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(30, 41, 59),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 34 },
                Font = new Font("Segoe UI", 9.5f)
            };

            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(30, 41, 59),
                SelectionForeColor = Color.FromArgb(148, 163, 184),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(226, 232, 240),
                SelectionBackColor = Color.FromArgb(30, 58, 80),
                SelectionForeColor = Color.White,
                Padding = new Padding(6, 0, 0, 0)
            };

            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(20, 30, 48),
                SelectionBackColor = Color.FromArgb(30, 58, 80)
            };

            return dgv;
        }

        private void StyleTabControl()
        {
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += (s, e) =>
            {
                var tab = tabControl.TabPages[e.Index];
                bool selected = tabControl.SelectedIndex == e.Index;
                e.Graphics.FillRectangle(
                    new SolidBrush(selected ? Color.FromArgb(30, 41, 59) : Color.FromArgb(15, 23, 42)),
                    e.Bounds);
                var textColor = selected ? Color.FromArgb(56, 189, 248) : Color.FromArgb(148, 163, 184);
                TextRenderer.DrawText(e.Graphics, tab.Text, tabControl.Font, e.Bounds, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                if (selected)
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(56, 189, 248)),
                        new Rectangle(e.Bounds.Left, e.Bounds.Bottom - 3, e.Bounds.Width, 3));
            };
        }

        // Controls
        private Panel pnlHeader = null!;
        private Label lblTitulo = null!, lblSubtitulo = null!, lblStatus = null!;
        private TabControl tabControl = null!;
        private TabPage tabDashboard = null!, tabCidades = null!, tabMonitoramentos = null!, tabAlertas = null!, tabRelatorio = null!;
        private Panel pnlCards = null!;
        private Panel cardCidades = null!, cardAlertas = null!, cardCriticos = null!, cardUltimaLeitura = null!;
        private Label lblAlertasAtivos = null!;
        private DataGridView dgvDashboard = null!, dgvCidades = null!, dgvMonitoramentos = null!, dgvAlertas = null!, dgvRelatorio = null!;
        private Button btnExecutarMonitoramento = null!, btnNovaCidade = null!, btnEditarCidade = null!, btnExcluirCidade = null!, btnSimularCidade = null!;
        private Button btnFiltrarMonitoramentos = null!, btnTodosMonitoramentos = null!, btnEncerrarAlerta = null!, btnAtualizarAlertas = null!, btnAtualizarRelatorio = null!;
        private ComboBox cboCidadesFiltro = null!;
        private CheckBox chkSomenteAtivos = null!;
        private Label lblFiltroCidade = null!;
        private StatusStrip statusBar = null!;
        private ToolStripStatusLabel lblStatusBar = null!, lblUltimaAtualizacao = null!;
    }
}
