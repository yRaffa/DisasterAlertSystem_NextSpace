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
            this.Text = "Disaster Alert System";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 252);
            this.ForeColor = Color.FromArgb(30, 41, 59);
            this.Font = new Font("Segoe UI", 9.5f);

            BuildStatusBar();
            BuildLayout();
        }

        private void BuildLayout()
        {
            // Main split: sidebar (fixed 220px) + content
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            BuildSidebar(mainTable);
            BuildMainArea(mainTable);

            this.Controls.Add(mainTable);
        }

        // ═══════════════════════════════════════════════════════════════
        // SIDEBAR — fixed 220px, never overlaps
        // ═══════════════════════════════════════════════════════════════
        private void BuildSidebar(TableLayoutPanel parent)
        {
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 40, 90),
                Padding = new Padding(0)
            };

            // Logo
            pnlLogo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.FromArgb(10, 28, 68)
            };

            // Logo: TableLayoutPanel prevents icon from overlapping text
            var logoTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(12, 8, 8, 8)
            };
            logoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
            logoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            logoTable.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            logoTable.RowStyles.Add(new RowStyle(SizeType.Percent, 45));

            lblLogoIcon = new Label
            {
                Text = "🛰",
                Font = new Font("Segoe UI Emoji", 18),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoTable.SetRowSpan(lblLogoIcon, 2);

            lblLogoText = new Label
            {
                Text = "DisasterAlert",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.BottomLeft
            };
            lblLogoSub = new Label
            {
                Text = "Monitoramento Orbital",
                Font = new Font("Segoe UI", 7f),
                ForeColor = Color.FromArgb(148, 180, 230),
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopLeft
            };

            logoTable.Controls.Add(lblLogoIcon, 0, 0);
            logoTable.Controls.Add(lblLogoText, 1, 0);
            logoTable.Controls.Add(lblLogoSub,  1, 1);
            pnlLogo.Controls.Add(logoTable);

            // Nav
            pnlNav = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 12, 10, 0)
            };

            var navDefs = new[]
            {
                ("📊", "Dashboard",       "Visão geral e alertas"),
                ("🏙️", "Cidades",         "Gerenciar cidades"),
                ("🌡️", "Monitoramentos",  "Histórico climático"),
                ("🚨", "Alertas",         "Alertas ativos"),
                ("📋", "Relatório",       "Resumo consolidado"),
            };

            _navButtons = new Button[navDefs.Length];
            int y = 0;
            for (int i = 0; i < navDefs.Length; i++)
            {
                var (icon, title, sub) = navDefs[i];
                int idx = i;

                var btn = new Button
                {
                    Width = 200, Height = 54,
                    Location = new Point(0, y),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = i == 0 ? Color.FromArgb(30, 80, 160) : Color.Transparent,
                    Cursor = Cursors.Hand,
                    Text = ""
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 65, 140);

                var lIcon  = new Label { Text = icon,  Font = new Font("Segoe UI Emoji", 13), Dock = DockStyle.Fill, BackColor = Color.Transparent, ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter };
                var lTitle = new Label { Text = title, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, BackColor = Color.Transparent, TextAlign = ContentAlignment.BottomLeft };
                var lSub   = new Label { Text = sub,   Font = new Font("Segoe UI", 7.5f), ForeColor = Color.FromArgb(160, 195, 240), Dock = DockStyle.Fill, BackColor = Color.Transparent, TextAlign = ContentAlignment.TopLeft };

                foreach (var lbl in new[] { lIcon, lTitle, lSub })
                    lbl.Click += (s, e) => btn.PerformClick();

                var navRow = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2, RowCount = 2,
                    BackColor = Color.Transparent,
                    Padding = new Padding(6, 4, 4, 4)
                };
                navRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
                navRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                navRow.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
                navRow.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
                navRow.SetRowSpan(lIcon, 2);
                navRow.Controls.Add(lIcon,  0, 0);
                navRow.Controls.Add(lTitle, 1, 0);
                navRow.Controls.Add(lSub,   1, 1);
                btn.Controls.Add(navRow);
                pnlNav.Controls.Add(btn);
                _navButtons[i] = btn;
                y += 58;
            }

            // Execute button
            btnExecutarMonitoramento = new Button
            {
                Width = 200, Height = 42,
                Location = new Point(0, y + 16),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Text = "  Executar Monitoramento",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(28, 0, 0, 0)
            };
            btnExecutarMonitoramento.FlatAppearance.BorderSize = 0;
            btnExecutarMonitoramento.FlatAppearance.MouseOverBackColor = Color.FromArgb(4, 120, 85);
            var lExecIco = new Label { Text = "🛰️", Font = new Font("Segoe UI Emoji", 12), AutoSize = true, BackColor = Color.Transparent, ForeColor = Color.White, Location = new Point(8, 10) };
            lExecIco.Click += (_, __) => btnExecutarMonitoramento.PerformClick();
            btnExecutarMonitoramento.Controls.Add(lExecIco);
            pnlNav.Controls.Add(btnExecutarMonitoramento);

            // Version
            lblVersion = new Label
            {
                Text = "v1.0  •  FIAP 2026",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(80, 120, 180),
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            pnlSidebar.Controls.Add(pnlNav);
            pnlSidebar.Controls.Add(pnlLogo);
            pnlSidebar.Controls.Add(lblVersion);

            parent.Controls.Add(pnlSidebar, 0, 0);
        }

        // ═══════════════════════════════════════════════════════════════
        // MAIN AREA
        // ═══════════════════════════════════════════════════════════════
        private void BuildMainArea(TableLayoutPanel parent)
        {
            pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 252) };

            // Top bar
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            pnlTopBar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);

            lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 40, 90),
                AutoSize = true,
                Location = new Point(24, 12),
                BackColor = Color.Transparent
            };
            lblPageSub = new Label
            {
                Text = "Visão geral do sistema de monitoramento",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(26, 42),
                BackColor = Color.Transparent
            };
            lblOnlineBadge = new Label
            {
                Text = "● Sistema Online",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74),
                AutoSize = true,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlTopBar.Controls.AddRange(new Control[] { lblPageTitle, lblPageSub, lblOnlineBadge });
            pnlTopBar.Resize += (_, __) =>
                lblOnlineBadge.Location = new Point(pnlTopBar.Width - lblOnlineBadge.Width - 20, 22);

            // Content host
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 252), Padding = new Padding(24, 20, 24, 20) };

            BuildDashboardPage();
            BuildCidadesPage();
            BuildMonitoramentosPage();
            BuildAlertasPage();
            BuildRelatorioPage();

            pnlMain.Controls.Add(pnlContent);
            pnlMain.Controls.Add(pnlTopBar);
            parent.Controls.Add(pnlMain, 1, 0);
        }

        // ═══════════════════════════════════════════════════════════════
        // DASHBOARD PAGE
        // ═══════════════════════════════════════════════════════════════
        private void BuildDashboardPage()
        {
            pageDashboard = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = true };

            // Cards
            var cardDefs = new[]
            {
                ("🏙️", "Cidades Monitoradas",  "0",     Color.FromArgb(37,  99, 235)),
                ("🚨", "Alertas Ativos",       "0",     Color.FromArgb(220, 38,  38)),
                ("⚠️", "Alertas Críticos",     "0",     Color.FromArgb(217,119,   6)),
                ("🛰️", "Última Leitura",       "--:--", Color.FromArgb(5,  150, 105)),
            };

            _cards = new Panel[4];
            _cardValueLabels = new Label[4];

            var flpCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 108,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 16)
            };

            for (int i = 0; i < cardDefs.Length; i++)
            {
                var (icon, title, val, accent) = cardDefs[i];
                var card = new Panel { Width = 195, Height = 88, BackColor = Color.White, Margin = new Padding(0, 0, 14, 0) };
                card.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10);
                    g.FillPath(Brushes.White, path);
                    g.DrawPath(new Pen(Color.FromArgb(226, 232, 240), 1), path);
                    // left accent
                    g.FillRectangle(new SolidBrush(accent), 0, 0, 4, card.Height);
                };

                // TableLayoutPanel inside card: prevents any label from overlapping
                var cardTable = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    BackColor = Color.Transparent,
                    Padding = new Padding(10, 10, 8, 8)
                };
                cardTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46));
                cardTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                cardTable.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
                cardTable.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

                var lblIcon = new Label
                {
                    Text = icon,
                    Font = new Font("Segoe UI Emoji", 20),
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    ForeColor = accent,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                cardTable.SetRowSpan(lblIcon, 2);

                var lblVal = new Label
                {
                    Text = val,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 40, 90),
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.BottomLeft
                };
                var lblTitle2 = new Label
                {
                    Text = title,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.TopLeft
                };

                cardTable.Controls.Add(lblIcon,   0, 0);
                cardTable.Controls.Add(lblVal,    1, 0);
                cardTable.Controls.Add(lblTitle2, 1, 1);
                card.Controls.Add(cardTable);
                _cards[i]            = card;
                _cardValueLabels[i]  = lblVal;   // still valid — lblVal is in scope
                flpCards.Controls.Add(card);
            }

            // Section label
            var lblSec = new Label
            {
                Text = "Alertas Ativos",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 40, 90),
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 0, 0, 8),
                BackColor = Color.Transparent
            };

            // Grid card
            pnlDashGrid = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            pnlDashGrid.Paint += PaintCard;

            dgvDashboard = BuildGrid();
            dgvDashboard.Dock = DockStyle.Fill;
            dgvDashboard.Columns.AddRange(
                ColTxt("Cidade",    "Cidade",       170),
                ColTxt("Nivel",     "Nível",        100),
                ColTxt("Tipo",      "Tipo",         130),
                ColTxt("Risco",     "Índice Risco", 110),
                ColTxt("Descricao", "Descrição",    400),
                ColTxt("DataHora",  "Data / Hora",  155)
            );
            pnlDashGrid.Controls.Add(dgvDashboard);

            pageDashboard.Controls.Add(pnlDashGrid);
            pageDashboard.Controls.Add(lblSec);
            pageDashboard.Controls.Add(flpCards);
            pnlContent.Controls.Add(pageDashboard);
        }

        // ═══════════════════════════════════════════════════════════════
        // CIDADES PAGE
        // ═══════════════════════════════════════════════════════════════
        private void BuildCidadesPage()
        {
            pageCidades = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };
            var tb = BuildToolbar();
            btnNovaCidade    = AddToolBtn(tb, "➕  Nova Cidade",     Color.FromArgb(37, 99, 235));
            btnEditarCidade  = AddToolBtn(tb, "✏️  Editar",           Color.FromArgb(71, 85, 105));
            btnExcluirCidade = AddToolBtn(tb, "🗑️  Excluir",          Color.FromArgb(220, 38, 38));
            btnSimularCidade = AddToolBtn(tb, "🛰️  Simular Leitura",  Color.FromArgb(5, 150, 105));

            var pg = GridCard();
            dgvCidades = BuildGrid(); dgvCidades.Dock = DockStyle.Fill;
            dgvCidades.Columns.AddRange(
                ColTxt("Id",           "ID",          50),
                ColTxt("Nome",         "Cidade",      180),
                ColTxt("Estado",       "UF",           58),
                ColTxt("Latitude",     "Latitude",    110),
                ColTxt("Longitude",    "Longitude",   110),
                ColTxt("Populacao",    "População",   140),
                ColTxt("DataCadastro", "Cadastro",    150)
            );
            pg.Controls.Add(dgvCidades);
            pageCidades.Controls.Add(pg);
            pageCidades.Controls.Add(tb);
            pnlContent.Controls.Add(pageCidades);
        }

        // ═══════════════════════════════════════════════════════════════
        // MONITORAMENTOS PAGE
        // ═══════════════════════════════════════════════════════════════
        private void BuildMonitoramentosPage()
        {
            pageMonitoramentos = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };
            var tb = BuildToolbar();

            lblFiltroCidade = new Label
            {
                Text = "Cidade:",
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(4, 10, 6, 0)
            };
            cboCidadesFiltro = new ComboBox
            {
                Width = 210,
                Height = 34,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 40, 90),
                FlatStyle = FlatStyle.Standard,   // Standard keeps the visible border
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 6, 10, 0)
            };
            btnFiltrarMonitoramentos = AddToolBtn(tb, "🔍  Filtrar",  Color.FromArgb(37, 99, 235));
            btnTodosMonitoramentos   = AddToolBtn(tb, "📋  Todos",    Color.FromArgb(71, 85, 105));

            var flp = (FlowLayoutPanel)tb.Tag;
            flp.Controls.Add(lblFiltroCidade);
            flp.Controls.Add(cboCidadesFiltro);

            var pg = GridCard();
            dgvMonitoramentos = BuildGrid(); dgvMonitoramentos.Dock = DockStyle.Fill;
            dgvMonitoramentos.Columns.AddRange(
                ColTxt("Id",       "ID",            50),
                ColTxt("Cidade",   "Cidade",        155),
                ColTxt("Chuva",    "Chuva (mm)",    108),
                ColTxt("Temp",     "Temp (°C)",      98),
                ColTxt("Umidade",  "Umidade (%)",   108),
                ColTxt("Vento",    "Vento (km/h)",  118),
                ColTxt("Risco",    "Índice Risco",  108),
                ColTxt("Fonte",    "Fonte",         155),
                ColTxt("DataHora", "Data / Hora",   155)
            );
            pg.Controls.Add(dgvMonitoramentos);
            pageMonitoramentos.Controls.Add(pg);
            pageMonitoramentos.Controls.Add(tb);
            pnlContent.Controls.Add(pageMonitoramentos);
        }

        // ═══════════════════════════════════════════════════════════════
        // ALERTAS PAGE
        // ═══════════════════════════════════════════════════════════════
        private void BuildAlertasPage()
        {
            pageAlertas = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };
            var tb = BuildToolbar();
            btnEncerrarAlerta   = AddToolBtn(tb, "✅  Encerrar Alerta", Color.FromArgb(5, 150, 105));
            btnAtualizarAlertas = AddToolBtn(tb, "🔄  Atualizar",        Color.FromArgb(71, 85, 105));
            chkSomenteAtivos = new CheckBox
            {
                Text = "Somente ativos",
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 9.5f),
                Checked = true, AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(8, 8, 0, 0)
            };
            ((FlowLayoutPanel)tb.Tag).Controls.Add(chkSomenteAtivos);

            var pg = GridCard();
            dgvAlertas = BuildGrid(); dgvAlertas.Dock = DockStyle.Fill;
            dgvAlertas.Columns.AddRange(
                ColTxt("Id",        "ID",            50),
                ColTxt("Cidade",    "Cidade",        155),
                ColTxt("Nivel",     "Nível",          98),
                ColTxt("Tipo",      "Tipo",           128),
                ColTxt("Risco",     "Índice Risco",   108),
                ColTxt("Descricao", "Descrição",      370),
                ColTxt("Ativo",     "Ativo",           65),
                ColTxt("DataHora",  "Data / Hora",    155)
            );
            pg.Controls.Add(dgvAlertas);
            pageAlertas.Controls.Add(pg);
            pageAlertas.Controls.Add(tb);
            pnlContent.Controls.Add(pageAlertas);
        }

        // ═══════════════════════════════════════════════════════════════
        // RELATÓRIO PAGE
        // ═══════════════════════════════════════════════════════════════
        private void BuildRelatorioPage()
        {
            pageRelatorio = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };
            var tb = BuildToolbar();
            btnAtualizarRelatorio = AddToolBtn(tb, "🔄  Atualizar Relatório", Color.FromArgb(37, 99, 235));

            var pg = GridCard();
            dgvRelatorio = BuildGrid(); dgvRelatorio.Dock = DockStyle.Fill;
            dgvRelatorio.Columns.AddRange(
                ColTxt("Cidade",        "Cidade",              155),
                ColTxt("Estado",        "UF",                   52),
                ColTxt("TotalMon",      "Monitoramentos",       128),
                ColTxt("MediaChuva",    "Chuva Média (mm)",     142),
                ColTxt("MediaTemp",     "Temp. Média (°C)",     138),
                ColTxt("IndiceRisco",   "Índice Risco Médio",   152),
                ColTxt("AlertasAtivos", "Alertas Ativos",       118),
                ColTxt("NivelAtual",    "Nível Atual",          108),
                ColTxt("UltimaAtt",     "Última Atualização",   155)
            );
            pg.Controls.Add(dgvRelatorio);
            pageRelatorio.Controls.Add(pg);
            pageRelatorio.Controls.Add(tb);
            pnlContent.Controls.Add(pageRelatorio);
        }

        // ═══════════════════════════════════════════════════════════════
        // STATUS BAR
        // ═══════════════════════════════════════════════════════════════
        private void BuildStatusBar()
        {
            statusBar = new StatusStrip
            {
                BackColor = Color.White,
                SizingGrip = false,
                Font = new Font("Segoe UI", 8.5f)
            };
            statusBar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, statusBar.Width, 0);

            lblStatusBar = new ToolStripStatusLabel
            {
                Text = "Sistema pronto.",
                ForeColor = Color.FromArgb(100, 116, 139),
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblUltimaAtualizacao = new ToolStripStatusLabel
            {
                Text = "",
                ForeColor = Color.FromArgb(100, 116, 139),
                Alignment = ToolStripItemAlignment.Right
            };
            statusBar.Items.AddRange(new ToolStripItem[] { lblStatusBar, lblUltimaAtualizacao });
            this.Controls.Add(statusBar);
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════════
        private Panel BuildToolbar()
        {
            var p = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.Transparent };
            var flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };
            p.Controls.Add(flp);
            p.Tag = flp;
            return p;
        }

        private Button AddToolBtn(Panel toolbar, string text, Color bg)
        {
            var flp = (FlowLayoutPanel)toolbar.Tag;
            var btn = new Button
            {
                Text = text,
                AutoSize = false,
                Height = 34,
                Width = TextRenderer.MeasureText(text, new Font("Segoe UI", 9, FontStyle.Bold)).Width + 28,
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(bg, 0.08f);
            btn.Paint += (s, e) =>
            {
                var b = (Button)s!;
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillPath(new SolidBrush(b.BackColor), RoundedRect(new Rectangle(0, 0, b.Width - 1, b.Height - 1), 8));
                TextRenderer.DrawText(g, b.Text, b.Font, b.ClientRectangle, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            flp.Controls.Add(btn);
            return btn;
        }

        private Panel GridCard()
        {
            var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            p.Paint += PaintCard;
            return p;
        }

        private DataGridView BuildGrid()
        {
            var dgv = new DataGridView
            {
                BackgroundColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                GridColor = Color.FromArgb(241, 245, 249),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 36 },
                Font = new Font("Segoe UI", 9.5f),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                ScrollBars = ScrollBars.Both
            };
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(248, 250, 252),
                SelectionForeColor = Color.FromArgb(71, 85, 105),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                SelectionBackColor = Color.FromArgb(239, 246, 255),
                SelectionForeColor = Color.FromArgb(30, 64, 175),
                Padding = new Padding(10, 0, 0, 0)
            };
            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(250, 252, 255),
                SelectionBackColor = Color.FromArgb(239, 246, 255)
            };
            return dgv;
        }

        private DataGridViewTextBoxColumn ColTxt(string name, string header, int width)
            => new() { Name = name, HeaderText = header, Width = width };

        internal static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle b, int r)
        {
            var p = new System.Drawing.Drawing2D.GraphicsPath();
            int d = r * 2;
            p.AddArc(b.X, b.Y, d, d, 180, 90);
            p.AddArc(b.Right - d, b.Y, d, d, 270, 90);
            p.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90);
            p.AddArc(b.X, b.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private void PaintCard(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillPath(Brushes.White, RoundedRect(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 10));
            g.DrawPath(new Pen(Color.FromArgb(226, 232, 240), 1), RoundedRect(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 10));
        }

        // ─── Controls ────────────────────────────────────────────────
        private Panel pnlSidebar = null!, pnlLogo = null!, pnlNav = null!;
        private Label lblLogoIcon = null!, lblLogoText = null!, lblLogoSub = null!, lblVersion = null!;
        private Button[] _navButtons = null!;
        private Panel pnlMain = null!, pnlTopBar = null!, pnlContent = null!;
        private Label lblPageTitle = null!, lblPageSub = null!, lblOnlineBadge = null!;
        private Panel pnlDashGrid = null!;
        private Panel[] _cards = null!;
        private Label[] _cardValueLabels = null!;
        private Panel pageDashboard = null!, pageCidades = null!, pageMonitoramentos = null!, pageAlertas = null!, pageRelatorio = null!;
        private DataGridView dgvDashboard = null!, dgvCidades = null!, dgvMonitoramentos = null!, dgvAlertas = null!, dgvRelatorio = null!;
        private Button btnExecutarMonitoramento = null!;
        private Button btnNovaCidade = null!, btnEditarCidade = null!, btnExcluirCidade = null!, btnSimularCidade = null!;
        private Button btnFiltrarMonitoramentos = null!, btnTodosMonitoramentos = null!;
        private Button btnEncerrarAlerta = null!, btnAtualizarAlertas = null!, btnAtualizarRelatorio = null!;
        private ComboBox cboCidadesFiltro = null!;
        private CheckBox chkSomenteAtivos = null!;
        private Label lblFiltroCidade = null!;
        private StatusStrip statusBar = null!;
        private ToolStripStatusLabel lblStatusBar = null!, lblUltimaAtualizacao = null!;
    }
}
