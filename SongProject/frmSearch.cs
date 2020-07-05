// Decompiled with JetBrains decompiler
// Type: SongProject.frmSearch
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SongProject
{
  public class frmSearch : Form
  {
    public List<string> songs;
    private string sgPath;
    private Thread thread;
    private IContainer components;
    private Label label1;
    public TextBox txtQuery;
    private Button button1;
    private ListView listVerses;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private Label lblStatus;

    public frmSearch(List<string> songs, string sgPath)
    {
      this.InitializeComponent();
      this.songs = songs;
      this.sgPath = sgPath;
    }

    public void StartSearching()
    {
      this.listVerses.Items.Clear();
      this.thread = new Thread((ThreadStart) (() => this.SearchThread()));
      this.thread.Start();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.StartSearching();
    }

    public void SearchThread()
    {
      string text = this.txtQuery.Text;
      for (int index1 = 0; index1 < this.songs.Count; ++index1)
      {
        SongFile song = this.GetSong(this.songs[index1]);
        if (song != null)
        {
          string str1 = song.Text.ToLower();
          string[] strArray1 = new string[9]
          {
            ",",
            ".",
            "!",
            "?",
            "ă",
            "î",
            "â",
            "ș",
            "ț"
          };
          string[] strArray2 = new string[9]
          {
            "",
            "",
            "",
            "",
            "a",
            "i",
            "a",
            "s",
            "t"
          };
          for (int index2 = 0; index2 < strArray1.Length; ++index2)
            str1 = str1.Replace(strArray1[index2], strArray2[index2]);
          if (song.Title.ToLower().Contains(text))
            this.AddItems(song.Title, "In title");
          else if (str1.Contains(text))
          {
            int num1 = str1.ToLower().IndexOf(text);
            int num2 = 30;
            string str2 = num1 - num2 < 0 ? (num1 + num2 >= song.Text.Length ? song.Text.Substring(0) : song.Text.Substring(0, num2 * 2)) : (num1 + num2 >= song.Text.Length ? song.Text.Substring(num1 - num2) : song.Text.Substring(num1 - num2, num2 * 2));
            this.AddItems(song.Title, "..." + str2 + "...");
          }
          this.UpdateStatus("Current Song : " + song.Title);
        }
      }
      this.UpdateStatus("Done. Found: " + (object) this.listVerses.Items.Count);
    }

    public SongFile GetSong(string name)
    {
      if (name.Replace(" ", "") == "")
        return (SongFile) null;
      SongFile songFile = new SongFile();
      if (!File.Exists(this.sgPath + name))
        return (SongFile) null;
      string s = File.ReadAllText(this.sgPath + name);
      try
      {
        songFile.Title = this.ExtractString(s, "title");
        songFile.Presentation = this.ExtractString(s, "presentation");
        songFile.Text = this.ExtractString(s, "lyrics");
      }
      catch
      {
        return (SongFile) null;
      }
      return songFile;
    }

    private string ExtractString(string s, string tag)
    {
      string str = "<" + tag + ">";
      int startIndex = s.IndexOf(str) + str.Length;
      int num = s.IndexOf("</" + tag + ">", startIndex);
      return s.Substring(startIndex, num - startIndex);
    }

    private void AddItems(string title, string around)
    {
      if (this.InvokeRequired)
        this.Invoke((Delegate) new frmSearch.AddItemsCallback(this.AddItems), (object) title, (object) around);
      else
        this.listVerses.Items.Add(new ListViewItem(new string[2]
        {
          title,
          around
        }));
    }

    private void UpdateStatus(string status)
    {
      if (this.InvokeRequired)
        this.Invoke((Delegate) new frmSearch.UpdateStatusCallback(this.UpdateStatus), (object) status);
      else
        this.lblStatus.Text = status;
    }

    private void frmSearch_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Escape)
        return;
      this.thread.Abort();
      this.UpdateStatus("Found : " + (object) this.listVerses.Items.Count);
    }

    private void frmSearch_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Escape)
        return;
      this.thread.Abort();
      this.UpdateStatus("Found : " + (object) this.listVerses.Items.Count);
    }

    private void listVerses_DoubleClick(object sender, EventArgs e)
    {
      ((frmMain) Application.OpenForms["frmMain"]).listSongs.SelectedItem = (object) this.listVerses.SelectedItems[0].SubItems[0].Text.Trim();
    }

    private void listVerses_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
      int num = (int) MessageBox.Show(e.SubItem.ToString());
    }

    private void frmSearch_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.thread.Abort();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (frmSearch));
      this.label1 = new Label();
      this.txtQuery = new TextBox();
      this.button1 = new Button();
      this.listVerses = new ListView();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.lblStatus = new Label();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Location = new Point(14, 9);
      this.label1.Name = "label1";
      this.label1.Size = new Size(42, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Query";
      this.txtQuery.Location = new Point(17, 25);
      this.txtQuery.Name = "txtQuery";
      this.txtQuery.Size = new Size(444, 21);
      this.txtQuery.TabIndex = 1;
      this.button1.Location = new Point(467, 23);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 2;
      this.button1.Text = "Search";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.listVerses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.listVerses.Columns.AddRange(new ColumnHeader[2]
      {
        this.columnHeader1,
        this.columnHeader2
      });
      this.listVerses.FullRowSelect = true;
      this.listVerses.GridLines = true;
      this.listVerses.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.listVerses.LabelWrap = false;
      this.listVerses.Location = new Point(17, 52);
      this.listVerses.MultiSelect = false;
      this.listVerses.Name = "listVerses";
      this.listVerses.Size = new Size(525, 264);
      this.listVerses.TabIndex = 3;
      this.listVerses.UseCompatibleStateImageBehavior = false;
      this.listVerses.View = View.Details;
      this.listVerses.DoubleClick += new EventHandler(this.listVerses_DoubleClick);
      this.columnHeader1.Text = "Song Title";
      this.columnHeader1.Width = 134;
      this.columnHeader2.Text = "Verse";
      this.columnHeader2.Width = 381;
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new Point(14, 319);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new Size((int) sbyte.MaxValue, 13);
      this.lblStatus.TabIndex = 5;
      this.lblStatus.Text = "No search query yet.";
      this.AcceptButton = (IButtonControl) this.button1;
      this.AutoScaleDimensions = new SizeF(7f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(552, 339);
      this.Controls.Add((Control) this.lblStatus);
      this.Controls.Add((Control) this.listVerses);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.txtQuery);
      this.Controls.Add((Control) this.label1);
      this.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.KeyPreview = true;
      this.Name = "frmSearch";
      this.Text = "SongProject - Search";
      this.FormClosing += new FormClosingEventHandler(this.frmSearch_FormClosing);
      this.KeyDown += new KeyEventHandler(this.frmSearch_KeyDown);
      this.KeyUp += new KeyEventHandler(this.frmSearch_KeyUp);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private delegate void AddItemsCallback(string title, string around);

    private delegate void UpdateStatusCallback(string status);
  }
}
