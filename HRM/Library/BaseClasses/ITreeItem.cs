using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Library.BaseClasses
{
    interface ITreeItem
    {
        string NodeID { get; }
        string NodeName { get; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }        
        string ParentID { get; }
        ObservableCollection<ITreeItem> Children { get; set; }
        ITreeItem Parent { get; set; }

        void ExpandTree(ITreeItem Branch);

    }
}
