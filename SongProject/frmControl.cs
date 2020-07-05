// Decompiled with JetBrains decompiler
// Type: SongProject.frmControl
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SongProject
{
  public class frmControl : Form
  {
    public string pres;
    private int offset;
    private projection frm;
    private IContainer components;
    public ListView listVerses;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private Button button1;
    private Button button2;
    public PictureBox previewBox;
    private Button button3;
    private Button button4;
    private Button button5;
    private Button button6;
    private Button button7;

    public frmControl(string presentation)
    {
      this.InitializeComponent();
      this.pres = presentation;
    }

    private void frmControl_Load(object sender, EventArgs e)
    {
      this.frm = (projection) Application.OpenForms["projection"];
      this.LoadText(this.frm.sg);
    }

    public void LoadText(List<Song> sg)
    {
      bool flag = false;
      string str = "";
      if (this.listVerses.Items.Count > 0)
      {
        str = this.listVerses.SelectedItems[0].SubItems[0].Text;
        flag = true;
      }
      this.listVerses.Items.Clear();
      this.listVerses.Items.Add(new ListViewItem(new string[2]
      {
        "",
        ""
      }));
      string presentation = this.frm.presentation;
      string[] strArray = presentation.Split(' ');
      if (presentation == "")
      {
        for (int index = 0; index < sg.Count; ++index)
          this.listVerses.Items.Add(new ListViewItem(new string[2]
          {
            sg[index].ID.ToUpper(),
            sg[index].Text.Trim()
          }));
      }
      else
      {
        for (int index1 = 0; index1 < strArray.Length; ++index1)
        {
          for (int index2 = 0; index2 < sg.Count; ++index2)
          {
            if (strArray[index1] == sg[index2].ID.ToUpper())
              this.listVerses.Items.Add(new ListViewItem(new string[2]
              {
                sg[index2].ID.ToUpper(),
                sg[index2].Text.Trim()
              }));
          }
        }
      }
      this.listVerses.Items.Add(new ListViewItem(new string[2]
      {
        "",
        ""
      }));
      if (!flag)
      {
        this.listVerses.Items[0].Selected = true;
      }
      else
      {
        for (int index = 1; index < this.listVerses.Items.Count; ++index)
        {
          if (this.listVerses.Items[index].SubItems[0].Text == str)
            this.listVerses.Items[index].Selected = true;
        }
      }
    }

    private void frmControl_KeyDown(object sender, KeyEventArgs e)
    {
      if (new List<string>()
      {
        "D1",
        "D2",
        "D3",
        "D4",
        "D5",
        "D6",
        "D7",
        "D8",
        "D9",
        "C"
      }.Contains(e.KeyData.ToString()))
      {
        e.Handled = true;
        string str = e.KeyData.ToString().Replace("D", "");
        if (str != "C")
        {
          foreach (ListViewItem listViewItem in this.listVerses.Items)
          {
            if (listViewItem.SubItems[0].Text == "V" + str || listViewItem.SubItems[0].Text == "S" + str)
              listViewItem.Selected = true;
          }
        }
        else
        {
          for (int index = this.listVerses.SelectedItems[0].Index + 1; index < this.listVerses.Items.Count; ++index)
          {
            if (this.listVerses.Items[index].SubItems[0].Text.StartsWith("C"))
            {
              this.listVerses.Items[index].Selected = true;
              break;
            }
          }
        }
      }
      else if (e.KeyCode == Keys.Escape)
      {
        e.SuppressKeyPress = e.Handled = true;
        this.Close();
      }
      else if (e.KeyCode == Keys.Up)
      {
        e.Handled = true;
        this.listVerses.Focus();
        int index = this.listVerses.SelectedItems[0].Index;
        if (index - 1 < 0)
          return;
        this.listVerses.Items[index - 1].Selected = true;
      }
      else if (e.KeyCode == Keys.Down)
      {
        e.Handled = true;
        this.listVerses.Focus();
        int index = this.listVerses.SelectedItems[0].Index;
        if (index + 1 >= this.listVerses.Items.Count)
          return;
        this.listVerses.Items[index + 1].Selected = true;
      }
      else if (e.KeyCode == Keys.N)
      {
        e.SuppressKeyPress = e.Handled = true;
        this.frm.NormalVerse();
      }
      else if (e.KeyCode == Keys.H)
      {
        e.SuppressKeyPress = e.Handled = true;
        Image image = this.previewBox.Image;
        Graphics graphics = Graphics.FromImage(image);
        graphics.DrawRectangle(new Pen((Brush) new SolidBrush(Color.LightBlue), 30f), 0, 0, this.frm.Width, this.frm.Height);
        graphics.DrawLine(new Pen((Brush) new SolidBrush(Color.LightBlue), 30f), new Point(0, 0), new Point(this.frm.Width, this.frm.Height));
        this.previewBox.Image = image;
        this.frm.HideVerse();
      }
      else if (e.KeyCode == Keys.W)
      {
        e.SuppressKeyPress = e.Handled = true;
        this.previewBox.BackColor = Color.White;
        this.frm.WhiteScreen();
      }
      else if (e.KeyCode == Keys.F)
      {
        Image image = this.previewBox.Image;
        Graphics graphics = Graphics.FromImage(image);
        graphics.DrawRectangle(new Pen((Brush) new SolidBrush(Color.DarkRed), 30f), 0, 0, this.frm.Width, this.frm.Height);
        graphics.DrawLine(new Pen((Brush) new SolidBrush(Color.DarkRed), 30f), new Point(0, 0), new Point(this.frm.Width, this.frm.Height));
        this.previewBox.Image = image;
        e.SuppressKeyPress = e.Handled = true;
        this.frm.FreezeVerse();
      }
      else
      {
        if (e.KeyCode != Keys.Next && e.KeyCode != Keys.Prior && (e.KeyCode != Keys.End && e.KeyCode != Keys.Home))
          return;
        e.SuppressKeyPress = e.Handled = true;
      }
    }

    private void frmControl_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.frm.Close();
    }

    private void listVerses_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
    }

    private void listVerses_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.listVerses.SelectedItems.Count <= 0)
        return;
      this.frm.SetVerse(this.listVerses.SelectedItems[0].SubItems[0].Text);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.offset = this.listVerses.SelectedItems[0].Index;
      this.frm.NormalVerse();
      this.listVerses.Focus();
      this.listVerses.Items[this.offset].Selected = true;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.offset = this.listVerses.SelectedItems[0].Index;
      Image image = this.previewBox.Image;
      Graphics graphics = Graphics.FromImage(image);
      graphics.DrawRectangle(new Pen((Brush) new SolidBrush(Color.LightBlue), 30f), 0, 0, this.frm.Width, this.frm.Height);
      graphics.DrawLine(new Pen((Brush) new SolidBrush(Color.LightBlue), 30f), new Point(0, 0), new Point(this.frm.Width, this.frm.Height));
      this.previewBox.Image = image;
      this.frm.HideVerse();
      this.listVerses.Focus();
      this.listVerses.Items[this.offset].Selected = true;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      Image image = this.previewBox.Image;
      Graphics graphics = Graphics.FromImage(image);
      graphics.DrawRectangle(new Pen((Brush) new SolidBrush(Color.DarkRed), 30f), 0, 0, this.frm.Width, this.frm.Height);
      graphics.DrawLine(new Pen((Brush) new SolidBrush(Color.DarkRed), 30f), new Point(0, 0), new Point(this.frm.Width, this.frm.Height));
      this.previewBox.Image = image;
      this.offset = this.listVerses.SelectedItems[0].Index;
      this.frm.FreezeVerse();
      this.listVerses.Focus();
      this.listVerses.Items[this.offset].Selected = true;
    }

    private void button4_Click(object sender, EventArgs e)
    {
      this.listVerses.Focus();
      int index = this.listVerses.SelectedItems[0].Index;
      if (index - 1 < 0)
        return;
      this.listVerses.Items[index - 1].Selected = true;
    }

    private void button5_Click(object sender, EventArgs e)
    {
      this.listVerses.Focus();
      int index = this.listVerses.SelectedItems[0].Index;
      if (index + 1 >= this.listVerses.Items.Count)
        return;
      this.listVerses.Items[index + 1].Selected = true;
    }

    private void button6_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void button7_Click(object sender, EventArgs e)
    {
      this.offset = this.listVerses.SelectedItems[0].Index;
      Image image = this.previewBox.Image;
      Graphics.FromImage(image).FillRectangle(Brushes.White, new Rectangle(0, 0, this.Width, this.Height));
      this.previewBox.Image = image;
      this.frm.WhiteScreen();
      this.listVerses.Focus();
      this.listVerses.Items[this.offset].Selected = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (frmControl));
      this.listVerses = new ListView();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.button1 = new Button();
      this.button2 = new Button();
      this.previewBox = new PictureBox();
      this.button3 = new Button();
      this.button4 = new Button();
      this.button5 = new Button();
      this.button6 = new Button();
      this.button7 = new Button();
      ((ISupportInitialize) this.previewBox).BeginInit();
      this.SuspendLayout();
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
      this.listVerses.Location = new Point(0, 0);
      this.listVerses.MultiSelect = false;
      this.listVerses.Name = "listVerses";
      this.listVerses.Size = new Size(440, 263);
      this.listVerses.TabIndex = 2;
      this.listVerses.UseCompatibleStateImageBehavior = false;
      this.listVerses.View = View.Details;
      this.listVerses.ColumnWidthChanging += new ColumnWidthChangingEventHandler(this.listVerses_ColumnWidthChanging);
      this.listVerses.SelectedIndexChanged += new EventHandler(this.listVerses_SelectedIndexChanged);
      this.listVerses.KeyDown += new KeyEventHandler(this.frmControl_KeyDown);
      this.columnHeader1.Text = "ID";
      this.columnHeader1.Width = 38;
      this.columnHeader2.Text = "Verse";
      this.columnHeader2.Width = 398;
      this.button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button1.FlatStyle = FlatStyle.Flat;
      this.button1.Location = new Point(456, 12);
      this.button1.Name = "button1";
      this.button1.Size = new Size(169, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "Normal ( N )";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button2.FlatStyle = FlatStyle.Flat;
      this.button2.Location = new Point(455, 60);
      this.button2.Name = "button2";
      this.button2.Size = new Size(169, 23);
      this.button2.TabIndex = 4;
      this.button2.Text = "Hide ( H )";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.previewBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.previewBox.BorderStyle = BorderStyle.FixedSingle;
      this.previewBox.Location = new Point(12, 269);
      this.previewBox.Name = "previewBox";
      this.previewBox.Size = new Size(361, 232);
      this.previewBox.SizeMode = PictureBoxSizeMode.StretchImage;
      this.previewBox.TabIndex = 5;
      this.previewBox.TabStop = false;
      this.button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button3.FlatStyle = FlatStyle.Flat;
      this.button3.Location = new Point(455, 89);
      this.button3.Name = "button3";
      this.button3.Size = new Size(169, 23);
      this.button3.TabIndex = 6;
      this.button3.Text = "Freeze ( F )";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new EventHandler(this.button3_Click);
      this.button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button4.FlatStyle = FlatStyle.Flat;
      this.button4.Location = new Point(457, 182);
      this.button4.Name = "button4";
      this.button4.Size = new Size(169, 23);
      this.button4.TabIndex = 7;
      this.button4.Text = "UP";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new EventHandler(this.button4_Click);
      this.button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button5.FlatStyle = FlatStyle.Flat;
      this.button5.Location = new Point(456, 211);
      this.button5.Name = "button5";
      this.button5.Size = new Size(169, 23);
      this.button5.TabIndex = 8;
      this.button5.Text = "Down";
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new EventHandler(this.button5_Click);
      this.button6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button6.FlatStyle = FlatStyle.Flat;
      this.button6.Location = new Point(455, 240);
      this.button6.Name = "button6";
      this.button6.Size = new Size(169, 23);
      this.button6.TabIndex = 9;
      this.button6.Text = "Exit";
      this.button6.UseVisualStyleBackColor = true;
      this.button6.Click += new EventHandler(this.button6_Click);
      this.button7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button7.FlatStyle = FlatStyle.Flat;
      this.button7.Location = new Point(457, 118);
      this.button7.Name = "button7";
      this.button7.Size = new Size(169, 23);
      this.button7.TabIndex = 10;
      this.button7.Text = "White ( W )";
      this.button7.UseVisualStyleBackColor = true;
      this.button7.Click += new EventHandler(this.button7_Click);
      this.AutoScaleDimensions = new SizeF(7f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(636, 513);
      this.Controls.Add((Control) this.button7);
      this.Controls.Add((Control) this.button6);
      this.Controls.Add((Control) this.button5);
      this.Controls.Add((Control) this.button4);
      this.Controls.Add((Control) this.button3);
      this.Controls.Add((Control) this.previewBox);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.listVerses);
      this.Font = new Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.KeyPreview = true;
      this.Name = "frmControl";
      this.Text = "SongProject - Control";
      this.FormClosing += new FormClosingEventHandler(this.frmControl_FormClosing);
      this.Load += new EventHandler(this.frmControl_Load);
      this.KeyDown += new KeyEventHandler(this.frmControl_KeyDown);
      ((ISupportInitialize) this.previewBox).EndInit();
      this.ResumeLayout(false);
    }
  }
}
