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

        public FormDashboard()
        {
            InitializeComponent();
            this.Load += FormDashboard_Load;
            btnExecutarMonitoramento.Click += BtnExecutarMonitoramento_Click;
            btnNovaCidade.Click += BtnNovaCidade_Click;
            btnEditarCidade.Click += BtnEditarCidade_Click;
            btnExcluirCidade.Click += BtnExcluirCidade_Click;
            btnSimularCidade.Click += BtnSimularCidade_Click;
            btnFiltrarMonitoramentos.Click += BtnFiltrarMonitoramentos_Click;
            btnTodosMonitoramentos.Click += BtnTodosMonitoramentos_Click;
            btnEncerrarAlerta.Click += BtnEncerrarAlerta_Click;
            btnAtualizarAlertas.Click += async (_, __) => await CarregarAlertasAsync();
            btnAtualizarRelatorio.Click += async (_, __) => await CarregarRelatorioAsync();
            tabControl.SelectedIndexChanged += async (_, __) => await AoTrocarTabAsync();

            // Posiciona status label no canto direito do header
            pnlHeader.Resize += (_, __) =>
                lblStatus.Location = new Point(pnlHeader.Width - lblStatus.Width - 20, 28);
        }

        private async void FormDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                SetStatus("Inicializando banco de dados...");
                await Database.DatabaseConfig.InicializarBancoDeDadosAsync();
                await Database.DatabaseConfig.SeedDadosIniciaisAsync();
                await CarregarDashboardAsync();
                await CarregarCidadesAsync();
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

        // ─── DASHBOARD ────────────────────────────────────────────────────────
        private async Task CarregarDashboardAsync()
        {
            try
            {
                var cidades = (await _cidadeRepo.ListarTodosAsync()).ToList();
                var alertasAtivos = (await _alertaRepo.ListarAtivosAsync()).ToList();
                int criticos = alertasAtivos.Count(a => a.Nivel == NivelAlerta.Critico);

                AtualizarCard(cardCidades, cidades.Count.ToString());
                AtualizarCard(cardAlertas, alertasAtivos.Count.ToString());
                AtualizarCard(cardCriticos, criticos.ToString());
                AtualizarCard(cardUltimaLeitura, DateTime.Now.ToString("HH:mm"));

                dgvDashboard.Rows.Clear();
                foreach (var alerta in alertasAtivos)
                {
                    int row = dgvDashboard.Rows.Add(
                        alerta.CidadeNome,
                        alerta.Nivel.ToString(),
                        alerta.Tipo.ToString(),
                        $"{alerta.IndiceRisco:F1}",
                        alerta.Descricao,
                        alerta.DataHoraAlerta.ToString("dd/MM/yyyy HH:mm")
                    );
                    ColorirLinhaAlerta(dgvDashboard.Rows[row], alerta.Nivel);
                }

                lblUltimaAtualizacao.Text = $"Atualizado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao carregar dashboard: {ex.Message}");
            }
        }

        private async void BtnExecutarMonitoramento_Click(object sender, EventArgs e)
        {
            btnExecutarMonitoramento.Enabled = false;
            btnExecutarMonitoramento.Text = "⏳  Processando leituras de satélite...";
            SetStatus("Executando ciclo de monitoramento via dados orbitais simulados (GOES-16)...");

            try
            {
                var alertas = await _alertaService.ExecutarCicloMonitoramentoAsync();
                await CarregarDashboardAsync();
                await CarregarComboFiltroAsync();

                string msg = alertas.Count > 0
                    ? $"✅ Ciclo concluído! {alertas.Count} alerta(s) gerado(s):\n\n" +
                      string.Join("\n", alertas.Select(a => $"• {a.CidadeNome}: [{a.Nivel}] {a.Tipo}"))
                    : "✅ Ciclo concluído! Nenhum alerta crítico gerado nesta leitura.";

                MessageBox.Show(msg, "Monitoramento Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetStatus($"Ciclo concluído. {alertas.Count} alerta(s) gerado(s).");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no ciclo de monitoramento:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus($"Erro: {ex.Message}");
            }
            finally
            {
                btnExecutarMonitoramento.Enabled = true;
                btnExecutarMonitoramento.Text = "🛰️  EXECUTAR CICLO DE MONITORAMENTO (SIMULAÇÃO SATÉLITE)";
            }
        }

        // ─── CIDADES ──────────────────────────────────────────────────────────
        private async Task CarregarCidadesAsync()
        {
            try
            {
                var cidades = await _cidadeRepo.ListarTodosAsync();
                dgvCidades.Rows.Clear();
                foreach (var c in cidades)
                {
                    dgvCidades.Rows.Add(c.Id, c.Nome, c.Estado, c.Latitude, c.Longitude,
                        $"{c.PopulacaoEstimada:N0}", c.DataCadastro.ToString("dd/MM/yyyy"));
                }
            }
            catch (Exception ex) { SetStatus($"Erro ao carregar cidades: {ex.Message}"); }
        }

        private async void BtnNovaCidade_Click(object sender, EventArgs e)
        {
            using var form = new FormCidade();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _cidadeRepo.InserirAsync(form.Cidade);
                    await CarregarCidadesAsync();
                    await CarregarComboFiltroAsync();
                    SetStatus($"Cidade '{form.Cidade.Nome}' cadastrada com sucesso.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar cidade:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnEditarCidade_Click(object sender, EventArgs e)
        {
            if (dgvCidades.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma cidade."); return; }
            int id = (int)dgvCidades.SelectedRows[0].Cells["Id"].Value;
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
                    MessageBox.Show($"Erro ao atualizar:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnExcluirCidade_Click(object sender, EventArgs e)
        {
            if (dgvCidades.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma cidade."); return; }
            int id = (int)dgvCidades.SelectedRows[0].Cells["Id"].Value;
            string nome = dgvCidades.SelectedRows[0].Cells["Nome"].Value.ToString()!;

            var confirm = MessageBox.Show($"Deseja excluir '{nome}' e todos seus dados?",
                "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    await _cidadeRepo.ExcluirAsync(id);
                    await CarregarCidadesAsync();
                    SetStatus($"Cidade '{nome}' excluída.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir:\n{ex.Message}\n\nVerifique se não há monitoramentos ou alertas vinculados.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnSimularCidade_Click(object sender, EventArgs e)
        {
            if (dgvCidades.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma cidade."); return; }
            int id = (int)dgvCidades.SelectedRows[0].Cells["Id"].Value;
            var cidade = await _cidadeRepo.BuscarPorIdAsync(id);
            if (cidade == null) return;

            try
            {
                var sim = new SimulacaoSateliteService();
                var leitura = sim.GerarLeituraSatelite(cidade);
                await _monitoramentoRepo.InserirAsync(leitura);

                var alerta = AlertaDesastre.GerarAlerta(cidade, leitura);
                string info = $"📡 Leitura Satélite — {cidade.Nome}\n\n" +
                              $"Chuva Acumulada: {leitura.ChuvaAcumuladaMm} mm\n" +
                              $"Temperatura: {leitura.TemperaturaC}°C\n" +
                              $"Umidade: {leitura.UmidadeRelativa}%\n" +
                              $"Velocidade do Vento: {leitura.VelocidadeVentoKmh} km/h\n\n" +
                              $"Índice de Risco Calculado: {alerta.IndiceRisco:F1}/100\n" +
                              $"Nível de Alerta: {alerta.Nivel}\n" +
                              $"Tipo de Desastre: {alerta.Tipo}";

                MessageBox.Show(info, "Simulação de Satélite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetStatus($"Simulação para {cidade.Nome} concluída. Risco: {alerta.IndiceRisco:F1}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── MONITORAMENTOS ───────────────────────────────────────────────────
        private async Task CarregarComboFiltroAsync()
        {
            var cidades = await _cidadeRepo.ListarTodosAsync();
            cboCidadesFiltro.DataSource = cidades.ToList();
            cboCidadesFiltro.DisplayMember = "Nome";
            cboCidadesFiltro.ValueMember = "Id";
        }

        private async void BtnFiltrarMonitoramentos_Click(object sender, EventArgs e)
        {
            if (cboCidadesFiltro.SelectedValue is int cidadeId)
                await CarregarMonitoramentosAsync(cidadeId);
        }

        private async void BtnTodosMonitoramentos_Click(object sender, EventArgs e)
            => await CarregarMonitoramentosAsync(null);

        private async Task CarregarMonitoramentosAsync(int? cidadeId = null)
        {
            try
            {
                IEnumerable<MonitoramentoClimatico> dados = cidadeId.HasValue
                    ? await _monitoramentoRepo.ListarPorCidadeAsync(cidadeId.Value)
                    : await _monitoramentoRepo.ListarTodosRecentesAsync(100);

                dgvMonitoramentos.Rows.Clear();
                foreach (var m in dados)
                {
                    double risco = m.CalcularIndiceRisco();
                    int row = dgvMonitoramentos.Rows.Add(
                        m.Id, m.CidadeNome,
                        $"{m.ChuvaAcumuladaMm:F1}",
                        $"{m.TemperaturaC:F1}",
                        $"{m.UmidadeRelativa:F1}",
                        $"{m.VelocidadeVentoKmh:F1}",
                        $"{risco:F1}",
                        m.Fonte.ToString(),
                        m.DataHoraRegistro.ToString("dd/MM/yyyy HH:mm")
                    );

                    // colore célula de risco
                    var riscoColor = risco >= 75 ? Color.FromArgb(254, 100, 100)
                        : risco >= 50 ? Color.FromArgb(251, 191, 36)
                        : risco >= 25 ? Color.FromArgb(251, 146, 60)
                        : Color.FromArgb(74, 222, 128);
                    dgvMonitoramentos.Rows[row].Cells["Risco"].Style.ForeColor = riscoColor;
                    dgvMonitoramentos.Rows[row].Cells["Risco"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        // ─── ALERTAS ──────────────────────────────────────────────────────────
        private async Task CarregarAlertasAsync()
        {
            try
            {
                IEnumerable<AlertaDesastre> alertas = chkSomenteAtivos.Checked
                    ? await _alertaRepo.ListarAtivosAsync()
                    : await _alertaRepo.ListarTodosAsync();

                dgvAlertas.Rows.Clear();
                foreach (var a in alertas)
                {
                    int row = dgvAlertas.Rows.Add(
                        a.Id, a.CidadeNome, a.Nivel.ToString(), a.Tipo.ToString(),
                        $"{a.IndiceRisco:F1}", a.Descricao,
                        a.Ativo ? "✅" : "❌",
                        a.DataHoraAlerta.ToString("dd/MM/yyyy HH:mm")
                    );
                    if (a.Ativo) ColorirLinhaAlerta(dgvAlertas.Rows[row], a.Nivel);
                }
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        private async void BtnEncerrarAlerta_Click(object sender, EventArgs e)
        {
            if (dgvAlertas.SelectedRows.Count == 0) { MessageBox.Show("Selecione um alerta."); return; }
            int id = (int)dgvAlertas.SelectedRows[0].Cells["Id"].Value;
            try
            {
                await _alertaService.EncerrarAlertaAsync(id);
                await CarregarAlertasAsync();
                await CarregarDashboardAsync();
                SetStatus($"Alerta #{id} encerrado com sucesso.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── RELATÓRIO ────────────────────────────────────────────────────────
        private async Task CarregarRelatorioAsync()
        {
            try
            {
                var relatorio = await _alertaService.ObterRelatorioAsync();
                dgvRelatorio.Rows.Clear();
                foreach (var r in relatorio)
                {
                    int row = dgvRelatorio.Rows.Add(
                        r.CidadeNome, r.Estado, r.TotalMonitoramentos,
                        $"{r.MediaChuva:F1}", $"{r.MediaTemperatura:F1}",
                        $"{r.IndiceRiscoMedio:F1}", r.AlertasAtivos,
                        r.NivelAlertaAtual,
                        r.UltimaAtualizacao.ToString("dd/MM/yyyy HH:mm")
                    );
                    // Cor pelo nível de alerta atual
                    var cor = r.NivelAlertaAtual switch
                    {
                        "Critico" => Color.FromArgb(254, 100, 100),
                        "Alto" => Color.FromArgb(251, 191, 36),
                        "Medio" => Color.FromArgb(251, 146, 60),
                        _ => Color.FromArgb(74, 222, 128)
                    };
                    dgvRelatorio.Rows[row].Cells["NivelAtual"].Style.ForeColor = cor;
                    dgvRelatorio.Rows[row].Cells["NivelAtual"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        // ─── HELPERS ──────────────────────────────────────────────────────────
        private async Task AoTrocarTabAsync()
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: await CarregarDashboardAsync(); break;
                case 1: await CarregarCidadesAsync(); break;
                case 2: await CarregarMonitoramentosAsync(); break;
                case 3: await CarregarAlertasAsync(); break;
                case 4: await CarregarRelatorioAsync(); break;
            }
        }

        private void ColorirLinhaAlerta(DataGridViewRow row, NivelAlerta nivel)
        {
            var cor = nivel switch
            {
                NivelAlerta.Critico => Color.FromArgb(60, 20, 20),
                NivelAlerta.Alto => Color.FromArgb(55, 40, 10),
                NivelAlerta.Medio => Color.FromArgb(50, 40, 10),
                _ => Color.FromArgb(15, 23, 42)
            };
            row.DefaultCellStyle.BackColor = cor;

            var nivelCor = nivel switch
            {
                NivelAlerta.Critico => Color.FromArgb(254, 100, 100),
                NivelAlerta.Alto => Color.FromArgb(251, 191, 36),
                NivelAlerta.Medio => Color.FromArgb(251, 146, 60),
                _ => Color.FromArgb(74, 222, 128)
            };
            // colore a célula de nível se existir
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (row.DataGridView?.Columns[cell.ColumnIndex]?.Name == "Nivel")
                {
                    cell.Style.ForeColor = nivelCor;
                    cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
            }
        }

        private void AtualizarCard(Panel card, string valor)
        {
            foreach (Control ctrl in card.Controls)
                if (ctrl is Label lbl && lbl.Font.Size > 15)
                    lbl.Text = valor;
        }

        private void SetStatus(string msg)
        {
            if (lblStatusBar != null)
                lblStatusBar.Text = msg;
        }
    }
}
