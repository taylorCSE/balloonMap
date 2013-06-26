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
        MySqlConnection Connection = new MySqlConnection();
        string flightId;

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

        private void UpdatePaths() {
            List<string> devices = new List<string>();

            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = "SELECT DeviceId from gps where FlightId = flightId group by DeviceId";

            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            while (Reader.Read()) {
                devices.Add(Reader.GetValue(0).ToString());
            }

            Reader.Close();

            foreach (string deviceId in devices) {
                Location[] locations = new Location[2];
                locations[0] = myMap.GetLocation(40, -84, 245);
                locations[1] = myMap.GetLocation(40, -86, 245);

                foreach (MapPoint.Shape i in myMap.Shapes)
                {
                    i.Delete();
                }
                myMap.Shapes.AddPolyline(locations);
            }
        }
        
        private void UpdatePins() {
            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = @"
                SELECT DeviceId, Timestamp, Lat, LatRef, Lon, LonRef, Altitude, Rate, PacketId 
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
                double lat = double.Parse(Reader.GetValue(2).ToString());
                string latRef = Reader.GetValue(3).ToString();
                double lon = double.Parse(Reader.GetValue(4).ToString());
                string lonRef = Reader.GetValue(5).ToString();
                string altitude = Reader.GetValue(6).ToString();
                string rate = Reader.GetValue(7).ToString();

                if (latRef == "S") {
                    lat *= -1;
                }

                if (lonRef == "W") {
                    lon *= -1;
                }

                Location location = myMap.GetLocation(lat, lon, 245);

                if (!String.IsNullOrEmpty(id)){
                    if (pins.ContainsKey(id)) {
                        pins[id].Location = location;
                    } else {
                        pins.Add(id, myMap.AddPushpin(location, id));
                    }

                    pins[id].Symbol = symbol;
                    pins[id].Note = "Alt: " + altitude + "\nRate: " + rate;
                    pins[id].BalloonState = GeoBalloonState.geoDisplayBalloon;
                }
            }

            Reader.Close();

            axMappointControl1.ActiveMap.Saved = true;
        }

        private void FlightComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            flightId = FlightComboBox.SelectedItem.ToString();

            foreach (var pin in pins) {
                pins[pin.Key].Delete();
            }

            pins.Clear();

            UpdatePins();
        }
    }
}