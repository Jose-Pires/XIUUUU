using System;
using XIUNetworkingLib;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using RSALib;
using System.Linq;
namespace SiUi
{
    public partial class Form1 : Form
    {
        XIUNetworking Networking;
        int PortUser;
        string Username;
        RSAlib myRsa;

        public Form1()
        {
            InitializeComponent();

            
        }

        private void tabConexão_Click(object sender, EventArgs e)
        {

        }

        private void btnConnectar_Click(object sender, EventArgs e)
        {
            string IpAdress = txtIp.Text;
            int Port = int.Parse(txtPort.Text);
            new Thread(delegate ()
            {
                try
                {
                    btnConnectar.Invoke((MethodInvoker)delegate () { btnConnectar.Enabled = false; });
                    txtPort.Invoke((MethodInvoker)delegate () { txtPort.Enabled = false; });
                    txtIp.Invoke((MethodInvoker)delegate () { txtIp.Enabled = false; });
                    txtDetalhes.Invoke((MethodInvoker)delegate () { txtDetalhes.Text = ""; });
                    lblSatusDetalhe.Invoke((MethodInvoker)delegate{lblSatusDetalhe.Text = "A tentar conectar";});

                    Networking.InitializeConnection(IpAdress, Port, Networking.Entity);

                }
                catch (Exception _e)
                {
                    MessageBox.Show("Erro a conectar ao servidor");

                    btnConnectar.Invoke((MethodInvoker)delegate () {btnConnectar.Enabled = true; });
                    txtPort.Invoke((MethodInvoker)delegate () { txtPort.Enabled = true; });
                    txtIp.Invoke((MethodInvoker)delegate () { txtIp.Enabled = true; });
                    lblSatusDetalhe.Invoke((MethodInvoker)delegate{lblSatusDetalhe.Text = "Não conectado";});
                    txtDetalhes.Invoke((MethodInvoker)delegate{ txtDetalhes.Text = _e.Message;});
                }
            }).Start();
        }

        private void Networking_NewUser(string message)
        {
            MessageBox.Show("new user");
        }

        private void Networking_Connected(ClientEventArgs e)
        {
            lblSatusDetalhe.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                btnConnectar.Invoke((MethodInvoker)delegate () { btnConnectar.Enabled = true; });
                txtPort.Invoke((MethodInvoker)delegate () { txtPort.Enabled = true; });
                txtIp.Invoke((MethodInvoker)delegate () { txtIp.Enabled = true; });
                lblSatusDetalhe.Text = "Conectado";
                lvEntidades.Items.Add(Encoding.ASCII.GetString(e.Data).Split('_')[1]);
            });
            Networking.server.Shutdown();
            //throw new NotImplementedException();
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PortUser = int.Parse(txtPortUser.Text);
            Username = txtNome.Text;

            Networking = new XIUNetworking(Username, PortUser);
            Networking.Connected += Networking_Connected;
            Networking.NewUser += Networking_NewUser;
            Networking.ConnectionLost += Networking_LostConnection;
            txtPort.Enabled = true;
            txtIp.Enabled = true;
            btnConnectar.Enabled = true;
            txtNome.Enabled = false;
            txtPortUser.Enabled = false;
            btnIniciar.Enabled = false;



            myRsa = new RSAlib(Username);
            txtPK.Text = (Convert.ToBase64String(myRsa.PublicKey.Exponent) + "\n" + Convert.ToBase64String(myRsa.PublicKey.Modulus));
            txtSK.Text = (Convert.ToBase64String(myRsa.PrivateKey.D));
        }

        private void Networking_LostConnection(byte[] data, ClientHandler instance, ClientEventArgs e)
        {
            lvEntidades.Items.Remove(lvEntidades.FindItemWithText(instance.Entity));
        }

        private void tabRsa_Click(object sender, EventArgs e)
        {

        }
    }
}
