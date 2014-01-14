using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISURLRewriteUtility.Entities
{
    public class Redirect
    {
        public Redirect(string line)
        {
            string[] redirect = line.Split(',');

            this.From = redirect[0].Trim();
            this.To = redirect[1].Trim();
        }

        public string To { get; set; }
        public string From { get; set; }

        public string Host
        {
            get
            {
                return URI.Host;
            }
        }

        public string Path
        {
            get
            {
                return URI.AbsolutePath;
            }
        }

        public Uri URI
        {
            get
            {
                var uri = new Uri(NormalizedFrom);

                return uri;
            }
        }

        private string NormalizedFrom
        {
            get
            {
                if (this.From.IndexOf("http://") < 0)
                {
                    return String.Format("http://{0}", this.From);
                }

                return this.From;
            }
        }
    }
}
