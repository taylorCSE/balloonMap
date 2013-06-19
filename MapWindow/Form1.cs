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
            }

            myMap = axMappointControl1.NewMap(MapPoint.GeoMapRegion.geoMapNorthAmerica);
            axMappointControl1.Units = GeoUnits.geoKm;

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
            UpdatePins();
        }
        
        private void UpdatePins() {
            MySqlCommand GPSCommand = Connection.CreateCommand();

            GPSCommand.CommandText = "SELECT DeviceId, max(Timestamp), Lat, LatRef, Lon, LonRef FROM `gps` where Lat < 90 and Lon < 180 and FlightId = '" + flightId + "' group by DeviceId";

            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            while (Reader.Read())
            {
                string id = Reader.GetValue(0).ToString();
                double lat = double.Parse(Reader.GetValue(2).ToString());
                double lon = -double.Parse(Reader.GetValue(4).ToString());

                Location location = myMap.GetLocation(lat, lon, 245);

                if (pins.ContainsKey(id)) {
                    pins[id].Location = location;
                } else {
                    pins.Add(id, myMap.AddPushpin(location, id));
                }
            }

            Reader.Close();
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