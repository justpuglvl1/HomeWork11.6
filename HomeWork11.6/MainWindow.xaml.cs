using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeWork11._6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Kernel core = new Kernel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_OnClick_Debug(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Чики Брики v.0.234", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearData()
        {
            empList.ItemsSource = null;
            CompanyList.ItemsSource = null;
            empList.Items.Clear();
            CompanyList.Items.Clear();
        }

        private void MenuItem_OnClick_Generate(object sender, RoutedEventArgs e)
        {
            ClearData();
            CompanyList.Items.Add(CreateTreeItem(core.CreateOrg(5)[0]));
        }

        private void MenuItem_OnClick_Clear(object sender, RoutedEventArgs e)
        {
            ClearData();
        }

        private void MenuItem_OnClick_Load(object sender, RoutedEventArgs e)
        {
            ClearData();
            CompanyList.Items.Add(CreateTreeItem(core.LoadData()[0]));
        }

        private void MenuItem_OnClick_Save(object sender, RoutedEventArgs e)
        {
            core.SaveData();
        }

        private void CompanyList_OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if (item.Items[0] != null)
                return;
            item.Items.Clear();
            var d = item.Tag as Organisation;
            var subDepartments = core.GetSubDepts(d.Id);

            foreach (Organisation dep in subDepartments)
                item.Items.Add(CreateTreeItem(dep));
        }

        private TreeViewItem CreateTreeItem(Organisation dept)
        {
            TreeViewItem item = new TreeViewItem
            {
                Header = dept.Title,
                Tag = dept
            };
            var subDept = core.GetSubDepts(dept.Id);
            if (subDept.Count > 0)
            {
                item.Items.Add(null);
            }
            item.Selected += Item_Selected;
            return item;
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            var dep = (e.OriginalSource as TreeViewItem).Tag as Organisation;
            empList.ItemsSource = dep.Employees;
        }
    }
}
