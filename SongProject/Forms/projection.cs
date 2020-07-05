// Decompiled with JetBrains decompiler
// Type: SongProject.Forms.projection
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SongProject.Forms
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
  [DebuggerNonUserCode]
  internal class projection
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal projection()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) projection.resourceMan, (object) null))
          projection.resourceMan = new ResourceManager("SongProject.Forms.projection", typeof (projection).Assembly);
        return projection.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return projection.resourceCulture;
      }
      set
      {
        projection.resourceCulture = value;
      }
    }

    internal static Bitmap pictureBox1_Image
    {
      get
      {
        return (Bitmap) projection.ResourceManager.GetObject("pictureBox1.Image", projection.resourceCulture);
      }
    }
  }
}
