﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using MapPoint;
using MySql.Data.MySqlClient;

namespace MapWindow
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        MapPoint.Map myMap;
        Dictionary<string, Pushpin> pins = new Dictionary<string, Pushpin>();
        Dictionary<string, MapPoint.Shape> paths = new Dictionary<string, MapPoint.Shape>();
        MySqlConnection Connection = new MySqlConnection();
        string flightId;
        List<string> devices;

        public Form1() {
            InitializeComponent();

            Connection.ConnectionString = "SERVER=localhost;DATABASE=balloontrack;UID=root;";

            try
            {
                Connection.Open();
            }
            catch
            {
                MessageBox.Show("Database Connection Failed");
                Environment.Exit(0);
            }

            myMap = axMappointControl1.NewMap(MapPoint.GeoMapRegion.geoMapNorthAmerica);
            axMappointControl1.Units = GeoUnits.geoKm;
            axMappointControl1.ActiveMap.Saved = true;

            FillFlightSelector();

            myTimer.Tick += new EventHandler(TimerEvent);
            myTimer.Interval = 5000;
            myTimer.Start();
        }

        private Location GetLocation(string lat, string latRef, string lon, string lonRef) {
            double lat_f = double.Parse(lat);
            double lon_f = double.Parse(lon);

            if (latRef == "S") {
                lat_f *= -1;
            }

            if (lonRef == "W") {
                lon_f *= -1;
            }

            return myMap.GetLocation(lat_f, lon_f, 0);
        }

        private void FillFlightSelector() {
            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = "SELECT FlightId FROM gps group by FlightId";

            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            while (Reader.Read()) {
                FlightComboBox.Items.Add(Reader.GetValue(0).ToString());
            }

            Reader.Close();
        }

        private void TimerEvent(Object myObject, EventArgs myEventArgs) {
            UpdatePaths();
            UpdatePins();
        }

        private void UpdateDevices() {
            devices = new List<string>();

            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = "SELECT DeviceId from gps where FlightId = flightId group by DeviceId";

            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            while (Reader.Read()) {
                devices.Add(Reader.GetValue(0).ToString());
            }

            Reader.Close();
        }

        private void UpdatePaths() {
            UpdateDevices();

            foreach (string deviceId in devices) {
                MySqlCommand GPSCommand = Connection.CreateCommand();

                GPSCommand.CommandText = @"SELECT Lat, LatRef, Lon, LonRef, PacketId
                                         FROM gps
                                         WHERE
                                             Lat < 90 and Lon < 180 and 
                                             UNIX_TIMESTAMP(Timestamp) % 300 = 0 and
                                             DeviceId = '" + deviceId + @"' and 
                                             FlightId = '" + flightId + @"'
                                         ORDER BY Timestamp DESC
                                         LIMIT 10";

                MySqlDataReader Reader = GPSCommand.ExecuteReader();

                List<Location> locations = new List<Location>();

                while (Reader.Read()) {
                    locations.Add(GetLocation(Reader.GetValue(0).ToString(), Reader.GetValue(1).ToString(), Reader.GetValue(2).ToString(), Reader.GetValue(3).ToString()));
                };

                if (paths.ContainsKey(deviceId)) {
                    paths[deviceId].Delete();
                    paths.Remove(deviceId);
                }

                if (locations.Count > 1) {
                    paths.Add(deviceId, myMap.Shapes.AddPolyline(locations.ToArray()));
                }

                Reader.Close();
            }
        }
        
        private void UpdatePins() {
            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = @"
                SELECT DeviceId, Timestamp, Lat, LatRef, Lon, LonRef, Altitude, Rate, Spd, Hdg, Status, PacketId
                FROM gps 
                WHERE 
                    Lat < 90 and Lon < 180 and 
                    FlightId = '" + flightId + @"' and
                    (DeviceId, Timestamp) IN
                    ( SELECT DeviceId, MAX(Timestamp) Timestamp
                      FROM gps
                      WHERE FlightId = '" + flightId + @"'
                      GROUP BY DeviceId
                    )";


            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            short symbol = 1000;

            while (Reader.Read()) {
                symbol++;
                if (symbol > 23) { symbol = 17; }

                string id = Reader.GetValue(0).ToString();
                string lat = Reader.GetValue(2).ToString();
                string latRef = Reader.GetValue(3).ToString();
                string lon = Reader.GetValue(4).ToString();
                string lonRef = Reader.GetValue(5).ToString();
                string altitude = Reader.GetValue(6).ToString();
                string rate = Reader.GetValue(7).ToString();
                string spd = Reader.GetValue(8).ToString();
                string hdg = Reader.GetValue(9).ToString();
                string status = Reader.GetValue(10).ToString();

                Location location = GetLocation(lat, latRef, lon, lonRef);

                if (!String.IsNullOrEmpty(id)){
                    if (pins.ContainsKey(id)) {
                        pins[id].Location = location;
                    } else {
                        pins.Add(id, myMap.AddPushpin(location, id));
                    }

                    pins[id].Symbol = symbol;
                    pins[id].Note = "Alt: " + altitude + "\nRate: " + rate +
                        "\nLocation: " + lat + latRef + " " + lon + lonRef +
                        "\nStatus: " + status +
                        "\nHeading: " + hdg +
                        "\nSpeed: " + spd;
                    pins[id].BalloonState = GeoBalloonState.geoDisplayBalloon;
                }
            }

            Reader.Close();

            axMappointControl1.ActiveMap.Saved = true;
        }

        private void FlightComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            foreach (MapPoint.Shape i in myMap.Shapes) {
                i.Delete();
            }
            paths.Clear();

            flightId = FlightComboBox.SelectedItem.ToString();

            foreach (var pin in pins) {
                pins[pin.Key].Delete();
            }

            pins.Clear();

            UpdatePins();
        }
    }
}