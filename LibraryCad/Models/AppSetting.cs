using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.Models
{
    public class AppSetting
    {
        public string Data;
        public int Status;

        public AppSetting(string data, int status)
        {
            this.Data = data;
            this.Status = status;
        }
    }
}
