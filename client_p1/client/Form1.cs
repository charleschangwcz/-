using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace client
{
    /*
     * .InvokeIfRequired(() =>
                {
                });
     * */
    public partial class Form1 : Form
    {
        /*
         * p(操作棋子) + 0/1(黑/白) + w/t/n(有人贏了/平手/還沒贏) + str(棋子id)
         * m(傳訊息) + str(訊息內容)
         * r(遊戲重開)
         */
        //擴充方法

        Socket client;//連接至server的socket
        IPAddress localAddr = IPAddress.Parse("127.0.0.1"); // 設定 IP172.25.94.122
        PictureBox[] pieces = new PictureBox[225];
        int[] xfixed = new int[15] { 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 2, 3, 3, 3, 3 };//橫行位置誤差
        int[] yfixed = new int[15] { 0, 0, 1, 1, 3, 3, 4, 6, 7, 7, 9, 10, 10, 11, 11 };//直行位置誤差
        int piece_size = 40, origin_x = 3, origin_y = 3;
        string left = "m[對手已離開對戰]";
        string piece_color = "0", player = "匿名使用者";
        string NameHint = "輸入使用者名稱...", MesHint = "輸入訊息...", HistoryHint = "[聊天歷史紀錄]";
        bool turn, re_rq, nameHasText = false, mesHasText = false, set_success = true;
        int port = 8000;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            init_piece();
        }
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "確定退出？", "退出視窗通知", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(left);// send data buffer
                client.Send(buffer, buffer.Length, SocketFlags.None);
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                client = null;
            }
            else
            {
                //Cancel 取得或設定數值，表示是否應該取消事件。
                e.Cancel = true;
            }
        }
        void init_piece()//初始化棋子
        {
            black_piece.Visible = false;
            white_piece.Visible = false;
            status.Visible = false;
            re_label.Visible = false;
            restart_btn.Visible = false;
            m_history.Visible = false;
            m_input.Visible = false;
            m_send_btn.Visible = false;
            name_label.Visible = false;
            m_history.ScrollBars = ScrollBars.Vertical;
            int x, y;
            for (int i = 0; i < 225; ++i)
            {
                x = origin_x + (i % 15) * piece_size + xfixed[i % 15];
                y = origin_y + (i / 15) * piece_size + yfixed[i / 15];
                pieces[i] = new PictureBox();
                pieces[i].Location = new Point(x, y);     // 設定位置
                pieces[i].Size = new Size(piece_size, piece_size);         // 設定寬高
                pieces[i].Name = i.ToString();         // 設定棋子名稱
                pieces[i].BackColor = Color.Transparent;
                //pieces[pid].BackgroundImage = Properties.Resources.white_piece;    // 讀取圖檔，顯示在pictureBox
                pieces[i].BackgroundImageLayout = ImageLayout.Stretch; //設定圖片layout
                pieces[i].Tag = "piece";
                this.Controls.Add(pieces[i]);  //把 PictureBox 加進 form 顯示出來.
            }
        }
        void set_chatroom()
        {
            m_history.InvokeIfRequired(() =>
            {
                m_history.Text = HistoryHint;
            });
            m_history.InvokeIfRequired(() =>
            {
                m_history.Visible = true;
            });
            m_input.InvokeIfRequired(() =>
            {
                m_input.Visible = true;
            });
            m_send_btn.InvokeIfRequired(() =>
            {
                m_send_btn.Visible = true;
            });
        }
        void set_piece()//設置棋子
        {
            for (int i = 0; i < 225; ++i)
            {
                pieces[i].Click += new EventHandler(piece_Click); // 設定所有的PictureBox的Click事件都呼叫相同的事件函式
                pieces[i].BackgroundImage = null;
                //this.Controls.Add(pieces[i]);  //把 PictureBox 加進 form 顯示出來.
            }
            if (piece_color[0] == '0')//黑方先攻
            {
                turn = true;
                //使用lambda
                status.InvokeIfRequired(() =>
                {
                    status.Visible = true;
                    status.Text = "你的回合";
                });
                white_piece.InvokeIfRequired(() =>
                {
                    white_piece.Visible = false;
                });
                black_piece.InvokeIfRequired(() =>
                {
                    black_piece.Visible = true;
                });
            }
            else
            {
                turn = false;
                status.InvokeIfRequired(() =>
                {
                    status.Visible = true;
                    status.Text = "對方的回合";
                });
                black_piece.InvokeIfRequired(() =>
                {
                    black_piece.Visible = false;
                });
                white_piece.InvokeIfRequired(() =>
                {
                    white_piece.Visible = true;
                });
            }
            re_rq = false;
            if (nameHasText)
                player = player_name.Text;
            else
                player = "匿名使用者" + piece_color;
            name_label.InvokeIfRequired(() =>
            {
                name_label.Text = "玩家名稱：" + player;
                name_label.Visible = true;
            });

            restart_btn.InvokeIfRequired(() =>
            {
                restart_btn.Text = "restart";
            });
            re_label.InvokeIfRequired(() =>
            {
                re_label.Text = "";
            });
            restart_btn.InvokeIfRequired(() =>
            {
                restart_btn.Visible = true;
                restart_btn.Enabled = true;
            });
        }
        void remove_p_click()//移除棋子的點擊事件
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl is PictureBox && (string)ctrl.Tag == "piece")
                {
                    ctrl.Click -= piece_Click;//取消
                }
            }
        }
        void remove_piece()//移除棋子
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl is PictureBox && (string)ctrl.Tag == "piece")
                {
                    //this.Controls.Remove(ctrl);
                    //ctrl.BackgroundImage.Dispose();
                    ctrl.BackgroundImage = null;
                }
            }
        }
        private void start_btn_Click(object sender, EventArgs e)//開始遊戲按鈕
        {
            try
            {
                start_btn.Visible = false;
                player_name.Visible = false;
                re_label.Visible = true;
                re_label.Text = "等待對手中，請等待...";
                //連線到server
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//建立TCP socket
                IPEndPoint ipLocal = new IPEndPoint(localAddr, port);//建立本機監聽位址及埠號 
                client.Connect(ipLocal);//連接至server
                Closing += new CancelEventHandler(Form1_Closing);
                //開始recieve多工
                Thread recv = new Thread(receive); // 持續監聽是否有Client連線及收值的執行緒
                recv.IsBackground = true;
                recv.Start();
            }
            catch (ArgumentNullException a)
            {
                Console.WriteLine("ArgumentNullException:{0}", a);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException:{0}", ex);
            }
        }
        private void receive()
        {
            try
            {
                // Socket[] socket = new Socket[] {client};
                // Socket.Select(socket, null, null, 500);//select模式
                byte[] buffer; // Receive data buffer
                               //int n;
                               // Thread.Sleep(5000);
                string str;
                while (true)
                {
                    buffer = new byte[1024];
                    int n = client.Receive(buffer);
                    if (n > 0)
                    {
                        str = Encoding.UTF8.GetString(buffer);
                        str.Substring(0, str.Length - 1);
                        if (str[0] == 'p')//操作棋子
                        {
                            put_piece(str[1], str.Substring(3));//呼叫函式改變棋子
                            game_status(str[1], str[2]);//呼叫函式檢查遊戲狀況
                        }
                        else if (str[0] == 'm')
                        {
                            show_mes(str);//顯示聊天訊息
                        }
                        else if (str[0] == 'r') //對手同意或請求重新開始
                        {
                            if (re_rq)//對手同意重新開始
                            {
                                re_label.InvokeIfRequired(() =>
                                {
                                    re_label.Text = "已發出重新開始請求，等待對手回應";
                                });
                            }
                            else//對手請求重新開始
                            {
                                restart_btn.InvokeIfRequired(() =>
                                {
                                    restart_btn.Text = "同意";
                                });
                                re_label.InvokeIfRequired(() =>
                                {
                                    re_label.Text = "對手請求重新開始";
                                });
                            }
                        }
                        else if (str[0] == 'a')
                        {
                            re_label.InvokeIfRequired(() =>
                            {
                                re_label.Text = "重置遊戲中，請等待...";
                            });
                            set_success = false;
                            remove_p_click();
                            remove_piece();
                            set_piece();
                            set_success = true;
                        }
                        else if (str[0] == '0' || str[0] == '1')//設置顏色
                        {

                            piece_color = str.Substring(0, 1);
                            set_piece();
                            set_chatroom();

                        }
                    }
                    else//server斷開連線
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        client = null;
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException:{0}", ex);
            }
        }

        private void m_input_Enter(object sender, EventArgs e)
        {
            if (mesHasText == false)
                m_input.Text = "";
            m_input.ForeColor = Color.Black;
        }

        private void m_input_Leave(object sender, EventArgs e)
        {
            if (m_input.Text == "")
            {
                m_input.Text = MesHint;
                m_input.ForeColor = Color.LightGray;
                mesHasText = false;
            }
            else
                mesHasText = true;
        }

        private void m_send_btn_Click(object sender, EventArgs e)
        {
            if (mesHasText)
            {
                string mes = "m[" + player + "]：" + m_input.Text;
                m_input.InvokeIfRequired(() =>
                {
                    m_input.Text = "";
                    mesHasText = false;
                });
                byte[] buffer = Encoding.UTF8.GetBytes(mes);// send data buffer
                client.Send(buffer, buffer.Length, SocketFlags.None);
            }
        }

        private void player_name_Enter(object sender, EventArgs e)
        {
            if (nameHasText == false)
                player_name.Text = "";
            player_name.ForeColor = Color.Black;
        }

        private void player_name_Leave(object sender, EventArgs e)
        {
            if (player_name.Text == "")
            {
                player_name.Text = NameHint;
                player_name.ForeColor = Color.LightGray;
                nameHasText = false;
            }
            else
                nameHasText = true;
        }
        void show_mes(string mes)
        {
            m_history.InvokeIfRequired(() =>
            {
                m_history.Text += "\r\n" + mes.Substring(1, mes.Length - 1);
                m_history.SelectionStart = m_history.Text.Length;
                m_history.ScrollToCaret();
            });

        }
        void game_status(char p, char stat)
        {
            if (stat == 'w') //有人贏了
            {
                remove_p_click();
                if (p == piece_color[0])
                {
                    status.InvokeIfRequired(() =>
                    {
                        status.Text = "你贏了";
                    });

                }
                else
                {
                    status.InvokeIfRequired(() =>
                    {
                        status.Text = "你輸了";
                    });
                }
            }
            else if (stat == 't')//平手
            {
                remove_p_click();
                status.InvokeIfRequired(() =>
                {
                    status.Text = "平手";
                });
            }
            else if (stat == 'n')//輪替回合
            {
                if (p == piece_color[0])//我方回合結束
                {
                    status.InvokeIfRequired(() =>
                    {
                        status.Text = "對方的回合";
                    });
                }
                else
                {
                    status.InvokeIfRequired(() =>
                    {
                        status.Text = "你的回合";
                    });
                }
                turn = !turn;
            }
        }

        private void restart_btn_Click(object sender, EventArgs e)
        {
            re_rq = true;
            string str = "r"; // r(請求/同意遊戲重開)
            byte[] buffer = Encoding.UTF8.GetBytes(str);// send data buffer
            client.Send(buffer, buffer.Length, SocketFlags.None);
            restart_btn.InvokeIfRequired(() =>
            {
                restart_btn.Enabled = false;
            });
        }

        void put_piece(char color, string name)//改變棋子圖片
        {
            int pid = Int32.Parse(name);
            if (color == '0') //下黑棋
                pieces[pid].BackgroundImage = Properties.Resources.black_piece;
            else if (color == '1') //下白棋
                pieces[pid].BackgroundImage = Properties.Resources.white_piece;
            pieces[pid].Click -= piece_Click;//取消棋子的點擊事件
        }
        void piece_Click(object sender, EventArgs e)
        {
            if (set_success && turn) //是我方的回合
            {
                // 將sender轉型成PictureBox
                PictureBox piece = sender as PictureBox;
                if (null == piece) return;
                var str = "p" + piece_color + "n" + piece.Name; // p(操作棋子) + 0/1(黑/白) + w/t/n(有人贏了/平手/還沒贏) + str(棋子id)
                byte[] buffer = Encoding.UTF8.GetBytes(str);// send data buffer
                client.Send(buffer, buffer.Length, SocketFlags.None);
            }
        }
    }
    public static class Extension
    {
        //非同步委派更新UI
        public static void InvokeIfRequired(
            this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)//在非當前執行緒內 使用委派
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
