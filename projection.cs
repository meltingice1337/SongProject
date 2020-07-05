// Decompiled with JetBrains decompiler
// Type: SongProject.projection
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SongProject
{
  public class projection : Form
  {
    public List<Song> sg;
    public string title;
    public string presentation;
    private projection.Modes mode;
    private IContainer components;
    private PictureBox pictureBox1;
    public Label lblContent;
    public Label lblTitle;
    private PictureBox freezePic;

    public projection()
    {
      this.InitializeComponent();
      this.sg = new List<Song>();
    }

    private void BoundToImage(Label lbl)
    {
      Point client = this.pictureBox1.PointToClient(this.PointToScreen(lbl.Location));
      lbl.Parent = (Control) this.pictureBox1;
      lbl.Location = client;
      lbl.BackColor = Color.Transparent;
    }

    public void SetText(List<string> song)
    {
      for (int index = 1; index < song.Count; ++index)
      {
        this.sg.Add(new Song());
        this.sg[index - 1].ID = song[index].Substring(0, song[index].IndexOf("]"));
        this.sg[index - 1].Text = song[index].Remove(0, this.sg[index - 1].ID.Length + 1);
      }
    }

    private void projection_Load(object sender, EventArgs e)
    {
      this.pictureBox1.Image = Image.FromFile("bg.png");
      this.mode = projection.Modes.Normal;
      this.lblTitle.Text = this.title;
      this.BoundToImage(this.lblTitle);
      this.BoundToImage(this.lblContent);
      this.lblTitle.Location = new Point(this.Width / 2 - this.lblTitle.Size.Width / 2, this.lblTitle.Location.Y);
      this.lblContent.Location = new Point(this.Width / 2 - this.lblContent.Size.Width / 2, this.lblContent.Location.Y);
      Rectangle rectangle = new Rectangle(new Point(0, 0), new Size(this.Size.Width - 5, this.Size.Height - this.lblTitle.Size.Height - 15));
      this.lblContent.Location = rectangle.Location;
      this.lblContent.Size = rectangle.Size;
      this.lblTitle.Visible = false;
      this.lblTitle.Text = "";
    }

    public void SetVerse(string ID)
    {
      this.lblContent.Visible = false;
      string str = "";
      for (int index = 0; index < this.sg.Count; ++index)
      {
        if (this.sg[index].ID.ToUpper() == ID.ToUpper())
          str = this.sg[index].Text;
      }
      this.lblContent.Text = this.FormatText(str.Trim());
      if (ID != "")
        this.lblTitle.Text = this.title;
      else
        this.lblContent.Text = this.lblTitle.Text = "";
      if (this.mode == projection.Modes.Normal)
        this.lblTitle.Visible = true;
      this.lblContent.Visible = true;
      this.SetPreviewImage();
    }

    public void SetPreviewImage()
    {
      new Thread(new ThreadStart(this.SetPreviewImageThread)).Start();
    }

    public void SetPreviewImageThread()
    {
      projection projection = this;
      using (Bitmap bmp = new Bitmap(projection.Width, projection.Height))
      {
        this.DrawBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
        using (Image image1 = (Image) bmp)
        {
          Image image2 = (Image) new Bitmap(image1.Width, image1.Height, PixelFormat.Format32bppArgb);
          using (Graphics graphics = Graphics.FromImage(image2))
            graphics.DrawImage(image1, 0, 0);
          frmControl openForm = (frmControl) Application.OpenForms["frmControl"];
          Graphics graphics1 = Graphics.FromImage(image2);
          if (this.mode == projection.Modes.Hide)
          {
            graphics1.DrawRectangle(new Pen((Brush) new SolidBrush(Color.LightBlue), 30f), 0, 0, this.Width, this.Height);
            graphics1.DrawLine(new Pen((Brush) new SolidBrush(Color.LightBlue), 30f), new Point(0, 0), new Point(this.Width, this.Height));
          }
          else if (this.mode == projection.Modes.Freeze)
          {
            graphics1.DrawRectangle(new Pen((Brush) new SolidBrush(Color.DarkRed), 30f), 0, 0, this.Width, this.Height);
            graphics1.DrawLine(new Pen((Brush) new SolidBrush(Color.DarkRed), 30f), new Point(0, 0), new Point(this.Width, this.Height));
          }
          else if (this.mode == projection.Modes.White)
            graphics1.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Width, this.Height));
          openForm.previewBox.Image = image2;
        }
      }
    }

    public void DrawBitmap(Bitmap bmp, Rectangle pt)
    {
      if (this.InvokeRequired)
        this.Invoke((Delegate) new projection.DrawBitmapCallBack(this.DrawBitmap), (object) bmp, (object) pt);
      else
        this.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
    }

    private void projection_FormClosing(object sender, FormClosingEventArgs e)
    {
      ((frmMain) Application.OpenForms["frmMain"]).isProjecting = false;
      this.lblTitle.Text = "";
      this.lblContent.Text = "";
    }

    public bool AproxEqual(int n1, int n2, int error)
    {
      return n1 > n2 && n1 - n2 <= error || n1 < n2 && n2 - n1 <= error;
    }

    public string FormatText(string str)
    {
      string str1 = "";
      int num1 = 35;
      using (StringReader stringReader = new StringReader(str))
      {
        string str2;
        while ((str2 = stringReader.ReadLine()) != null)
        {
          int length = str2.Length;
          string str3 = str2.Trim();
          if (length > num1)
          {
            int num2 = length / 2;
            str1 += str3.Substring(0, length / 2);
            for (int index = length / 2; index < length && str3[index] != ' '; ++index)
            {
              str1 += str3[index].ToString();
              num2 = index;
            }
            str1 = str1 + Environment.NewLine + str3.Substring(num2 + 1).Trim();
          }
          else
            str1 += str3;
          str1 += Environment.NewLine;
        }
      }
      return str1;
    }

    public void HideVerse()
    {
      if (this.mode == projection.Modes.Hide)
      {
        this.NormalVerse();
      }
      else
      {
        Bitmap bitmap = new Bitmap(this.Width, this.Height);
        Graphics.FromImage((Image) bitmap).FillRectangle(Brushes.Black, new Rectangle(0, 0, this.Width, this.Height));
        this.freezePic.Image = (Image) bitmap;
        this.freezePic.Visible = true;
        this.mode = projection.Modes.Hide;
        this.SetPreviewImage();
      }
    }

    public void WhiteScreen()
    {
      if (this.mode == projection.Modes.White)
      {
        this.NormalVerse();
      }
      else
      {
        Bitmap bitmap = new Bitmap(this.Width, this.Height);
        Graphics.FromImage((Image) bitmap).FillRectangle(Brushes.White, new Rectangle(0, 0, this.Width, this.Height));
        this.freezePic.Image = (Image) bitmap;
        this.freezePic.Visible = true;
        this.mode = projection.Modes.White;
        this.SetPreviewImage();
      }
    }

    public void FreezeVerse()
    {
      if (this.mode == projection.Modes.Freeze)
      {
        this.NormalVerse();
      }
      else
      {
        projection projection = this;
        using (Bitmap bmp = new Bitmap(projection.Width, projection.Height))
        {
          this.DrawBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
          using (Image image1 = (Image) bmp)
          {
            Image image2 = (Image) new Bitmap(image1.Width, image1.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(image2))
              graphics.DrawImage(image1, 0, 0);
            this.freezePic.Image = image2;
          }
        }
        this.freezePic.Visible = true;
        this.mode = projection.Modes.Freeze;
        this.SetPreviewImage();
      }
    }

    public void NormalVerse()
    {
      if (this.mode == projection.Modes.Normal)
        return;
      this.lblTitle.Visible = this.lblContent.Visible = true;
      this.freezePic.Visible = false;
      this.mode = projection.Modes.Normal;
      this.SetPreviewImage();
    }

    private void label2_TextChanged(object sender, EventArgs e)
    {
      if (this.lblContent.Text == "")
        return;
      this.lblContent.Font = new Font(this.lblContent.Font.FontFamily, 150f, this.lblContent.Font.Style);
      this.lblContent.Text = this.lblContent.Text.Trim();
      while (this.lblContent.Height < TextRenderer.MeasureText(this.lblContent.Text, new Font(this.lblContent.Font.FontFamily, this.lblContent.Font.Size, this.lblContent.Font.Style)).Height)
        this.lblContent.Font = new Font(this.lblContent.Font.FontFamily, this.lblContent.Font.Size - 1f, this.lblContent.Font.Style);
      while (this.lblContent.Width < TextRenderer.MeasureText(this.lblContent.Text, new Font(this.lblContent.Font.FontFamily, this.lblContent.Font.Size, this.lblContent.Font.Style)).Width)
        this.lblContent.Font = new Font(this.lblContent.Font.FontFamily, this.lblContent.Font.Size - 1f, this.lblContent.Font.Style);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (projection));
      this.lblContent = new Label();
      this.lblTitle = new Label();
      this.pictureBox1 = new PictureBox();
      this.freezePic = new PictureBox();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      ((ISupportInitialize) this.freezePic).BeginInit();
      this.SuspendLayout();
      this.lblContent.Font = new Font("Trebuchet MS", 54f, FontStyle.Regular, GraphicsUnit.Point, (byte) 238);
      this.lblContent.ForeColor = Color.White;
      this.lblContent.Location = new Point(12, 9);
      this.lblContent.Name = "lblContent";
      this.lblContent.Size = new Size(508, 296);
      this.lblContent.TabIndex = 2;
      this.lblContent.TextAlign = ContentAlignment.MiddleCenter;
      this.lblContent.TextChanged += new EventHandler(this.label2_TextChanged);
      this.lblTitle.Anchor = AnchorStyles.Bottom;
      this.lblTitle.AutoSize = true;
      this.lblTitle.Font = new Font("Trebuchet MS", 30f, FontStyle.Italic, GraphicsUnit.Point, (byte) 238);
      this.lblTitle.ForeColor = Color.White;
      this.lblTitle.Location = new Point(231, 327);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new Size(37, 49);
      this.lblTitle.TabIndex = 4;
      this.lblTitle.Text = ".";
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(532, 398);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.freezePic.Dock = DockStyle.Fill;
      this.freezePic.Location = new Point(0, 0);
      this.freezePic.Name = "freezePic";
      this.freezePic.Size = new Size(532, 398);
      this.freezePic.SizeMode = PictureBoxSizeMode.StretchImage;
      this.freezePic.TabIndex = 9;
      this.freezePic.TabStop = false;
      this.freezePic.Visible = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(532, 398);
      this.Controls.Add((Control) this.freezePic);
      this.Controls.Add((Control) this.lblTitle);
      this.Controls.Add((Control) this.lblContent);
      this.Controls.Add((Control) this.pictureBox1);
      this.FormBorderStyle = FormBorderStyle.None;
      this.Name = "projection";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "projectoion#proj#";
      this.FormClosing += new FormClosingEventHandler(this.projection_FormClosing);
      this.Load += new EventHandler(this.projection_Load);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      ((ISupportInitialize) this.freezePic).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private enum Modes
    {
      Normal,
      Hide,
      Freeze,
      White,
    }

    private delegate void DrawBitmapCallBack(Bitmap bmp, Rectangle pt);
  }
}
