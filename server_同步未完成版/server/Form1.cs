using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {
        string left = "對手已離開對戰";
        Socket m_server;//接聽client的socket
        IPAddress localAddr = IPAddress.Parse("127.0.0.1"); // 設定 IP127.0.0.1
        int[] dx = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };//8個方向x值
        int[] dy = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };//8個方向y值
        int port = 8000;
        struct ThreadArgs /* Structure of arguments to pass to client thread */
        {
            public Socket[] sockets;
        };
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Thread m_thrListening = new Thread(Listening); // 持續監聽是否有Client連線及收值的執行緒
                m_thrListening.IsBackground = true;
                m_thrListening.Start();
            }
            catch (SocketException ex)
            {
                Console.WriteLine("thread errror", ex);
            }
        }
        private void Listening()
        {
            try
            {
                m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//建立TCP socket
                IPEndPoint ipLocal = new IPEndPoint(localAddr, port);//建立本機監聽位址及埠號 
                m_server.Bind(ipLocal);//socket連繫到該位址
                m_server.Listen(6);// 啟動監聽
                Console.WriteLine("Waiting for a connection...");
                ParameterizedThreadStart Par = new ParameterizedThreadStart(create_room);//建立可傳參數的多執行緒委派
                while (true)
                {
                    Socket client1 = m_server.Accept(); // 要等有Client建立連線後才會繼續往下執行
                   /* label1.InvokeIfRequired(() =>
                    {
                        label1.Text = client1.RemoteEndPoint.ToString();
                    });*/
                    
                    Socket client2 = m_server.Accept();
                    /*label2.InvokeIfRequired(() =>
                    {
                        label2.Text = client1.RemoteEndPoint.ToString();
                    });*/
                    ThreadArgs players = new ThreadArgs();
                    players.sockets = new Socket[2] { client1, client2 };
                    Thread room = new Thread(Par);//建立Thread
                    room.IsBackground = true;
                    room.Start(players);//開始一個執行緒
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }
        private void create_room(object o)//創造一個對戰
        {
            ThreadArgs players = (ThreadArgs)o;
            Socket p1 = players.sockets[0];
            Socket p2 = players.sockets[1];
            label1.InvokeIfRequired(() =>
            {
                label1.Text = p1.RemoteEndPoint.ToString();
            });
            label2.InvokeIfRequired(() =>
            {
                label2.Text = p2.RemoteEndPoint.ToString();
            });
            try
            {
                Socket.Select(players.sockets,null,null,500);//select模式
                int[,] board = new int[15,15];
                int  piece_n;
                byte[] buffer = new byte[1024]; // Receive data buffer
                bool rp1 = true, rp2 = true;//要不要開始這局遊戲
                //分配黑白方
                buffer = Encoding.ASCII.GetBytes("0");
                p1.Send(buffer);
                buffer = Encoding.ASCII.GetBytes("1");
                p2.Send(buffer);
                while (rp1 && rp2)//第一次開始或雙方都同意重來
                {
                    piece_n = 0;
                    rp1 = false;
                    rp2 = false;
                    for (int i = 0; i < 15; ++i)
                    {
                        for (int j = 0; j < 15; ++j)
                            board[i, j] = -1;
                    }
                    while (true)//開始收送資料
                    {
                        if (p1 != null) 
                        {
                            buffer = new byte[1024];
                            int n = p1.Receive(buffer);
                            if (n > 0) //p1有資料傳進
                            {
                                string str = Encoding.ASCII.GetString(buffer);
                                str=str.Substring(0, str.Length-1);
                                label1.InvokeIfRequired(() =>
                                {
                                    label1.Text = str;
                                });
                                if (str[0] == 'p') //沒有人贏和移動旗子
                                {
                                    ++piece_n;
                                    int loc= Int32.Parse(str.Substring(3,str.Length-3));
                                    int y = (loc / 15), x = loc % 15;
                                    board[y, x] = 0;
                                    if (make_line(board, y, x))
                                    {
                                        str = str.Replace('n', 'w');//連成線新增勝利訊息
                                        label1.InvokeIfRequired(() =>
                                        {
                                            label1.Text = str;
                                        });
                                    }
                                    else if (piece_n == 225)
                                    {
                                        str = str.Replace('n', 't');
                                    }
                                    //server移動後才傳回給玩家讓他們移動棋子
                                    buffer =  Encoding.ASCII.GetBytes(str);
                                    p1.Send(buffer);
                                    p2.Send(buffer);
                                }
                                else if (str[0] == 'm') //傳送訊息
                                {

                                }
                                else if (str[0] == 'r')//重來一場
                                {
                                    rp1 = true;
                                    if(!rp2)
                                        p2.Send(buffer);
                                }
                            }
                            else//p1斷開連線
                            {
                                p1 = null;
                                if (p2 != null) //p2還在才通知p2
                                {
                                    buffer = Encoding.ASCII.GetBytes(left);
                                    p2.Send(buffer);
                                }
                            }

                        }
                        
                        if(p2 != null)
                        {
                            buffer = new byte[1024];
                            int n = p2.Receive(buffer);
                            if (n > 0) //p2有資料傳進
                            {
                                string str = Encoding.ASCII.GetString(buffer);
                                str = str.Substring(0, str.Length - 1);
                                label2.InvokeIfRequired(() =>
                                {
                                    label2.Text = str;
                                });
                                if (str[0] == 'p') //沒有人贏和移動旗子
                                {
                                    ++piece_n;
                                    int loc = Int32.Parse(str.Substring(3));
                                    int y = (loc / 15), x = loc % 15;
                                    while (y >= 15)
                                        y= (loc / 15);
                                    while(x >= 15)
                                        x= (loc % 15);
                                    board[y, x] = 1;
                                    if (make_line(board, y, x))
                                    {
                                        str = str.Replace('n', 'w');//連成線新增勝利訊息
                                    }
                                    else if (piece_n == 225)
                                    {
                                        str = str.Replace('n', 't');
                                    }
                                    //server移動後才傳回給玩家讓他們移動棋子
                                    buffer = Encoding.ASCII.GetBytes(str);
                                    p2.Send(buffer);
                                    p1.Send(buffer); 
                                }
                                else if (str[0] == 'm') //傳送訊息
                                {

                                }
                                else if (str[0] == 'r')//重來一場
                                {
                                    rp2 = true;
                                    if(!rp1)
                                        p1.Send(buffer);
                                }
                            }
                            else//p2斷開連線
                            {
                                
                                p2 = null;
                                if (p1 != null) //p1還在才通知p1
                                {
                                    buffer = Encoding.ASCII.GetBytes(left);
                                    p1.Send(buffer);
                                }
                            }

                        }
                        
                        if (rp1 && rp2)//雙方要求重來一場
                        {
                            break;
                        }
                        if (p1 == null && p2 == null)//玩家都離開了
                        {
                            p1.Shutdown(SocketShutdown.Both);
                            p1.Close();
                            p2.Shutdown(SocketShutdown.Both);
                            p2.Close();
                            break;
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                p1.Shutdown(SocketShutdown.Both);
                p1.Close();
                p2.Shutdown(SocketShutdown.Both);
                p2.Close();
                Console.WriteLine("SocketException: {0}", ex);
            }
        }
        bool make_line(int[,] b, int Y, int X)//判斷
        {
            int[] w = new int[4] { 0, 0, 0, 0 };//四種方向連線
            for (int i = 0; i < 8; ++i) //判斷8個方向
            {
                int y = Y, x = X, j = 0;
                for (; j < 4; ++j) 
                {
                    y += dy[i];
                    x += dx[i];
                    if (y < 0 || x < 0 || y >= 15 || x >= 15 || b[y, x] != b[Y, X]) //無法連成線
                    {
                        break;
                    }
                }
                w[i % 4] += j;
                if (w[i % 4] >= 4) //五個連成一線
                {
                    return true;
                }
            }
            return false;
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
