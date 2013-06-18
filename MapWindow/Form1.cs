/* 
 * Author: Ryan Mann
 * This code takes in data from a serial port an maps it onto a stand alone MapPoint map.
 * The map has a start run button, close map button, zoom bar, a Ballon locator box, database, and UI
 * 
 * Still in progress:
*/
using System;                       // Needed for basic system commands.
using System.IO;                    // Needed to access input and output streams.
using System.Collections.Generic;   // Needed to access generic collections.
using System.ComponentModel;        // Needed to edit component Model.
using System.Data;                  // Needed to control data types.
using System.Data.SqlClient;        // Needed to run SQL code.
using System.Drawing;               // Needed to draw lines on map.
using System.Linq;                  // Used for linking.
using System.Text;                  // Used to interact with text.
using System.Windows.Forms;         // Needed to make windows form.
using MapPoint;                     // Needed to use MapPoint.
using MySql.Data.MySqlClient;       // Needed to use MYSQL.
using System.Globalization;         // Needed for converting HEX.

namespace MapWindow
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        MapPoint.Map myMap;

        MySqlConnection Connection = new MySqlConnection();                   // Setting up Database Connection.

        // This is the method to run when the timer is raised. 
        private void UpdatePins(Object myObject, EventArgs myEventArgs) {
            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = "SELECT DeviceId, max(Timestamp), Lat, LatRef, Lon, LonRef FROM `gps` where Lat < 90 and Lon < 180 and FlightId = 'taylor05' group by DeviceId";

            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            while (Reader.Read())
            {
                string id = Reader.GetValue(0).ToString();
                double lat = double.Parse(Reader.GetValue(2).ToString());
                double lon = -double.Parse(Reader.GetValue(4).ToString());

                myMap.AddPushpin(myMap.GetLocation(lat, lon, 245), id);
            }

            Reader.Close();
        }

        // Starting up Form1.
        public Form1() {
            // Initializing Form1.
            InitializeComponent();

            Connection.ConnectionString = "SERVER=localhost;DATABASE=balloontrack;UID=root;";

            // Opening database connection.
            try
            {
                Connection.Open();
            }
            catch
            {
                // Handle connection error
            }

            myMap = axMappointControl1.NewMap(MapPoint.GeoMapRegion.geoMapNorthAmerica);  // Getting Map.
            MapPoint.MapFeatures features = myMap.MapFeatures;                    // Getting Map Features.
            axMappointControl1.Units = GeoUnits.geoKm;                            // Setting Units of map to Kilometers.

            myTimer.Tick += new EventHandler(UpdatePins);
            myTimer.Interval = 5000;
            myTimer.Start();
        }

        // What happens when program status is changed.
        private void program_UpdateChanged(object sender, EventArgs e)
        {
        }

        // What happens when program error is changed.
        private void program_ErrorChanged(object sender, EventArgs e)
        {
        }

        // What happens when program total is changed.
        private void program_TotalChanged(object sender, EventArgs e)
        {
        }
    }
}