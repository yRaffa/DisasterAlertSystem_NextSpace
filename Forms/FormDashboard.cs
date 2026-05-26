using DisasterAlert.Models;
using DisasterAlert.Repositories;
using DisasterAlert.Services;

namespace DisasterAlert.Forms
{
    public partial class FormDashboard : Form
    {
        private readonly AlertaService _alertaService = new();
        private readonly CidadeRepository _cidadeRepo = new();
        private readonly MonitoramentoRepository _monitoramentoRepo = new();
        private readonly AlertaRepository _alertaRepo = new();

        private int _currentPage = 0;

        private readonly (string Title, string Sub, Panel? Page)[] _pages = null!;

        public FormDashboard()
        {
            InitializeComponent();

            _pages = new[]
            {
                ("Dashboard",      "Visão geral do sistema de monitoramento",  (Panel?)null),
                ("Cidades",        "Gerenciar cidades monitoradas",             (Panel?)null),
                ("Monitoramentos", "Histórico de leituras climáticas",          (Panel?)null),
                ("Alertas",        "Alertas de desastres gerados",              (Panel?)null),
                ("Relatório",      "Resumo consolidado por cidade",             (Panel?)null),
            };

            // assign page references after InitializeComponent
            _pages[0] = (_pages[0].Title, _pages[0].Sub, pageDashboard);
            _pages[1] = (_pages[1].Title, _pages[1].Sub, pageCidades);
            _pages[2] = (_pages[2].Title, _pages[2].Sub, pageMonitoramentos);
            _pages[3] = (_pages[3].Title, _pages[3].Sub, pageAlertas);
            _pages[4] = (_pages[4].Title, _pages[4].Sub, pageRelatorio);

            WireEvents();
            this.Load += async (_, __) => await OnLoadAsync();
        }

        private void WireEvents()
        {
            for (int i = 0; i < _navButtons.Length; i++)
            {
                int idx = i;
                _navButtons[i].Click += async (_, __) => await NavigateToAsync(idx);
            }

            btnExecutarMonitoramento.Click   += BtnExecutar_Click;
            btnNovaCidade.Click              += BtnNovaCidade_Click;
            btnEditarCidade.Click            += BtnEditarCidade_Click;
            btnExcluirCidade.Click           += BtnExcluirCidade_Click;
            btnSimularCidade.Click           += BtnSimularCidade_Click;
            btnFiltrarMonitoramentos.Click   += BtnFiltrar_Click;
            btnTodosMonitoramentos.Click     += async (_, __) => await CarregarMonitoramentosAsync();
            btnEncerrarAlerta.Click          += BtnEncerrar_Click;
            btnAtualizarAlertas.Click        += async (_, __) => await CarregarAlertasAsync();
            btnAtualizarRelatorio.Click      += async (_, __) => await CarregarRelatorioAsync();
            chkSomenteAtivos.CheckedChanged  += async (_, __) => await CarregarAlertasAsync();
        }

        private async Task OnLoadAsync()
        {
            try
            {
                SetStatus("Inicializando banco de dados...");
                await Database.DatabaseConfig.InicializarBancoDeDadosAsync();
                await Database.DatabaseConfig.SeedDadosIniciaisAsync();
                await CarregarDashboardAsync();
                await CarregarComboFiltroAsync();
                SetStatus("Sistema carregado com sucesso.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar:\n{ex.Message}", "Erro de Inicialização",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus($"Erro: {ex.Message}");
            }
        }

        // ─── NAVIGATION ───────────────────────────────────────────────────
        private async Task NavigateToAsync(int idx)
        {
            _currentPage = idx;

            // Toggle nav button styles
            for (int i = 0; i < _navButtons.Length; i++)
                _navButtons[i].BackColor = i == idx
                    ? Color.FromArgb(30, 80, 160)
                    : Color.Transparent;

            // Show/hide pages
            foreach (var (_, _, page) in _pages)
                if (page != null) page.Visible = false;
            if (_pages[idx].Page != null)
                _pages[idx].Page!.Visible = true;

            lblPageTitle.Text = _pages[idx].Title;
            lblPageSub.Text   = _pages[idx].Sub;

            switch (idx)
            {
                case 0: await CarregarDashboardAsync(); break;
                case 1: await CarregarCidadesAsync(); break;
                case 2: await CarregarMonitoramentosAsync(); break;
                case 3: await CarregarAlertasAsync(); break;
                case 4: await CarregarRelatorioAsync(); break;
            }
        }

        // ─── DASHBOARD ────────────────────────────────────────────────────
        private async Task CarregarDashboardAsync()
        {
            try
            {
                var cidades = (await _cidadeRepo.ListarTodosAsync()).ToList();
                var alertas = (await _alertaRepo.ListarAtivosAsync()).ToList();
                int criticos = alertas.Count(a => a.Nivel == NivelAlerta.Critico);

                _cardValueLabels[0].Text = cidades.Count.ToString();
                _cardValueLabels[1].Text = alertas.Count.ToString();
                _cardValueLabels[2].Text = criticos.ToString();
                _cardValueLabels[3].Text = DateTime.Now.ToString("HH:mm");

                // Refresh card panels
                foreach (var c in _cards) c.Invalidate();

                dgvDashboard.Rows.Clear();
                foreach (var a in alertas)
                {
                    int row = dgvDashboard.Rows.Add(
                        a.CidadeNome, a.Nivel.ToString(), a.Tipo.ToString(),
                        $"{a.IndiceRisco:F1}", a.Descricao,
                        a.DataHoraAlerta.ToString("dd/MM/yyyy HH:mm"));
                    AplicarCorAlerta(dgvDashboard.Rows[row], a.Nivel);
                }

                lblUltimaAtualizacao.Text = $"Atualizado: {DateTime.Now:dd/MM HH:mm:ss}";
            }
            catch (Exception ex) { SetStatus($"Erro dashboard: {ex.Message}"); }
        }

        private async void BtnExecutar_Click(object? sender, EventArgs e)
        {
            btnExecutarMonitoramento.Enabled = false;
            btnExecutarMonitoramento.Text = "  Processando...";
            SetStatus("Executando ciclo de monitoramento via dados orbitais simulados...");

            try
            {
                var alertas = await _alertaService.ExecutarCicloMonitoramentoAsync();
                await CarregarDashboardAsync();
                await CarregarComboFiltroAsync();

                string msg = alertas.Count > 0
                    ? $"✅ Ciclo concluído!\n\n{alertas.Count} alerta(s) gerado(s):\n\n" +
                      string.Join("\n", alertas.Select(a => $"  • {a.CidadeNome}: [{a.Nivel}] {a.Tipo}"))
                    : "✅ Ciclo concluído!\nNenhum alerta crítico nesta leitura.";

                MessageBox.Show(msg, "Monitoramento Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetStatus($"Ciclo concluído — {alertas.Count} alerta(s).");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExecutarMonitoramento.Enabled = true;
                btnExecutarMonitoramento.Text = "  Executar Monitoramento";
            }
        }

        // ─── CIDADES ──────────────────────────────────────────────────────
        private async Task CarregarCidadesAsync()
        {
            try
            {
                var cidades = await _cidadeRepo.ListarTodosAsync();
                dgvCidades.Rows.Clear();
                foreach (var c in cidades)
                    dgvCidades.Rows.Add(c.Id, c.Nome, c.Estado, c.Latitude, c.Longitude,
                        $"{c.PopulacaoEstimada:N0}", c.DataCadastro.ToString("dd/MM/yyyy"));
            }
            catch (Exception ex) { SetStatus($"Erro cidades: {ex.Message}"); }
        }

        private async void BtnNovaCidade_Click(object? sender, EventArgs e)
        {
            using var form = new FormCidade();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _cidadeRepo.InserirAsync(form.Cidade);
                    await CarregarCidadesAsync();
                    await CarregarComboFiltroAsync();
                    SetStatus($"Cidade '{form.Cidade.Nome}' cadastrada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnEditarCidade_Click(object? sender, EventArgs e)
        {
            if (dgvCidades.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma cidade."); return; }
            int id = Convert.ToInt32(dgvCidades.SelectedRows[0].Cells["Id"].Value);
            var cidade = await _cidadeRepo.BuscarPorIdAsync(id);
            if (cidade == null) return;

            using var form = new FormCidade(cidade);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _cidadeRepo.AtualizarAsync(form.Cidade);
                    await CarregarCidadesAsync();
                    SetStatus($"Cidade '{form.Cidade.Nome}' atualizada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnExcluirCidade_Click(object? sender, EventArgs e)
        {
            if (dgvCidades.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma cidade."); return; }
            int id = Convert.ToInt32(dgvCidades.SelectedRows[0].Cells["Id"].Value);
            string nome = dgvCidades.SelectedRows[0].Cells["Nome"].Value?.ToString() ?? "";

            if (MessageBox.Show($"Excluir '{nome}' e todos seus dados?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    await _cidadeRepo.ExcluirAsync(id);
                    await CarregarCidadesAsync();
                    SetStatus($"Cidade '{nome}' excluída.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir.\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnSimularCidade_Click(object? sender, EventArgs e)
        {
            if (dgvCidades.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma cidade."); return; }
            int id = Convert.ToInt32(dgvCidades.SelectedRows[0].Cells["Id"].Value);
            var cidade = await _cidadeRepo.BuscarPorIdAsync(id);
            if (cidade == null) return;

            try
            {
                var sim = new SimulacaoSateliteService();
                var leitura = sim.GerarLeituraSatelite(cidade);
                await _monitoramentoRepo.InserirAsync(leitura);
                var alerta = AlertaDesastre.GerarAlerta(cidade, leitura);

                MessageBox.Show(
                    $"📡  Leitura de Satélite — {cidade.Nome}\n\n" +
                    $"Chuva Acumulada:    {leitura.ChuvaAcumuladaMm} mm\n" +
                    $"Temperatura:        {leitura.TemperaturaC}°C\n" +
                    $"Umidade:            {leitura.UmidadeRelativa}%\n" +
                    $"Velocidade do Vento:{leitura.VelocidadeVentoKmh} km/h\n\n" +
                    $"Índice de Risco:    {alerta.IndiceRisco:F1} / 100\n" +
                    $"Nível de Alerta:    {alerta.Nivel}\n" +
                    $"Tipo de Desastre:   {alerta.Tipo}",
                    "Simulação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SetStatus($"Simulação {cidade.Nome} — risco: {alerta.IndiceRisco:F1}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── MONITORAMENTOS ───────────────────────────────────────────────
        private async Task CarregarComboFiltroAsync()
        {
            var cidades = (await _cidadeRepo.ListarTodosAsync()).ToList();
            cboCidadesFiltro.DataSource = cidades;
            cboCidadesFiltro.DisplayMember = "Nome";
            cboCidadesFiltro.ValueMember = "Id";
        }

        private async void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            if (cboCidadesFiltro.SelectedValue is int id)
                await CarregarMonitoramentosAsync(id);
        }

        private async Task CarregarMonitoramentosAsync(int? cidadeId = null)
        {
            try
            {
                var dados = cidadeId.HasValue
                    ? await _monitoramentoRepo.ListarPorCidadeAsync(cidadeId.Value)
                    : await _monitoramentoRepo.ListarTodosRecentesAsync(100);

                dgvMonitoramentos.Rows.Clear();
                foreach (var m in dados)
                {
                    double risco = m.CalcularIndiceRisco();
                    int row = dgvMonitoramentos.Rows.Add(
                        m.Id, m.CidadeNome,
                        $"{m.ChuvaAcumuladaMm:F1}", $"{m.TemperaturaC:F1}",
                        $"{m.UmidadeRelativa:F1}", $"{m.VelocidadeVentoKmh:F1}",
                        $"{risco:F1}", m.Fonte.ToString(),
                        m.DataHoraRegistro.ToString("dd/MM/yyyy HH:mm"));

                    var riscoColor = risco >= 75 ? Color.FromArgb(210,   0,   0)   // Crítico - vermelho
                        : risco >= 50            ? Color.FromArgb(185, 100,   0)   // Alto    - laranja
                        : risco >= 25            ? Color.FromArgb(255, 200,   0)   // Médio   - amarelo
                        :                          Color.FromArgb(  0, 200,  10);  // Baixo   - verde
                    dgvMonitoramentos.Rows[row].Cells["Risco"].Style.ForeColor = riscoColor;
                    dgvMonitoramentos.Rows[row].Cells["Risco"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        // ─── ALERTAS ──────────────────────────────────────────────────────
        private async Task CarregarAlertasAsync()
        {
            try
            {
                var alertas = chkSomenteAtivos.Checked
                    ? await _alertaRepo.ListarAtivosAsync()
                    : await _alertaRepo.ListarTodosAsync();

                dgvAlertas.Rows.Clear();
                foreach (var a in alertas)
                {
                    int row = dgvAlertas.Rows.Add(
                        a.Id, a.CidadeNome, a.Nivel.ToString(), a.Tipo.ToString(),
                        $"{a.IndiceRisco:F1}", a.Descricao,
                        a.Ativo ? "Sim" : "Não",
                        a.DataHoraAlerta.ToString("dd/MM/yyyy HH:mm"));
                    if (a.Ativo) AplicarCorAlerta(dgvAlertas.Rows[row], a.Nivel);
                }
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        private async void BtnEncerrar_Click(object? sender, EventArgs e)
        {
            if (dgvAlertas.SelectedRows.Count == 0) { MessageBox.Show("Selecione um alerta."); return; }
            int id = Convert.ToInt32(dgvAlertas.SelectedRows[0].Cells["Id"].Value);
            try
            {
                await _alertaService.EncerrarAlertaAsync(id);
                await CarregarAlertasAsync();
                await CarregarDashboardAsync();
                SetStatus($"Alerta #{id} encerrado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── RELATÓRIO ────────────────────────────────────────────────────
        private async Task CarregarRelatorioAsync()
        {
            try
            {
                var rel = await _alertaService.ObterRelatorioAsync();
                dgvRelatorio.Rows.Clear();
                foreach (var r in rel)
                {
                    int row = dgvRelatorio.Rows.Add(
                        r.CidadeNome, r.Estado, r.TotalMonitoramentos,
                        $"{r.MediaChuva:F1}", $"{r.MediaTemperatura:F1}",
                        $"{r.IndiceRiscoMedio:F1}", r.AlertasAtivos,
                        r.NivelAlertaAtual,
                        r.UltimaAtualizacao.ToString("dd/MM/yyyy HH:mm"));

                    var cor = r.NivelAlertaAtual switch
                    {
                        "Critico" => Color.FromArgb(210,   0,   0),
                        "Alto"    => Color.FromArgb(185, 100,   0),
                        "Medio"   => Color.FromArgb(255, 200,   0),
                        _         => Color.FromArgb(  0, 200,  10)
                    };
                    dgvRelatorio.Rows[row].Cells["NivelAtual"].Style.ForeColor = cor;
                    dgvRelatorio.Rows[row].Cells["NivelAtual"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        // ─── HELPERS ──────────────────────────────────────────────────────
        private void AplicarCorAlerta(DataGridViewRow row, NivelAlerta nivel)
        {
            // Baixo=Verde, Médio=Amarelo, Alto=Laranja, Crítico=Vermelho
            var (bg, fg) = nivel switch
            {
                NivelAlerta.Critico => (Color.FromArgb(255, 235, 235), Color.FromArgb(210,   0,   0)),
                NivelAlerta.Alto    => (Color.FromArgb(255, 243, 220), Color.FromArgb(185, 100,   0)),
                NivelAlerta.Medio   => (Color.FromArgb(255, 252, 200), Color.FromArgb(180, 140,   0)),
                _                   => (Color.FromArgb(230, 255, 230), Color.FromArgb(  0, 200,  10))
            };
            row.DefaultCellStyle.BackColor = bg;
            row.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(bg, 0.3f);

            foreach (DataGridViewCell cell in row.Cells)
                if (row.DataGridView?.Columns[cell.ColumnIndex]?.Name == "Nivel")
                {
                    cell.Style.ForeColor = fg;
                    cell.Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
        }

        private void SetStatus(string msg) => lblStatusBar.Text = msg;
    }
}
