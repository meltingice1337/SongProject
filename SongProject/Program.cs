// Decompiled with JetBrains decompiler
// Type: SongProject.Program
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System;
using System.Windows.Forms;

namespace SongProject
{
  internal sealed class Program
  {
    [STAThread]
    private static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new frmMain());
    }
  }
}
