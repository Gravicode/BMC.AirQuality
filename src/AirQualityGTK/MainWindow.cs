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

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
            sensor = new SensorAQ();
            sensor.DataReceived+= (a, e) => {
                _label1.Text = $"Data: {e.Data}";
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
