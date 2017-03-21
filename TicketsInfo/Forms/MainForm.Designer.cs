namespace TicketsInfo.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.输入查询ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.余票数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时刻表数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.票价数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.输入查询ToolStripMenuItem,
            this.余票数据ToolStripMenuItem,
            this.时刻表数据ToolStripMenuItem,
            this.票价数据ToolStripMenuItem,
            this.设置ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(889, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 输入查询ToolStripMenuItem
            // 
            this.输入查询ToolStripMenuItem.Name = "输入查询ToolStripMenuItem";
            this.输入查询ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.输入查询ToolStripMenuItem.Text = "输入查询";
            this.输入查询ToolStripMenuItem.Click += new System.EventHandler(this.输入查询ToolStripMenuItem_Click);
            // 
            // 余票数据ToolStripMenuItem
            // 
            this.余票数据ToolStripMenuItem.Name = "余票数据ToolStripMenuItem";
            this.余票数据ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.余票数据ToolStripMenuItem.Text = "余票数据";
            this.余票数据ToolStripMenuItem.Click += new System.EventHandler(this.余票数据ToolStripMenuItem_Click);
            // 
            // 时刻表数据ToolStripMenuItem
            // 
            this.时刻表数据ToolStripMenuItem.Name = "时刻表数据ToolStripMenuItem";
            this.时刻表数据ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.时刻表数据ToolStripMenuItem.Text = "时刻表数据";
            this.时刻表数据ToolStripMenuItem.Click += new System.EventHandler(this.时刻表数据ToolStripMenuItem_Click);
            // 
            // 票价数据ToolStripMenuItem
            // 
            this.票价数据ToolStripMenuItem.Name = "票价数据ToolStripMenuItem";
            this.票价数据ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.票价数据ToolStripMenuItem.Text = "票价数据";
            this.票价数据ToolStripMenuItem.Click += new System.EventHandler(this.票价数据ToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            this.设置ToolStripMenuItem.Click += new System.EventHandler(this.设置ToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "票价查询";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 518);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "12306数据获取";
            this.SizeChanged += new System.EventHandler(this.Form_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 余票数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 时刻表数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 票价数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 输入查询ToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}