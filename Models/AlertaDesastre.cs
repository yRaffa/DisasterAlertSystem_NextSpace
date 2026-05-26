namespace DisasterAlert.Models
{
    public enum NivelAlerta
    {
        Baixo = 1,
        Medio = 2,
        Alto = 3,
        Critico = 4
    }

    public enum TipoDesastre
    {
        Enchente,
        Deslizamento,
        Seca,
        Tempestade,
        Nenhum
    }

    public class AlertaDesastre
    {
        public int Id { get; set; }
        public int CidadeId { get; set; }
        public string CidadeNome { get; set; } = string.Empty;
        public NivelAlerta Nivel { get; set; }
        public TipoDesastre Tipo { get; set; }
        public double IndiceRisco { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraAlerta { get; set; } = DateTime.Now;
        public DateTime? DataHoraEncerramento { get; set; }

        // Regra de negócio: determina nível e tipo a partir do índice de risco e dados climáticos
        public static AlertaDesastre GerarAlerta(Cidade cidade, MonitoramentoClimatico monitoramento)
        {
            double indice = monitoramento.CalcularIndiceRisco();
            NivelAlerta nivel;
            TipoDesastre tipo;

            if (indice >= 75)
                nivel = NivelAlerta.Critico;
            else if (indice >= 50)
                nivel = NivelAlerta.Alto;
            else if (indice >= 25)
                nivel = NivelAlerta.Medio;
            else
                nivel = NivelAlerta.Baixo;

            // Determina tipo de desastre provável
            if (monitoramento.ChuvaAcumuladaMm >= 80 && monitoramento.UmidadeRelativa >= 85)
                tipo = TipoDesastre.Deslizamento;
            else if (monitoramento.ChuvaAcumuladaMm >= 60)
                tipo = TipoDesastre.Enchente;
            else if (monitoramento.VelocidadeVentoKmh >= 60)
                tipo = TipoDesastre.Tempestade;
            else if (monitoramento.ChuvaAcumuladaMm < 5 && monitoramento.TemperaturaC >= 35)
                tipo = TipoDesastre.Seca;
            else
                tipo = TipoDesastre.Nenhum;

            return new AlertaDesastre
            {
                CidadeId = cidade.Id,
                CidadeNome = cidade.Nome,
                Nivel = nivel,
                Tipo = tipo,
                IndiceRisco = indice,
                Descricao = GerarDescricao(nivel, tipo, monitoramento),
                Ativo = nivel >= NivelAlerta.Medio
            };
        }

        private static string GerarDescricao(NivelAlerta nivel, TipoDesastre tipo, MonitoramentoClimatico m)
        {
            string nivelStr = nivel switch
            {
                NivelAlerta.Critico => "CRÍTICO",
                NivelAlerta.Alto => "ALTO",
                NivelAlerta.Medio => "MÉDIO",
                _ => "BAIXO"
            };

            string tipoStr = tipo switch
            {
                TipoDesastre.Enchente => "risco de enchente",
                TipoDesastre.Deslizamento => "risco de deslizamento",
                TipoDesastre.Tempestade => "tempestade severa",
                TipoDesastre.Seca => "condições de seca",
                _ => "monitoramento padrão"
            };

            return $"[{nivelStr}] {tipoStr} — Chuva: {m.ChuvaAcumuladaMm}mm | Temp: {m.TemperaturaC}°C | Vento: {m.VelocidadeVentoKmh}km/h";
        }
    }
}
