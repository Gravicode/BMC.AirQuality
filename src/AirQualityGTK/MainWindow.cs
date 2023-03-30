using System;
using System.Linq;
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
        [UI] private Button _btnstart = null;
        [UI] private Button _btnstop = null;
        [UI] private Box _box1 = null;

        LineSeries PM1Series, PM25Series, PM10Series, ParticleSeries03, ParticleSeries05;
        int count = 0;
        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            sensor = new SensorAQ();
           
            DeleteEvent += Window_DeleteEvent;
            _btnstart.Clicked += (a,b)=>{
                sensor.Start();
            };
            _btnstop.Clicked += (a, b) => {
                sensor.Stop();
            };
            var plotPM = new PlotView();
            var plotParticle = new PlotView();
          
            _box1.PackStart(plotPM,true,true,0);
            _box1.PackStart(plotParticle, true,true,0);

            PM1Series = new LineSeries() { MarkerType = MarkerType.Circle };
            PM1Series.Title = "PM1";
            PM1Series.Points.Add(new DataPoint(0, 0));

            PM25Series = new LineSeries() { MarkerType = MarkerType.Circle };
            PM25Series.Title = "PM25";
            PM25Series.Points.Add(new DataPoint(0, 0));

            PM10Series = new LineSeries() { MarkerType = MarkerType.Circle };
            PM10Series.Title = "PM10";
            PM10Series.Points.Add(new DataPoint(0, 0));

            var modelPM = new PlotModel { Title = "Concentration PM 1.0, 2.5, 10" };
            modelPM.Series.Add(PM1Series);
            modelPM.Series.Add(PM25Series);
            modelPM.Series.Add(PM10Series);
            //new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)")
            plotPM.Model = modelPM;
            plotPM.Show();

            ParticleSeries03 = new LineSeries() { MarkerType = MarkerType.Diamond };
            ParticleSeries03.Title = "Particle 0.3";
            ParticleSeries03.Points.Add(new DataPoint(0, 0));

            ParticleSeries05 = new LineSeries() { MarkerType = MarkerType.Diamond };
            ParticleSeries05.Title = "Particle 0.5";
            ParticleSeries05.Points.Add(new DataPoint(0, 0));
            
            var modelParticle = new PlotModel { Title = "Number Particle diameter of 0.3/0.5 um per 0.1L" };
            modelParticle.Series.Add(ParticleSeries03);
            modelParticle.Series.Add(ParticleSeries05);
            //new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)")
            plotParticle.Model = modelParticle;
            plotParticle.Show();
            
            sensor.DataReceived += (a, e) => {
                //_label1.Text = $"Data: {e.Data.ToString()}";
                var datas = sensor.GetData().TakeLast(10);
                Gtk.Application.Invoke(delegate
                {
                    PM1Series.Points.Clear();
                    PM25Series.Points.Clear();
                    PM10Series.Points.Clear();
                    ParticleSeries03.Points.Clear();
                    ParticleSeries05.Points.Clear();
                    count = 0;
                    foreach (var item in datas)
                    {
                        PM1Series.Points.Add(new DataPoint(count, item.PM1));
                        PM25Series.Points.Add(new DataPoint(count, item.PM25));
                        PM10Series.Points.Add(new DataPoint(count, item.PM10));
                        ParticleSeries03.Points.Add(new DataPoint(count, item.ParticleNum03));
                        ParticleSeries05.Points.Add(new DataPoint(count, item.ParticleNum05));
                        count++;
                    }
                    _label1.Text = $"Update: {DateTime.Now}, AQ: {sensor.MeasureAirQuality()}";
                    //modelParticle.Series.Clear();
                    //modelParticle.Series.Add(ParticleSeries03);
                    //modelParticle.Series.Add(ParticleSeries05);
                    //plotParticle.Model = modelParticle;
                    plotPM.InvalidatePlot(true);
                    plotParticle.InvalidatePlot(true);
                });
            };
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

      
    }
}
