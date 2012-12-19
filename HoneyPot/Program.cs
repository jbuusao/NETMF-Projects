/*
 * HoneyPot
 * 
 * A simple honey-pot web server that mimics a Netgear router on port 80. Logs on attempts (with provided login/passwords) on the SD Card
 * Usage:
 * 1) Initialize the date-time:  http://{host}/time?year={yyy}&month={mm}&day={dd}&hour={HH}&minute={MM}&tz={TZ}
 * 2) Clear log files            http://{host}/clear
 * 3) Make sure that your honeypot is reachable via Internet
 * 4) Retrieve the logs          http://{host}/log.csv
 *      
 * 17 Jan 2011  -- Quiche31 - initial release
 * 
 * */
using System;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using astra.http;

public class Program
{
    HttpImplementation webServer;
    Log logger = new Log("HoneyPot");

    public static void Main()
    {
        new Program();
    }

    public Program()
    {
        bool wifly = false;
        if (wifly)
            webServer = new HttpWiflyImpl(processRequest, 80, HttpWiflyImpl.DeviceType.crystal_14_MHz, SPI.SPI_module.SPI1, SecretLabs.NETMF.Hardware.NetduinoPlus.Pins.GPIO_PIN_D10);
        else
            webServer = new HttpSocketImpl(processRequest);
        Debug.Print("Listening on " + webServer.getIP());
        webServer.Listen();
    }

    void processRequest(HttpContext context)
    {
        Boolean redirecting = false;
        String authURL = "/cgi-bin/check_hj.html";
        String tz = "GMT+1";
        String basic = "Basic ";
        String[] keys = { "username", "password" };
        String ip = null;
        String content = "";
        String target = context.Request.Path;
        Boolean isLocal = context.Request.Host != null && context.Request.Host.IndexOf("192.168.") != -1;

        if (isLocal && target == "/time")
        {
            try
            {
                int year = int.Parse(context.Request.getParameter("year"));
                int month = int.Parse(context.Request.getParameter("month"));
                int day = int.Parse(context.Request.getParameter("day"));
                int hour = int.Parse(context.Request.getParameter("hour"));
                int minute = int.Parse(context.Request.getParameter("minute"));
                tz = context.Request.getParameter("tz");
                Utility.SetLocalTime(new DateTime(year, month, day, hour, minute, 0));
            }
            catch (ArgumentNullException) { }
            content = "<html><h1>" + getDate(DateTime.Now, tz) + "</h1></html>\n";
        }
        else if (isLocal && target == "/log.csv")
        {
            StringBuilder sb = new StringBuilder();
            logger.List(sb, keys);
            content = sb.ToString();
            context.Response.ContentType = "text/csv";
        }
        else if (isLocal && target == "/clear")
        {
            logger.clear();
            content = "cleared";
        }
        else if (isLocal && target == "/stat")
        {
            Debug.GC(true);
        }
        else if (target == "/favicon.ico")
        {
            context.Response.ContentType = "image/jpg";
            context.Response.LastModified = "Mon, 27 Jan 2011 08:45:19 GMT";
            context.Response.ContentLength = Resources.icon1.Length;
            context.Response.BinaryWrite(Resources.icon1);
            context.Response.Close();
        }
        else if (target == authURL)
        {
            content = Resources.redirectForm;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(Resources.unAuthorizedForm, authURL);
            content = sb.ToString();
            context.Response.ErrorCode = "401 Authorization Required";
            context.Response.Add("WWW-Authenticate", "Basic realm=\"NETGEAR WNR2000\"");
            String credentials = context.Request.Authorization;
            int i;
            if (credentials != null && credentials.Length != 0 && (i = credentials.IndexOf(basic)) != -1)
            {
                byte[] dataDecoded = System.ConvertBase64.FromBase64String(credentials.Substring(i + basic.Length));
                string decodedCredentials = new string(Encoding.UTF8.GetChars(dataDecoded));
                String username = decodedCredentials;
                String password = "";
                if ((i = decodedCredentials.IndexOf(':')) != -1)
                {
                    username = decodedCredentials.Substring(0, i);
                    password = decodedCredentials.Substring(i + 1);
                }
                logger.Save("username=" + username + "\r\npassword=" + password);
            }
        }
        else
        {
            context.Response.setRedirect(authURL);
            redirecting = true;
        }
        int l = content.Length;

        Debug.Print("\n\nRequest received for " + context.Request.RawUrl);

        if (context.Response.ContentType == null || context.Response.ContentType.Length == 0)
            context.Response.ContentType = "text/html";
        context.Response.ContentLength = l;
        context.Response.Add("server", "Boa/0.94.11");
        context.Response.Add("Cache-Control", "no-cache");
        context.Response.Add("Pragma", "no-cache");
        context.Response.Add("Expires", "0");
        context.Response.Date = getDate(DateTime.Now, tz);
        context.Response.Write(content);
        if (!redirecting)
            context.Response.Close();
        redirecting = false;
    }

    static String getDate(DateTime dt, String tz)
    {
        return day[(int)dt.DayOfWeek] + ", " + dt.Day + ' ' + month[dt.Month - 1] + ' ' + dt.Year + ' ' +
            digits(dt.Hour) + ':' + digits(dt.Minute) + ':' + digits(dt.Second) + ' ' + tz;
    }

    static String digits(int t)
    {
        return t < 10 ? "0" + t : "" + t;
    }

    static String[] day = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
    static String[] month = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
}
