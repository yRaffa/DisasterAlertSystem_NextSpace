# 🌐 **Global Solution 2026 | C# Software Development** 🌐
# 🛰️ Disaster Alert System - NextSpace 🛰️
## Sistema de Monitoramento de Desastres Naturais via Dados de Satélites

### Integrantes do Grupo
| Nome | RM |
|---|---|
| André Luiz Fernandes de Queiroz | 554503 |
| Marcos Vinicius da Silva Costa | 555490 |
| Paulo Poças | 556080 |
| Rafael Bocchi | 557603 |
| Rafael Federici de Oliveira | 554736 |

---

## 📋 Sobre o Projeto

O **Disaster Alert System** é um protótipo funcional desenvolvido em **Windows Forms com C# (.NET 8)** que simula o monitoramento de desastres naturais em cidades brasileiras com base em dados orbitais obtidos via satélites.

A aplicação simula leituras de sensores dos satélites **GOES-16 (NOAA/NASA)** e dados do **INPE**, calcula índices de risco e gera alertas automáticos para cidades em situação crítica.

---

## 🛰️ Relação com a Indústria Espacial

Os satélites de observação da Terra fornecem dados contínuos de temperatura, umidade, cobertura de nuvens e precipitação. O INPE (Instituto Nacional de Pesquisas Espaciais) utiliza esses dados para monitorar enchentes, queimadas e seca no Brasil.

Esta aplicação simula como esses dados seriam consumidos e processados por um sistema de alerta precoce municipal.

**Caso o sitema entre em prudção, aqui estão as APIs que podem ser integradas:**
- [NASA Earthdata API](https://earthdata.nasa.gov/)
- [INPE Dados Abertos](https://queimadas.dgi.inpe.br/)
- [Copernicus Data Space Ecosystem](https://dataspace.copernicus.eu/)

---

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server
- Windows

---

## 🗄️ Configuração do Banco de Dados

1. Abra o **DBeaver** e conecte ao seu SQL Server
2. Crie um banco de dados chamado `DisasterAlertDB`:
   ```sql
   CREATE DATABASE DisasterAlertDB;
   ```
3. Abra o arquivo `Database/DatabaseConfig.cs` e ajuste a `ConnectionString`:
   ```csharp
   "Server=localhost;Database=DisasterAlertDB;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
   > Se usar autenticação SQL Server (usuário/senha), use:
   > `"Server=localhost;Database=DisasterAlertDB;User Id=seu_usuario;Password=sua_senha;TrustServerCertificate=True;"`

4. As tabelas são criadas automaticamente na primeira execução.

---

## ▶️ Como Executar

### Via Terminal
```bash
cd DisasterAlert
dotnet restore
dotnet run
```

### Via Visual Studio
1. Abra `DisasterAlert.csproj`
2. Pressione `F5` para executar

---

## 🗂️ Estrutura do Projeto

```
DisasterAlert/
├── Models/
│   ├── Cidade.cs                      # Entidade cidade com dados geográficos
│   ├── MonitoramentoClimatico.cs      # Leitura de dados do satélite + cálculo de risco
│   ├── AlertaDesastre.cs              # Alerta gerado com nível e tipo de desastre
│   └── RelatorioResumo.cs             # DTO para relatório consolidado
├── Repositories/
│   ├── CidadeRepository.cs            # CRUD de cidades
│   ├── MonitoramentoRepository.cs     # Persistência de monitoramentos
│   └── AlertaRepository.cs            # Persistência e consulta de alertas + relatório
├── Services/
│   ├── SimulacaoSateliteService.cs    # Simula leituras de satélite 
│   └── AlertaService.cs               # Orquestra ciclo: satélite → monitoramento → alerta
├── Forms/
│   ├── FormDashboard.cs               # Tela principal com 5 abas
│   ├── FormDashboard.Designer.cs      # Layout e estilos da interface
│   └── FormCidade.cs                  # Formulário CRUD de cidades
├── Database/
│   └── DatabaseConfig.cs              # Conexão, criação de tabelas e seed de dados
├── Program.cs                         # Entry point
└── DisasterAlert.csproj               # Dependências: Dapper, Microsoft.Data.SqlClient
```

---

## 💻 Como Usar

### 1. Dashboard
- Exibe **cards** com total de cidades, alertas ativos e nível crítico
- Lista alertas ativos coloridos por nível (vermelho = crítico, amarelo = alto)
- **Botão principal**: `🛰️ EXECUTAR CICLO DE MONITORAMENTO` — simula leitura de todos os satélites e gera alertas automaticamente

### 2. Cidades
- **CRUD completo**: cadastrar, editar, excluir cidades
- Botão `🛰️ Simular Monitoramento`: gera e exibe uma leitura individual da cidade selecionada

### 3. Monitoramentos
- Histórico de todas as leituras registradas
- Filtro por cidade
- Índice de risco colorido por nível

### 4. Alertas
- Lista de alertas com opção de filtrar ativos/histórico
- Botão `✅ Encerrar Alerta` para registrar resolução

### 5. Relatório
- Resumo consolidado por cidade: médias, índice de risco médio, alertas ativos

---

## 📐 Regra de Negócio (Cálculo de Índice de Risco)

O índice é calculado em `MonitoramentoClimatico.CalcularIndiceRisco()`:

| Variável | Condição | Pontos |
|----------|----------|--------|
| Chuva acumulada | ≥ 100mm | +40 |
| Chuva acumulada | ≥ 60mm  | +25 |
| Chuva acumulada | ≥ 30mm  | +10 |
| Temperatura | ≥ 38°C ou ≤ 5°C | +20 |
| Umidade relativa | ≥ 90% | +20 |
| Vento | ≥ 80 km/h | +20 |

**Níveis de Alerta:**

| Intervalo | Nível | Alerta |
|---|---|---|
| 0–24 | Baixo | 🟢 |
| 25–49 | Médio | 🟡 |
| 50–74 | Alto | 🟠 |
| 75–100 | Crítico | 🔴 |

---

## 📚 Referências

- **INPE** — Introdução ao Sensoriamento Remoto. Disponível em: http://www.inpe.br/
- **Florenzano, T. G.** — *Iniciação em Sensoriamento Remoto*. Oficina de Textos, 2011. *(referência em português)*
- **AEB** — Fundamentos de Sensoriamento Remoto. Agência Espacial Brasileira.
- **NASA Earthdata** — Earth Observation Data. https://earthdata.nasa.gov/
- **NOAA GOES-16** — Geostationary Operational Environmental Satellite. https://www.goes.noaa.gov/
- **Copernicus Programme** — European Earth Observation Programme. https://www.copernicus.eu/

---

## ⚠️ Limitações Conhecidas

- Os dados climáticos são **simulados** com base em perfis regionais históricos, não em APIs em tempo real
