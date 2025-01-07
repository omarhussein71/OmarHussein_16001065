using MySql.Data.MySqlClient;
using System;

namespace PharmacyManagement
{
    class Program
    {
        static string connectionString = "Server=localhost;Database=pharmacy;User ID=root;Password=yourpassword;";
        static MySqlConnection connection = new MySqlConnection(connectionString);

        static void Main(string[] args)
        {
            try
            {
                connection.Open();
                Console.WriteLine("Welcome to the Pharmacy Management System");

                while (true)
                {
                    Console.WriteLine("\n1. Add Pharmacist");
                    Console.WriteLine("2. Add Medicine");
                    Console.WriteLine("3. Soft Delete Medicine");
                    Console.WriteLine("4. Show Available Medicines");
                    Console.WriteLine("5. Exit");
                    Console.Write("Choose an option: ");

                    int choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            Console.Write("Enter First Name: ");
                            string firstName = Console.ReadLine();
                            Console.Write("Enter Last Name: ");
                            string lastName = Console.ReadLine();
                            Console.Write("Enter Email: ");
                            string email = Console.ReadLine();
                            Console.Write("Enter Phone: ");
                            string phone = Console.ReadLine();
                            Console.Write("Enter Address: ");
                            string address = Console.ReadLine();
                            AddPharmacist(firstName, lastName, email, phone, address);
                            break;
                        case 2:
                            Console.Write("Enter Medicine Name: ");
                            string name = Console.ReadLine();
                            Console.Write("Enter Manufacturer: ");
                            string manufacturer = Console.ReadLine();
                            Console.Write("Enter Stock: ");
                            int stock = int.Parse(Console.ReadLine());
                            Console.Write("Enter Price: ");
                            decimal price = decimal.Parse(Console.ReadLine());
                            AddMedicine(name, manufacturer, stock, price);
                            break;
                        case 3:
                            Console.Write("Enter Medicine ID to delete: ");
                            int medicineId = int.Parse(Console.ReadLine());
                            SoftDeleteMedicine(medicineId);
                            break;
                        case 4:
                            ShowAvailableMedicines();
                            break;
                        case 5:
                            connection.Close();
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        static void AddPharmacist(string firstName, string lastName, string email, string phone, string address)
        {
            try
            {
                string query1 = "INSERT INTO Pharmacist (FirstName, LastName) VALUES (@FirstName, @LastName)";
                int pharmacistID;

                using (MySqlCommand cmd = new MySqlCommand(query1, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.ExecuteNonQuery();
                    pharmacistID = (int)cmd.LastInsertedId; // الحصول على الرقم التعريفي للصيدلي
                }

                string query2 = "INSERT INTO ContactInfo (PharmacistID, Email, Phone, Address) VALUES (@PharmacistID, @Email, @Phone, @Address)";
                using (MySqlCommand cmd = new MySqlCommand(query2, connection))
                {
                    cmd.Parameters.AddWithValue("@PharmacistID", pharmacistID);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Pharmacist added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding pharmacist: {ex.Message}");
            }
        }

        static void AddMedicine(string name, string manufacturer, int stock, decimal price)
        {
            try
            {
                string query = "INSERT INTO Medicine (Name, Manufacturer, Stock, Price) VALUES (@Name, @Manufacturer, @Stock, @Price)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Manufacturer", manufacturer);
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Medicine added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding medicine: {ex.Message}");
            }
        }

        static void SoftDeleteMedicine(int medicineId)
        {
            try
            {
                string query = "UPDATE Medicine SET Stock = 0 WHERE MedicineID = @MedicineID";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MedicineID", medicineId);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Medicine marked as deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting medicine: {ex.Message}");
            }
        }

        static void ShowAvailableMedicines()
        {
            try
            {
                string query = "SELECT * FROM Medicine WHERE Stock > 0";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\nAvailable Medicines:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["MedicineID"]}, Name: {reader["Name"]}, Stock: {reader["Stock"]}, Price: {reader["Price"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing medicines: {ex.Message}");
            }
        }
    }
}
