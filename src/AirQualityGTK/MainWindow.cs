using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace AirQualityGTK
{
    class MainWindow : Window
    {
        SensorAQ sensor;

        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;
        [UI] private Button _btnstart = null;
        [UI] private Button _btnstop = null;

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            sensor = new SensorAQ();
            sensor.DataReceived += (a, e) => {
                _label1.Text = $"Data: {e.Data}";
            };
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
            _btnstart.Clicked += (a,b)=>{
                sensor.Start();
            };
            _btnstop.Clicked += (a, b) => {
                sensor.Stop();
            };
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            _counter++;
            
            _label1.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
        }
    }
}
