using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Simple1
{
    [DisplayName("扫描功能插件")]
    public class ScanerPlugin : IScanerPlugin
    {
        public string DoScan(string target)
        {
            var mockResult = $"Target={target} : this is the scaner result from ScanerPlugin.";
            return mockResult;
        }
    }
}
