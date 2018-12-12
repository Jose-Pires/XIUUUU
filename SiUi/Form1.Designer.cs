namespace SiUi
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConexão = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHelpInicialização = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.lblPortUser = new System.Windows.Forms.Label();
            this.txtPortUser = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnHelpConexão = new System.Windows.Forms.Button();
            this.lvEntidades = new System.Windows.Forms.ListView();
            this.btnConnectar = new System.Windows.Forms.Button();
            this.lblEntidades = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.txtDetalhes = new System.Windows.Forms.TextBox();
            this.lblSatusDetalhe = new System.Windows.Forms.Label();
            this.lblDetalhes = new System.Windows.Forms.Label();
            this.tabRsa = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnHelpRsaMensagens = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnHelpRsaChaves = new System.Windows.Forms.Button();
            this.lblPK = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSK = new System.Windows.Forms.TextBox();
            this.txtPK = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnHelpRsaEnivar = new System.Windows.Forms.Button();
            this.lblEntidade = new System.Windows.Forms.Label();
            this.cbEntidade = new System.Windows.Forms.ComboBox();
            this.txtMensagem = new System.Windows.Forms.TextBox();
            this.lblMensagem = new System.Windows.Forms.Label();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.tabMerkle = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnGerar = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lvMyPuzzles = new System.Windows.Forms.ListView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.tabControl1.SuspendLayout();
            this.tabConexão.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabRsa.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabMerkle.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConexão);
            this.tabControl1.Controls.Add(this.tabRsa);
            this.tabControl1.Controls.Add(this.tabMerkle);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(2, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(950, 495);
            this.tabControl1.TabIndex = 0;
            // 
            // tabConexão
            // 
            this.tabConexão.Controls.Add(this.groupBox2);
            this.tabConexão.Controls.Add(this.groupBox1);
            this.tabConexão.Location = new System.Drawing.Point(4, 22);
            this.tabConexão.Name = "tabConexão";
            this.tabConexão.Padding = new System.Windows.Forms.Padding(3);
            this.tabConexão.Size = new System.Drawing.Size(942, 469);
            this.tabConexão.TabIndex = 0;
            this.tabConexão.Text = "Conexão";
            this.tabConexão.UseVisualStyleBackColor = true;
            this.tabConexão.Click += new System.EventHandler(this.tabConexão_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHelpInicialização);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtNome);
            this.groupBox2.Controls.Add(this.btnIniciar);
            this.groupBox2.Controls.Add(this.lblPortUser);
            this.groupBox2.Controls.Add(this.txtPortUser);
            this.groupBox2.Location = new System.Drawing.Point(8, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(926, 100);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inicialização";
            // 
            // btnHelpInicialização
            // 
            this.btnHelpInicialização.Location = new System.Drawing.Point(897, 19);
            this.btnHelpInicialização.Name = "btnHelpInicialização";
            this.btnHelpInicialização.Size = new System.Drawing.Size(23, 23);
            this.btnHelpInicialização.TabIndex = 16;
            this.btnHelpInicialização.Text = "?";
            this.btnHelpInicialização.UseVisualStyleBackColor = true;
            this.btnHelpInicialização.Click += new System.EventHandler(this.btnHelpInicialização_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(517, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 23);
            this.label3.TabIndex = 14;
            this.label3.Text = "Nome";
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(619, 30);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(214, 20);
            this.txtNome.TabIndex = 15;
            // 
            // btnIniciar
            // 
            this.btnIniciar.Location = new System.Drawing.Point(150, 58);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(75, 23);
            this.btnIniciar.TabIndex = 13;
            this.btnIniciar.Text = "Iniciar Escuta";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblPortUser
            // 
            this.lblPortUser.AutoSize = true;
            this.lblPortUser.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPortUser.Location = new System.Drawing.Point(6, 27);
            this.lblPortUser.Name = "lblPortUser";
            this.lblPortUser.Size = new System.Drawing.Size(121, 23);
            this.lblPortUser.TabIndex = 11;
            this.lblPortUser.Text = "Definir Porta";
            // 
            // txtPortUser
            // 
            this.txtPortUser.Location = new System.Drawing.Point(150, 31);
            this.txtPortUser.Name = "txtPortUser";
            this.txtPortUser.Size = new System.Drawing.Size(76, 20);
            this.txtPortUser.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnHelpConexão);
            this.groupBox1.Controls.Add(this.lvEntidades);
            this.groupBox1.Controls.Add(this.btnConnectar);
            this.groupBox1.Controls.Add(this.lblEntidades);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.lblPort);
            this.groupBox1.Controls.Add(this.lblIP);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.txtIp);
            this.groupBox1.Controls.Add(this.txtDetalhes);
            this.groupBox1.Controls.Add(this.lblSatusDetalhe);
            this.groupBox1.Controls.Add(this.lblDetalhes);
            this.groupBox1.Location = new System.Drawing.Point(8, 122);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(926, 326);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conexão";
            // 
            // btnHelpConexão
            // 
            this.btnHelpConexão.Location = new System.Drawing.Point(897, 19);
            this.btnHelpConexão.Name = "btnHelpConexão";
            this.btnHelpConexão.Size = new System.Drawing.Size(23, 23);
            this.btnHelpConexão.TabIndex = 17;
            this.btnHelpConexão.Text = "?";
            this.btnHelpConexão.UseVisualStyleBackColor = true;
            this.btnHelpConexão.Click += new System.EventHandler(this.btnHelpConexão_Click);
            // 
            // lvEntidades
            // 
            this.lvEntidades.Enabled = false;
            this.lvEntidades.Location = new System.Drawing.Point(619, 39);
            this.lvEntidades.Name = "lvEntidades";
            this.lvEntidades.Size = new System.Drawing.Size(214, 274);
            this.lvEntidades.TabIndex = 6;
            this.lvEntidades.UseCompatibleStateImageBehavior = false;
            // 
            // btnConnectar
            // 
            this.btnConnectar.Enabled = false;
            this.btnConnectar.Location = new System.Drawing.Point(148, 124);
            this.btnConnectar.Name = "btnConnectar";
            this.btnConnectar.Size = new System.Drawing.Size(75, 23);
            this.btnConnectar.TabIndex = 10;
            this.btnConnectar.Text = "Conectar";
            this.btnConnectar.UseVisualStyleBackColor = true;
            this.btnConnectar.Click += new System.EventHandler(this.btnConnectar_Click);
            // 
            // lblEntidades
            // 
            this.lblEntidades.AutoSize = true;
            this.lblEntidades.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntidades.Location = new System.Drawing.Point(517, 39);
            this.lblEntidades.Name = "lblEntidades";
            this.lblEntidades.Size = new System.Drawing.Size(96, 23);
            this.lblEntidades.TabIndex = 7;
            this.lblEntidades.Text = "Entidades";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(6, 39);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(68, 23);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Estado";
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // lblPort
            // 
            this.lblPort.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.Location = new System.Drawing.Point(6, 95);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(71, 23);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Porta";
            // 
            // lblIP
            // 
            this.lblIP.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIP.Location = new System.Drawing.Point(6, 72);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(71, 23);
            this.lblIP.TabIndex = 1;
            this.lblIP.Text = "IP ";
            // 
            // txtPort
            // 
            this.txtPort.Enabled = false;
            this.txtPort.Location = new System.Drawing.Point(147, 95);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(76, 20);
            this.txtPort.TabIndex = 2;
            // 
            // txtIp
            // 
            this.txtIp.Enabled = false;
            this.txtIp.Location = new System.Drawing.Point(147, 72);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(275, 20);
            this.txtIp.TabIndex = 3;
            // 
            // txtDetalhes
            // 
            this.txtDetalhes.Enabled = false;
            this.txtDetalhes.Location = new System.Drawing.Point(147, 161);
            this.txtDetalhes.Multiline = true;
            this.txtDetalhes.Name = "txtDetalhes";
            this.txtDetalhes.Size = new System.Drawing.Size(275, 152);
            this.txtDetalhes.TabIndex = 9;
            // 
            // lblSatusDetalhe
            // 
            this.lblSatusDetalhe.AutoSize = true;
            this.lblSatusDetalhe.Location = new System.Drawing.Point(144, 46);
            this.lblSatusDetalhe.Name = "lblSatusDetalhe";
            this.lblSatusDetalhe.Size = new System.Drawing.Size(79, 13);
            this.lblSatusDetalhe.TabIndex = 5;
            this.lblSatusDetalhe.Text = "Not Connected";
            // 
            // lblDetalhes
            // 
            this.lblDetalhes.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetalhes.Location = new System.Drawing.Point(6, 157);
            this.lblDetalhes.Name = "lblDetalhes";
            this.lblDetalhes.Size = new System.Drawing.Size(88, 23);
            this.lblDetalhes.TabIndex = 8;
            this.lblDetalhes.Text = "Detalhes";
            // 
            // tabRsa
            // 
            this.tabRsa.Controls.Add(this.groupBox5);
            this.tabRsa.Controls.Add(this.groupBox4);
            this.tabRsa.Controls.Add(this.groupBox3);
            this.tabRsa.Location = new System.Drawing.Point(4, 22);
            this.tabRsa.Name = "tabRsa";
            this.tabRsa.Padding = new System.Windows.Forms.Padding(3);
            this.tabRsa.Size = new System.Drawing.Size(942, 469);
            this.tabRsa.TabIndex = 1;
            this.tabRsa.Text = "RSA";
            this.tabRsa.UseVisualStyleBackColor = true;
            this.tabRsa.Click += new System.EventHandler(this.tabRsa_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnHelpRsaMensagens);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.comboBox1);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(9, 205);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(342, 255);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Mensagens Recebidas";
            // 
            // btnHelpRsaMensagens
            // 
            this.btnHelpRsaMensagens.Location = new System.Drawing.Point(313, 10);
            this.btnHelpRsaMensagens.Name = "btnHelpRsaMensagens";
            this.btnHelpRsaMensagens.Size = new System.Drawing.Size(23, 23);
            this.btnHelpRsaMensagens.TabIndex = 17;
            this.btnHelpRsaMensagens.Text = "?";
            this.btnHelpRsaMensagens.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 86);
            this.label4.TabIndex = 9;
            this.label4.Text = "Mensagem Decifrada";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(115, 102);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(221, 147);
            this.textBox1.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(115, 72);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Decifrar";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(115, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(221, 21);
            this.comboBox1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Mensagem";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnHelpRsaChaves);
            this.groupBox4.Controls.Add(this.lblPK);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtSK);
            this.groupBox4.Controls.Add(this.txtPK);
            this.groupBox4.Location = new System.Drawing.Point(362, 22);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(584, 279);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Chaves Pessoais";
            // 
            // btnHelpRsaChaves
            // 
            this.btnHelpRsaChaves.Location = new System.Drawing.Point(551, 9);
            this.btnHelpRsaChaves.Name = "btnHelpRsaChaves";
            this.btnHelpRsaChaves.Size = new System.Drawing.Size(23, 23);
            this.btnHelpRsaChaves.TabIndex = 18;
            this.btnHelpRsaChaves.Text = "?";
            this.btnHelpRsaChaves.UseVisualStyleBackColor = true;
            // 
            // lblPK
            // 
            this.lblPK.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPK.Location = new System.Drawing.Point(13, 40);
            this.lblPK.Name = "lblPK";
            this.lblPK.Size = new System.Drawing.Size(136, 23);
            this.lblPK.TabIndex = 6;
            this.lblPK.Text = "Chave Publica";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "Chave Privada";
            // 
            // txtSK
            // 
            this.txtSK.Location = new System.Drawing.Point(168, 150);
            this.txtSK.Multiline = true;
            this.txtSK.Name = "txtSK";
            this.txtSK.Size = new System.Drawing.Size(406, 109);
            this.txtSK.TabIndex = 9;
            // 
            // txtPK
            // 
            this.txtPK.Location = new System.Drawing.Point(168, 40);
            this.txtPK.Multiline = true;
            this.txtPK.Name = "txtPK";
            this.txtPK.Size = new System.Drawing.Size(406, 94);
            this.txtPK.TabIndex = 8;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnHelpRsaEnivar);
            this.groupBox3.Controls.Add(this.lblEntidade);
            this.groupBox3.Controls.Add(this.cbEntidade);
            this.groupBox3.Controls.Add(this.txtMensagem);
            this.groupBox3.Controls.Add(this.lblMensagem);
            this.groupBox3.Controls.Add(this.btnEnviar);
            this.groupBox3.Location = new System.Drawing.Point(9, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(346, 177);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Enviar Segredo";
            // 
            // btnHelpRsaEnivar
            // 
            this.btnHelpRsaEnivar.Location = new System.Drawing.Point(317, 9);
            this.btnHelpRsaEnivar.Name = "btnHelpRsaEnivar";
            this.btnHelpRsaEnivar.Size = new System.Drawing.Size(23, 23);
            this.btnHelpRsaEnivar.TabIndex = 19;
            this.btnHelpRsaEnivar.Text = "?";
            this.btnHelpRsaEnivar.UseVisualStyleBackColor = true;
            this.btnHelpRsaEnivar.Click += new System.EventHandler(this.btnHelpRsaEnivar_Click);
            // 
            // lblEntidade
            // 
            this.lblEntidade.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntidade.Location = new System.Drawing.Point(6, 27);
            this.lblEntidade.Name = "lblEntidade";
            this.lblEntidade.Size = new System.Drawing.Size(91, 23);
            this.lblEntidade.TabIndex = 2;
            this.lblEntidade.Text = "Entidade";
            // 
            // cbEntidade
            // 
            this.cbEntidade.FormattingEnabled = true;
            this.cbEntidade.Location = new System.Drawing.Point(115, 31);
            this.cbEntidade.Name = "cbEntidade";
            this.cbEntidade.Size = new System.Drawing.Size(121, 21);
            this.cbEntidade.TabIndex = 1;
            // 
            // txtMensagem
            // 
            this.txtMensagem.Location = new System.Drawing.Point(115, 65);
            this.txtMensagem.Name = "txtMensagem";
            this.txtMensagem.Size = new System.Drawing.Size(227, 20);
            this.txtMensagem.TabIndex = 3;
            // 
            // lblMensagem
            // 
            this.lblMensagem.AutoSize = true;
            this.lblMensagem.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensagem.Location = new System.Drawing.Point(6, 61);
            this.lblMensagem.Name = "lblMensagem";
            this.lblMensagem.Size = new System.Drawing.Size(103, 23);
            this.lblMensagem.TabIndex = 4;
            this.lblMensagem.Text = "Mensagem";
            // 
            // btnEnviar
            // 
            this.btnEnviar.Location = new System.Drawing.Point(115, 100);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(75, 23);
            this.btnEnviar.TabIndex = 5;
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.UseVisualStyleBackColor = true;
            // 
            // tabMerkle
            // 
            this.tabMerkle.Controls.Add(this.groupBox6);
            this.tabMerkle.Location = new System.Drawing.Point(4, 22);
            this.tabMerkle.Name = "tabMerkle";
            this.tabMerkle.Size = new System.Drawing.Size(942, 469);
            this.tabMerkle.TabIndex = 2;
            this.tabMerkle.Text = "Merkle";
            this.tabMerkle.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnGerar);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.lvMyPuzzles);
            this.groupBox6.Location = new System.Drawing.Point(4, 18);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(935, 195);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Geração de Puzzles";
            // 
            // btnGerar
            // 
            this.btnGerar.Location = new System.Drawing.Point(10, 166);
            this.btnGerar.Name = "btnGerar";
            this.btnGerar.Size = new System.Drawing.Size(75, 23);
            this.btnGerar.TabIndex = 4;
            this.btnGerar.Text = "Gerar";
            this.btnGerar.UseVisualStyleBackColor = true;
            this.btnGerar.Click += new System.EventHandler(this.btnGerar_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 23);
            this.label5.TabIndex = 3;
            this.label5.Text = "Puzzles";
            // 
            // lvMyPuzzles
            // 
            this.lvMyPuzzles.GridLines = true;
            this.lvMyPuzzles.Location = new System.Drawing.Point(103, 20);
            this.lvMyPuzzles.Name = "lvMyPuzzles";
            this.lvMyPuzzles.Size = new System.Drawing.Size(826, 169);
            this.lvMyPuzzles.TabIndex = 0;
            this.lvMyPuzzles.UseCompatibleStateImageBehavior = false;
            this.lvMyPuzzles.View = System.Windows.Forms.View.List;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(942, 469);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(942, 469);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 506);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabConexão.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabRsa.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabMerkle.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRsa;
        private System.Windows.Forms.TabPage tabConexão;
        private System.Windows.Forms.TabPage tabMerkle;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblSatusDetalhe;
        private System.Windows.Forms.ListView lvEntidades;
        private System.Windows.Forms.Label lblDetalhes;
        private System.Windows.Forms.Label lblEntidades;
        private System.Windows.Forms.TextBox txtDetalhes;
        private System.Windows.Forms.Label lblEntidade;
        private System.Windows.Forms.ComboBox cbEntidade;
        private System.Windows.Forms.Button btnConnectar;
        private System.Windows.Forms.Label lblMensagem;
        private System.Windows.Forms.TextBox txtMensagem;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Label lblPK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.TextBox txtPortUser;
        private System.Windows.Forms.Label lblPortUser;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSK;
        private System.Windows.Forms.TextBox txtPK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnHelpInicialização;
        private System.Windows.Forms.Button btnHelpConexão;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnHelpRsaMensagens;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHelpRsaChaves;
        private System.Windows.Forms.Button btnHelpRsaEnivar;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnGerar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView lvMyPuzzles;
    }
}

