using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VkPlayer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=ID_APP&display=popup&redirect_uri=https://oauth.vk.com/blank.html&scope=audio&response_type=token&v=5.37");
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripStatusLabel1.Text = "Загрузка..."; 
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Загружено";
            try
            {
                string url = webBrowser1.Url.ToString();
                string l = url.Split('#')[1];
                if (l[0] == 'a')
                {
                    Settings1.Default.token = l.Split('&')[0].Split('=')[1];
                    Settings1.Default.id = l.Split('=')[3];
                    Settings1.Default.auth = true;
                    this.Close();
                }
            }
            catch { }

        }
    }
}
