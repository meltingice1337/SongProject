// Decompiled with JetBrains decompiler
// Type: HookClass
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using SongProject;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal class HookClass
{
  private const int WM_KEYUP = 257;
  private static HookClass.HookProc keyProc;
  private static IntPtr keyHook;

  [DllImport("kernel32.dll")]
  private static extern IntPtr GetModuleHandle(string moduleName);

  [DllImport("user32.dll")]
  private static extern IntPtr SetWindowsHookEx(
    int idHook,
    HookClass.HookProc lpfn,
    IntPtr hMod,
    uint dwThreadId);

  [DllImport("user32.dll")]
  public static extern int UnhookWindowsHookEx(IntPtr hhook);

  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(
    IntPtr hhk,
    int nCode,
    uint wParam,
    IntPtr lParam);

  private static IntPtr LowLevelKeyboardProc(int nCode, uint wParam, IntPtr lParam)
  {
    if (nCode >= 0)
    {
      HookClass.KBDLLHOOKSTRUCT structure = (HookClass.KBDLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof (HookClass.KBDLLHOOKSTRUCT));
      if (wParam == 257U)
      {
        frmControl openForm = (frmControl) Application.OpenForms["frmControl"];
        if (openForm != null)
        {
          int count = openForm.listVerses.Items.Count;
          int selectedIndex = openForm.listVerses.SelectedIndices[0];
          if (structure.vkCode == 34)
          {
            if (selectedIndex + 1 <= count - 1)
              openForm.listVerses.Items[selectedIndex + 1].Selected = true;
            return HookClass.CallNextHookEx(HookClass.keyHook, 0, 0U, IntPtr.Zero);
          }
          if (structure.vkCode == 33)
          {
            if (selectedIndex - 1 >= 0)
              openForm.listVerses.Items[selectedIndex - 1].Selected = true;
            return HookClass.CallNextHookEx(HookClass.keyHook, 0, 0U, IntPtr.Zero);
          }
        }
      }
    }
    return HookClass.CallNextHookEx(HookClass.keyHook, nCode, wParam, lParam);
  }

  public static void StartHook()
  {
    HookClass.keyProc = new HookClass.HookProc(HookClass.LowLevelKeyboardProc);
    HookClass.keyHook = HookClass.SetWindowsHookEx(13, HookClass.keyProc, HookClass.GetModuleHandle((string) null), 0U);
  }

  private struct KBDLLHOOKSTRUCT
  {
    public int vkCode;
    public int scanCode;
    public int flags;
    public int time;
    public IntPtr dwExtraInfo;
  }

  private delegate IntPtr HookProc(int nCode, uint wParam, IntPtr lParam);
}
