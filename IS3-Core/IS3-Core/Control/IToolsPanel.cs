using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    //Summary
    //    拓展工具控件接口
    //
    public interface IToolsPanel
    {
        ToolTreeItem toolboxesTree { get; set; }
        void init(List<ToolTreeItem> toolTreeList);
    }
}
