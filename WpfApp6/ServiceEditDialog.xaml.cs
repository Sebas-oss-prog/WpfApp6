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
using System.Windows.Shapes;

namespace WpfApp6
{
    public partial class ServiceEditDialog : Window
    {
        public Service Service { get; private set; }

        public ServiceEditDialog()
        {
            InitializeComponent();
            Service = new Service();
            DataContext = Service;
        }

        public ServiceEditDialog(Service serviceToEdit)
        {
            InitializeComponent();
            Service = new Service
            {
                ServiceID = serviceToEdit.ServiceID,
                ServiceName = serviceToEdit.ServiceName,
                Description = serviceToEdit.Description,
                Category = serviceToEdit.Category,
                BasePrice = serviceToEdit.BasePrice,
                DurationDays = serviceToEdit.DurationDays,
                IsActive = serviceToEdit.IsActive
            };
            DataContext = Service;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
