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
            Size = new Size(460, 420);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            ForeColor = Color.FromArgb(30, 41, 59);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9.5f);

            // Header strip
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.FromArgb(15, 40, 90),
                Padding = new Padding(24, 0, 0, 0)
            };

            var lblHeaderTitle = new Label
            {
                Text = Cidade.Id == 0 ? "➕  Nova Cidade" : "✏️  Editar Cidade",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(24, 12)
            };
            var lblHeaderSub = new Label
            {
                Text = "Preencha os dados geográficos da cidade",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 180, 230),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(26, 38)
            };
            pnlHeader.Controls.AddRange(new Control[] { lblHeaderTitle, lblHeaderSub });

            // Form body
            var pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(28, 20, 28, 0)
            };

            var fields = new[]
            {
                ("Cidade *",       "Ex: Florianópolis"),
                ("Estado (UF) *",  "Ex: SC"),
                ("Latitude *",     "Ex: -27.5954"),
                ("Longitude *",    "Ex: -48.5480"),
                ("População *",    "Ex: 516524"),
            };

            var txts = new[] { txtNome = new TextBox(), txtEstado = new TextBox(), txtLatitude = new TextBox(), txtLongitude = new TextBox(), txtPopulacao = new TextBox() };

            int y = 16;
            for (int i = 0; i < fields.Length; i++)
            {
                var lbl = new Label
                {
                    Text = fields[i].Item1,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point(28, y)
                };

                var txt = txts[i];
                txt.Width = 370;
                txt.Height = 34;
                txt.Location = new Point(28, y + 20);
                txt.BackColor = Color.FromArgb(248, 250, 252);
                txt.ForeColor = Color.FromArgb(30, 41, 59);
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.Font = new Font("Segoe UI", 10);

                // Placeholder via GotFocus/LostFocus on Tag
                txt.Tag = fields[i].Item2;
                txt.ForeColor = Color.FromArgb(148, 163, 184);
                txt.Text = fields[i].Item2;
                txt.GotFocus += (s, _) =>
                {
                    var t = (TextBox)s!;
                    if (t.Text == (string)t.Tag!) { t.Text = ""; t.ForeColor = Color.FromArgb(30, 41, 59); }
                };
                txt.LostFocus += (s, _) =>
                {
                    var t = (TextBox)s!;
                    if (string.IsNullOrEmpty(t.Text)) { t.Text = (string)t.Tag!; t.ForeColor = Color.FromArgb(148, 163, 184); }
                };

                pnlBody.Controls.Add(lbl);
                pnlBody.Controls.Add(txt);
                y += 60;
            }

            // Buttons
            var pnlBtns = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(28, 12, 28, 12)
            };
            pnlBtns.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, pnlBtns.Width, 0);

            btnSalvar = new Button
            {
                Text = "💾  Salvar",
                Width = 130,
                Height = 38,
                Location = new Point(pnlBtns.Width - 280, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnSalvar.FlatAppearance.BorderSize = 0;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 110,
                Height = 38,
                Location = new Point(pnlBtns.Width - 136, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnCancelar.FlatAppearance.BorderSize = 1;
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

            pnlBtns.Controls.AddRange(new Control[] { btnSalvar, btnCancelar });

            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlBtns);
            this.Controls.Add(pnlHeader);

            btnSalvar.Click += BtnSalvar_Click;
            btnCancelar.Click += (_, __) => DialogResult = DialogResult.Cancel;
        }

        private string GetFieldValue(TextBox txt)
        {
            string placeholder = (string)(txt.Tag ?? "");
            return txt.Text == placeholder ? "" : txt.Text.Trim();
        }

        private void PreencherFormulario(Cidade c)
        {
            SetField(txtNome, c.Nome);
            SetField(txtEstado, c.Estado);
            SetField(txtLatitude, c.Latitude.ToString());
            SetField(txtLongitude, c.Longitude.ToString());
            SetField(txtPopulacao, c.PopulacaoEstimada.ToString());
        }

        private void SetField(TextBox txt, string val)
        {
            txt.Text = val;
            txt.ForeColor = Color.FromArgb(30, 41, 59);
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                string nome = GetFieldValue(txtNome);
                string estado = GetFieldValue(txtEstado);
                string latStr = GetFieldValue(txtLatitude);
                string lonStr = GetFieldValue(txtLongitude);
                string popStr = GetFieldValue(txtPopulacao);

                if (string.IsNullOrWhiteSpace(nome))
                    throw new Exception("Nome da cidade é obrigatório.");
                if (string.IsNullOrWhiteSpace(estado) || estado.Length > 2)
                    throw new Exception("Estado deve ter a sigla de 2 letras (ex: SP).");
                if (!double.TryParse(latStr.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double lat))
                    throw new Exception("Latitude inválida. Use formato: -23.5505");
                if (!double.TryParse(lonStr.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    throw new Exception("Longitude inválida. Use formato: -46.6333");
                if (!double.TryParse(popStr.Replace(".", "").Replace(",", "."), out double pop) || pop <= 0)
                    throw new Exception("População inválida.");

                Cidade.Nome = nome;
                Cidade.Estado = estado.ToUpper();
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
