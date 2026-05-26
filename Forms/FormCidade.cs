using DisasterAlert.Models;

namespace DisasterAlert.Forms
{
    public class FormCidade : Form
    {
        public Cidade Cidade { get; private set; } = new();

        private TextBox txtNome = null!, txtEstado = null!, txtLatitude = null!, txtLongitude = null!, txtPopulacao = null!;
        private Button btnSalvar = null!, btnCancelar = null!;

        public FormCidade(Cidade? cidade = null)
        {
            if (cidade != null) Cidade = cidade;
            BuildUI();
            if (cidade != null) PreencherFormulario(cidade);
        }

        private void BuildUI()
        {
            Text = Cidade.Id == 0 ? "Nova Cidade" : "Editar Cidade";
            Size = new Size(440, 380);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(15, 23, 42);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var pnl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(20),
                ColumnStyles = { new ColumnStyle(SizeType.Percent, 40), new ColumnStyle(SizeType.Percent, 60) }
            };

            void AddField(string label, TextBox txt, int row, string placeholder = "")
            {
                txt.BackColor = Color.FromArgb(30, 41, 59);
                txt.ForeColor = Color.White;
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.Font = new Font("Segoe UI", 10);
                txt.Height = 32;
                txt.Dock = DockStyle.Fill;

                var lbl = new Label
                {
                    Text = label,
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Font = new Font("Segoe UI", 9),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 10, 0)
                };

                pnl.Controls.Add(lbl, 0, row);
                pnl.Controls.Add(txt, 1, row);
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            }

            txtNome = new TextBox();
            txtEstado = new TextBox();
            txtLatitude = new TextBox();
            txtLongitude = new TextBox();
            txtPopulacao = new TextBox();

            AddField("Cidade *", txtNome, 0);
            AddField("Estado (UF) *", txtEstado, 1);
            AddField("Latitude *", txtLatitude, 2);
            AddField("Longitude *", txtLongitude, 3);
            AddField("População *", txtPopulacao, 4);

            var lbl5 = new Label
            {
                Text = "* Campos obrigatórios",
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8),
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };
            pnl.Controls.Add(lbl5, 0, 5);
            pnl.SetColumnSpan(lbl5, 2);
            pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

            // Botões
            var pnlBotoes = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };

            btnSalvar = new Button
            {
                Text = "💾  Salvar",
                BackColor = Color.FromArgb(56, 189, 248),
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Width = 120,
                Height = 36,
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Width = 110,
                Height = 36,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            pnlBotoes.Controls.AddRange(new Control[] { btnSalvar, btnCancelar });
            pnl.Controls.Add(pnlBotoes, 0, 6);
            pnl.SetColumnSpan(pnlBotoes, 2);
            pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            this.Controls.Add(pnl);

            btnSalvar.Click += BtnSalvar_Click;
            btnCancelar.Click += (_, __) => DialogResult = DialogResult.Cancel;
        }

        private void PreencherFormulario(Cidade c)
        {
            txtNome.Text = c.Nome;
            txtEstado.Text = c.Estado;
            txtLatitude.Text = c.Latitude.ToString();
            txtLongitude.Text = c.Longitude.ToString();
            txtPopulacao.Text = c.PopulacaoEstimada.ToString();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNome.Text))
                    throw new Exception("Nome da cidade é obrigatório.");
                if (string.IsNullOrWhiteSpace(txtEstado.Text) || txtEstado.Text.Length > 2)
                    throw new Exception("Estado deve ter a sigla de 2 letras (ex: SP).");
                if (!double.TryParse(txtLatitude.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double lat))
                    throw new Exception("Latitude inválida. Use formato: -23.5505");
                if (!double.TryParse(txtLongitude.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    throw new Exception("Longitude inválida. Use formato: -46.6333");
                if (!double.TryParse(txtPopulacao.Text.Replace(".", "").Replace(",", "."), out double pop) || pop <= 0)
                    throw new Exception("População inválida.");

                Cidade.Nome = txtNome.Text.Trim();
                Cidade.Estado = txtEstado.Text.Trim().ToUpper();
                Cidade.Latitude = lat;
                Cidade.Longitude = lon;
                Cidade.PopulacaoEstimada = pop;

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Dados Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
