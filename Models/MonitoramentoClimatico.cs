namespace DisasterAlert.Models
{
    public enum FonteDados
    {
        SimuladoGOES16,
        SimuladoINPE,
        ManualUsuario
    }

    public class MonitoramentoClimatico
    {
        public int Id { get; set; }
        public int CidadeId { get; set; }
        public string CidadeNome { get; set; } = string.Empty;
        public double ChuvaAcumuladaMm { get; set; }
        public double TemperaturaC { get; set; }
        public double UmidadeRelativa { get; set; }
        public double VelocidadeVentoKmh { get; set; }
        public FonteDados Fonte { get; set; }
        public DateTime DataHoraRegistro { get; set; } = DateTime.Now;

        // Regra de negócio: índice de risco calculado com base nos dados climáticos
        public double CalcularIndiceRisco()
        {
            double indice = 0;

            // Peso da chuva acumulada (maior impacto)
            if (ChuvaAcumuladaMm >= 100) indice += 40;
            else if (ChuvaAcumuladaMm >= 60) indice += 25;
            else if (ChuvaAcumuladaMm >= 30) indice += 10;

            // Peso da temperatura extrema
            if (TemperaturaC >= 38 || TemperaturaC <= 5) indice += 20;
            else if (TemperaturaC >= 35 || TemperaturaC <= 10) indice += 10;

            // Peso da umidade
            if (UmidadeRelativa >= 90) indice += 20;
            else if (UmidadeRelativa >= 80) indice += 10;

            // Peso do vento
            if (VelocidadeVentoKmh >= 80) indice += 20;
            else if (VelocidadeVentoKmh >= 50) indice += 10;

            return Math.Min(indice, 100); // máximo 100
        }
    }
}
