using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class PagedList<T> : List<T>
    {
        private IQueryable<T> source;

        public PagedList(int pageSize)
        {
            this.PageSize = pageSize;
        }

        public PagedList(IQueryable<T> source, int index, int pageSize):this(pageSize)
        {
            Initialize(source, index);
        }

        public void Initialize(IQueryable<T> source, int index)
        {
            this.source = source;
            this.TotalCount = source.Count();
            this.PageIndex = index;
            this.Clear();
            this.AddRange(source.Skip(index * PageSize).Take(PageSize).ToList());
        }

        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public IQueryable<T> Source
        {
            get
            {
                return source;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) * PageSize) < TotalCount;
            }
        }

        public int PageCount
        {
            get
            {
                return (int)Math.Floor((((double)TotalCount - 1d) / (double)PageSize)) + 1;
            }
        }
    }
}
