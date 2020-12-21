using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ClientClasses
{

    public partial class ClientForm : Form
    {
        private Client m_client;
        private string m_clientName;
        private int m_option;
        private string m_recieverName;

        public ClientForm(Client client)
        {
            InitializeComponent();
            m_clientName = this.Text;
            m_client = client;
            InputField.Select();

        }
        public void UpdateChatWindow(string message)
        {
            if (MessageWindow.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UpdateChatWindow(message);
                }));
            }
            else
            {
                MessageWindow.Text += message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            m_option = (int)ActionList.SelectedIndex;
            m_clientName = this.Text;
            m_recieverName = (string)ClientList.SelectedItem;
            switch (m_option)
            {
                case 0: // Sends Data for a Global Message
                    m_client.SendData(InputField.Text,m_clientName, m_option);
                    break;
                case 1: // Sends Data for a Private Message
                    m_client.SendData(InputField.Text, m_recieverName, m_option);
                    break;
                case 2: // Sends Data for NickName
                    
                    this.Text = InputField.Text;
                    m_client.SendData(InputField.Text,m_clientName, m_option);
                    break;
                default:
                    break;
            }
            
            
            
            InputField.Clear();
        }

        private void ActionList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.Text != "Unconnected")
                SubmitButton.Text = (string)ActionList.SelectedItem;
            else
            {
                ActionList.SelectedIndex = 2;
                UpdateChatWindow("Choose an initial name first");
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            m_clientName = this.Text;
            if (loginButton.Text == "Login")
            {
                MessageWindow.Clear();
                loginButton.Text = "Disconnect";
                MessageWindow.Visible = true;
                MessageWindow.Enabled = true;
                InputField.Visible = true;
                InputField.Enabled = true;
                ClientList.Visible = true;
                ClientList.Enabled = true;
                SubmitButton.Visible = true;
                SubmitButton.Enabled = true;
                ActionList.Visible = true;
                ActionList.Enabled = true;
                m_client.Login(m_clientName);
                ActionList.SelectedIndex = 2;
                SubmitButton.Text = (string)ActionList.SelectedItem;
            }
            else
            {
                loginButton.Text = "Login";
                MessageWindow.Clear();
                ClientList.Items.Clear();
                MessageWindow.Visible = false;
                MessageWindow.Enabled = false;
                InputField.Visible = false;
                InputField.Enabled = false;
                ClientList.Visible = false;
                ClientList.Enabled = false;
                SubmitButton.Visible = false;
                SubmitButton.Enabled = false;
                ActionList.Visible = false;
                ActionList.Enabled = false;
                m_client.SendData("Disconnected",m_clientName,3);
                this.Text = "Unconnected";
            }
        }
        public void UpdateClientList(string newName, string oldName)
        {
            if (ClientList.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UpdateClientList(newName, oldName);
                }));
            }
            else
            {
                if (ClientList.Items.Contains((object)oldName))
                {
                    ClientList.Items.Remove((object)oldName);
                }
                if (!ClientList.Items.Contains((object)newName))
                {
                    if (newName != "Disconnected" && newName != "Left")
                        ClientList.Items.Add((object)newName);
                }
                
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_client.SendData("Left",this.Text, 3);
        }
    }
}
