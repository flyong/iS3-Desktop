using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
   public interface IProjectList
    {
        event EventHandler<Project> ProjectViewTriggle;
        ProjectList myProjectList { get; set; }
    }
}
