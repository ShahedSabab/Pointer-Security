using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointerSecurity
{
    public class Pic
    {
        public string id { get; set; }
        public string loc { get; set; }
        public string date { get; set; }
    }

    public class RootObject
    {
        public List<Pic> pics { get; set; }
        public int success { get; set; }
    }
}
