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
            this.txtNome = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.txtPortUser = new System.Windows.Forms.TextBox();
            this.lblPortUser = new System.Windows.Forms.Label();
            this.btnConnectar = new System.Windows.Forms.Button();
            this.txtDetalhes = new System.Windows.Forms.TextBox();
            this.lblDetalhes = new System.Windows.Forms.Label();
            this.lblEntidades = new System.Windows.Forms.Label();
            this.lvEntidades = new System.Windows.Forms.ListView();
            this.lblSatusDetalhe = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.tabRsa = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPK = new System.Windows.Forms.Label();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.lblMensagem = new System.Windows.Forms.Label();
            this.txtMensagem = new System.Windows.Forms.TextBox();
            this.lblEntidade = new System.Windows.Forms.Label();
            this.cbEntidade = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtPK = new System.Windows.Forms.TextBox();
            this.txtSK = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tabControl1.SuspendLayout();
            this.tabConexão.SuspendLayout();
            this.tabRsa.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConexão);
            this.tabControl1.Controls.Add(this.tabRsa);
            this.tabControl1.Controls.Add(this.tabPage1);
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
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(619, 30);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(214, 20);
            this.txtNome.TabIndex = 15;
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
            // txtPortUser
            // 
            this.txtPortUser.Location = new System.Drawing.Point(150, 31);
            this.txtPortUser.Name = "txtPortUser";
            this.txtPortUser.Size = new System.Drawing.Size(76, 20);
            this.txtPortUser.TabIndex = 12;
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
            // txtDetalhes
            // 
            this.txtDetalhes.Enabled = false;
            this.txtDetalhes.Location = new System.Drawing.Point(147, 161);
            this.txtDetalhes.Multiline = true;
            this.txtDetalhes.Name = "txtDetalhes";
            this.txtDetalhes.Size = new System.Drawing.Size(275, 152);
            this.txtDetalhes.TabIndex = 9;
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
            // lvEntidades
            // 
            this.lvEntidades.Enabled = false;
            this.lvEntidades.Location = new System.Drawing.Point(619, 39);
            this.lvEntidades.Name = "lvEntidades";
            this.lvEntidades.Size = new System.Drawing.Size(214, 274);
            this.lvEntidades.TabIndex = 6;
            this.lvEntidades.UseCompatibleStateImageBehavior = false;
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
            // txtIp
            // 
            this.txtIp.Enabled = false;
            this.txtIp.Location = new System.Drawing.Point(147, 98);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(275, 20);
            this.txtIp.TabIndex = 3;
            // 
            // txtPort
            // 
            this.txtPort.Enabled = false;
            this.txtPort.Location = new System.Drawing.Point(147, 75);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(76, 20);
            this.txtPort.TabIndex = 2;
            // 
            // lblIP
            // 
            this.lblIP.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIP.Location = new System.Drawing.Point(6, 98);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(71, 23);
            this.lblIP.TabIndex = 1;
            this.lblIP.Text = "IP ";
            // 
            // lblPort
            // 
            this.lblPort.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.Location = new System.Drawing.Point(6, 75);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(71, 23);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Porta";
            // 
            // tabRsa
            // 
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
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "Chave Privada";
            // 
            // lblPK
            // 
            this.lblPK.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPK.Location = new System.Drawing.Point(6, 27);
            this.lblPK.Name = "lblPK";
            this.lblPK.Size = new System.Drawing.Size(136, 23);
            this.lblPK.TabIndex = 6;
            this.lblPK.Text = "Chave Publica";
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
            // txtMensagem
            // 
            this.txtMensagem.Location = new System.Drawing.Point(115, 65);
            this.txtMensagem.Name = "txtMensagem";
            this.txtMensagem.Size = new System.Drawing.Size(227, 20);
            this.txtMensagem.TabIndex = 3;
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
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(793, 411);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(793, 411);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(793, 411);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtPK
            // 
            this.txtPK.Location = new System.Drawing.Point(161, 27);
            this.txtPK.Multiline = true;
            this.txtPK.Name = "txtPK";
            this.txtPK.Size = new System.Drawing.Size(406, 94);
            this.txtPK.TabIndex = 8;
            // 
            // txtSK
            // 
            this.txtSK.Location = new System.Drawing.Point(161, 137);
            this.txtSK.Multiline = true;
            this.txtSK.Name = "txtSK";
            this.txtSK.Size = new System.Drawing.Size(406, 109);
            this.txtSK.TabIndex = 9;
            // 
            // groupBox1
            // 
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
            // groupBox2
            // 
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblEntidade);
            this.groupBox3.Controls.Add(this.cbEntidade);
            this.groupBox3.Controls.Add(this.txtMensagem);
            this.groupBox3.Controls.Add(this.lblMensagem);
            this.groupBox3.Controls.Add(this.btnEnviar);
            this.groupBox3.Location = new System.Drawing.Point(9, 35);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(346, 198);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Enviar Segredo";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblPK);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtSK);
            this.groupBox4.Controls.Add(this.txtPK);
            this.groupBox4.Location = new System.Drawing.Point(362, 35);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(584, 266);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Chaves Pessoais";
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
            this.tabRsa.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRsa;
        private System.Windows.Forms.TabPage tabConexão;
        private System.Windows.Forms.TabPage tabPage1;
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
    }
}

