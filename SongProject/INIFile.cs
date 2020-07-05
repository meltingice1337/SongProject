// Decompiled with JetBrains decompiler
// Type: INIFile
// Assembly: SongProject, Version=0.9.7.0, Culture=neutral, PublicKeyToken=null
// MVID: F2A3432A-C342-451A-9DBB-7BF8102450B2
// Assembly location: C:\Users\dariu\Desktop\SongProject\SongProject.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

internal class INIFile
{
  private object m_Lock = new object();
  private Dictionary<string, Dictionary<string, string>> m_Sections = new Dictionary<string, Dictionary<string, string>>();
  private Dictionary<string, Dictionary<string, string>> m_Modified = new Dictionary<string, Dictionary<string, string>>();
  private string m_FileName;
  private bool m_Lazy;
  private bool m_AutoFlush;
  private bool m_CacheModified;

  internal string FileName
  {
    get
    {
      return this.m_FileName;
    }
  }

  public INIFile(string FileName)
  {
    this.Initialize(FileName, false, false);
  }

  public INIFile(string FileName, bool Lazy, bool AutoFlush)
  {
    this.Initialize(FileName, Lazy, AutoFlush);
  }

  private void Initialize(string FileName, bool Lazy, bool AutoFlush)
  {
    this.m_FileName = FileName;
    this.m_Lazy = Lazy;
    this.m_AutoFlush = AutoFlush;
    if (this.m_Lazy)
      return;
    this.Refresh();
  }

  private string ParseSectionName(string Line)
  {
    if (!Line.StartsWith("["))
      return (string) null;
    if (!Line.EndsWith("]"))
      return (string) null;
    return Line.Length < 3 ? (string) null : Line.Substring(1, Line.Length - 2);
  }

  private bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
  {
    int length1;
    if ((length1 = Line.IndexOf('=')) <= 0)
      return false;
    int length2 = Line.Length - length1 - 1;
    Key = Line.Substring(0, length1).Trim();
    if (Key.Length <= 0)
      return false;
    Value = length2 > 0 ? Line.Substring(length1 + 1, length2).Trim() : "";
    return true;
  }

  internal void Refresh()
  {
    lock (this.m_Lock)
    {
      StreamReader streamReader = (StreamReader) null;
      try
      {
        this.m_Sections.Clear();
        this.m_Modified.Clear();
        try
        {
          streamReader = new StreamReader(this.m_FileName);
        }
        catch (FileNotFoundException ex)
        {
          return;
        }
        Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
        string Key = (string) null;
        string str1 = (string) null;
        string str2;
        while ((str2 = streamReader.ReadLine()) != null)
        {
          string Line = str2.Trim();
          string sectionName = this.ParseSectionName(Line);
          if (sectionName != null)
          {
            if (this.m_Sections.ContainsKey(sectionName))
            {
              dictionary = (Dictionary<string, string>) null;
            }
            else
            {
              dictionary = new Dictionary<string, string>();
              this.m_Sections.Add(sectionName, dictionary);
            }
          }
          else if (dictionary != null && this.ParseKeyValuePair(Line, ref Key, ref str1) && !dictionary.ContainsKey(Key))
            dictionary.Add(Key, str1);
        }
      }
      finally
      {
        streamReader?.Close();
      }
    }
  }

  internal void Flush()
  {
    lock (this.m_Lock)
      this.PerformFlush();
  }

  private void PerformFlush()
  {
    if (!this.m_CacheModified)
      return;
    this.m_CacheModified = false;
    bool flag1 = File.Exists(this.m_FileName);
    string str1 = Path.ChangeExtension(this.m_FileName, "$n$");
    StreamWriter streamWriter1 = (StreamWriter) null;
    StreamWriter streamWriter2 = new StreamWriter(str1);
    try
    {
      Dictionary<string, string> dictionary1 = (Dictionary<string, string>) null;
      if (flag1)
      {
        StreamReader streamReader = (StreamReader) null;
        try
        {
          streamReader = new StreamReader(this.m_FileName);
          string Key = (string) null;
          string str2 = (string) null;
          bool flag2 = true;
          while (flag2)
          {
            string Line = streamReader.ReadLine();
            flag2 = Line != null;
            bool flag3;
            string key1;
            if (flag2)
            {
              flag3 = true;
              Line = Line.Trim();
              key1 = this.ParseSectionName(Line);
            }
            else
            {
              flag3 = false;
              key1 = (string) null;
            }
            if (key1 != null || !flag2)
            {
              if (dictionary1 != null && dictionary1.Count > 0)
              {
                foreach (string key2 in dictionary1.Keys)
                {
                  if (dictionary1.TryGetValue(key2, out str2))
                  {
                    streamWriter2.Write(key2);
                    streamWriter2.Write('=');
                    streamWriter2.WriteLine(str2);
                  }
                }
                streamWriter2.WriteLine();
                dictionary1.Clear();
              }
              if (flag2 && !this.m_Modified.TryGetValue(key1, out dictionary1))
                dictionary1 = (Dictionary<string, string>) null;
            }
            else if (dictionary1 != null && this.ParseKeyValuePair(Line, ref Key, ref str2) && dictionary1.TryGetValue(Key, out str2))
            {
              flag3 = false;
              dictionary1.Remove(Key);
              streamWriter2.Write(Key);
              streamWriter2.Write('=');
              streamWriter2.WriteLine(str2);
            }
            if (flag3)
              streamWriter2.WriteLine(Line);
          }
          streamReader.Close();
          streamReader = (StreamReader) null;
        }
        finally
        {
          streamReader?.Close();
        }
      }
      foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair1 in this.m_Modified)
      {
        Dictionary<string, string> dictionary2 = keyValuePair1.Value;
        if (dictionary2.Count > 0)
        {
          streamWriter2.WriteLine();
          streamWriter2.Write('[');
          streamWriter2.Write(keyValuePair1.Key);
          streamWriter2.WriteLine(']');
          foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
          {
            streamWriter2.Write(keyValuePair2.Key);
            streamWriter2.Write('=');
            streamWriter2.WriteLine(keyValuePair2.Value);
          }
          dictionary2.Clear();
        }
      }
      this.m_Modified.Clear();
      streamWriter2.Close();
      streamWriter2 = (StreamWriter) null;
      File.Copy(str1, this.m_FileName, true);
      File.Delete(str1);
    }
    finally
    {
      streamWriter2?.Close();
      streamWriter1 = (StreamWriter) null;
    }
  }

  internal string GetValue(string SectionName, string Key, string DefaultValue)
  {
    if (this.m_Lazy)
    {
      this.m_Lazy = false;
      this.Refresh();
    }
    lock (this.m_Lock)
    {
      Dictionary<string, string> dictionary;
      string str;
      return !this.m_Sections.TryGetValue(SectionName, out dictionary) || !dictionary.TryGetValue(Key, out str) ? DefaultValue : str;
    }
  }

  internal void SetValue(string SectionName, string Key, string Value)
  {
    if (this.m_Lazy)
    {
      this.m_Lazy = false;
      this.Refresh();
    }
    lock (this.m_Lock)
    {
      this.m_CacheModified = true;
      Dictionary<string, string> dictionary;
      if (!this.m_Sections.TryGetValue(SectionName, out dictionary))
      {
        dictionary = new Dictionary<string, string>();
        this.m_Sections.Add(SectionName, dictionary);
      }
      if (dictionary.ContainsKey(Key))
        dictionary.Remove(Key);
      dictionary.Add(Key, Value);
      if (!this.m_Modified.TryGetValue(SectionName, out dictionary))
      {
        dictionary = new Dictionary<string, string>();
        this.m_Modified.Add(SectionName, dictionary);
      }
      if (dictionary.ContainsKey(Key))
        dictionary.Remove(Key);
      dictionary.Add(Key, Value);
      if (!this.m_AutoFlush)
        return;
      this.PerformFlush();
    }
  }

  private string EncodeByteArray(byte[] Value)
  {
    if (Value == null)
      return (string) null;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (byte num in Value)
    {
      string str = Convert.ToString(num, 16);
      int length = str.Length;
      if (length > 2)
      {
        stringBuilder.Append(str.Substring(length - 2, 2));
      }
      else
      {
        if (length < 2)
          stringBuilder.Append("0");
        stringBuilder.Append(str);
      }
    }
    return stringBuilder.ToString();
  }

  private byte[] DecodeByteArray(string Value)
  {
    if (Value == null)
      return (byte[]) null;
    int length1 = Value.Length;
    if (length1 < 2)
      return new byte[0];
    int length2 = length1 / 2;
    byte[] numArray = new byte[length2];
    for (int index = 0; index < length2; ++index)
      numArray[index] = Convert.ToByte(Value.Substring(index * 2, 2), 16);
    return numArray;
  }

  internal bool GetValue(string SectionName, string Key, bool DefaultValue)
  {
    int result;
    return int.TryParse(this.GetValue(SectionName, Key, DefaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)), out result) ? result != 0 : DefaultValue;
  }

  internal int GetValue(string SectionName, string Key, int DefaultValue)
  {
    int result;
    return int.TryParse(this.GetValue(SectionName, Key, DefaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : DefaultValue;
  }

  internal long GetValue(string SectionName, string Key, long DefaultValue)
  {
    long result;
    return long.TryParse(this.GetValue(SectionName, Key, DefaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : DefaultValue;
  }

  internal double GetValue(string SectionName, string Key, double DefaultValue)
  {
    double result;
    return double.TryParse(this.GetValue(SectionName, Key, DefaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : DefaultValue;
  }

  internal byte[] GetValue(string SectionName, string Key, byte[] DefaultValue)
  {
    string str = this.GetValue(SectionName, Key, this.EncodeByteArray(DefaultValue));
    try
    {
      return this.DecodeByteArray(str);
    }
    catch (FormatException ex)
    {
      return DefaultValue;
    }
  }

  internal DateTime GetValue(string SectionName, string Key, DateTime DefaultValue)
  {
    DateTime result;
    return DateTime.TryParse(this.GetValue(SectionName, Key, DefaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out result) ? result : DefaultValue;
  }

  internal void SetValue(string SectionName, string Key, bool Value)
  {
    this.SetValue(SectionName, Key, Value ? "1" : "0");
  }

  internal void SetValue(string SectionName, string Key, int Value)
  {
    this.SetValue(SectionName, Key, Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  internal void SetValue(string SectionName, string Key, long Value)
  {
    this.SetValue(SectionName, Key, Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  internal void SetValue(string SectionName, string Key, double Value)
  {
    this.SetValue(SectionName, Key, Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  internal void SetValue(string SectionName, string Key, byte[] Value)
  {
    this.SetValue(SectionName, Key, this.EncodeByteArray(Value));
  }

  internal void SetValue(string SectionName, string Key, DateTime Value)
  {
    this.SetValue(SectionName, Key, Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
