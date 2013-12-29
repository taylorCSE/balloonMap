using System;
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
            myTimer.Interval = 10000;
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
            UpdatePins();
        }

        private void UpdatePaths() {
            foreach (string deviceId in devices) {
                MySqlCommand GPSCommand = Connection.CreateCommand();

                GPSCommand.CommandText = @"(SELECT Lat, LatRef, Lon, LonRef, PacketId
                                         FROM gps
                                         WHERE
                                             Lat < 90 and Lon < 180 and
                                             Lat > 0 and Lon > 0 and
                                             UNIX_TIMESTAMP(Timestamp) % 300 = 0 and
                                             DeviceId = '" + deviceId + @"' and 
                                             FlightId = '" + flightId + @"'
                                         ORDER BY Timestamp DESC
                                         LIMIT 4)
                                         UNION ALL
                                         (SELECT Lat, LatRef, Lon, LonRef, PacketId
                                         FROM gps
                                         WHERE
                                             DeviceId = '" + deviceId + @"' and 
                                             FlightId = '" + flightId + @"' and
                                             Timestamp = 
                                             (
                                                SELECT min(Timestamp) from gps WHERE
                                                DeviceId = '" + deviceId + @"' and 
                                                FlightId = '" + flightId + @"'
                                              )
                                         )";

                MySqlDataReader Reader = GPSCommand.ExecuteReader();

                List<Location> locations = new List<Location>();

                locations.Add(pins[deviceId].Location);
                while (Reader.Read()) {
                    locations.Add(GetLocation(Reader.GetValue(0).ToString(), Reader.GetValue(1).ToString(), Reader.GetValue(2).ToString(), Reader.GetValue(3).ToString()));
                };

                if (paths.ContainsKey(deviceId)) {
                    MapPoint.Shape path = paths[deviceId];
                    paths.Remove(deviceId);
                    path.Delete();
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
                SELECT Lat, LatRef, Lon, LonRef, DeviceId, Timestamp, Altitude, Rate, Spd, Hdg, Status, PacketId
                FROM gps 
                WHERE 
                    (FlightId, DeviceId, PacketId) IN
                    (
                        SELECT * FROM
                        ( SELECT FlightId, DeviceId, MAX(PacketId) PacketId
                            FROM gps
                            WHERE FlightId = '" + flightId + @"'
                            GROUP BY DeviceId
                        ) AS SUBQUERY
                    )";

            MySqlDataReader Reader = GPSCommand.ExecuteReader();

            short symbol = 1000;

            devices = new List<string>();

            while (Reader.Read())
            {
                symbol++;
                if (symbol > 23) { symbol = 17; }

                string lat = Reader.GetValue(0).ToString();
                string latRef = Reader.GetValue(1).ToString();
                string lon = Reader.GetValue(2).ToString();
                string lonRef = Reader.GetValue(3).ToString();
                string id = Reader.GetValue(4).ToString();
                string altitude = Reader.GetValue(6).ToString();
                string rate = Reader.GetValue(7).ToString();
                string spd = Reader.GetValue(8).ToString();
                string hdg = Reader.GetValue(9).ToString();
                string status = Reader.GetValue(10).ToString();

                Location location = GetLocation(lat, latRef, lon, lonRef);

                if (!String.IsNullOrEmpty(id)){
                    devices.Add(id);
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

            UpdatePaths();

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