using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab3
{
    class BlackList
    {
        List<string> forbittenDomains;
        public BlackList()
        {
            forbittenDomains = new List<string> { };
            StreamReader f = new StreamReader("Config.txt");
            string domain;
            while ((domain = f.ReadLine()) != null)
            {
                forbittenDomains.Add(domain);
            }
            f.Close();
        }
        public bool isForbitten(string domain)
        {
            return forbittenDomains.Contains(domain);
        }
    }
}
