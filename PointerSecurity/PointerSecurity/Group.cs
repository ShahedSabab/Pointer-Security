using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointerSecurity
{
    public class Group<T> : List<T>
    {
        public DateTime Date
        {
            get;
            set;
        }
        public Group(DateTime date, IEnumerable<T> items)
            : base(items)
        {
            this.Date = date;
        }
    }
}
