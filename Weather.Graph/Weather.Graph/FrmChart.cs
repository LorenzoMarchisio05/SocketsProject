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

        public FrmChart()
        {
            InitializeComponent();

            CreateTabs();

            foreach(TabPage page in tabControlStations.TabPages)
            {
                var chartArea = new ChartArea();

                var chart = new Chart
                {
                    Name = $"chart{page.Text}",
                    Dock = DockStyle.Fill,
                    Parent = page
                };

                chart.ChartAreas.Add(chartArea);

                CreateSeriesOnGraph(page, chart);

                chart.Legends.Add(new Legend
                {
                    Title = "Graphs"
                });

                page.Controls.Add(chart);
            }
        }

        private static void CreateSeriesOnGraph(TabPage page, Chart chart)
        {
            var temperatures = new Series("Temperatures");
            var humidities = new Series("Humidity");

            temperatures.ChartType = SeriesChartType.Spline;
            temperatures.Color = Color.Blue;
            temperatures.IsValueShownAsLabel = true;
            temperatures.IsVisibleInLegend = true;

            humidities.ChartType = SeriesChartType.Spline;
            humidities.Color = Color.Red;
            humidities.IsValueShownAsLabel = true;
            humidities.IsVisibleInLegend = true;

            var results = GetWeatherDataFromDb(page.Text);

            foreach (DataRow row in results.Rows)
            {
                var temperature = Convert.ToDouble(row[0]);
                var humidity = Convert.ToInt32(row[1]);
                var time = row[2]?.ToString();

                temperatures.Points.Add(temperature);
                humidities.Points.Add(humidity);
                humidities.Points[humidities.Points.Count - 1].AxisLabel = time;
            }

            chart.Series.Add(temperatures);
            chart.Series.Add(humidities);
        }

        private static DataTable GetWeatherDataFromDb(string stationName)
        {
            var command = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT TOP 25 temperature, humidity, dateTime
                                    FROM weatherdata
                                    WHERE stationName = @name
                                    ORDER BY id DESC;"
            };


            command
                .Parameters
                .Add(new SqlParameter("@name", SqlDbType.VarChar, 75) { Value = stationName });

            return _adoNetController.ExecuteQuery(command);
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
            tabControlStations.TabPages.Add( new TabPage(tabName) { Name = tabName });
        }

    }
}
