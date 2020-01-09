using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application
{
    public class PagedData<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }
    }
}
