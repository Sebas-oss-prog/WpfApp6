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
    public partial class ClientWindow : Window
    {
        private List<Service> selectedServices = new List<Service>();

        public ClientWindow()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            string query = "SELECT * FROM Services WHERE IsActive = 1";
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
                    DurationDays = row["DurationDays"] != DBNull.Value ? Convert.ToInt32(row["DurationDays"]) : (int?)null
                });
            }

            servicesGrid.ItemsSource = services;
        }

        private void AddServices_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления выбранных услуг
            // (упрощённая реализация)
            if (servicesGrid.SelectedItem is Service selectedService)
            {
                selectedServices.Add(selectedService);
                selectedServicesGrid.ItemsSource = null;
                selectedServicesGrid.ItemsSource = selectedServices;
            }
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Добавляем клиента
                string insertClient = @"
                INSERT INTO Clients (CompanyName, ContactPerson, Phone, Email, RegistrationDate, IsActive)
                VALUES (@CompanyName, @ContactPerson, @Phone, @Email, GETDATE(), 1);
                SELECT SCOPE_IDENTITY();";

                SqlParameter[] clientParams = {
                new SqlParameter("@CompanyName", txtCompanyName.Text),
                new SqlParameter("@ContactPerson", txtContactPerson.Text),
                new SqlParameter("@Phone", txtPhone.Text),
                new SqlParameter("@Email", txtEmail.Text)
            };

                int clientId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertClient, clientParams));

                // 2. Добавляем заказ
                decimal totalAmount = selectedServices.Sum(s => s.BasePrice);

                string insertOrder = @"
                INSERT INTO Orders (ClientID, OrderDate, TotalAmount, Status)
                VALUES (@ClientID, GETDATE(), @TotalAmount, 'Новый');
                SELECT SCOPE_IDENTITY();";

                SqlParameter[] orderParams = {
                new SqlParameter("@ClientID", clientId),
                new SqlParameter("@TotalAmount", totalAmount)
            };

                int orderId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertOrder, orderParams));

                // 3. Добавляем позиции заказа
                foreach (var service in selectedServices)
                {
                    string insertOrderItem = @"
                    INSERT INTO OrderItems (OrderID, ServiceID, Quantity, UnitPrice, Status)
                    VALUES (@OrderID, @ServiceID, 1, @UnitPrice, 'Не начато')";

                    SqlParameter[] itemParams = {
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@ServiceID", service.ServiceID),
                    new SqlParameter("@UnitPrice", service.BasePrice)
                };

                    DatabaseHelper.ExecuteNonQuery(insertOrderItem, itemParams);
                }

                MessageBox.Show("Заказ успешно оформлен!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}");
            }
        }
    }
}
