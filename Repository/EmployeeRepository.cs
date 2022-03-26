using dbChange.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.Data.SqlClient;

namespace dbChange.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<SignalServer>();
        string connectionString = "";
        public EmployeeRepository(IConfiguration configuration,
                IHubContext context)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDependency.Start(connectionString);
                string commandText = "SELECT * FROM dbo.Employees";

                SqlCommand cmd = new SqlCommand(commandText, conn);
                SqlDependency dependency = new SqlDependency(cmd);

                dependency.OnChange += new OnChangeEventHandler(dbChangeNotification);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var employee = new Employee
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Age = Convert.ToInt32(reader["Age"])
                    };
                    employees.Add(employee);
                }
            }


            return employees;
        }

        private void dbChangeNotification(object sender, SqlNotificationEventArgs e)
        {
            _context.Clients.All.SendAsync("refreshEmployees");
        }
    }
}