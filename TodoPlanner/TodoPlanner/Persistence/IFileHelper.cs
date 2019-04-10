using System;
using System.Collections.Generic;
using System.Text;

namespace TodoPlanner.Persistence
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
