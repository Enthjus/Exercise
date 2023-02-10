using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.Models
{
    public class StyleInfo
    {
        public List<String> TextStyles { get; set; }
        public List<String> DimStyles { get; set; }
        public List<String> TableStyles { get; set; }
        public List<String> MLeaderStyles { get; set; }
    }
}
