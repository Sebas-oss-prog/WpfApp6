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
    public partial class ClientEditDialog : Window
    {
        public Client Client { get; private set; }

        public ClientEditDialog()
        {
            InitializeComponent();
            Client = new Client();
            DataContext = Client;
        }

        public ClientEditDialog(Client clientToEdit)
        {
            InitializeComponent();
            Client = new Client
            {
                ClientID = clientToEdit.ClientID,
                CompanyName = clientToEdit.CompanyName,
                ContactPerson = clientToEdit.ContactPerson,
                Phone = clientToEdit.Phone,
                Email = clientToEdit.Email,
                IsActive = clientToEdit.IsActive
            };
            DataContext = Client;
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
