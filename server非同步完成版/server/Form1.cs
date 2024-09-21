using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace server
{
    public partial class Form1 : Form
    {
        
        Socket m_server;//接聽client的socket
        IPAddress localAddr = IPAddress.Parse("127.0.0.1"); // 設定 本機IP127.0.0.1
        int[] dx = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };//8個方向x值
        int[] dy = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };//8個方向y值
        int port = 8000;
        public class info
        {
            public Socket[] sockets;
            public int rp = 0;
            public int piece_n = 0;
            public int[,] board = new int[15, 15];
            
        }
        public class StateObject
        {
            public Socket client;              // Socket连接的对象 
            public int color;
            public const int BufferSize = 1024;            // 客户端接受socket数据的缓冲区字节数大小 
            public byte[] buffer = new byte[BufferSize];  // 客户端接受socket数据的缓冲区
            public info rm_info;
            public String response = String.Empty;
        }
        struct players
        {
            public Socket []sockets; 
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
                    Socket client2 = m_server.Accept();
                    players p = new players();
                    p.sockets = new Socket[2];
                    p.sockets[0] = client1;
                    p.sockets[1] = client2;
                    Thread room = new Thread(Par);//建立Thread
                    room.IsBackground = true;
                    room.Start(p);//開始一個執行緒
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }
        private void create_room(object o)//創造一個對戰
        {
            try
            {
                players player = (players)o;
                info room_info = new info();
                StateObject s1 = new StateObject();
                StateObject s2 = new StateObject();
                room_info.sockets = player.sockets;
                Socket p1 = room_info.sockets[0];
                Socket p2 = room_info.sockets[1];
                label1.InvokeIfRequired(() =>
                {
                    label1.Text = p1.RemoteEndPoint.ToString();
                });
                label2.InvokeIfRequired(() =>
                {
                    label2.Text = p2.RemoteEndPoint.ToString();
                });
                s1.rm_info = room_info;
                s2.rm_info = room_info;
                s1.client = p1;
                s2.client = p2;
                //分配黑白方
                byte []Col = new byte[1];
                s1.color = 0;
                Col = Encoding.UTF8.GetBytes("0");
                p1.Send(Col);
                s2.color = 1;
                Col = Encoding.UTF8.GetBytes("1");
                p2.Send(Col);
                reset_board(room_info.board);
                //開始非同步接收
                p1.BeginReceive(s1.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), s1);
                p2.BeginReceive(s2.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), s2);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // 拿到state object
                StateObject state = (StateObject)ar.AsyncState;
                // 看看从服务端收到多少字节的数据
                int bytesRead = state.client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // 收到多少，就把它格式化一下，编码成字符串
                    state.response = Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
                    string str = state.response;
                    str = str.Substring(0, str.Length );
                    label1.InvokeIfRequired(() =>
                    {
                        label1.Text = str;
                    });
                    if (str[0] == 'p') //沒有人贏和移動旗子
                    {
                        ++state.rm_info.piece_n;
                        int loc = Int32.Parse(str.Substring(3, str.Length - 3));
                        int y = (loc / 15), x = loc % 15;
                        state.rm_info.board[y, x] = state.color;
                        if (make_line(state.rm_info.board, y, x))
                        {
                            str = str.Replace('n', 'w');//連成線新增勝利訊息
                            label1.InvokeIfRequired(() =>
                            {
                                label1.Text = str;
                            });
                        }
                        else if (state.rm_info.piece_n == 225)
                        {
                            str = str.Replace('n', 't');
                        }
                        //server移動後才傳回給玩家讓他們移動棋子
                        state.buffer = Encoding.UTF8.GetBytes(str);
                        for (int i = 0; i < 2; ++i)
                        {
                            if (state.rm_info.sockets[i] != null)
                            {
                                state.rm_info.sockets[i].Send(state.buffer);
                            }
                        }
                    }
                    else if (str[0] == 'm') //傳送訊息
                    {
                        for (int i = 0; i < 2; ++i)
                        {
                            if (state.rm_info.sockets[i] != null)
                            {
                                state.rm_info.sockets[i].Send(state.buffer);
                            }
                        }
                    }
                    else if (str[0] == 'r')//重來一場
                    {
                        ++state.rm_info.rp;
                        if (state.rm_info.rp == 2)
                        {
                            state.buffer = Encoding.UTF8.GetBytes("a");
                            state.rm_info.piece_n = 0;
                            state.rm_info.rp = 0;
                            reset_board(state.rm_info.board);
                        }
                        for (int i = 0; i < 2; ++i)
                        {
                            if (state.rm_info.sockets[i] != null) 
                            {
                                state.rm_info.sockets[i].Send(state.buffer);
                            }
                        }
                    }
                    state.buffer = new byte[StateObject.BufferSize];
                    // 因为不是一次收完所有服务端要发送过来的数据，所以再继续首收发
                    state.client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                }
                else//客戶段離線
                {
                    state.rm_info.sockets[state.color] = null;
                    for (int i = 0; i < 2; ++i) 
                    {
                        if (state.rm_info.sockets[i] != null)
                        {
                            state.rm_info.sockets[i].Send(state.buffer);
                        }
                    }
                    state.client.Shutdown(SocketShutdown.Both);
                    state.client.Close();
                }
            }
            catch (Exception e) 
            {  
                 Console.WriteLine(e.ToString());  
            }
        }
        void reset_board(int[,] b)
        {
            for (int i = 0; i < 15; ++i)
            {
                for (int j = 0; j < 15; ++j)
                    b[i, j] = -1;
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
