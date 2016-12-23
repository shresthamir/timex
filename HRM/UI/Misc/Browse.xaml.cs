using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HRM.UI.Misc
{
    /// <summary>
    /// Interaction logic for Browse.xaml
    /// </summary>
    public partial class Browse : Window
    {
        public Browse()
        {
            InitializeComponent();            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "Ok")
            {
                if (SearchGrid.SelectedItem == null)
                {
                    MessageBox.Show("Please select an Item in the grid first", "Search Item", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                this.DialogResult = true;
            }
            else
                this.DialogResult = false;
            this.Close();
        }
    }

    class BrowseViewModel : RootModel
    {
        IEnumerable<object> _ItemList;
        string _SearchText;
        private ICollectionView _dataGridCollection;
        private List<string> PropertyList;

        public IEnumerable<object> ItemList { get { return _ItemList; } set { _ItemList = value; OnPropertyChanged("ItemList"); } }
        public string SearchText 
        { 
            get { return _SearchText; } 
            set 
            { 
                _SearchText = value; 
                OnPropertyChanged("SearchText");
                if (_dataGridCollection != null)
                {
                    _dataGridCollection.Refresh();
                }
            } 
        }
        public ICollectionView DataGridCollection { get { return _dataGridCollection; } set { _dataGridCollection = value; OnPropertyChanged("DataGridCollection"); } }


        public BrowseViewModel(IEnumerable<object> Source, params string[] Properties)
        {
            SearchText = string.Empty;
            ItemList = Source;
            PropertyList = new List<string>(Properties);
            DataGridCollection = CollectionViewSource.GetDefaultView(ItemList);
            DataGridCollection.Filter = new Predicate<object>(Filter);

        }
        public bool Filter(object obj)
        {
            if (obj != null)
            {
                foreach (string Props in PropertyList)
                {
                    object obj1 = obj;
                    if (obj1 == null)
                        continue;
                    PropertyInfo pi;
                    foreach (string str in Props.Split('.'))
                    {
                        pi = obj1.GetType().GetProperty(str);
                        obj1 = pi.GetValue(obj1, null);
                    }
                    if (obj1 != null && obj1.ToString().ToUpper().Contains(SearchText.ToUpper()))
                        return true;
                }
                return false;
            }
            return false;
        }
    }
}
