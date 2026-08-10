using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.DAL;

    public partial class BudgetPriority
    {
        public byte PriorityId { get; set; }

        public string Description { get; set; } = null!;

        public virtual ICollection<SpendingBucket> Buckets { get; set; } = new List<SpendingBucket>();
    }

