using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netcloud_App
{
    public class Main_MenuFlyoutMenuItem
    {
        public Main_MenuFlyoutMenuItem()
        {
            TargetType = typeof(Main_MenuFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}