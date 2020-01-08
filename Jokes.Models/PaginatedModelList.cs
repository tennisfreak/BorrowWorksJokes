using System;
using System.Collections.Generic;
using System.Text;

namespace Jokes.Models
{
    public class PaginatedModelList<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public int TotalRecordCount { get; set; }

        public List<T> Items { get; set; }

        public PaginatedModelList()
        {
            Items = new List<T>();
        }

    }
}
