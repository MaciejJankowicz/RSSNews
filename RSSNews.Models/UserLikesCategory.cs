using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSNews.Models
{
    public class UserLikesCategory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Category { get; set; }

        public int Likes { get; set; }
    }
}
