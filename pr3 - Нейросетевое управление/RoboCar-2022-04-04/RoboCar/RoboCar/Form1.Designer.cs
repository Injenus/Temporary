
namespace RoboCar
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pb = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bt_txt = new System.Windows.Forms.Button();
            this.bt_reset = new System.Windows.Forms.Button();
            this.tb_ip = new System.Windows.Forms.TextBox();
            this.bt_folder = new System.Windows.Forms.Button();
            this.cb_auto_change_goal = new System.Windows.Forms.CheckBox();
            this.nud_sim_speed = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_sim_speed)).BeginInit();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb.Location = new System.Drawing.Point(10, 10);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(957, 701);
            this.pb.TabIndex = 0;
            this.pb.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bt_txt
            // 
            this.bt_txt.Location = new System.Drawing.Point(1028, 206);
            this.bt_txt.Name = "bt_txt";
            this.bt_txt.Size = new System.Drawing.Size(75, 23);
            this.bt_txt.TabIndex = 1;
            this.bt_txt.Text = "SaveTxt";
            this.bt_txt.UseVisualStyleBackColor = true;
            this.bt_txt.Click += new System.EventHandler(this.bt_txt_Click);
            // 
            // bt_reset
            // 
            this.bt_reset.Location = new System.Drawing.Point(1028, 120);
            this.bt_reset.Name = "bt_reset";
            this.bt_reset.Size = new System.Drawing.Size(75, 23);
            this.bt_reset.TabIndex = 2;
            this.bt_reset.Text = "Reset";
            this.bt_reset.UseVisualStyleBackColor = true;
            this.bt_reset.Click += new System.EventHandler(this.bt_reset_Click);
            // 
            // tb_ip
            // 
            this.tb_ip.Location = new System.Drawing.Point(988, 34);
            this.tb_ip.Name = "tb_ip";
            this.tb_ip.Size = new System.Drawing.Size(142, 23);
            this.tb_ip.TabIndex = 3;
            // 
            // bt_folder
            // 
            this.bt_folder.Location = new System.Drawing.Point(1028, 291);
            this.bt_folder.Name = "bt_folder";
            this.bt_folder.Size = new System.Drawing.Size(75, 23);
            this.bt_folder.TabIndex = 1;
            this.bt_folder.Text = "Folder";
            this.bt_folder.UseVisualStyleBackColor = true;
            this.bt_folder.Click += new System.EventHandler(this.bt_folder_Click);
            // 
            // cb_auto_change_goal
            // 
            this.cb_auto_change_goal.AutoSize = true;
            this.cb_auto_change_goal.Location = new System.Drawing.Point(1028, 379);
            this.cb_auto_change_goal.Name = "cb_auto_change_goal";
            this.cb_auto_change_goal.Size = new System.Drawing.Size(123, 19);
            this.cb_auto_change_goal.TabIndex = 4;
            this.cb_auto_change_goal.Text = "Auto Change Goal";
            this.cb_auto_change_goal.UseVisualStyleBackColor = true;
            // 
            // nud_sim_speed
            // 
            this.nud_sim_speed.Location = new System.Drawing.Point(1010, 447);
            this.nud_sim_speed.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_sim_speed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_sim_speed.Name = "nud_sim_speed";
            this.nud_sim_speed.Size = new System.Drawing.Size(120, 23);
            this.nud_sim_speed.TabIndex = 5;
            this.nud_sim_speed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1010, 429);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Sim Speed";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 720);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nud_sim_speed);
            this.Controls.Add(this.cb_auto_change_goal);
            this.Controls.Add(this.tb_ip);
            this.Controls.Add(this.bt_reset);
            this.Controls.Add(this.bt_folder);
            this.Controls.Add(this.bt_txt);
            this.Controls.Add(this.pb);
            this.Name = "Form1";
            this.Text = "RoboCar, S. Diane, 2022";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_sim_speed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button bt_txt;
        private System.Windows.Forms.Button bt_reset;
        private System.Windows.Forms.TextBox tb_ip;
        private System.Windows.Forms.Button bt_folder;
        private System.Windows.Forms.CheckBox cb_auto_change_goal;
        private System.Windows.Forms.NumericUpDown nud_sim_speed;
        private System.Windows.Forms.Label label1;
    }
}

