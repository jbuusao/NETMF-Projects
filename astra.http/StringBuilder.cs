/*
 * $Id: StringBuilder.cs 6 2012-04-04 12:11:18Z jbuusao $
 * 
 * A custom StringBuilder implementation that also does formattting ("{0}...{1}..." etc)
 *  
 * Use this code for whatever you want. Modify it, redistribute it, at will
 * Just keep this header intact, however, and add your own modifications to it!
 * 
 * 10 Jan 2011  -- Quiche31 - Added suppport for AppendFormat
 * 12 Jan 2012  -- Arbiter  - Added GC to optimize dynamic memory footprint
 * 
 * */

using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;

namespace astra.http
{
    public class StringBuilder
    {
        char[] buffer = null;
        int size = 0;

        public StringBuilder(int size = 20)
        {
            buffer = new char[size];
        }

        public void Append(char c)
        {
            if (size + 1 >= buffer.Length)
            {
                char[] buffer2 = new char[buffer.Length * 4 / 3];
                Array.Copy(buffer, buffer2, buffer.Length);
                buffer = buffer2;
                Debug.GC(true);
            }
            buffer[size++] = c;
            buffer[size] = '\0';
        }

        public void Append(String s)
        {
            foreach (char c in s)
                Append(c);
        }

        public void Clear()
        {
            size = 0;
            buffer[0] = '\0';
        }
        public override String ToString()
        {
            return new String(buffer);
        }

        public void AppendFormat(String format, params object[] args)
        {
            int i = 0;
            foreach (object arg in args)
                format = substitute(format, "{" + (i++) + "}", arg);
            Clear();
            Append(format);
        }

        String substitute(string format, String key, object val)
        {
            int i;
            if (val == null) val = string.Empty;
            if ((i = format.IndexOf(key)) != -1)
                format = format.Substring(0, i) + val.ToString() + format.Substring(i + key.Length);
            Debug.GC(true);
            return format;
        }
    }
}