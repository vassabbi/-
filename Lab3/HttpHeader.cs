using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab3
{
    class HttpHeader
    {
        public string Source { get; set; }
        public string HTTPVersion { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public void FindHost()
        {
            Regex regex = new Regex(@"Host: (((?<host>.+?):(?<port>\d+?))|(?<host>.+?))\s+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match m = regex.Match(Source);
            this.Host = m.Groups["host"].Value;
            if (!int.TryParse(m.Groups["port"].Value, out int port))
            {
                port = 80;
            }
            this.Port = port;
        }
        public HttpHeader(byte[] source)
        {
            this.Source = Encoding.ASCII.GetString(source);
            string httpInfo;
            if (Source.IndexOf("\r\n") >= 0)
            {
                httpInfo = Source.Substring(0, Source.IndexOf("\r\n"));
            }
            else
            {
                this.StatusCode = 0;
                this.StatusMessage = "Error";
                return;
            }
            Regex regex = new Regex(@"(?<method>.+)\s+(?<path>.+)\s+HTTP/(?<version>[\d\.]+)", RegexOptions.Multiline);
            if (regex.IsMatch(httpInfo))
            {
                Match m = regex.Match(httpInfo);
                this.Method = m.Groups["method"].Value.ToUpper();
                this.Path = m.Groups["path"].Value;
                this.HTTPVersion = m.Groups["version"].Value;
            }
            else
            {
                regex = new Regex(@"HTTP/(?<version>[\d\.]+)\s+(?<status>\d+)\s*(?<msg>.*)");
                Match m = regex.Match(httpInfo);
                this.HTTPVersion = m.Groups["version"].Value;
                this.StatusCode = int.Parse(m.Groups["status"].Value);
                this.StatusMessage = m.Groups["msg"].Value;

            }
            FindHost();
        }
    }
}
