namespace DisasterAlert.Models
{
    public class Cidade
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double PopulacaoEstimada { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public override string ToString() => $"{Nome} - {Estado}";
    }
}
