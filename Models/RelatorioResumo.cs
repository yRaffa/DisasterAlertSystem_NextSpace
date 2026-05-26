namespace DisasterAlert.Models
{
    public class RelatorioResumo
    {
        public string CidadeNome { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int TotalMonitoramentos { get; set; }
        public double MediaChuva { get; set; }
        public double MediaTemperatura { get; set; }
        public double IndiceRiscoMedio { get; set; }
        public int AlertasAtivos { get; set; }
        public string NivelAlertaAtual { get; set; } = "Baixo";
        public DateTime UltimaAtualizacao { get; set; }
    }
}
