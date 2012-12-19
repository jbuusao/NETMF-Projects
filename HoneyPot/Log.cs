/*
 * Log
 * 
 * A simple Log capability that uses the SD Card
 * Usage:
 * 1) Initialize the Log capability, passing some arbitrary application name (8.3 format):
 *     Log storage = new Log("MyApp");
 * 2) Make sure that you have initialized the system clock (used for timestamping)
 * 3) Create log entries (you can structure them as a, multi-line, colon-separated, string)
 *     storage.save("att1=foo\natt2=bar\natt3=gasp\n");
 *      
 * 18 Jan 2011  -- Quiche31 - initial release
 * 
 * */
using System;
using Microsoft.SPOT;   
using Microsoft.SPOT.IO;
using Microsoft.SPOT.Hardware;
using System.Collections;
using System.Text;

using System.IO;
using astra.http;

public class Log
{
    String root = null;
    static String error = "Unexpected error: no SD Card in slot, or SD reader unreachable";

    public Log(String context)
    {
        root = @"\SD\" + context;
        try
        {
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            root = root + "\\";
        }
        catch (IOException)
        {
            Debug.Print(error);
            root = null;
        }
    }

    public void clear()
    {
        if (root != null)
        {
            foreach (String dir in Directory.GetDirectories(root))
            {
                foreach (String filepath in Directory.GetFiles(dir))
                {
                    try
                    {
                        File.Delete(filepath);
                    }
                    catch (IOException)
                    {
                        Debug.Print("Unable to delete file " + filepath);
                    }
                }
            }
        }
    }

    public void Save(String content)
    {
        if (root != null)
        {
            DateTime now = DateTime.Now;
            String date = now.ToString("dd/MM/yy");
            String time = now.ToString("HH:mm:ss");
            // Build a folder name with the date
            String folderName = now.ToString("yy-MM-dd");
            // Build a filename that has the timestamp to the milliseconds (2 digits), hence filling in the 8 characters length
            String fileName = now.ToString("HHmmss") + (now.Millisecond % 100) + ".txt";
            String header = "Date=" + date + "\r\nTime=" + time + "\r\n";
            String path = root + folderName;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                path = path + "\\" + fileName;
                FileStream fStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                byte[] buffer = UTF8Encoding.UTF8.GetBytes(header + content);
                fStream.Write(buffer, 0, buffer.Length);
                fStream.Close();
                Debug.Print("  Created: " + path);
            }
            catch (IOException e)
            {
                Debug.Print(e.StackTrace);
            }
        }
    }

    public void List(StringBuilder sb, String []keys)
    {
        Hashtable info = new Hashtable();
        if (root == null)
            sb.Append(error);
        else
        {
            foreach (String dir in Directory.GetDirectories(root))
            {
                foreach (String filepath in Directory.GetFiles(dir))
                {
                    try
                    {
                        FileInfo fInfo = new FileInfo(filepath);
                        if (fInfo.Length < 2048)
                        {
                            int size = (int)fInfo.Length;
                            FileStream fStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None, size);
                            byte[] buffer = new byte[size];
                            fStream.Read(buffer, 0, buffer.Length);
                            parseContents(new String(UTF8Encoding.UTF8.GetChars(buffer)), info);
                            sb.Append((String)info["Date"] + ';' + (String)info["Time"]);
                            for (int i = 0; i < keys.Length; i++)
                                sb.Append(";" + (String)info[keys[i]]);
                            sb.Append('\n');
                            fStream.Close();
                        }
                    }
                    catch (IOException e)
                    {
                        Debug.Print(e.StackTrace);
                    }
                }
            }
        }
    }

    public void parseContents(String s, Hashtable info)
    {
        int i0, i1;
        while (s.Length != 0)
        {
            if ((i0 = s.IndexOf('=')) != -1)
            {
                if ((i1 = s.IndexOf('\n')) != -1)
                {
                    info[s.Substring(0, i0).Trim()] = s.Substring(i0 + 1, i1 - i0 - 1).Trim();
                    s = s.Substring(i1 + 1);
                }
                else
                {   // last attribute-value pair
                    info[s.Substring(0, i0).Trim()] = s.Substring(i0 + 1).Trim();
                    break;
                }
            }
            else
                break;
        }
    }
}
