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
using System.Data;
using System.Data.SqlClient;

namespace WpfApp6
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadClients();
            LoadServices();
            LoadOrders();
        }

        private void LoadClients()
        {
            string query = "SELECT * FROM Clients";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            List<Client> clients = new List<Client>();
            foreach (DataRow row in dt.Rows)
            {
                clients.Add(new Client
                {
                    ClientID = Convert.ToInt32(row["ClientID"]),
                    CompanyName = row["CompanyName"].ToString(),
                    ContactPerson = row["ContactPerson"]?.ToString(),
                    Phone = row["Phone"].ToString(),
                    Email = row["Email"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }

            clientsGrid.ItemsSource = clients;
        }

        private void LoadServices()
        {
            string query = "SELECT * FROM Services";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            List<Service> services = new List<Service>();
            foreach (DataRow row in dt.Rows)
            {
                services.Add(new Service
                {
                    ServiceID = Convert.ToInt32(row["ServiceID"]),
                    ServiceName = row["ServiceName"].ToString(),
                    Category = row["Category"].ToString(),
                    BasePrice = Convert.ToDecimal(row["BasePrice"]),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }

            servicesGrid.ItemsSource = services;
        }

        private void LoadOrders()
        {
            string query = @"
            SELECT o.OrderID, c.CompanyName, o.OrderDate, o.TotalAmount, o.Status
            FROM Orders o
            JOIN Clients c ON o.ClientID = c.ClientID";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            List<OrderViewModel> orders = new List<OrderViewModel>();
            foreach (DataRow row in dt.Rows)
            {
                orders.Add(new OrderViewModel
                {
                    OrderID = Convert.ToInt32(row["OrderID"]),
                    CompanyName = row["CompanyName"].ToString(),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = row["Status"].ToString()
                });
            }

            ordersGrid.ItemsSource = orders;
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ClientEditDialog();
            if (dialog.ShowDialog() == true)
            {
                string query = @"
                INSERT INTO Clients (CompanyName, ContactPerson, Phone, Email, IsActive)
                VALUES (@CompanyName, @ContactPerson, @Phone, @Email, @IsActive)";

                SqlParameter[] parameters = {
                new SqlParameter("@CompanyName", dialog.Client.CompanyName),
                new SqlParameter("@ContactPerson", dialog.Client.ContactPerson),
                new SqlParameter("@Phone", dialog.Client.Phone),
                new SqlParameter("@Email", dialog.Client.Email),
                new SqlParameter("@IsActive", dialog.Client.IsActive)
            };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadClients();
            }
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ServiceEditDialog();
            if (dialog.ShowDialog() == true)
            {
                string query = @"
                INSERT INTO Services (ServiceName, Description, Category, BasePrice, DurationDays, IsActive)
                VALUES (@ServiceName, @Description, @Category, @BasePrice, @DurationDays, @IsActive)";

                SqlParameter[] parameters = {
                new SqlParameter("@ServiceName", dialog.Service.ServiceName),
                new SqlParameter("@Description", (object)dialog.Service.Description ?? DBNull.Value),
                new SqlParameter("@Category", dialog.Service.Category),
                new SqlParameter("@BasePrice", dialog.Service.BasePrice),
                new SqlParameter("@DurationDays", (object)dialog.Service.DurationDays ?? DBNull.Value),
                new SqlParameter("@IsActive", dialog.Service.IsActive)
            };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadServices();
            }
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            if (servicesGrid.SelectedItem is Service selectedService)
            {
                var dialog = new ServiceEditDialog(selectedService);
                if (dialog.ShowDialog() == true)
                {
                    string query = @"
                    UPDATE Services 
                    SET ServiceName = @ServiceName,
                        Description = @Description,
                        Category = @Category,
                        BasePrice = @BasePrice,
                        DurationDays = @DurationDays,
                        IsActive = @IsActive
                    WHERE ServiceID = @ServiceID";

                    SqlParameter[] parameters = {
                    new SqlParameter("@ServiceID", dialog.Service.ServiceID),
                    new SqlParameter("@ServiceName", dialog.Service.ServiceName),
                    new SqlParameter("@Description", (object)dialog.Service.Description ?? DBNull.Value),
                    new SqlParameter("@Category", dialog.Service.Category),
                    new SqlParameter("@BasePrice", dialog.Service.BasePrice),
                    new SqlParameter("@DurationDays", (object)dialog.Service.DurationDays ?? DBNull.Value),
                    new SqlParameter("@IsActive", dialog.Service.IsActive)
                };

                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                    LoadServices();
                }
            }
        }

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (servicesGrid.SelectedItem is Service selectedService)
            {
                if (MessageBox.Show("Удалить услугу?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string query = "DELETE FROM Services WHERE ServiceID = @ServiceID";
                    SqlParameter parameter = new SqlParameter("@ServiceID", selectedService.ServiceID);

                    DatabaseHelper.ExecuteNonQuery(query, new[] { parameter });
                    LoadServices();
                }
            }
        }


        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (clientsGrid.SelectedItem is Client selectedClient)
            {
                var dialog = new ClientEditDialog(selectedClient);
                if (dialog.ShowDialog() == true)
                {
                    string query = @"
                    UPDATE Clients 
                    SET CompanyName = @CompanyName, 
                        ContactPerson = @ContactPerson, 
                        Phone = @Phone, 
                        Email = @Email, 
                        IsActive = @IsActive
                    WHERE ClientID = @ClientID";

                    SqlParameter[] parameters = {
                    new SqlParameter("@ClientID", dialog.Client.ClientID),
                    new SqlParameter("@CompanyName", dialog.Client.CompanyName),
                    new SqlParameter("@ContactPerson", dialog.Client.ContactPerson),
                    new SqlParameter("@Phone", dialog.Client.Phone),
                    new SqlParameter("@Email", dialog.Client.Email),
                    new SqlParameter("@IsActive", dialog.Client.IsActive)
                };

                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                    LoadClients();
                }
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (clientsGrid.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show("Удалить клиента?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string query = "DELETE FROM Clients WHERE ClientID = @ClientID";
                    SqlParameter parameter = new SqlParameter("@ClientID", selectedClient.ClientID);

                    DatabaseHelper.ExecuteNonQuery(query, new[] { parameter });
                    LoadClients();
                }
            }
        }

        // Аналогичные методы для услуг (AddService_Click, EditService_Click, DeleteService_Click)
    }

    public class Client
    {
        public int ClientID { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class Service
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
        public int? DurationDays { get; set; }
        public bool IsActive { get; set; } = true;
        public int Quantity { get; set; } = 1;
        public decimal Subtotal => BasePrice * Quantity;
    }

    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public string CompanyName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}

