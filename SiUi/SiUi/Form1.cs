using System;
using XIUNetworkingLib;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using RSALib;
using MerklePuzzlesLib;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using TAClientLib;
using System.Numerics;
using DiffieHellmanLib;
using PBKDF2Lib;

namespace SiUi
{
    public partial class Form1 : Form
    {
        StringModels StringModel = new StringModels();
        List<string> EntidadesConectadas = new List<string>();
        XIUNetworking Networking;
        int PortUser;
        string Username;
        RSAlib myRsa;
        Puzzles myPuzzle;
        List<byte[]> MerkleList = new List<byte[]>();
        TAClient taClient;
        DiffieHellman myDiffie;


        public Form1()
        {
            InitializeComponent();


        }

        #region NewUser
        private void Networking_NewUser(string message)
        {
            MessageBox.Show("Estabelecida uma nova ligação","Conexão");
        }
        #endregion

        #region Network Connected
        private void Networking_Connected(ClientEventArgs e)
        {

            lblSatusDetalhe.Invoke((MethodInvoker)delegate { lblSatusDetalhe.Text = "Conectado"; });
            btnConnectar.Invoke((MethodInvoker)delegate () { btnConnectar.Enabled = true; });
            txtPort.Invoke((MethodInvoker)delegate () { txtPort.Enabled = true; });
            txtIp.Invoke((MethodInvoker)delegate () { txtIp.Enabled = true; });

            string entity = Encoding.ASCII.GetString(e.Data).Split('_')[1];
            EntidadesConectadas.Add(entity);
            lvEntidades.Invoke((MethodInvoker)delegate { lvEntidades.Items.Add(entity); });
            cbEntidadeRsa.Invoke((MethodInvoker)delegate { cbEntidadeRsa.Items.Add(entity); });
            cbEntPuzzleEnvi.Invoke((MethodInvoker)delegate { cbEntPuzzleEnvi.Items.Add(entity); });
            cbEntityDH.Invoke((MethodInvoker)delegate { cbEntityDH.Items.Add(entity); });

            byte[] Message = Client.BuildPacket(myRsa.PublicKey, type: 1);
            Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);

            //Networking.server.Shutdown();
            //throw new NotImplementedException();
        }
        #endregion

        #region MessageReceived
        private void Networking_SVMessageReceived(byte[] data, ClientHandler instance, ClientEventArgs e)
        {
            switch (e.PacketType)
            {
                case 1:
                    RSAParameters EntityPK = JsonConvert.DeserializeObject<RSAParameters>(Encoding.ASCII.GetString(e.Data));
                    myRsa.AddEntintyPublicKey(instance.Entity, EntityPK);
                    byte[] Message = Client.BuildPacket(myRsa.PublicKey, type: 1);
                    instance.SendMessage(Message);
                    string entity = instance.Entity;
                    EntidadesConectadas.Add(entity);
                    lvEntidades.Invoke((MethodInvoker)delegate { lvEntidades.Items.Add(entity); });
                    cbEntidadeRsa.Invoke((MethodInvoker)delegate { cbEntidadeRsa.Items.Add(entity); });
                    break;
                case 2:
                    byte[] encrypted = e.Data;
                    string decrypted = myRsa.DecryptToString(encrypted);
                    cbRsaMensagem.Invoke((MethodInvoker)delegate { cbRsaMensagem.Items.Add(new ComboBoxItem { Text = (instance.Entity + " " + DateTime.Now.ToString()), Value = decrypted }); });
                    MessageBox.Show("Recebeu uma mensagem de " + instance.Entity + "\n Usando RSA");
                    break;
                case 3:
                    MerkleList.Add(e.Data);
                    prgMerkleSync.Invoke((MethodInvoker)delegate { prgMerkleSync.Value++; });
                    break;
                case 4:
                    if (Encoding.ASCII.GetString(e.Data).Equals("A Enviar Puzzles"))
                    {
                        MessageBox.Show("A Receber Puzzles de " + instance.Entity,"Merkle Puzzles");
                        lblPuzzlesEnviar.Invoke((MethodInvoker)delegate { lblPuzzlesEnviar.Text = "A Receber Puzzles"; });
                        prgMerkleSync.Invoke((MethodInvoker)delegate { prgMerkleSync.Maximum = 256; prgMerkleSync.Value = 0; });
                    }
                    else
                    {
                        lblPuzzlesEnviar.Invoke((MethodInvoker)delegate { lblPuzzlesEnviar.Text = "Terminado"; });
                        MessageBox.Show("Finalizada a receção dos Puzzles de " + instance.Entity, "Merkle Puzzles");
                        myPuzzle.Conexoes.Add(new Puzzles.PuzzlesConnection { Entity = instance.Entity, MensagemList = MerkleList });
                        cbEntPuzzleEnvi.Invoke((MethodInvoker)delegate { cbEntPuzzleEnvi.Items.Remove(instance.Entity); });
                        cbPuzzleDecifrar.Invoke((MethodInvoker)delegate { cbPuzzleDecifrar.Items.Add(instance.Entity); });
                        MerkleList = new List<byte[]>();
                    }
                    break;
                case 5:
                    string puzzle = Encoding.ASCII.GetString(e.Data);
                    MessageBox.Show("Entidade " + instance.Entity + " escolheu " + puzzle);
                    lvMyPuzzles.Invoke((MethodInvoker)delegate
                    {
                        myPuzzle.Conexoes.Add(new Puzzles.PuzzlesConnection
                        {
                            Entity = instance.Entity,
                            Key = Convert.FromBase64String(lvMyPuzzles.FindItemWithText(puzzle).Text.Split('|')[1])
                        });
                    });
                    cbEntidadesMerkle.Invoke((MethodInvoker)delegate { cbEntidadesMerkle.Items.Add(instance.Entity); });
                    break;
                case 6:
                    byte[] encryptedMessage = e.Data;
                    byte[] decriptedMessage = myPuzzle.Decript(encryptedMessage, myPuzzle.Conexoes.Where(var => var.Entity.Equals(instance.Entity)).FirstOrDefault().Key);
                    string decryptedMerkle = Encoding.UTF8.GetString(decriptedMessage);
                    cbMerkleRecebidas.Invoke((MethodInvoker)delegate { cbMerkleRecebidas.Items.Add(new ComboBoxItem { Text = (instance.Entity + " " + DateTime.Now.ToString()), Value = decryptedMerkle }); });
                    MessageBox.Show("Recebeu uma mensagem de " + instance.Entity + "\n Usando Merkle");
                    break;
                case 7:
                    Random r = new Random();
                    DiffieHellman.PacketHelper EntityDiffie = JsonConvert.DeserializeObject<DiffieHellman.PacketHelper>(Encoding.ASCII.GetString(e.Data));
                    myDiffie = new DiffieHellman();
                    myDiffie.ImportN(BigInteger.Parse(EntityDiffie.N), EntityDiffie.G, EntityDiffie.IV);
                    myDiffie.GenerateA(r.Next(0, 1000000000));
                    myDiffie.GenerateKey(BigInteger.Parse(EntityDiffie.A));
                    myDiffie.Conected.Add(new DiffieHellman.Conections
                    {
                        btKey = myDiffie.btKey,
                        Entity = instance.Entity,
                        IV = myDiffie.IV,
                        strKey = myDiffie.strKey
                    });

                    string entityaux = instance.Entity;
                    DiffieHellman.PacketHelper packet = new DiffieHellman.PacketHelper
                    {
                        A = myDiffie.A.ToString(),
                        G = myDiffie.G,
                        N = myDiffie.N.ToString(),
                        IV = myDiffie.IV
                    };

                    byte[] Messageaux = Client.BuildPacket(packet, type: 8);

                    if (Networking.server.clientHandlers.Where(var => var.Key == entityaux).Count() > 0)
                    {
                        Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entityaux).Value.SendMessage(Messageaux);
                    }
                    else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entityaux).Count() > 0)
                    {
                        Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entityaux).ClientNetworking.SendRequest(Messageaux);
                    }
                    cbDHEntidadeEnviar.Invoke((MethodInvoker)delegate { cbDHEntidadeEnviar.Items.Add(instance.Entity); });

                    break;
                case 8:
                    DiffieHellman.PacketHelper EntityDiffieAux = JsonConvert.DeserializeObject<DiffieHellman.PacketHelper>(Encoding.ASCII.GetString(e.Data));
                    myDiffie.ImportN(BigInteger.Parse(EntityDiffieAux.N), EntityDiffieAux.G, EntityDiffieAux.IV);
                    myDiffie.GenerateKey(BigInteger.Parse(EntityDiffieAux.A));

                    myDiffie.Conected.Add(new DiffieHellman.Conections
                    {
                        btKey = myDiffie.btKey,
                        Entity = instance.Entity,
                        IV = myDiffie.IV,
                        strKey = myDiffie.strKey
                    });

                    MessageBox.Show("Gerada Chave DiffieHellman com " + instance.Entity);
                    cbDHEntidadeEnviar.Invoke((MethodInvoker)delegate { cbDHEntidadeEnviar.Items.Add(instance.Entity); });
                    break;
                case 9:
                    byte[] DHEncryptedMessage = e.Data;
                    DiffieHellman.Conections DhConection = myDiffie.Conected.Where(var => var.Entity.Equals(instance.Entity)).FirstOrDefault();
                    byte[] DHDecryptedMessage = myDiffie.Decrypt(DHEncryptedMessage, DhConection.btKey, DhConection.IV);
                    cbDhReceived.Invoke((MethodInvoker)delegate { cbDhReceived.Items.Add(new ComboBoxItem { Text = (instance.Entity + " " + DateTime.Now.ToString()), Value = Encoding.UTF8.GetString(DHDecryptedMessage) }); });
                    MessageBox.Show("Recebeu uma mensagem de " + instance.Entity + "\n Usando DiffieHellman");
                    break;
            }
        }

        private void Networking_MessageReceived(byte[] data, ClientInstance instance, ClientEventArgs e)
        {
            switch (e.PacketType)
            {
                case 1:
                    RSAParameters EntityPK = JsonConvert.DeserializeObject<RSAParameters>(Encoding.ASCII.GetString(e.Data));
                    myRsa.AddEntintyPublicKey(instance.RemoteEntity, EntityPK);
                    break;
                case 2:
                    byte[] encrypted = e.Data;
                    string decrypted = myRsa.DecryptToString(encrypted);
                    cbRsaMensagem.Invoke((MethodInvoker)delegate { cbRsaMensagem.Items.Add(new ComboBoxItem { Text = (instance.RemoteEntity + " " + DateTime.Now.ToString()), Value = decrypted }); });
                    MessageBox.Show("Recebeu uma mensagem de " + instance.RemoteEntity + "\n Usando RSA");
                    break;
                case 3:
                    MerkleList.Add(e.Data);
                    prgMerkleSync.Invoke((MethodInvoker)delegate { prgMerkleSync.Value++; });
                    break;
                case 4:
                    if (Encoding.ASCII.GetString(e.Data).Equals("A Enviar Puzzles"))
                    {
                        MessageBox.Show("A Receber Puzzles de " + instance.RemoteEntity, "Merkle Puzzles");
                        lblPuzzlesEnviar.Invoke((MethodInvoker)delegate { lblPuzzlesEnviar.Text = "A Receber Puzzles"; });
                        prgMerkleSync.Invoke((MethodInvoker)delegate { prgMerkleSync.Maximum = 256; prgMerkleSync.Value = 0; });
                    }
                    else
                    {
                        lblPuzzlesEnviar.Invoke((MethodInvoker)delegate { lblPuzzlesEnviar.Text = "Terminado"; });
                        MessageBox.Show("Finalizada a receção dos Puzzles de " + instance.RemoteEntity, "Merkle Puzzles");
                        myPuzzle.Conexoes.Add(new Puzzles.PuzzlesConnection { Entity = instance.RemoteEntity, MensagemList = MerkleList });
                        cbEntPuzzleEnvi.Invoke((MethodInvoker)delegate { cbEntPuzzleEnvi.Items.Remove(instance.RemoteEntity); });
                        cbPuzzleDecifrar.Invoke((MethodInvoker)delegate { cbPuzzleDecifrar.Items.Add(instance.RemoteEntity); });
                    }
                    break;
                case 5:
                    string puzzle = Encoding.ASCII.GetString(e.Data);
                    MessageBox.Show("Entidade " + instance.RemoteEntity + " escolheu " + puzzle);
                    lvMyPuzzles.Invoke((MethodInvoker)delegate
                    {
                        myPuzzle.Conexoes.Add(new Puzzles.PuzzlesConnection
                        {
                            Entity = instance.RemoteEntity,
                            Key = Convert.FromBase64String(lvMyPuzzles.FindItemWithText(puzzle).Text.Split('|')[1])
                        });
                    });
                    cbEntidadesMerkle.Invoke((MethodInvoker)delegate { cbEntidadesMerkle.Items.Add(instance.RemoteEntity); });
                    break;
                case 6:
                    byte[] encryptedMessage = e.Data;
                    byte[] decriptedMessage = myPuzzle.Decript(encryptedMessage, myPuzzle.Conexoes.Where(var => var.Entity.Equals(instance.RemoteEntity)).FirstOrDefault().Key);
                    string decryptedMerkle = Encoding.UTF8.GetString(decriptedMessage);
                    cbMerkleRecebidas.Invoke((MethodInvoker)delegate { cbMerkleRecebidas.Items.Add(new ComboBoxItem { Text = (instance.RemoteEntity + " " + DateTime.Now.ToString()), Value = decryptedMerkle }); });
                    MessageBox.Show("Recebeu uma mensagem de " + instance.RemoteEntity + "\n Usando Merkle");
                    break;
                case 7:
                    Random r = new Random();
                    DiffieHellman.PacketHelper EntityDiffie = JsonConvert.DeserializeObject<DiffieHellman.PacketHelper>(Encoding.ASCII.GetString(e.Data));
                    myDiffie = new DiffieHellman();
                    myDiffie.ImportN(BigInteger.Parse(EntityDiffie.N), EntityDiffie.G, EntityDiffie.IV);
                    myDiffie.GenerateA(r.Next(0, 1000000000));
                    myDiffie.GenerateKey(BigInteger.Parse(EntityDiffie.A));
                    myDiffie.Conected.Add(new DiffieHellman.Conections
                    {
                        btKey = myDiffie.btKey,
                        Entity = instance.RemoteEntity,
                        IV = myDiffie.IV,
                        strKey = myDiffie.strKey
                    });

                    string entity = instance.RemoteEntity;
                    DiffieHellman.PacketHelper packet = new DiffieHellman.PacketHelper
                    {
                        A = myDiffie.A.ToString(),
                        G = myDiffie.G,
                        N = myDiffie.N.ToString(),
                        IV = myDiffie.IV
                    };

                    byte[] Message = Client.BuildPacket(packet, type: 8);

                    if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
                    {
                        Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
                    }
                    else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
                    {
                        Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
                    }

                    cbDHEntidadeEnviar.Invoke((MethodInvoker)delegate { cbDHEntidadeEnviar.Items.Add(instance.RemoteEntity); });
                    break;
                case 8:
                    DiffieHellman.PacketHelper EntityDiffieAux = JsonConvert.DeserializeObject<DiffieHellman.PacketHelper>(Encoding.ASCII.GetString(e.Data));
                    myDiffie.ImportN(BigInteger.Parse(EntityDiffieAux.N), EntityDiffieAux.G, EntityDiffieAux.IV);
                    myDiffie.GenerateKey(BigInteger.Parse(EntityDiffieAux.A));

                    myDiffie.Conected.Add(new DiffieHellman.Conections
                    {
                        btKey = myDiffie.btKey,
                        Entity = instance.RemoteEntity,
                        IV = myDiffie.IV,
                        strKey = myDiffie.strKey
                    });
                    cbDHEntidadeEnviar.Invoke((MethodInvoker)delegate { cbDHEntidadeEnviar.Items.Add(instance.RemoteEntity); });
                    MessageBox.Show("Gerada Chave DiffieHellman com " + instance.RemoteEntity);
                    break;
                case 9:
                    byte[] DHEncryptedMessage = e.Data;
                    DiffieHellman.Conections DhConection = myDiffie.Conected.Where(var => var.Entity.Equals(instance.RemoteEntity)).FirstOrDefault();
                    byte [] DHDecryptedMessage = myDiffie.Decrypt(DHEncryptedMessage,DhConection.btKey,DhConection.IV);
                    cbDhReceived.Invoke((MethodInvoker)delegate { cbDhReceived.Items.Add(new ComboBoxItem { Text = (instance.RemoteEntity + " " + DateTime.Now.ToString()), Value = Encoding.UTF8.GetString(DHDecryptedMessage) }); });
                    MessageBox.Show("Recebeu uma mensagem de " + instance.RemoteEntity + "\n Usando DiffieHellman");
                    break;

            }
        }
        #endregion

        #region ConnectionLost
        private void Networking_LostConnection(byte[] data, ClientHandler instance, ClientEventArgs e)
        {
            EntidadesConectadas = EntidadesConectadas.Where(val => !val.Contains(instance.Entity)).ToList();
            lvEntidades.Invoke((MethodInvoker)delegate { lvEntidades.Items.Remove(lvEntidades.FindItemWithText(instance.Entity)); });
            cbEntidadeRsa.Invoke((MethodInvoker)delegate { cbEntidadeRsa.Items.Remove(instance.Entity); });
        }

        private void Networking_ClConnectionLost(byte[] data, ClientInstance instance, ClientEventArgs e)
        {
            EntidadesConectadas = EntidadesConectadas.Where(val => !val.Contains(instance.RemoteEntity)).ToList();
            lvEntidades.Invoke((MethodInvoker)delegate { lvEntidades.Items.Remove(lvEntidades.FindItemWithText(instance.RemoteEntity)); });
            cbEntidadeRsa.Invoke((MethodInvoker)delegate { cbEntidadeRsa.Items.Remove(instance.RemoteEntity); });
        }
        #endregion

        #region Botões Conexão

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            btnGerar_Click(null, null);
            Username = txtNome.Text;

            if (string.IsNullOrEmpty(txtPortUser.Text) || string.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Existem dados por preencher", "Alerta");
            }
            else
            {
                try
                {
                    PortUser = int.Parse(txtPortUser.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Por favor introduza um número no Port", "Alerta");
                    return;
                }

                Networking = new XIUNetworking(Username, PortUser);
                Networking.Connected += Networking_Connected;
                Networking.NewUser += Networking_NewUser;
                Networking.ConnectionLost += Networking_LostConnection;
                Networking.CLConnectionLost += Networking_ClConnectionLost;
                Networking.MessageReceived += Networking_MessageReceived;
                Networking.SVMessageReceived += Networking_SVMessageReceived;

                txtPort.Enabled = true;
                txtIp.Enabled = true;
                btnConnectar.Enabled = true;
                grpRsaEnviar.Enabled = true;
                grpRsaReceber.Enabled = true;
                grpGerarPuzzles.Enabled = true;
                grpDHGerar.Enabled = true;
                txtNome.Enabled = false;
                txtPortUser.Enabled = false;
                btnIniciar.Enabled = false;


                myRsa = new RSAlib(Username);
                txtPK.Text = (Convert.ToBase64String(myRsa.PublicKey.Exponent) + "\n" + Convert.ToBase64String(myRsa.PublicKey.Modulus));
                txtSK.Text = (Convert.ToBase64String(myRsa.PrivateKey.D));
            }
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
                    lblSatusDetalhe.Invoke((MethodInvoker)delegate { lblSatusDetalhe.Text = "A tentar conectar"; });

                    Networking.InitializeConnection(IpAdress, Port, Networking.Entity);

                }
                catch (Exception _e)
                {
                    MessageBox.Show("Erro a conectar ao servidor");

                    btnConnectar.Invoke((MethodInvoker)delegate () { btnConnectar.Enabled = true; });
                    txtPort.Invoke((MethodInvoker)delegate () { txtPort.Enabled = true; });
                    txtIp.Invoke((MethodInvoker)delegate () { txtIp.Enabled = true; });
                    lblSatusDetalhe.Invoke((MethodInvoker)delegate { lblSatusDetalhe.Text = "Não conectado"; });
                    txtDetalhes.Invoke((MethodInvoker)delegate { txtDetalhes.Text = _e.Message; });
                }
            }).Start();
        }

        #endregion

        #region Botões RSA
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            string mensagem = txtMensagemRsa.Text;
            txtMensagemRsa.Text = "";
            string entity = cbEntidadeRsa.SelectedItem.ToString();
            byte[] encrypted = myRsa.EncryptToByte(mensagem, entity);

            byte[] Message = Client.BuildPacket(encrypted, type: 2);
            if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
            {
                Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
            }
            else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
            {
                Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
            }
            else
            {

            }
        }

        private void RsaDecifrar_Click(object sender, EventArgs e)
        {
            txtRsaMensagemDec.Text = "";
            ComboBoxItem item = (ComboBoxItem)cbRsaMensagem.SelectedItem;
            txtRsaMensagemDec.Text = item.Value;

        }
        #endregion

        #region Botões Puzzles
        private void btnGerar_Click(object sender, EventArgs e)
        {
            lvMyPuzzles.Clear();

            myPuzzle = new Puzzles();
            myPuzzle.GerarListaDeMensagens();
            myPuzzle.GerarListaDeChaves();

            foreach (var mensagem in myPuzzle.listaMes)
            {
                lvMyPuzzles.Items.Add(Encoding.UTF8.GetString(mensagem));
            }

            grpEnviarPuzzles.Enabled = true;
            grpObterPuzzles.Enabled = true;
            grpPuzzlesEnviar.Enabled = true;
            grpMerkleMensagensRecebidas.Enabled = true;
            
        }

        private void btnPuzzleDecifrar_Click(object sender, EventArgs e)
        {
            string entity = cbPuzzleDecifrar.SelectedItem.ToString();
            List<byte[]> ListaMensagens = myPuzzle.Conexoes.Where(var => var.Entity.Equals(entity)).FirstOrDefault().MensagemList;
            byte[] mensagemEscolhida = myPuzzle.EscolherMensagemCifradaAleatoria(ListaMensagens);
            lblPuzzlesEnviar.Text = "A Decifrar";
            string mensagem = mensagem = myPuzzle.DecifrarMensagemEscolhida(mensagemEscolhida);

            lblPuzzlesEnviar.Text = "Terminado";
            txtPuzzleDecifrado.Text = mensagem.Split('|')[0];
            txtPuzzleKeyDecifrada.Text = mensagem.Split('|')[1];
        }

        private void btnObter_Click(object sender, EventArgs e)
        {
            if (lvMyPuzzles.Items.Count <= 0)
            {
                MessageBox.Show("Puzzles não gerados, por favor gere os puzzles primeiro", "Alerta");
                return;
            }
            else
            {
                int Puzzle = 0;
                try
                {
                    Puzzle = int.Parse(txtPuzzleObter.Text);
                    if (Puzzle == 0)
                    {
                        MessageBox.Show("Por favor introduza um número diferente de 0", "Alerta");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Numero Invalido", "Alerta");
                    return;
                }
                if (Puzzle == 0)
                {
                    MessageBox.Show("Por favor introduza um número", "Alerta");
                    return;
                }
                else
                {
                    var x = lvMyPuzzles.FindItemWithText("Puzzle" + Puzzle.ToString() + "|");
                    if (x != null)
                    {
                        txtChaveObter.Text = x.Text.Split('|')[1];
                    }
                    else
                    {
                        MessageBox.Show("Puzzle não existe", "Alerta");
                        return;
                    }
                }
            }
        }

        private void btnEnviarPuzzles_Click(object sender, EventArgs e)
        {
            string entity = cbEntPuzzleEnvi.SelectedItem.ToString();

            List<byte[]> ListaCifrada = myPuzzle.CifrarListaMensagens(myPuzzle.listaMes, myPuzzle.listaPi);
            byte[] Message = Client.BuildPacket(Encoding.ASCII.GetBytes("A Enviar Puzzles"), type: 4);
            lblPuzzlesEnviar.Text = "A Enviar Puzzles";
            if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
            {
                Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
            }
            else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
            {
                Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
            }

            foreach (var Cifrado in ListaCifrada)
            {
                byte[] Message2 = Client.BuildPacket(Cifrado, type: 3);
                if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
                {
                    Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message2);
                }
                else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
                {
                    Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message2);
                }
                Thread.Sleep(10);
            }

            Message = Client.BuildPacket(Encoding.ASCII.GetBytes("Terminado"), type: 4);
            lblPuzzlesEnviar.Text = "Terminado";
            if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
            {
                Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
            }
            else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
            {
                Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPuzzleKeyDecifrada.Text) || string.IsNullOrEmpty(txtPuzzleDecifrado.Text))
            {
                MessageBox.Show("Existem campos por preencher", "Alerta");
            }
            else
            {
                string entity = cbPuzzleDecifrar.SelectedItem.ToString();
                myPuzzle.Conexoes.Where(var => var.Entity.Equals(entity)).FirstOrDefault().Key = Convert.FromBase64String(txtPuzzleKeyDecifrada.Text);
                byte[] Message = Client.BuildPacket(Encoding.ASCII.GetBytes(txtPuzzleDecifrado.Text), type: 5);
                if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
                {
                    Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
                    cbEntidadesMerkle.Invoke((MethodInvoker)delegate { cbEntidadesMerkle.Items.Add(entity); });
                }
                else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
                {
                    Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
                    cbEntidadesMerkle.Invoke((MethodInvoker)delegate { cbEntidadesMerkle.Items.Add(entity); });
                }
            }

        }


        private void btnPuzzleEnviarMensagem_Click(object sender, EventArgs e)
        {
            string entity = cbEntidadesMerkle.SelectedItem.ToString();
            byte[] encryptedMessage = Encoding.UTF8.GetBytes(txtMensagemMerkle.Text);
            byte[] encryptedMerkle = myPuzzle.Encript(encryptedMessage, myPuzzle.Conexoes.Where(var => var.Entity.Equals(entity)).FirstOrDefault().Key);
            byte[] Message = Client.BuildPacket(encryptedMerkle, type: 6);

            if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
            {
                Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
            }
            else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
            {
                Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
            }


        }

        private void btnDecifrarMerkleMessage_Click(object sender, EventArgs e)
        {
            txtMerkleMessage.Text = "";
            ComboBoxItem item = (ComboBoxItem)cbMerkleRecebidas.SelectedItem;
            txtMerkleMessage.Text = item.Value;
        }
        #endregion

        #region DiffieHellman

        private void btnDHMensEnv_Click(object sender, EventArgs e)
        {
            string entity = cbDHEntidadeEnviar.SelectedItem.ToString();
            byte[] message = Encoding.UTF8.GetBytes(txtDHMensagemEnv.Text);
            DiffieHellman.Conections conection = myDiffie.Conected.Where(var => var.Entity.Equals(entity)).FirstOrDefault();
            byte[] encryptedMessage = myDiffie.Encrypt(message, conection.btKey, conection.IV);

            byte[] Message = Client.BuildPacket(encryptedMessage, type: 9);

            if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
            {
                Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
            }
            else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
            {
                Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
            }
        }

        private void btDhDecifrarMessage_Click(object sender, EventArgs e)
        {
            txtDhDeciferMessage.Text = "";
            ComboBoxItem item = (ComboBoxItem)cbDhReceived.SelectedItem;
            txtDhDeciferMessage.Text = item.Value;
        }

        private void btnDHGerar_Click(object sender, EventArgs e)
        {
            myDiffie = new DiffieHellman();
            txtDHN.Text = myDiffie.N.ToString();
            txtDHG.Text = myDiffie.G.ToString();

            grpDHEnviar.Enabled = true;
            grpDHReceber.Enabled = true;
            grpDHCalcularChave.Enabled = true;
        }

        private void btnDHEstabelecer_Click(object sender, EventArgs e)
        {
            string entity = cbEntityDH.SelectedItem.ToString();
            myDiffie.N = BigInteger.Parse(txtDHN.Text);
            myDiffie.G = int.Parse(txtDHG.Text);
            myDiffie.GenerateA(BigInteger.Parse(txtDHX.Text));

            DiffieHellman.PacketHelper packet = new DiffieHellman.PacketHelper
            {
                A = myDiffie.A.ToString(),
                G = myDiffie.G,
                N = myDiffie.N.ToString(),
                IV = myDiffie.IV
            };

            byte[] Message = Client.BuildPacket(packet, type: 7);

            if (Networking.server.clientHandlers.Where(var => var.Key == entity).Count() > 0)
            {
                Networking.server.clientHandlers.FirstOrDefault(var => var.Key == entity).Value.SendMessage(Message);
            }
            else if (Networking.ClientInstances.Where(var => var.RemoteEntity == entity).Count() > 0)
            {
                Networking.ClientInstances.FirstOrDefault(var => var.RemoteEntity == entity).ClientNetworking.SendRequest(Message);
            }


        }


        #endregion

        #region HelpButtons
        private void btnHelpInicialização_Click(object sender, EventArgs e)
        {
            MessageBox.Show(StringModel.HpConexãoInicio, "Ajuda Inicialização");
        }

        private void btnHelpConexão_Click(object sender, EventArgs e)
        {
            MessageBox.Show(StringModel.HpConexãoConexao, "Ajuda Conexão");
        }

        private void btnHelpRsaEnivar_Click(object sender, EventArgs e)
        {
            MessageBox.Show(StringModel.HpRsaMensagem, "Ajuda RSA Mensagem");
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Networking.server.Shutdown();
                myRsa.DeleteAllFromContainer();
            }
            catch
            {

            }
        }
        #region botões Ta
        private void btnTaLigar_Click(object sender, EventArgs e)
        {
            try
            {
                taClient = new TAClient(Username, txtTaKey.Text);

                taClient.EntityListReceived += TaClient_EntityListReceived;
                taClient.Disconnected += TaClient_Disconnected;
                taClient.KeyReceived += TaClient_KeyReceived;
                taClient.TimeMissmatch += TaClient_TimeMissmatch;
                taClient.InvalidCommand += TaClient_InvalidCommand;
                taClient.ErrorConnecting += TaClient_ErrorConnecting; //:D
                taClient.EntityNotFound += TaClient_EntityNotFound;
                taClient.ClientRejected += TaClient_ClientRejected;
                taClient.InvalidHMAC += TaClient_InvalidHMAC;
                taClient.Connected += TaClient_Connected;
                taClient.Kicked += TaClient_Kicked;
                taClient.AttemptConnect(txtTaIp.Text, int.Parse(txtTaPort.Text));

            }
            catch (InvalidKeyException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ClientRejectedException ex)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro :/ ");
            }
        }
        #endregion

        #region Eventos Ta
        private void TaClient_KeyReceived(byte[] key, System.Net.IPAddress remoteIP, int remotePORT)
        {
            throw new NotImplementedException();
        }

        private void TaClient_EntityListReceived(List<(string, string)> e)
        {
            foreach (var entity in e) {
                lvTaEntidades.Invoke((MethodInvoker)delegate { lvTaEntidades.Items.Add(entity.Item1 + " " + entity.Item2); });
            }
        }

        private void TaClient_Disconnected()
        {
            MessageBox.Show("Agente de Confiança não é de confiança para se manter online por isso está offline");
        }

        private void TaClient_Kicked()
        {
            throw new NotImplementedException();
        }

        private void TaClient_Connected()
        {
            MessageBox.Show("Ligação bem sucedida");
        }

        private void TaClient_InvalidHMAC(ServerCommandEventArgs e, string originalHMAC = "", string computedHMAC = "")
        {
            throw new NotImplementedException();
        }

        private void TaClient_ClientRejected(ServerCommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TaClient_EntityNotFound(ServerCommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TaClient_ErrorConnecting(string Message)
        {
            throw new NotImplementedException();
        }

        private void TaClient_InvalidCommand(string Message)
        {
            throw new NotImplementedException();
        }

        private void TaClient_TimeMissmatch(string Message)
        {
            throw new NotImplementedException();
        }
        #endregion

        

        private void btnTaObterEnt_Click(object sender, EventArgs e)
        {
            taClient.RequestConnectedEntities();
        }

        private void btnNegocios_Click(object sender, EventArgs e)
        {
            foreach ( var item in lvTaEntidades.SelectedItems)
            {
                taClient.RequestKey(item.ToString(),PortUser.ToString());
            }
        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void btnPBKDF2_Click(object sender, EventArgs e)
        {
            string Password = txtPbkPassw.Text;
            string Iterations = txtIterations.Text;
            string Size = txtSize.Text;
            int SaltSize = int.Parse(txtSaltSize.Text);


            byte[] Salt = PBKDF2.GerarSal(SaltSize);
            byte[] Cifer = PBKDF2.Derivar(PBKDF2.StringToByte(Password), Salt, int.Parse(Iterations), int.Parse(Size));
            txtPbkCifra.Text = Encoding.ASCII.GetString(Cifer);

        }
    }
}
