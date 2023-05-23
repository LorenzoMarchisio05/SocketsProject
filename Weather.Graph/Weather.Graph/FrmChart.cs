using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Wheater.Commons.DB;

namespace Weather.Graph
{
    public partial class FrmChart : Form
    {
        private const string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\l.marchisio.2268\Desktop\SocketsProject\DB\weatherStationDB.mdf;Integrated Security=True;Connect Timeout=30";

        private static readonly AdoNetController _adoNetController = new AdoNetController(_connectionString);

        private static readonly ChartArea chartArea = new ChartArea();

        public FrmChart()
        {
            InitializeComponent();

            CreateTabs();

            foreach(TabPage page in tabControlStations.TabPages)
            {
                var chart = new Chart
                {
                    Name = $"chart{page.Name}",
                    Dock = DockStyle.Fill,
                    Parent = page
                };

                chart.ChartAreas.Add(chartArea);

                Series serie = new Series(page.Name);

                var results = GetTemperatureHumidity(page);

                foreach (DataRow row in results.Rows)
                {
                    var temperature = Convert.ToDouble(row[0]);
                    var humidity = Convert.ToInt32(row[1]);

                    MessageBox.Show($"{temperature} {humidity}");
                }


                chart.Series.Add(serie);

                page.Controls.Add(chart);
            }
        }

        private static DataTable GetTemperatureHumidity(TabPage page)
        {
            var command = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT temperature, humidity
                                    FROM weatherdata
                                    WHERE stationName = @name;"
            };

            command
                .Parameters
                .Add(new SqlParameter("@name", SqlDbType.VarChar, 75) { Value = page.Name });

            var results = _adoNetController.ExecuteQuery(command);
            return results;
        }

        private void CreateTabs()
        {
            tabControlStations.TabPages.Clear();

            var command = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT DISTINCT stationName
                                FROM weatherdata;"
            };

            var results = _adoNetController.ExecuteQuery(command);

            if (results is null)
            {
                return;
            }

            foreach (DataRow row in results.Rows)
            {
                var name = row[0]?.ToString();
                createTab(name);
            }
        }

        private void createTab(string tabName)
        {
            tabControlStations.TabPages.Add(tabName);
        }

    }
}
