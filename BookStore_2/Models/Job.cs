using System;
using System.Collections.Generic;

namespace BookStore_2.Models
{
    public partial class Job
    {
        public Job()
        {
            User = new HashSet<User>();
        }

        public short JobId { get; set; }
        public string JobDesc { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
