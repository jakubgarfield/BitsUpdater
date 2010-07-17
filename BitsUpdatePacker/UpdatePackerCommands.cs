using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BitsUpdatePacker
{
    public sealed class UpdatePackerCommands
    {
        public static readonly RoutedUICommand CreatePackage = new RoutedUICommand("CreatePackage", "CreatePackage", typeof(UpdatePackerCommands));
        public static readonly RoutedUICommand SelectFolder = new RoutedUICommand("SelectFolder", "SelectFolder", typeof(UpdatePackerCommands));
        public static readonly RoutedUICommand SelectFile = new RoutedUICommand("SelectFile", "SelectFile", typeof(UpdatePackerCommands));
        public static readonly RoutedUICommand AddNewTemplate = new RoutedUICommand("AddNewTemplate", "AddNewTemplate", typeof(UpdatePackerCommands));
        public static readonly RoutedUICommand RemoveTemplate = new RoutedUICommand("RemoveTemplate", "RemoveTemplate", typeof(UpdatePackerCommands));
    }
}
