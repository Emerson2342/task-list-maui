using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskListMaui.Source.Main.ValueObjects
{
    public class Email
    {
        public string Address { get; set; } = string.Empty;
        public Email(string address)
        {
            Address = address;            
        }
    }


}
