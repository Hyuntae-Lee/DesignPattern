using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    public class ProjectResourceStorage
    {
        public List<ProjectResource> projectResources;

        public ProjectResourceStorage()
        {
            projectResources = new List<ProjectResource>();
        }
    }
}
