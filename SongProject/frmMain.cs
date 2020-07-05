// Decompiled with JetBrains decompiler
// Type: SongProject.frmMain
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SongProject
{
    public class frmMain : Form
    {
        public string sgPath = "";
        private INIFile settings = new INIFile("Settings.ini", false, true);
        public projection frm;
        public bool isProjecting;
        private bool newSong;
        public bool _saved;
        private bool skipTextChange;
        public string OldTitle;
        private IContainer components;
        private Button btnAnnounce;
        private Label label1;
        private TextBox txtTitle;
        private TextBox txtPresentation;
        private Label label2;
        private TextBox txtBody;
        public ListBox listSongs;
        private Label label4;
        private ComboBox boxFind;
        private Label label3;
        private ListBox listSaved;
        private Button btnAddNew;
        private ContextMenuStrip favContext;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ContextMenuStrip lstContext;
        private ToolStripMenuItem addToFavoritesToolStripMenuItem;
        private ToolStripMenuItem deleteSongToolStripMenuItem;
        private PictureBox saveImg;

        public frmMain()
        {
            this.InitializeComponent();
            this.sgPath = this.settings.GetValue("Settings", "Path", "") + "//";
            if (Directory.Exists(this.sgPath))
                return;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.settings.SetValue("Settings", "Path", folderBrowserDialog.SelectedPath);
                this.sgPath = folderBrowserDialog.SelectedPath + "//";
            }
            else
                this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = frmMain.ShowDialog("Announce", "Enter title ", "Anunt");
            string presentation = "[1] " + Environment.NewLine + frmMain.ShowDialog("Annoucne", "Enter text ", "");
            projection projection = new projection();
            Screen[] allScreens = Screen.AllScreens;
            if (allScreens.Length > 1)
                this.setFormLocation((Form)projection, allScreens[1]);
            else
                this.setFormLocation((Form)projection, allScreens[0]);
            projection.SetText(new List<string>((IEnumerable<string>)presentation.Split('[')));
            projection.title = str;
            projection.presentation = "1";
            projection.Show();
            new frmControl(presentation).Show();
            this.frm = (projection)Application.OpenForms["projection"];
            this.isProjecting = true;
            this.Log("Anunt - " + str + ": " + presentation.Replace(Environment.NewLine, "").Replace("[1] ", ""));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.newSong = true;
            this.OldTitle = "";
            this.txtBody.Text = this.txtPresentation.Text = this.txtTitle.Text = "";
            this.savedStatus(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.SetSaved(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.SetSaved(false);
        }

        private void SetSaved(bool status)
        {
            if (status)
            {
                this.Text = this.Text.Replace("*", "");
            }
            else
            {
                if (status || this.Text.Contains("*"))
                    return;
                this.Text += "*";
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isProjecting)
            {
                if (this.listSongs.SelectedItem.ToString() != this.txtTitle.Text)
                {
                    int num = (int)MessageBox.Show("You can't edit another song while projecting !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                this.listSongs.SelectedItem = (object)this.txtTitle.Text;
            }
            else
            {
                this.boxFind.Text = "";
                object selectedItem = this.listSongs.SelectedItem;
                if (!this._saved & this.txtTitle.Text.Replace(" ", "") != "")
                {
                    if (MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        if (File.Exists(this.sgPath + (object)this.txtTitle) && this.newSong)
                        {
                            int num = (int)MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            return;
                        }
                        this.ArrangeText();
                        string contents = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <song><title>" + this.txtTitle.Text + "</title><author></author><copyright></copyright><presentation>" + this.txtPresentation.Text + "</presentation><ccli></ccli><capo print=\"false\"></capo><key></key> <aka></aka><key_line></key_line><user1></user1><user2></user2><user3></user3> <theme></theme><tempo></tempo><time_sig></time_sig><lyrics>" + this.txtBody.Text + "</lyrics>\r\n  <hymn_number></hymn_number></song>";
                        if (this.OldTitle != this.txtTitle.Text && File.Exists(this.sgPath + this.OldTitle))
                            File.Delete(this.sgPath + this.OldTitle);
                        File.WriteAllText(this.sgPath + this.txtTitle.Text, contents);
                        this.OldTitle = this.txtTitle.Text;
                        this.txtBody.Select(this.txtBody.Text.Length, 1);
                        this._saved = true;
                        this.savedStatus(true);
                        this._Refresh();
                        this.newSong = false;
                    }
                    else
                        this.newSong = false;
                }
                this.listSongs.SelectedItem = selectedItem;
                this.GetSong(this.listSongs.SelectedItem.ToString());
                this.listSongs.SelectedItem = selectedItem;
                this.listSongs.Focus();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                return;
            if (e.KeyValue >= 48 & e.KeyValue <= 90)
                this.boxFind.Text += ((char)e.KeyValue).ToString().ToLower();
            this.boxFind.Focus();
            this.boxFind.SelectionStart = this.boxFind.Text.Length;
            e.SuppressKeyPress = true;
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            int num = this.listSongs.IndexFromPoint(e.Location);
            if (num < 0)
                return;
            this.listSongs.SelectedIndex = num;
            this.lstContext.Show((Control)this.listSongs, e.Location);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._saved & this.txtTitle.Text.Replace(" ", "") != "" && MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                if (File.Exists(this.sgPath + (object)this.txtTitle) && this.newSong)
                {
                    int num = (int)MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                this.ArrangeText();
                string contents = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <song><title>" + this.txtTitle.Text + "</title><author></author><copyright></copyright><presentation>" + this.txtPresentation.Text + "</presentation><ccli></ccli><capo print=\"false\"></capo><key></key> <aka></aka><key_line></key_line><user1></user1><user2></user2><user3></user3> <theme></theme><tempo></tempo><time_sig></time_sig><lyrics>" + this.txtBody.Text + "</lyrics>\r\n  <hymn_number></hymn_number></song>";
                if (this.OldTitle != this.txtTitle.Text && File.Exists(this.sgPath + this.OldTitle))
                    File.Delete(this.sgPath + this.OldTitle);
                File.WriteAllText(this.sgPath + this.txtTitle.Text, contents);
                this.OldTitle = this.txtTitle.Text;
                this.txtBody.Select(this.txtBody.Text.Length, 1);
                this._saved = true;
                this.savedStatus(true);
                this.newSong = false;
                this._Refresh();
            }
            Environment.Exit(0);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this._Refresh();
            foreach (FileSystemInfo file in new DirectoryInfo(this.sgPath).GetFiles("*.*", SearchOption.TopDirectoryOnly))
                this.boxFind.AutoCompleteCustomSource.Add(file.Name);
            this.saveImg.Visible = false;
            this.boxFind.GotFocus += new EventHandler(this.boxFind_GotFocus);
            this.listSongs.GotFocus += new EventHandler(this.boxFind_GotFocus);
            HookClass.StartHook();
        }

        private void boxFind_GotFocus(object sender, EventArgs e)
        {
            if (!this.isProjecting)
                return;
            this.txtBody.Focus();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D2)
                this.Project(1);
            if (e.Control && e.KeyCode == Keys.D1)
                this.Project(0);
            else if (e.KeyCode == Keys.Escape && this.isProjecting)
            {
                this.frm = (projection)Application.OpenForms["projection"];
                this.frm.Close();
                Application.OpenForms["frmControl"].Close();
                this.isProjecting = false;
            }
            else if (e.Control && e.KeyCode == Keys.F)
                this.boxFind.Focus();
            else if (e.Control && e.KeyCode == Keys.S && !this._saved)
            {
                if (File.Exists(this.sgPath + (object)this.txtTitle) && this.newSong)
                {
                    int num1 = (int)MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    this.ArrangeText();
                    string contents = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <song><title>" + this.txtTitle.Text + "</title><author></author><copyright></copyright><presentation>" + this.txtPresentation.Text + "</presentation><ccli></ccli><capo print=\"false\"></capo><key></key> <aka></aka><key_line></key_line><user1></user1><user2></user2><user3></user3> <theme></theme><tempo></tempo><time_sig></time_sig><lyrics>" + this.txtBody.Text + "</lyrics>\r\n  <hymn_number></hymn_number></song>";
                    if (this.OldTitle != this.txtTitle.Text && File.Exists(this.sgPath + this.OldTitle))
                        File.Delete(this.sgPath + this.OldTitle);
                    File.WriteAllText(this.sgPath + this.txtTitle.Text, contents);
                    this.OldTitle = this.txtTitle.Text;
                    this.txtBody.Select(this.txtBody.Text.Length, 1);
                    this._saved = true;
                    int selectedIndex = this.listSongs.SelectedIndex;
                    this.savedStatus(true);
                    this._Refresh();
                    this.listSongs.SelectedItem = (object)this.OldTitle;
                    this.newSong = false;
                    this.listSongs.Focus();
                    if (!this.isProjecting)
                        return;
                    this.frm.sg.Clear();
                    this.frm.SetText(new List<string>((IEnumerable<string>)this.txtBody.Text.Split('[')));
                    this.frm.title = this.txtTitle.Text;
                    this.frm.presentation = this.txtPresentation.Text;
                    ((frmControl)Application.OpenForms["frmControl"]).LoadText(this.frm.sg);
                }
            }
            else if (e.KeyCode == Keys.Return && this.boxFind.Focused && this.boxFind.Text != "")
            {
                if (this.isProjecting)
                {
                    this.boxFind.Text = "";
                    int num2 = (int)MessageBox.Show("You can't edit another song while projecting !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    this.listSongs.SelectedItem = (object)this.boxFind.Text;
                    if (this.listSongs.Items.Contains((object)this.boxFind.Text) || !(this.boxFind.Text.Trim() != ""))
                        return;
                    frmSearch openForm = (frmSearch)Application.OpenForms["frmSearch"];
                    if (openForm == null)
                    {
                        List<string> songs = new List<string>();
                        foreach (string str in this.listSongs.Items)
                            songs.Add(str);
                        frmSearch frmSearch = new frmSearch(songs, this.sgPath);
                        frmSearch.txtQuery.Text = this.boxFind.Text;
                        frmSearch.Show();
                        frmSearch.StartSearching();
                    }
                    else
                    {
                        openForm.txtQuery.Text = this.boxFind.Text;
                        openForm.StartSearching();
                        openForm.Focus();
                    }
                }
            }
            else
            {
                if (e.KeyCode != Keys.Delete || this.txtBody.Focused)
                    return;
                if (this.isProjecting && this.isProjecting)
                {
                    int num2 = (int)MessageBox.Show("You can't delete this song while you projecting It !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    if (this.listSaved.Focused && this.listSaved.SelectedIndex != -1)
                        this.listSaved.Items.Remove(this.listSaved.SelectedItem);
                    if (!this.listSongs.Focused || this.listSongs.SelectedIndex == -1 || MessageBox.Show("Are you sure you want to delete this song ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
                        return;
                    File.Delete(this.sgPath + this.listSongs.SelectedItem.ToString());
                    this.txtPresentation.Text = this.txtTitle.Text = this.txtBody.Text = "";
                    this._Refresh();
                }
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.Focus();
            this.BringToFront();
            this.boxFind.Focus();
        }

        private void txtPresentation_TextChanged(object sender, EventArgs e)
        {
            if (this.skipTextChange)
                return;
            this.savedStatus(false);
            int selectionStart = this.txtPresentation.SelectionStart;
            int selectionLength = this.txtPresentation.SelectionLength;
            this.txtPresentation.Text = this.txtPresentation.Text.ToUpper();
            this.txtPresentation.SelectionStart = selectionStart;
            this.txtPresentation.SelectionLength = selectionLength;
        }

        private void txtOrder_TextChanged(object sender, EventArgs e)
        {
            if (this.skipTextChange)
                return;
            this.savedStatus(false);
            int selectionStart = this.txtPresentation.SelectionStart;
            this.txtPresentation.Text = this.txtPresentation.Text.ToUpper();
            this.txtPresentation.SelectionStart = selectionStart;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listSaved.SelectedItem != null)
            {
                this.GetSong(this.listSaved.SelectedItem.ToString());
                this.txtBody.SelectionStart = this.txtBody.Text.Length - 1;
            }
        }

        private void lstContext_Opening(object sender, CancelEventArgs e)
        {
            if (this.listSongs.SelectedIndex != -1)
                return;
            this.lstContext.Items[0].Enabled = true;
        }

        private void addToFavoritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listSongs.SelectedIndex == -1 || this.listSaved.Items.Contains(this.listSongs.SelectedItem))
                return;
            this.listSaved.Items.Add((object)this.listSongs.SelectedItem.ToString());
        }

        private void deleteSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this song ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
                return;
            File.Delete(this.sgPath + this.listSongs.SelectedItem.ToString());
            this._Refresh();
            this.txtBody.Text = this.txtTitle.Text = this.txtPresentation.Text = "";
        }

        private void favContext_Opening(object sender, CancelEventArgs e)
        {
            if (this.listSaved.SelectedIndex != -1)
                return;
            e.Cancel = true;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listSaved.Items.Remove(this.listSaved.SelectedItem);
        }

        public static string ShowDialog(string title, string text, string placeHolder = "")
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = title;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label label1 = new Label();
            label1.Left = 50;
            label1.Top = 20;
            label1.Text = text;
            Label label2 = label1;
            TextBox textBox1 = new TextBox();
            textBox1.Left = 50;
            textBox1.Top = 50;
            textBox1.Width = 400;
            textBox1.Text = placeHolder;
            TextBox textBox2 = textBox1;
            Button button1 = new Button();
            button1.Text = "Ok";
            button1.Left = 350;
            button1.Width = 100;
            button1.Top = 70;
            Button button2 = button1;
            button2.Click += (EventHandler)((sender, e) => prompt.Close());
            prompt.Controls.Add((Control)textBox2);
            prompt.Controls.Add((Control)button2);
            prompt.Controls.Add((Control)label2);
            prompt.AcceptButton = (IButtonControl)button2;
            int num = (int)prompt.ShowDialog();
            return textBox2.Text;
        }

        public void Project(int i)
        {
            if (this.txtBody.Text.Trim() == "" || this.txtTitle.Text.Trim() == "")
            {
                int num1 = (int)MessageBox.Show("You cannot project a song without a title or without text !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                if (!this._saved)
                {
                    if (MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        if (File.Exists(this.sgPath + (object)this.txtTitle) && this.newSong)
                        {
                            int num2 = (int)MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            return;
                        }
                        this.ArrangeText();
                        string contents = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <song><title>" + this.txtTitle.Text + "</title><author></author><copyright></copyright><presentation>" + this.txtPresentation.Text + "</presentation><ccli></ccli><capo print=\"false\"></capo><key></key> <aka></aka><key_line></key_line><user1></user1><user2></user2><user3></user3> <theme></theme><tempo></tempo><time_sig></time_sig><lyrics>" + this.txtBody.Text + "</lyrics>\r\n  <hymn_number></hymn_number></song>";
                        if (this.OldTitle != this.txtTitle.Text && File.Exists(this.sgPath + this.OldTitle))
                            File.Delete(this.sgPath + this.OldTitle);
                        File.WriteAllText(this.sgPath + this.txtTitle.Text, contents);
                        this.OldTitle = this.txtTitle.Text;
                        this.txtBody.Select(this.txtBody.Text.Length, 1);
                        this._saved = true;
                        this.savedStatus(true);
                        this._Refresh();
                        this.newSong = false;
                    }
                    else if (this.newSong)
                    {
                        int num2 = (int)MessageBox.Show("You cannot project a new song without saving it first !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }
                }
                if (this.isProjecting)
                    return;
                projection projection = new projection();
                Screen[] allScreens = Screen.AllScreens;
                this.setFormLocation((Form)projection, allScreens[i]);
                projection.SetText(new List<string>((IEnumerable<string>)this.txtBody.Text.Split('[')));
                projection.title = this.txtTitle.Text;
                projection.presentation = this.txtPresentation.Text;
                projection.Show();
                new frmControl(this.txtPresentation.Text).Show();
                this.frm = (projection)Application.OpenForms["projection"];
                this.isProjecting = true;
                this.Log(this.txtTitle.Text);
            }
        }

        public void Log(string text)
        {
            File.AppendAllText("log.txt", DateTime.Now.ToString() + " - " + text + Environment.NewLine);
        }

        public void ChangePath()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.sgPath = folderBrowserDialog.SelectedPath + "\\";
                int num = (int)MessageBox.Show("Path changed successfully !" + Environment.NewLine + "Application will now exit !");
                Environment.Exit(0);
            }
            else
                this.Close();
        }

        public void _Refresh()
        {
            this.boxFind.AutoCompleteCustomSource.Clear();
            this.listSongs.Items.Clear();
            this.boxFind.BeginUpdate();
            this.listSongs.BeginUpdate();
            FileInfo[] files = new DirectoryInfo(this.sgPath).GetFiles("*.*", SearchOption.TopDirectoryOnly);
            List<string> stringList = new List<string>();
            foreach (FileInfo fileInfo in files)
                stringList.Add(fileInfo.Name);
            this.listSongs.Items.AddRange((object[])stringList.ToArray());
            this.boxFind.AutoCompleteCustomSource.AddRange(stringList.ToArray());
            this.listSongs.EndUpdate();
            this.boxFind.EndUpdate();
            this.boxFind.Text = "";
        }

        private void setFormLocation(Form form, Screen screen)
        {
            Rectangle bounds = screen.Bounds;
            form.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            form.StartPosition = FormStartPosition.Manual;
        }

        public void ArrangeText()
        {
            using (StringReader stringReader = new StringReader(this.txtBody.Text))
            {
                this.txtBody.Text = "";
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    try
                    {
                        if (str[0] != ' ' && str[str.Length - 1] != ']')
                            str = " " + str;
                        TextBox txtBody = this.txtBody;
                        txtBody.Text = txtBody.Text + str + Environment.NewLine;
                    }
                    catch
                    {
                        this.txtBody.Text += Environment.NewLine;
                    }
                }
            }
            this.boxFind.Text = "";
            this.boxFind.AutoCompleteCustomSource.Clear();
            foreach (FileSystemInfo file in new DirectoryInfo(this.sgPath).GetFiles("*.*"))
                this.boxFind.AutoCompleteCustomSource.Add(file.Name);
        }

        private string ExtractString(string s, string tag)
        {
            string str = "<" + tag + ">";
            int startIndex = s.IndexOf(str) + str.Length;
            int num = s.IndexOf("</" + tag + ">", startIndex);
            return s.Substring(startIndex, num - startIndex);
        }

        public void savedStatus(bool sts)
        {
            this._saved = sts;
            this.SetSaved(sts);
        }

        public void GetSong(string name)
        {
            if (name.Replace(" ", "") == "")
                return;
            if (File.Exists(this.sgPath + name))
            {
                string s = File.ReadAllText(this.sgPath + name);
                try
                {
                    this.skipTextChange = true;
                    this.txtTitle.Text = this.ExtractString(s, "title");
                    this.OldTitle = this.txtTitle.Text;
                    this.txtPresentation.Text = this.ExtractString(s, "presentation");
                    this.txtBody.Text = this.ExtractString(s, "lyrics");
                    this.txtBody.Focus();
                    this.savedStatus(true);
                    this.skipTextChange = false;
                }
                catch
                {
                    int num = (int)MessageBox.Show("Unable to read this song !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                int num1 = (int)MessageBox.Show("That song doesnt exist !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void boxFind_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void boxFind_TextChanged(object sender, EventArgs e)
        {
            if (this.boxFind.Text == "!path")
                this.ChangePath();
            if (!(this.boxFind.Text == "!search"))
                return;
            List<string> songs = new List<string>();
            foreach (string str in this.listSongs.Items)
                songs.Add(str);
            new frmSearch(songs, this.sgPath).Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnAnnounce = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtPresentation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.listSongs = new System.Windows.Forms.ListBox();
            this.lstContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToFavoritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.boxFind = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listSaved = new System.Windows.Forms.ListBox();
            this.favContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.saveImg = new System.Windows.Forms.PictureBox();
            this.lstContext.SuspendLayout();
            this.favContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saveImg)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAnnounce
            // 
            this.btnAnnounce.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnAnnounce.Location = new System.Drawing.Point(12, 42);
            this.btnAnnounce.Name = "btnAnnounce";
            this.btnAnnounce.Size = new System.Drawing.Size(114, 27);
            this.btnAnnounce.TabIndex = 0;
            this.btnAnnounce.Text = "Announce";
            this.btnAnnounce.UseVisualStyleBackColor = true;
            this.btnAnnounce.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(258, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Title:";
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTitle.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(357, 12);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(274, 22);
            this.txtTitle.TabIndex = 2;
            this.txtTitle.TextChanged += new System.EventHandler(this.txtPresentation_TextChanged);
            // 
            // txtPresentation
            // 
            this.txtPresentation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPresentation.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPresentation.Location = new System.Drawing.Point(357, 46);
            this.txtPresentation.Name = "txtPresentation";
            this.txtPresentation.Size = new System.Drawing.Size(274, 22);
            this.txtPresentation.TabIndex = 4;
            this.txtPresentation.TextChanged += new System.EventHandler(this.txtPresentation_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(258, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Presentation:";
            // 
            // txtBody
            // 
            this.txtBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBody.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtBody.Location = new System.Drawing.Point(261, 75);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBody.Size = new System.Drawing.Size(370, 308);
            this.txtBody.TabIndex = 5;
            this.txtBody.TextChanged += new System.EventHandler(this.txtPresentation_TextChanged);
            // 
            // listSongs
            // 
            this.listSongs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listSongs.ContextMenuStrip = this.lstContext;
            this.listSongs.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listSongs.FormattingEnabled = true;
            this.listSongs.Location = new System.Drawing.Point(12, 75);
            this.listSongs.Name = "listSongs";
            this.listSongs.Size = new System.Drawing.Size(237, 303);
            this.listSongs.TabIndex = 7;
            this.listSongs.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listSongs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            this.listSongs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseUp);
            // 
            // lstContext
            // 
            this.lstContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToFavoritesToolStripMenuItem,
            this.deleteSongToolStripMenuItem});
            this.lstContext.Name = "lstContext";
            this.lstContext.Size = new System.Drawing.Size(144, 48);
            this.lstContext.Opening += new System.ComponentModel.CancelEventHandler(this.lstContext_Opening);
            // 
            // addToFavoritesToolStripMenuItem
            // 
            this.addToFavoritesToolStripMenuItem.Name = "addToFavoritesToolStripMenuItem";
            this.addToFavoritesToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.addToFavoritesToolStripMenuItem.Text = "Add to saved";
            this.addToFavoritesToolStripMenuItem.Click += new System.EventHandler(this.addToFavoritesToolStripMenuItem_Click);
            // 
            // deleteSongToolStripMenuItem
            // 
            this.deleteSongToolStripMenuItem.Name = "deleteSongToolStripMenuItem";
            this.deleteSongToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.deleteSongToolStripMenuItem.Text = "Delete song";
            this.deleteSongToolStripMenuItem.Click += new System.EventHandler(this.deleteSongToolStripMenuItem_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(9, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Search:";
            // 
            // boxFind
            // 
            this.boxFind.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.boxFind.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.boxFind.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.boxFind.FormattingEnabled = true;
            this.boxFind.Location = new System.Drawing.Point(67, 13);
            this.boxFind.Name = "boxFind";
            this.boxFind.Size = new System.Drawing.Size(185, 21);
            this.boxFind.TabIndex = 9;
            this.boxFind.SelectedIndexChanged += new System.EventHandler(this.boxFind_SelectedIndexChanged);
            this.boxFind.TextChanged += new System.EventHandler(this.boxFind_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(682, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Saved Songs";
            // 
            // listSaved
            // 
            this.listSaved.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSaved.ContextMenuStrip = this.favContext;
            this.listSaved.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listSaved.FormattingEnabled = true;
            this.listSaved.Location = new System.Drawing.Point(646, 42);
            this.listSaved.Name = "listSaved";
            this.listSaved.Size = new System.Drawing.Size(150, 342);
            this.listSaved.TabIndex = 11;
            this.listSaved.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // favContext
            // 
            this.favContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.favContext.Name = "favContext";
            this.favContext.Size = new System.Drawing.Size(180, 26);
            this.favContext.Opening += new System.ComponentModel.CancelEventHandler(this.favContext_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.deleteToolStripMenuItem.Text = "Remove from saved";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnAddNew.Location = new System.Drawing.Point(132, 42);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(117, 27);
            this.btnAddNew.TabIndex = 12;
            this.btnAddNew.Text = "Add new";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.button2_Click);
            // 
            // saveImg
            // 
            this.saveImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveImg.Location = new System.Drawing.Point(577, 345);
            this.saveImg.Name = "saveImg";
            this.saveImg.Size = new System.Drawing.Size(32, 32);
            this.saveImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.saveImg.TabIndex = 14;
            this.saveImg.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 397);
            this.Controls.Add(this.saveImg);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.listSaved);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.boxFind);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.listSongs);
            this.Controls.Add(this.txtBody);
            this.Controls.Add(this.txtPresentation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAnnounce);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(824, 435);
            this.Name = "frmMain";
            this.Text = "SongProject 0.9.7-beta1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.lstContext.ResumeLayout(false);
            this.favContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.saveImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
