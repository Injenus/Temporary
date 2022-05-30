
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
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb.Location = new System.Drawing.Point(30, 28);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(757, 545);
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
            this.bt_txt.Location = new System.Drawing.Point(857, 84);
            this.bt_txt.Name = "bt_txt";
            this.bt_txt.Size = new System.Drawing.Size(75, 23);
            this.bt_txt.TabIndex = 1;
            this.bt_txt.Text = "SaveTxt";
            this.bt_txt.UseVisualStyleBackColor = true;
            this.bt_txt.Click += new System.EventHandler(this.bt_txt_Click);
            // 
            // bt_reset
            // 
            this.bt_reset.Location = new System.Drawing.Point(857, 113);
            this.bt_reset.Name = "bt_reset";
            this.bt_reset.Size = new System.Drawing.Size(75, 23);
            this.bt_reset.TabIndex = 2;
            this.bt_reset.Text = "Reset";
            this.bt_reset.UseVisualStyleBackColor = true;
            this.bt_reset.Click += new System.EventHandler(this.bt_reset_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 605);
            this.Controls.Add(this.bt_reset);
            this.Controls.Add(this.bt_txt);
            this.Controls.Add(this.pb);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button bt_txt;
        private System.Windows.Forms.Button bt_reset;
    }
}

