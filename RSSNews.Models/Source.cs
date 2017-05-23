using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSNews.Models
{
    public class Source
    {
        [Key]
        public string Address { get; set; }
        public string Category { get; set; }
        
        public virtual ICollection<News> News { get; set; }
        public Source()
        {
            News = new HashSet<News>();
        }
    }
}
