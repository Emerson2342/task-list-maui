using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskListMaui
{
   public static class Configuration
    {
            public static string IpAddress => Application.Current?.Resources["IpAddress"] as string ?? string.Empty;
            public static string LocalHost => Application.Current?.Resources["LocalHost"] as string ?? string.Empty;

        
    }
}
