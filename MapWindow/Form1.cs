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
        //String Distance;                                                      // Distance string from MapPoint pin.
        String Direction = string.Empty;                                      // Direction string from GPS Heading.
        MapPoint.Location[] CurrentLoc = new Location[10000];                 // Current Location.
        MapPoint.Location[] LastLoc = new Location[10000];                    // Last Location.
        //LastLoc = myMap.GetLocation(41, (-86), 50);                           // Generic Starting Position in Indiana.
        //Start = LastLoc;                                                      // Marking generic starting location.
        String[] Balloons = new string[10000];                                // Keeps balloon ID's.

        //------------------------------------------------Variables used in Parsing section-----------------------------------------------------
        byte[] input = new byte[46];                                          // Input from Serial Port.
        String[] inputString = new String[46];                                // Input turned into Hex format.
        String[] GPSstring = new String[7];                                   // GPS input split.
        String GPSstring2 = String.Empty;                                     // Input from GPS as characters.
        string[] Lat = new string[2];                                         // String to used in Latitude degrees calculation.
        string[] Lon = new string[2];                                         // String to used in Longitude degrees calculation.

        //------------------------------------------------Variables used to get data from Parsing to Mapping------------------------------------
        string Status = string.Empty;                                         // Status input from serial port.
        string LatR = string.Empty;                                           // Latitude value from serial port.
        string LonR = string.Empty;                                           // Longitude value from serial port.
        string[] CRC = new string[2];                                         // CRC value from serial port.
        string ID = string.Empty;                                             // ID value from serial port.
        string COM = string.Empty;                                            // Command from serial port.
        string DP = string.Empty;                                             // Digital Payload from serial port.
        string[] APconvert = new string[36];                                  // Analog Payload from serial port.
        decimal[] AP = new decimal[18];                                       // Analog Payload converted.

        private MySqlDataAdapter data = new MySqlDataAdapter();               // Data value used in checking for duplicate Flight IDs.

        string current = string.Empty;                                        // Used when changeing Port names.
        string UpdateStatus = string.Empty;                                   // Program Status.

        string Error = string.Empty;                                          // Program Status.

        string FlightID = string.Empty;                                       // Flight ID.

        bool DBconnection = new bool();                                       // DBconnection On or Off.
        string FlightDescription = string.Empty;                              // Description of the flight.

        TextWriter file;                                                      // Setting up text file to write to.

        string selected = string.Empty;                                       // Used in Balloon find and zoom.

        MySqlConnection Connection = new MySqlConnection();                   // Setting up Database Connection.

        MapPoint.Shape[] shape = new Shape[10000];                            // Used to draw lines for the balloons.

        // Starting up Form1.
        public Form1()
        {
            // Initializing Form1.
            InitializeComponent();

            Connection.ConnectionString = "SERVER=localhost;DATABASE=balloontrack;UID=root;";

            // Opening database connection.
            try {
                Connection.Open();
            }
            catch {
                // Handle connection error
            }

            MapPoint.Map myMap = axMappointControl1.NewMap(MapPoint.GeoMapRegion.geoMapNorthAmerica);  // Getting Map.
            MapPoint.MapFeatures features = myMap.MapFeatures;                    // Getting Map Features.
            axMappointControl1.Units = GeoUnits.geoKm;                            // Setting Units of map to Kilometers.

            myMap.AddPushpin(myMap.GetLocation(40.467222, -85.5, 285), "Upland");

            MySqlCommand CommandAIPDump = Connection.CreateCommand();

            CommandAIPDump.CommandText = "SELECT FlightId, max(Timestamp), Lat, LatRef, Lon, LonRef FROM `gps` where Lat < 90 and Lon < 180 group by FlightId";

            MySqlDataReader Reader;
            Reader = CommandAIPDump.ExecuteReader();

            while (Reader.Read())
            {
                string id = Reader.GetValue(0).ToString();
                double lat = double.Parse(Reader.GetValue(2).ToString());
                double lon = -double.Parse(Reader.GetValue(4).ToString());

                myMap.AddPushpin(myMap.GetLocation(lat, lon, 245), id);
            }

            Reader.Close();
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

        private void DBconnect_CheckedChanged(object sender, EventArgs e)
        {
            // Database connection string.
            string Connect = "SERVER=localhost;DATABASE=balloontrack;UID=root;";

            if (DBconnection == true)
            {
                try
                {
                    // Connecting to database.
                    Connection.ConnectionString = Connect;

                    // Checking to see if you database connection is wanted.
                    if (DBconnection)
                    {
                        // Opening database connection.
                        try
                        {
                            Connection.Open();
                            if (((File.Exists("C:\\Documents and Settings\\CCLI\\Desktop\\Flight TXT Files\\" + FlightID + ".txt"))))
                            {
                                try
                                {
                                    file.Close();
                                    // Updating program status.
                                    UpdateStatus = "Text file closed.";
                                    this.Invoke(new EventHandler(program_UpdateChanged));
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch
                        {
                            // Updating program status.
                            UpdateStatus = "Please turn on WAMP.";
                            this.Invoke(new EventHandler(program_UpdateChanged));
                        }
                    }
                }
                catch
                {
                    // Updating program status.
                    UpdateStatus = "DataBase already open.";
                    this.Invoke(new EventHandler(program_UpdateChanged));
                }
            }
            else
            {
                Connection.Close();

                if ((!(File.Exists("C:\\Documents and Settings\\CCLI\\Desktop\\Flight TXT Files\\" + FlightID + ".txt"))))
                {
                    file = new StreamWriter("C:\\Documents and Settings\\CCLI\\Desktop\\Flight TXT Files\\" + FlightID + ".txt");
                    file.WriteLine(FlightID);
                    // Updating program status.
                    UpdateStatus = "Text file is opened.";
                    this.Invoke(new EventHandler(program_UpdateChanged));
                }
            }
        }
    }
}