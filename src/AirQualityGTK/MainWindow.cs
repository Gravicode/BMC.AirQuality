using System;
using Gtk;
using OxyPlot;
using OxyPlot.GtkSharp;
using OxyPlot.Series;
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
        [UI] private Box _box1 = null;

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            sensor = new SensorAQ();
            sensor.DataReceived += (a, e) => {
                _label1.Text = $"Data: {e.Data.ToString()}";
            };
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
            _btnstart.Clicked += (a,b)=>{
                sensor.Start();
            };
            _btnstop.Clicked += (a, b) => {
                sensor.Stop();
            };
            var plotView = new PlotView();
          
            _box1.PackStart(plotView,true,true,0);

            var myModel = new PlotModel { Title = "Example 1" };
            myModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            plotView.Model = myModel;
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
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
