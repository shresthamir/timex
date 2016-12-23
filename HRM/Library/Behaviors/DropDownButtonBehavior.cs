using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace HRM.Library.Behaviors
{
    public class DropDownButtonBehavior : Behavior<Button>
    {
        private long attachedCount;
        private bool isContextMenuOpen;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
            if (AssociatedObject.DataContext != null && (AssociatedObject.DataContext as RootViewModel).HasGroup)
                AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.DataContext != null && (AssociatedObject.DataContext as RootViewModel).HasGroup)
                AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
            else
                AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button source = sender as Button;
            if (source != null && source.ContextMenu != null)
            {
                // Only open the ContextMenu when it is not already open. If it is already open,
                // when the button is pressed the ContextMenu will lose focus and automatically close.
                if (!isContextMenuOpen)
                {
                    source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                    Interlocked.Increment(ref attachedCount);
                    // If there is a drop-down assigned to this button, then position and display it 
                    source.ContextMenu.PlacementTarget = source;
                    source.ContextMenu.Placement = PlacementMode.Bottom;
                    source.ContextMenu.IsOpen = true;
                    isContextMenuOpen = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            isContextMenuOpen = false;
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
                Interlocked.Decrement(ref attachedCount);
            }
        }
    }
}
