using System;
using System.Collections.Generic;
using System.Text;

namespace Simple1
{
    public interface IScanerPlugin
    {
        string DoScan(string target);
    }
}
