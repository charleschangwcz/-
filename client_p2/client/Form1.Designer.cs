namespace client
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.black_piece = new System.Windows.Forms.PictureBox();
            this.white_piece = new System.Windows.Forms.PictureBox();
            this.start_btn = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.restart_btn = new System.Windows.Forms.Button();
            this.re_label = new System.Windows.Forms.Label();
            this.name_label = new System.Windows.Forms.Label();
            this.m_input = new System.Windows.Forms.TextBox();
            this.m_history = new System.Windows.Forms.TextBox();
            this.m_send_btn = new System.Windows.Forms.Button();
            this.player_name = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.black_piece)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.white_piece)).BeginInit();
            this.SuspendLayout();
            // 
            // black_piece
            // 
            this.black_piece.BackColor = System.Drawing.Color.Transparent;
            this.black_piece.BackgroundImage = global::client.Properties.Resources.black_piece;
            this.black_piece.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.black_piece.Location = new System.Drawing.Point(638, 42);
            this.black_piece.Name = "black_piece";
            this.black_piece.Size = new System.Drawing.Size(40, 40);
            this.black_piece.TabIndex = 1;
            this.black_piece.TabStop = false;
            // 
            // white_piece
            // 
            this.white_piece.BackColor = System.Drawing.Color.Transparent;
            this.white_piece.BackgroundImage = global::client.Properties.Resources.white_piece;
            this.white_piece.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.white_piece.Location = new System.Drawing.Point(638, 42);
            this.white_piece.Name = "white_piece";
            this.white_piece.Size = new System.Drawing.Size(40, 40);
            this.white_piece.TabIndex = 2;
            this.white_piece.TabStop = false;
            // 
            // start_btn
            // 
            this.start_btn.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.start_btn.Location = new System.Drawing.Point(897, 146);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(212, 68);
            this.start_btn.TabIndex = 3;
            this.start_btn.Text = "Start";
            this.start_btn.UseVisualStyleBackColor = true;
            this.start_btn.Click += new System.EventHandler(this.start_btn_Click);
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.status.Location = new System.Drawing.Point(696, 42);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(81, 40);
            this.status.TabIndex = 4;
            this.status.Text = "回合";
            // 
            // restart_btn
            // 
            this.restart_btn.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.restart_btn.Location = new System.Drawing.Point(897, 32);
            this.restart_btn.Name = "restart_btn";
            this.restart_btn.Size = new System.Drawing.Size(212, 68);
            this.restart_btn.TabIndex = 5;
            this.restart_btn.Text = "restart";
            this.restart_btn.UseVisualStyleBackColor = true;
            this.restart_btn.Click += new System.EventHandler(this.restart_btn_Click);
            // 
            // re_label
            // 
            this.re_label.AutoSize = true;
            this.re_label.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.re_label.Location = new System.Drawing.Point(642, 167);
            this.re_label.Name = "re_label";
            this.re_label.Size = new System.Drawing.Size(96, 26);
            this.re_label.TabIndex = 6;
            this.re_label.Text = "系統提示";
            // 
            // name_label
            // 
            this.name_label.AutoSize = true;
            this.name_label.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.name_label.Location = new System.Drawing.Point(637, 107);
            this.name_label.Name = "name_label";
            this.name_label.Size = new System.Drawing.Size(254, 31);
            this.name_label.TabIndex = 7;
            this.name_label.Text = "玩家名稱：匿名使用者";
            // 
            // m_input
            // 
            this.m_input.BackColor = System.Drawing.Color.WhiteSmoke;
            this.m_input.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.m_input.ForeColor = System.Drawing.Color.LightGray;
            this.m_input.Location = new System.Drawing.Point(638, 544);
            this.m_input.MaxLength = 50;
            this.m_input.Name = "m_input";
            this.m_input.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.m_input.Size = new System.Drawing.Size(397, 43);
            this.m_input.TabIndex = 8;
            this.m_input.Text = "輸入訊息...";
            this.m_input.Enter += new System.EventHandler(this.m_input_Enter);
            this.m_input.Leave += new System.EventHandler(this.m_input_Leave);
            // 
            // m_history
            // 
            this.m_history.BackColor = System.Drawing.Color.WhiteSmoke;
            this.m_history.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.m_history.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.m_history.Location = new System.Drawing.Point(638, 228);
            this.m_history.Multiline = true;
            this.m_history.Name = "m_history";
            this.m_history.ReadOnly = true;
            this.m_history.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_history.Size = new System.Drawing.Size(471, 297);
            this.m_history.TabIndex = 9;
            this.m_history.Text = "[聊天歷史紀錄]";
            // 
            // m_send_btn
            // 
            this.m_send_btn.BackColor = System.Drawing.Color.Azure;
            this.m_send_btn.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.m_send_btn.Location = new System.Drawing.Point(1042, 544);
            this.m_send_btn.Name = "m_send_btn";
            this.m_send_btn.Size = new System.Drawing.Size(67, 43);
            this.m_send_btn.TabIndex = 10;
            this.m_send_btn.Text = "send";
            this.m_send_btn.UseVisualStyleBackColor = false;
            this.m_send_btn.Click += new System.EventHandler(this.m_send_btn_Click);
            // 
            // player_name
            // 
            this.player_name.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.player_name.ForeColor = System.Drawing.Color.LightGray;
            this.player_name.Location = new System.Drawing.Point(638, 168);
            this.player_name.MaxLength = 10;
            this.player_name.Name = "player_name";
            this.player_name.Size = new System.Drawing.Size(253, 39);
            this.player_name.TabIndex = 11;
            this.player_name.Text = "輸入使用者名稱...";
            this.player_name.Enter += new System.EventHandler(this.player_name_Enter);
            this.player_name.Leave += new System.EventHandler(this.player_name_Leave);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.BackgroundImage = global::client.Properties.Resources.backgd;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1145, 619);
            this.Controls.Add(this.re_label);
            this.Controls.Add(this.player_name);
            this.Controls.Add(this.m_send_btn);
            this.Controls.Add(this.m_history);
            this.Controls.Add(this.m_input);
            this.Controls.Add(this.name_label);
            this.Controls.Add(this.restart_btn);
            this.Controls.Add(this.status);
            this.Controls.Add(this.start_btn);
            this.Controls.Add(this.white_piece);
            this.Controls.Add(this.black_piece);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.black_piece)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.white_piece)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox black_piece;
        private System.Windows.Forms.PictureBox white_piece;
        private System.Windows.Forms.Button start_btn;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Button restart_btn;
        private System.Windows.Forms.Label re_label;
        private System.Windows.Forms.Label name_label;
        private System.Windows.Forms.TextBox m_input;
        private System.Windows.Forms.TextBox m_history;
        private System.Windows.Forms.Button m_send_btn;
        private System.Windows.Forms.TextBox player_name;
    }
}

