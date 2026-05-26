using DisasterAlert.Models;
using DisasterAlert.Repositories;

namespace DisasterAlert.Services
{
    public class AlertaService
    {
        private readonly AlertaRepository _alertaRepo;
        private readonly MonitoramentoRepository _monitoramentoRepo;
        private readonly CidadeRepository _cidadeRepo;
        private readonly SimulacaoSateliteService _sateliteService;

        public AlertaService()
        {
            _alertaRepo = new AlertaRepository();
            _monitoramentoRepo = new MonitoramentoRepository();
            _cidadeRepo = new CidadeRepository();
            _sateliteService = new SimulacaoSateliteService();
        }

        /// <summary>
        /// Executa ciclo completo: simula dados de satélite → registra monitoramento → gera alertas
        /// </summary>
        public async Task<List<AlertaDesastre>> ExecutarCicloMonitoramentoAsync()
        {
            var cidades = (await _cidadeRepo.ListarTodosAsync()).ToList();
            var alertasGerados = new List<AlertaDesastre>();

            var leituras = _sateliteService.GerarLeituraEmLote(cidades);

            foreach (var leitura in leituras)
            {
                // Salva monitoramento
                await _monitoramentoRepo.InserirAsync(leitura);

                // Busca cidade correspondente
                var cidade = cidades.First(c => c.Id == leitura.CidadeId);

                // Gera alerta baseado na regra de negócio
                var alerta = AlertaDesastre.GerarAlerta(cidade, leitura);

                // Salva alerta apenas se nível Médio ou superior
                if (alerta.Nivel >= NivelAlerta.Medio)
                {
                    alerta.Id = await _alertaRepo.InserirAsync(alerta);
                    alertasGerados.Add(alerta);
                }
            }

            return alertasGerados;
        }

        public async Task<IEnumerable<AlertaDesastre>> ObterAlertasAtivosAsync()
            => await _alertaRepo.ListarAtivosAsync();

        public async Task EncerrarAlertaAsync(int id)
            => await _alertaRepo.EncerrarAlertaAsync(id);

        public async Task<IEnumerable<RelatorioResumo>> ObterRelatorioAsync()
            => await _alertaRepo.GerarRelatorioAsync();
    }
}
