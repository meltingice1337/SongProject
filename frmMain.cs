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
        this.setFormLocation((Form) projection, allScreens[1]);
      else
        this.setFormLocation((Form) projection, allScreens[0]);
      projection.SetText(new List<string>((IEnumerable<string>) presentation.Split('[')));
      projection.title = str;
      projection.presentation = "1";
      projection.Show();
      new frmControl(presentation).Show();
      this.frm = (projection) Application.OpenForms["projection"];
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
          int num = (int) MessageBox.Show("You can't edit another song while projecting !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        this.listSongs.SelectedItem = (object) this.txtTitle.Text;
      }
      else
      {
        this.boxFind.Text = "";
        object selectedItem = this.listSongs.SelectedItem;
        if (!this._saved & this.txtTitle.Text.Replace(" ", "") != "")
        {
          if (MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
          {
            if (File.Exists(this.sgPath + (object) this.txtTitle) && this.newSong)
            {
              int num = (int) MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
        this.boxFind.Text += ((char) e.KeyValue).ToString().ToLower();
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
      this.lstContext.Show((Control) this.listSongs, e.Location);
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!this._saved & this.txtTitle.Text.Replace(" ", "") != "" && MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
      {
        if (File.Exists(this.sgPath + (object) this.txtTitle) && this.newSong)
        {
          int num = (int) MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
        this.frm = (projection) Application.OpenForms["projection"];
        this.frm.Close();
        Application.OpenForms["frmControl"].Close();
        this.isProjecting = false;
      }
      else if (e.Control && e.KeyCode == Keys.F)
        this.boxFind.Focus();
      else if (e.Control && e.KeyCode == Keys.S && !this._saved)
      {
        if (File.Exists(this.sgPath + (object) this.txtTitle) && this.newSong)
        {
          int num1 = (int) MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
          this.listSongs.SelectedItem = (object) this.OldTitle;
          this.newSong = false;
          this.listSongs.Focus();
          if (!this.isProjecting)
            return;
          this.frm.sg.Clear();
          this.frm.SetText(new List<string>((IEnumerable<string>) this.txtBody.Text.Split('[')));
          this.frm.title = this.txtTitle.Text;
          this.frm.presentation = this.txtPresentation.Text;
          ((frmControl) Application.OpenForms["frmControl"]).LoadText(this.frm.sg);
        }
      }
      else if (e.KeyCode == Keys.Return && this.boxFind.Focused && this.boxFind.Text != "")
      {
        if (this.isProjecting)
        {
          this.boxFind.Text = "";
          int num2 = (int) MessageBox.Show("You can't edit another song while projecting !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        else
        {
          this.listSongs.SelectedItem = (object) this.boxFind.Text;
          if (this.listSongs.Items.Contains((object) this.boxFind.Text) || !(this.boxFind.Text.Trim() != ""))
            return;
          frmSearch openForm = (frmSearch) Application.OpenForms["frmSearch"];
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
          int num2 = (int) MessageBox.Show("You can't delete this song while you projecting It !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
      this.GetSong(this.listSaved.SelectedItem.ToString());
      this.txtBody.SelectionStart = this.txtBody.Text.Length - 1;
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
      this.listSaved.Items.Add((object) this.listSongs.SelectedItem.ToString());
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
      button2.Click += (EventHandler) ((sender, e) => prompt.Close());
      prompt.Controls.Add((Control) textBox2);
      prompt.Controls.Add((Control) button2);
      prompt.Controls.Add((Control) label2);
      prompt.AcceptButton = (IButtonControl) button2;
      int num = (int) prompt.ShowDialog();
      return textBox2.Text;
    }

    public void Project(int i)
    {
      if (this.txtBody.Text.Trim() == "" || this.txtTitle.Text.Trim() == "")
      {
        int num1 = (int) MessageBox.Show("You cannot project a song without a title or without text !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else
      {
        if (!this._saved)
        {
          if (MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
          {
            if (File.Exists(this.sgPath + (object) this.txtTitle) && this.newSong)
            {
              int num2 = (int) MessageBox.Show("A song with this title already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
            int num2 = (int) MessageBox.Show("You cannot project a new song without saving it first !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
          }
        }
        if (this.isProjecting)
          return;
        projection projection = new projection();
        Screen[] allScreens = Screen.AllScreens;
        this.setFormLocation((Form) projection, allScreens[i]);
        projection.SetText(new List<string>((IEnumerable<string>) this.txtBody.Text.Split('[')));
        projection.title = this.txtTitle.Text;
        projection.presentation = this.txtPresentation.Text;
        projection.Show();
        new frmControl(this.txtPresentation.Text).Show();
        this.frm = (projection) Application.OpenForms["projection"];
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
        int num = (int) MessageBox.Show("Path changed successfully !" + Environment.NewLine + "Application will now exit !");
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
      this.listSongs.Items.AddRange((object[]) stringList.ToArray());
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
          int num = (int) MessageBox.Show("Unable to read this song !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
      }
      else
      {
        int num1 = (int) MessageBox.Show("That song doesnt exist !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (frmMain));
      this.btnAnnounce = new Button();
      this.label1 = new Label();
      this.txtTitle = new TextBox();
      this.txtPresentation = new TextBox();
      this.label2 = new Label();
      this.txtBody = new TextBox();
      this.listSongs = new ListBox();
      this.lstContext = new ContextMenuStrip(this.components);
      this.addToFavoritesToolStripMenuItem = new ToolStripMenuItem();
      this.deleteSongToolStripMenuItem = new ToolStripMenuItem();
      this.label4 = new Label();
      this.boxFind = new ComboBox();
      this.label3 = new Label();
      this.listSaved = new ListBox();
      this.favContext = new ContextMenuStrip(this.components);
      this.deleteToolStripMenuItem = new ToolStripMenuItem();
      this.btnAddNew = new Button();
      this.saveImg = new PictureBox();
      this.lstContext.SuspendLayout();
      this.favContext.SuspendLayout();
      ((ISupportInitialize) this.saveImg).BeginInit();
      this.SuspendLayout();
      this.btnAnnounce.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.btnAnnounce.Location = new Point(12, 42);
      this.btnAnnounce.Name = "btnAnnounce";
      this.btnAnnounce.Size = new Size(114, 27);
      this.btnAnnounce.TabIndex = 0;
      this.btnAnnounce.Text = "Announce";
      this.btnAnnounce.UseVisualStyleBackColor = true;
      this.btnAnnounce.Click += new EventHandler(this.button1_Click);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(258, 16);
      this.label1.Name = "label1";
      this.label1.Size = new Size(36, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Title:";
      this.txtTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtTitle.Font = new Font("Verdana", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtTitle.Location = new Point(357, 12);
      this.txtTitle.Name = "txtTitle";
      this.txtTitle.Size = new Size(274, 22);
      this.txtTitle.TabIndex = 2;
      this.txtTitle.TextChanged += new EventHandler(this.txtPresentation_TextChanged);
      this.txtPresentation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtPresentation.Font = new Font("Verdana", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtPresentation.Location = new Point(357, 46);
      this.txtPresentation.Name = "txtPresentation";
      this.txtPresentation.Size = new Size(274, 22);
      this.txtPresentation.TabIndex = 4;
      this.txtPresentation.TextChanged += new EventHandler(this.txtPresentation_TextChanged);
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label2.Location = new Point(258, 49);
      this.label2.Name = "label2";
      this.label2.Size = new Size(83, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Presentation:";
      this.txtBody.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.txtBody.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.txtBody.Location = new Point(261, 75);
      this.txtBody.Multiline = true;
      this.txtBody.Name = "txtBody";
      this.txtBody.ScrollBars = ScrollBars.Vertical;
      this.txtBody.Size = new Size(370, 308);
      this.txtBody.TabIndex = 5;
      this.txtBody.TextChanged += new EventHandler(this.txtPresentation_TextChanged);
      this.listSongs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.listSongs.ContextMenuStrip = this.lstContext;
      this.listSongs.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.listSongs.FormattingEnabled = true;
      this.listSongs.Location = new Point(12, 75);
      this.listSongs.Name = "listSongs";
      this.listSongs.Size = new Size(237, 303);
      this.listSongs.TabIndex = 7;
      this.listSongs.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
      this.listSongs.KeyDown += new KeyEventHandler(this.listBox1_KeyDown);
      this.listSongs.MouseUp += new MouseEventHandler(this.listBox1_MouseUp);
      this.lstContext.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.addToFavoritesToolStripMenuItem,
        (ToolStripItem) this.deleteSongToolStripMenuItem
      });
      this.lstContext.Name = "lstContext";
      this.lstContext.Size = new Size(144, 48);
      this.lstContext.Opening += new CancelEventHandler(this.lstContext_Opening);
      this.addToFavoritesToolStripMenuItem.Name = "addToFavoritesToolStripMenuItem";
      this.addToFavoritesToolStripMenuItem.Size = new Size(143, 22);
      this.addToFavoritesToolStripMenuItem.Text = "Add to saved";
      this.addToFavoritesToolStripMenuItem.Click += new EventHandler(this.addToFavoritesToolStripMenuItem_Click);
      this.deleteSongToolStripMenuItem.Name = "deleteSongToolStripMenuItem";
      this.deleteSongToolStripMenuItem.Size = new Size(143, 22);
      this.deleteSongToolStripMenuItem.Text = "Delete song";
      this.deleteSongToolStripMenuItem.Click += new EventHandler(this.deleteSongToolStripMenuItem_Click);
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.label4.Location = new Point(9, 16);
      this.label4.Name = "label4";
      this.label4.Size = new Size(52, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "Search:";
      this.boxFind.AutoCompleteMode = AutoCompleteMode.Suggest;
      this.boxFind.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.boxFind.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.boxFind.FormattingEnabled = true;
      this.boxFind.Location = new Point(67, 13);
      this.boxFind.Name = "boxFind";
      this.boxFind.Size = new Size(185, 21);
      this.boxFind.TabIndex = 9;
      this.boxFind.SelectedIndexChanged += new EventHandler(this.boxFind_SelectedIndexChanged);
      this.boxFind.TextChanged += new EventHandler(this.boxFind_TextChanged);
      this.label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label3.Location = new Point(682, 21);
      this.label3.Name = "label3";
      this.label3.Size = new Size(82, 13);
      this.label3.TabIndex = 10;
      this.label3.Text = "Saved Songs";
      this.listSaved.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      this.listSaved.ContextMenuStrip = this.favContext;
      this.listSaved.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.listSaved.FormattingEnabled = true;
      this.listSaved.Location = new Point(646, 42);
      this.listSaved.Name = "listSaved";
      this.listSaved.Size = new Size(150, 342);
      this.listSaved.TabIndex = 11;
      this.listSaved.SelectedIndexChanged += new EventHandler(this.listBox2_SelectedIndexChanged);
      this.favContext.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.deleteToolStripMenuItem
      });
      this.favContext.Name = "favContext";
      this.favContext.Size = new Size(180, 26);
      this.favContext.Opening += new CancelEventHandler(this.favContext_Opening);
      this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
      this.deleteToolStripMenuItem.Size = new Size(179, 22);
      this.deleteToolStripMenuItem.Text = "Remove from saved";
      this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
      this.btnAddNew.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.btnAddNew.Location = new Point(132, 42);
      this.btnAddNew.Name = "btnAddNew";
      this.btnAddNew.Size = new Size(117, 27);
      this.btnAddNew.TabIndex = 12;
      this.btnAddNew.Text = "Add new";
      this.btnAddNew.UseVisualStyleBackColor = true;
      this.btnAddNew.Click += new EventHandler(this.button2_Click);
      this.saveImg.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.saveImg.Location = new Point(577, 345);
      this.saveImg.Name = "saveImg";
      this.saveImg.Size = new Size(32, 32);
      this.saveImg.SizeMode = PictureBoxSizeMode.StretchImage;
      this.saveImg.TabIndex = 14;
      this.saveImg.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(808, 397);
      this.Controls.Add((Control) this.saveImg);
      this.Controls.Add((Control) this.btnAddNew);
      this.Controls.Add((Control) this.listSaved);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.boxFind);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.listSongs);
      this.Controls.Add((Control) this.txtBody);
      this.Controls.Add((Control) this.txtPresentation);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.txtTitle);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.btnAnnounce);
      this.KeyPreview = true;
      this.MinimumSize = new Size(824, 435);
      this.Name = "frmMain";
      this.Text = "SongProject 0.9.7 BETA";
      this.FormClosing += new FormClosingEventHandler(this.frmMain_FormClosing);
      this.Load += new EventHandler(this.frmMain_Load);
      this.Shown += new EventHandler(this.frmMain_Shown);
      this.KeyDown += new KeyEventHandler(this.frmMain_KeyDown);
      this.lstContext.ResumeLayout(false);
      this.favContext.ResumeLayout(false);
      ((ISupportInitialize) this.saveImg).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
