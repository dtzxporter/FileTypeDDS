using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTypeBootstrap
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return "DTZxPorter";
            }
        }

        public string Copyright
        {
            get
            {
                return "2017 DTZxPorter";
            }
        }

        public string DisplayName
        {
            get
            {
                return "FileTypeBootstrap";
            }
        }

        public Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://modme.co");
            }
        }
    }
}
