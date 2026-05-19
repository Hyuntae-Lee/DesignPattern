using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    class Project
    {
        public List<ProjectTask> projectTasks;

        public Project()
        {
            projectTasks = new List<ProjectTask>();
        }
    }
}
