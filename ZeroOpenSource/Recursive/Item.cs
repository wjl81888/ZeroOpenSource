using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recursive
{
    public class Item : BaseTree<Item>
    {
        public string Id { get; set; } = null!;

        public string ParentId { get; set; } = null!;

        public List<Item> ChildList { get; set; } = null!;
    }
}
