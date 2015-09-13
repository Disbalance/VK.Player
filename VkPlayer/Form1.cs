using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace VkPlayer
{
    public partial class Form1 : Form
    {
        public bool loop = false;
        public bool random = false;
        public List<Audio> audioList;
        WMPLib.IWMPPlaylist PlayList;
        WMPLib.IWMPMedia Media;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetAsyncKeyState(int vkey);


        public Form1()
        {
            InitializeComponent();
            MethodInvoker mi = new MethodInvoker(WaitKey);
            mi.BeginInvoke(null, null);
        }

        private void WaitKey()
        {
            while (this.IsHandleCreated)
            {
                short res1 = GetAsyncKeyState(VK_HOME);
                short res2 = GetAsyncKeyState(VK_END);
                short res3 = GetAsyncKeyState(VK_NUM_PLUS);
                short res4 = GetAsyncKeyState(VK_NUM_MINUS);

                if (res1 != 0)
                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                if (res2 != 0)
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                if (res3 != 0)
                    axWindowsMediaPlayer1.Ctlcontrols.next();
                if (res4 != 0)
                    axWindowsMediaPlayer1.Ctlcontrols.previous();
            }
        }

        public const int VK_NUM_PLUS = 0x6B;
        public const int VK_NUM_MINUS = 0x6D;
        public const int VK_HOME = 0x24;
        public const int VK_END = 0x23;

        private void Form1_Load(object sender, EventArgs e)
        {
            new Form2().Show();
            backgroundWorker1.RunWorkerAsync();
        }

        public class Audio
        {
            public int aid { get; set; }
            public int ower_id { get; set; }
            public string artist { get; set; }
            public string title { get; set; }
            public int duration { get; set; }
            public string url { get; set; }
            public string lurics_id { get; set; }
            public int genre { get; set; }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth) { Thread.Sleep(500); }

            WebRequest request =
               WebRequest.Create("https://api.vk.com/method/audio.get?ower_id" + Settings1.Default.id + "&need_user=0&access_token=" + Settings1.Default.token);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

            JToken token = JToken.Parse(responseFromServer);
            audioList = Enumerable.Skip(token["response"].Children(), 0).Select(c => c.ToObject<Audio>()).ToList();

            this.Invoke((MethodInvoker)delegate
            {
                PlayList = axWindowsMediaPlayer1.playlistCollection.newPlaylist("vkPlayList");
                for (int i = 0; i < audioList.Count(); i++)
                {
                    Media = axWindowsMediaPlayer1.newMedia(audioList[i].url);
                    PlayList.appendItem(Media);
                    listBox1.Items.Add(audioList[i].artist + " - " + audioList[i].title);
                }
                axWindowsMediaPlayer1.currentPlaylist = PlayList;
                axWindowsMediaPlayer1.settings.setMode("loop", false);
                axWindowsMediaPlayer1.settings.setMode("shuffle", false);
                axWindowsMediaPlayer1.Ctlcontrols.stop();
            });
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.Ctlcontrols.currentItem = axWindowsMediaPlayer1.currentPlaylist.get_Item(listBox1.SelectedIndex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (random)
            {
                random = false;
                button2.Text = "Случайно:ВЫКЛ";
                axWindowsMediaPlayer1.settings.setMode("shuffle", false);

            }
            else
            {
                random = true;
                button2.Text = "Случайно:ВКЛ";
                axWindowsMediaPlayer1.settings.setMode("shuffle", true);
            }
        }

    }
}
