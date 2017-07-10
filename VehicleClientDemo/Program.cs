using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Timers;

namespace VehicleClientDemo
{
    class Program
    {
        private static Vehicle car = new Vehicle("502WRY");
        static void Main(string[] args)
        {
            //startTimer();
            car.Drive();
            //Console.WriteLine("Vehicle stoped and stop updating database. Press any key to finish...");
            Console.ReadKey(true);
        }
    }
    public class Vehicle
    {
        private double _latitude = -27.4698;
        private double _longitude = 153.0251;
        public string RegistrationPlate { get { return _plate; } }
        public double Latitude
        {
            get
            {
                _latitude += _latitude + (new Random().NextDouble()) / 3000;
                return _latitude;
            }
        }
        public double Longitude
        {
            get
            {
                _longitude += (new Random().NextDouble()) / 3000;
                return _longitude;
            }
        }
        public double Altitude {
            get { return new Random().NextDouble() * 50; }
            set { } }
        private readonly string _plate;
        private readonly string userName = "silversr";
        private readonly string password = "diskedit7_SR";
        public Vehicle(string registrationPlate)
        {
            _plate = registrationPlate;
        }
        private SqlConnection _connection;
        public void Drive()
        {
            //build connection
            try
            {
                /*
                string connectString = "Data Source=(localdb)\\MSSQLLocalDB;"
                    + "Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;"
                    + "ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    */
                string connectString = "Server = tcp:vehiclemonitorv1db-server.database.windows.net,1433;"
                    + "Initial Catalog = VehicleMonitorV1DB; Persist Security Info = False;"
                    + $"User ID = {userName}; Password ={password}; MultipleActiveResultSets = False; Encrypt = True;"
                    + "TrustServerCertificate = False; Connection Timeout = 30;";
                //using (SqlConnection connection = new SqlConnection(connectString))
                SqlConnection connection = new SqlConnection(connectString);
                //{
                    connection.Open();
                    Console.WriteLine("Connection Opened");
                    this._connection = connection;
                    this.startTimer();
                    //UpdateCoordinateToDB(connection);
                //}
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            //update 
        }
        public void UpdateCoordinateToDB(SqlConnection connection)
        {
            //connect to SQL
            StringBuilder sb = new StringBuilder();
            string sql;
            //update data to the database 
            string toUpdate = this._plate;
            double lat = this.Latitude;
            double lon = this.Longitude;
            sb.Clear();
            sb.Append($"UPDATE VehicleMonitorV1DB.dbo.Vehicles SET Latitude = {lat}, Longitude = {lon}, Altitude = {this.Altitude}");
            sb.Append("WHERE RegistrationPlate = @registrationPlate");
            sql = sb.ToString();
            using (SqlCommand cmd = new SqlCommand(sql, connection))

            {

                cmd.Parameters.AddWithValue("@registrationPlate", toUpdate);
                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " row(s) updated");
                Console.WriteLine($"new vehicle coordinate is lat = {lat}, lon = {lon}, altitude = {Altitude}");

            }
        }
        private void startTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 2000;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"Current time: {DateTime.Now}");
            this.UpdateCoordinateToDB(this._connection);
        }
    }
}
