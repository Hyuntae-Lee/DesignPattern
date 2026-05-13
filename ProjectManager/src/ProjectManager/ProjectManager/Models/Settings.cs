using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    public class Settings
    {
        public DateTime BeginTime { set; get; }
        public DateTime EndTime { set; get; }

        public Settings()
        {
            BeginTime = new DateTime(DateTime.Now.Year, 1, 1);
            EndTime = new DateTime(DateTime.Now.Year, 12, 31);
        }
    }
}
